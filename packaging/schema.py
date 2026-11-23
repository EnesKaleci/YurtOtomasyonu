import sys, zipfile, xml.etree.ElementTree as E
src,out=sys.argv[1:]
with zipfile.ZipFile(src) as z: root=E.fromstring(z.read("model.xml"))
ns={"d":"http://schemas.microsoft.com/sqlserver/dac/Serialization/2012/02"}
model=root.find("d:Model",ns)
def props(e): return {p.get("Name"):p.get("Value",p.findtext("d:Value",default="",namespaces=ns)) for p in e.findall("d:Property",ns)}
def rel(e,n): return e.find("d:Relationship[@Name='"+n+"']",ns)
def ref(e,n): return rel(e,n).find(".//d:References",ns).get("Name")
sql=[]
for table in model.findall("d:Element[@Type='SqlTable']",ns):
    columns=[]
    for col in rel(table,"Columns").findall("d:Entry/d:Element",ns):
        p=props(col); spec=rel(col,"TypeSpecifier").find("d:Entry/d:Element",ns); sp=props(spec)
        typ=ref(spec,"Type")
        if typ in ("[varchar]","[nvarchar]","[char]","[nchar]","[varbinary]","[binary]"):
            typ+="("+("max" if sp.get("IsMax")=="True" else sp.get("Length","1"))+")"
        elif typ in ("[decimal]","[numeric]"): typ+="("+sp.get("Precision","18")+","+sp.get("Scale","0")+")"
        name=col.get("Name").split(".")[-1]
        columns.append(name+" "+typ+(" IDENTITY(1,1)" if p.get("IsIdentity")=="True" else "")+(" NOT NULL" if p.get("IsNullable")=="False" else " NULL"))
    sql.append("CREATE TABLE "+table.get("Name")+"("+",\n".join(columns)+");")
for d in model.findall("d:Element[@Type='SqlDefaultConstraint']",ns):
    sql.append("ALTER TABLE "+ref(d,"DefiningTable")+" ADD DEFAULT "+props(d)["DefaultExpressionScript"]+" FOR "+ref(d,"ForColumn").split(".")[-1]+";")
for k in model.findall("d:Element[@Type='SqlPrimaryKeyConstraint']",ns):
    cols=[r.get("Name").split(".")[-1] for r in rel(k,"ColumnSpecifications").findall(".//d:References",ns)]
    if cols: sql.append("ALTER TABLE "+ref(k,"DefiningTable")+" ADD PRIMARY KEY("+",".join(cols)+");")
open(out,"w",encoding="utf-8-sig").write("\n".join(sql))
print("Schema generated without importing archived user data:",len(sql),"statements")
