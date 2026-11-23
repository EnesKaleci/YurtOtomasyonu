using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
namespace __NS__ {
internal static class Distribution {
    const string Database = "__DB__", Instance = "__INSTANCE__";
    internal static readonly string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EnesKaleci", "__APP__");
    internal static string ConnectionString { get { return @"Data Source=(LocalDB)\"+Instance+";Initial Catalog="+Database+";Integrated Security=True;Connect Timeout=60;MultipleActiveResultSets=True"; } }
    static int Run(string file,string args) {
        using(var p=Process.Start(new ProcessStartInfo(file,args){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true})) {
            var stdout=p.StandardOutput.ReadToEndAsync(); var stderr=p.StandardError.ReadToEndAsync();
            if(!p.WaitForExit(60000)) { p.Kill(); throw new Exception("Veritabani motoru zaman asimina ugradi."); }
            if(p.ExitCode!=0) throw new Exception("LocalDB: "+stderr.Result+" "+stdout.Result);
            return p.ExitCode;
        }
    }
    internal static void Initialize(bool smoke) {
        Directory.CreateDirectory(DataDirectory);
        string tool=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),@"Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe");
        if(!File.Exists(tool)) tool=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),@"Microsoft SQL Server\160\LocalDB\Binn\SqlLocalDB.exe");
        if(!File.Exists(tool)) throw new Exception("SQL Server 2022 LocalDB bulunamadi. Kurulum paketini yeniden calistirin veya prerequisites klasorundeki SqlLocalDB.msi dosyasini kurun.");
        Run(tool,"create "+Instance+" 16.0"); Run(tool,"start "+Instance);
        using(var c=new SqlConnection(ConnectionString.Replace("Initial Catalog="+Database,"Initial Catalog=master"))) {
            c.Open(); using(var q=c.CreateCommand()) {
                q.CommandText="SELECT DB_ID(@db)"; q.Parameters.AddWithValue("@db",Database);
                if(q.ExecuteScalar()==DBNull.Value) {
                    q.Parameters.Clear();
                    string mdf=Path.Combine(DataDirectory,Database+".mdf"), ldf=Path.Combine(DataDirectory,Database+"_log.ldf");
                    q.CommandText=File.Exists(mdf) ? "CREATE DATABASE ["+Database+"] ON (FILENAME=N'"+mdf.Replace("'","''")+"') FOR ATTACH" :
                        "CREATE DATABASE ["+Database+"] ON PRIMARY (NAME=N'"+Database+"',FILENAME=N'"+mdf.Replace("'","''")+"') LOG ON (NAME=N'"+Database+"_log',FILENAME=N'"+ldf.Replace("'","''")+"')";
                    q.CommandTimeout=120; q.ExecuteNonQuery();
                }
            }
        }
        using(var c=new SqlConnection(ConnectionString)) {
            c.Open(); using(var q=c.CreateCommand()) {
                q.CommandText="SELECT OBJECT_ID('dbo.__EKSetup')";
                if(q.ExecuteScalar()==DBNull.Value) {
                    using(var tx=c.BeginTransaction()) { q.Transaction=tx;
                        q.CommandText=File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"schema.sql"))+"\n__SEED__\nCREATE TABLE dbo.__EKSetup(Version int NOT NULL); INSERT dbo.__EKSetup VALUES(1);";
                        q.CommandTimeout=120; q.ExecuteNonQuery(); tx.Commit(); q.Transaction=null;
                    }
                }
                if(smoke) {
                    using(var tx=c.BeginTransaction()) { q.Transaction=tx;
                        q.CommandText="__INSERT_ADMIN__"; q.Parameters.AddWithValue("@u","ci_probe"); q.Parameters.AddWithValue("@p","Probe-12345"); q.ExecuteNonQuery();
                        q.Parameters.Clear(); q.CommandText="SELECT COUNT(*) FROM __ADMIN_TABLE__ WHERE __USER_COL__='ci_probe'";
                        if(Convert.ToInt32(q.ExecuteScalar())!=1) throw new Exception("CRUD insert verification failed");
                        q.CommandText="UPDATE __ADMIN_TABLE__ SET __PASS_COL__='Probe-54321' WHERE __USER_COL__='ci_probe'"; if(q.ExecuteNonQuery()!=1) throw new Exception("CRUD update failed");
                        q.CommandText="DELETE FROM __ADMIN_TABLE__ WHERE __USER_COL__='ci_probe'"; if(q.ExecuteNonQuery()!=1) throw new Exception("CRUD delete failed");
                        tx.Rollback(); q.Transaction=null;
                    }
                    File.WriteAllText(Path.Combine(DataDirectory,"smoke.txt"),"PASS: schema, seed, parameterized admin insert, read, update, delete, rollback.");
                    return;
                }
                q.CommandText="SELECT COUNT(*) FROM __ADMIN_TABLE__"; if(Convert.ToInt32(q.ExecuteScalar())==0) CreateAdmin(c);
            }
        }
    }
    static void CreateAdmin(SqlConnection c) {
        using(var f=new Form { Text="Ilk kurulum - Yonetici hesabi",ClientSize=new Size(400,210),StartPosition=FormStartPosition.CenterScreen,FormBorderStyle=FormBorderStyle.FixedDialog,MaximizeBox=false,MinimizeBox=false }) {
            var u=new TextBox {Left=150,Top=35,Width=220,MaxLength=20};
            var p=new TextBox {Left=150,Top=80,Width=220,MaxLength=20,UseSystemPasswordChar=true};
            var b=new Button {Text="Hesabi olustur",Left=150,Top=135,Width=220};
            f.Controls.AddRange(new Control[]{new Label{Text="Kullanici / Sicil no",Left=15,Top=38,Width=130},u,new Label{Text="Sifre (8-20 karakter)",Left=15,Top=83,Width=135},p,b}); f.AcceptButton=b;
            b.Click+=(s,e)=> { if(u.Text.Trim().Length==0||p.Text.Length<8) {MessageBox.Show("Kullanici adi ve en az 8 karakterli sifre girin.");return;}
                using(var q=c.CreateCommand()){q.CommandText="__INSERT_ADMIN__";q.Parameters.AddWithValue("@u",u.Text.Trim());q.Parameters.AddWithValue("@p",p.Text);q.ExecuteNonQuery();}
                f.DialogResult=DialogResult.OK;f.Close();
            };
            if(f.ShowDialog()!=DialogResult.OK) throw new OperationCanceledException("Ilk kurulum iptal edildi.");
        }
    }
    internal static void Error(Exception ex,bool smoke) {
        Directory.CreateDirectory(DataDirectory); File.AppendAllText(Path.Combine(DataDirectory,"error.log"),DateTime.Now.ToString("s")+" "+ex+Environment.NewLine);
        if(!smoke) MessageBox.Show(ex.Message+"\n\nAyrinti: "+Path.Combine(DataDirectory,"error.log"),"Uygulama baslatilamadi",MessageBoxButtons.OK,MessageBoxIcon.Error);
    }
}
}
