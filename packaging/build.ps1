param([ValidateSet('Yurt','Cafe')][string]$Kind)
$ErrorActionPreference='Stop'
$root=$PWD.Path
if($Kind -eq 'Cafe') {
 $archive=Get-ChildItem -File -Filter '*.rar' | Select-Object -First 1
 & 7z x $archive.FullName -osource -y
 if($LASTEXITCODE){throw 'RAR extraction failed'}
 $project=Get-ChildItem source -Recurse -Filter '*.csproj' | Select-Object -First 1
 $bacpac=Get-ChildItem source -Recurse -Filter '*.bacpac' | Select-Object -First 1
 $app='CafeSiparis';$db='CafeData';$instance='EKCafe2022'
 $admin='[dbo].[Kullanıcı]';$user='[SicilNo]';$pass='[Sifre]'
 $insert="INSERT [dbo].[Kullanıcı] (SicilNo,Sifre,Pozisyon,Admin) VALUES (@u,@p,'Yonetici',1)"
 $seed=''
 $forms='new Form1(), new frmAdmin(), new CafeSipariş.Personel.frmPersonel()'
} else {
 $project=Get-Item 'YurtKasis/YurtKasis/YurtKasis.csproj'
 $bacpac=Get-Item 'YurtOtomasyon.bacpac'
 $app='YurtOtomasyonu';$db='YurtOtomasyon';$instance='EKYurt2022'
 $admin='[dbo].[Admin]';$user='[YoneticiAd]';$pass='[YoneticiSifre]'
 $insert='INSERT dbo.Admin (YoneticiAd,YoneticiSifre) VALUES (@u,@p)'
 $seed="INSERT dbo.Bolumler(BolumAd) VALUES ('Genel'); DECLARE @i int=101; WHILE @i<=120 BEGIN INSERT dbo.Odalar(OdaNo,OdaKapasite,OdaAktif,OdaDurum) VALUES(CONVERT(char(3),@i),'4','0',1); SET @i=@i+1; END;"
 $forms='new FrmAdminGiris(), new FrmAna(), new FrmOgrListe(), new FrmOgrKayit()'
}
$dir=$project.Directory.FullName
[xml]$proj=Get-Content -LiteralPath $project.FullName -Raw
$ns=New-Object Xml.XmlNamespaceManager($proj.NameTable);$ns.AddNamespace('m','http://schemas.microsoft.com/developer/msbuild/2003')
$namespace=$proj.SelectSingleNode('//m:RootNamespace',$ns).InnerText
$proj.SelectSingleNode('//m:AssemblyName',$ns).InnerText=$app
if($Kind -eq 'Yurt') {
 foreach($node in @($proj.SelectNodes('//m:COMReference',$ns))){[void]$node.ParentNode.RemoveChild($node)}
 foreach($f in Get-ChildItem $dir -Filter 'FrmAna*.cs') {
  $lines=Get-Content -LiteralPath $f.FullName | Where-Object {$_ -notmatch 'AxWMPLib|axWindowsMediaPlayer1'}
  Set-Content -LiteralPath $f.FullName -Value $lines -Encoding utf8BOM
 }
 $sqlFile=Join-Path $dir 'SqlBaglantim.cs'
 $text=Get-Content $sqlFile -Raw
 $text=[regex]::Replace($text,'new SqlConnection\("[^"]*"\)','new SqlConnection(Distribution.ConnectionString)')
 Set-Content $sqlFile $text -Encoding utf8BOM
 $settings=Join-Path $dir 'Properties/Settings.Designer.cs'
 $text=Get-Content $settings -Raw
 $text=$text.Replace('return ((string)(this["YurtOtomasyonConnectionString"]));','return Distribution.ConnectionString;')
 Set-Content $settings $text -Encoding utf8BOM
} else {
 $ctx=Join-Path $dir 'Model1.Context.cs'
 $text=Get-Content -LiteralPath $ctx -Raw
 $text=$text.Replace(': base("name=CafeSiparişDataBaseEntities")',': base(new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder { Metadata = "res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl", Provider = "System.Data.SqlClient", ProviderConnectionString = Distribution.ConnectionString }.ConnectionString)')
 Set-Content -LiteralPath $ctx -Value $text -Encoding utf8BOM
}
$boot=Get-Content packaging/Distribution.cs -Raw
foreach($pair in @{
 '__NS__'=$namespace;'__DB__'=$db;'__APP__'=$app;'__INSTANCE__'=$instance;
 '__ADMIN_TABLE__'=$admin;'__USER_COL__'=$user;'__PASS_COL__'=$pass;
 '__INSERT_ADMIN__'=$insert;'__SEED__'=$seed
}.GetEnumerator()) {$boot=$boot.Replace($pair.Key,$pair.Value)}
Set-Content (Join-Path $dir 'Distribution.cs') $boot -Encoding utf8BOM
$compile=$proj.CreateElement('Compile',$proj.DocumentElement.NamespaceURI);$compile.SetAttribute('Include','Distribution.cs')
$group=$proj.CreateElement('ItemGroup',$proj.DocumentElement.NamespaceURI);[void]$group.AppendChild($compile);[void]$proj.DocumentElement.AppendChild($group)
$proj.Save($project.FullName)
$login=if($Kind -eq 'Yurt'){'FrmAdminGiris'}else{'Form1'}
$program=@"
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
namespace $namespace {
internal static class Program {
[STAThread] static int Main(string[] args) {
bool smoke=args.Contains("--distribution-smoke");
Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
bool owner; using(var mutex=new Mutex(true,@"Local\$app.Distribution",out owner)) {
if(!owner) return 2;
try {
Distribution.Initialize(smoke);
if(smoke) {
foreach(Form f in new Form[] { $forms }) {
using(f) using(var timer=new System.Windows.Forms.Timer { Interval=700 }) {
timer.Tick+=(s,e)=>{timer.Stop();f.Close();};f.Shown+=(s,e)=>timer.Start(); Application.Run(f);
}}
return 0;
}
Application.Run(new $login()); return 0;
} catch(OperationCanceledException) {return 1;}
catch(Exception ex){Distribution.Error(ex,smoke);return 1;}
}}
}}
"@
Set-Content (Join-Path $dir 'Program.cs') $program -Encoding utf8BOM
$out=Join-Path $root "build/$app"
New-Item $out -ItemType Directory -Force | Out-Null
python packaging/schema.py $bacpac.FullName (Join-Path $out 'schema.sql')
if($LASTEXITCODE){throw 'Schema generation failed'}
$solution=Get-ChildItem $project.Directory.Parent.FullName -Filter '*.sln' | Select-Object -First 1
if($solution){nuget restore $solution.FullName -NonInteractive}
if($LASTEXITCODE){throw 'NuGet restore failed'}
msbuild $project.FullName /m /p:Configuration=Release "/p:OutputPath=$out/" /p:Platform=AnyCPU /verbosity:minimal
if($LASTEXITCODE){throw 'Build failed'}
Get-ChildItem $out -Recurse -Filter '*.pdb' | Remove-Item
$config=Join-Path $out "$app.exe.config"
if(Test-Path $config){
 [xml]$cfg=Get-Content -LiteralPath $config -Raw
 if($cfg.configuration.connectionStrings){$cfg.configuration.connectionStrings.RemoveAll()}
 $cfg.Save($config)
}
$prereq=New-Item (Join-Path $out 'prerequisites') -ItemType Directory -Force
Invoke-WebRequest 'https://download.microsoft.com/download/3/8/d/38de7036-2433-4207-8eae-06e247e17b25/SqlLocalDB.msi' -OutFile (Join-Path $prereq 'SqlLocalDB.msi')
$sig=Get-AuthenticodeSignature (Join-Path $prereq 'SqlLocalDB.msi')
if($sig.Status -ne 'Valid' -or $sig.SignerCertificate.Subject -notmatch 'Microsoft Corporation'){throw 'LocalDB signature verification failed'}
$localdb='C:\Program Files\Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe'
if(!(Test-Path $localdb)){
 $install=Start-Process msiexec.exe -ArgumentList ('/i "'+(Join-Path $prereq 'SqlLocalDB.msi')+'" /qn IACCEPTSQLLOCALDBLICENSETERMS=YES /norestart') -WindowStyle Hidden -Wait -PassThru
 if($install.ExitCode -notin 0,3010){throw "CI LocalDB installation failed: $($install.ExitCode)"}
}
1..2 | ForEach-Object {
 $p=Start-Process (Join-Path $out "$app.exe") -ArgumentList '--distribution-smoke' -WindowStyle Hidden -PassThru
 if(!$p.WaitForExit(120000)){ $p.Kill();throw 'Smoke timed out' }
 if($p.ExitCode -ne 0){
  Get-Content "$env:LOCALAPPDATA/EnesKaleci/$app/error.log" -ErrorAction SilentlyContinue
  throw "Database/UI smoke failed: $($p.ExitCode)"
 }
}
Get-Content "$env:LOCALAPPDATA/EnesKaleci/$app/smoke.txt"
@"
$app - Windows 10/11 x64
1. Onerilen: $app-Setup.exe kurulumunu kullanin.
2. SQL Server 2022 LocalDB yoksa kurulum sonunda Microsoft kurulum sihirbazi acilir.
3. Ilk acilista kendi yonetici kullanici adinizi ve sifrenizi belirleyin.
4. Kayitlariniz %LOCALAPPDATA%\EnesKaleci\$app klasorundedir. Guncelleme ve kaldirma verilerinizi silmez.
5. ZIP kullanirsaniz tum dosyalari ayni klasore cikarin; gerekirse prerequisites\SqlLocalDB.msi dosyasini kurun.
Gereksinimler: Windows 10 1809+ / Windows 11 x64, .NET Framework 4.7.2 veya ustu.
Cafe Excel aktarimi Microsoft Excel gerektirir.
Kapsam: Tek bilgisayar ve Windows kullanicisi icin yerel kullanim.
Paket imzasizdir; Windows guvenlik politikaniz calistirmayi engelleyebilir.
"@ | Set-Content (Join-Path $out 'KULLANIM.txt') -Encoding utf8BOM
@("Source: $env:GITHUB_SHA",'PASS: Release build; new database; CRUD rollback; seed; published forms; two launches.') | Set-Content (Join-Path $out 'DOGRULAMA.txt')
Get-ChildItem $out -File -Recurse | Sort-Object FullName | ForEach-Object {"$((Get-FileHash $_.FullName).Hash)  $([IO.Path]::GetRelativePath($out,$_.FullName))"} | Set-Content (Join-Path $out 'SHA256SUMS.txt')
$iss=@"
[Setup]
AppId=EnesKaleci.$app
AppName=$app
AppVersion=1.0.0
AppPublisher=Enes Kaleci
DefaultDirName={localappdata}\Programs\EnesKaleci\$app
DefaultGroupName=$app
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=$root\build
OutputBaseFilename=$app-Setup
Compression=lzma2
SolidCompression=yes
UninstallDisplayIcon={app}\$app.exe
[Files]
Source: "$out\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion
[Icons]
Name: "{group}\$app"; Filename: "{app}\$app.exe"
Name: "{autodesktop}\$app"; Filename: "{app}\$app.exe"
[Run]
Filename: "{sys}\msiexec.exe"; Parameters: "/i ""{app}\prerequisites\SqlLocalDB.msi"""; StatusMsg: "Microsoft SQL Server LocalDB kuruluyor..."; Flags: waituntilterminated; Check: NeedsLocalDB
[Code]
function NeedsLocalDB: Boolean;
begin
 Result := not FileExists(ExpandConstant('{commonpf64}\Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe'));
end;
"@
Set-Content build/setup.iss $iss -Encoding utf8BOM
& 'C:\Program Files (x86)\Inno Setup 6\ISCC.exe' build/setup.iss
if($LASTEXITCODE){throw 'Installer build failed'}
$installed=Join-Path $env:RUNNER_TEMP "$app-installed"
$p=Start-Process "build/$app-Setup.exe" -ArgumentList ('/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /DIR="'+$installed+'"') -WindowStyle Hidden -Wait -PassThru
if($p.ExitCode -ne 0){throw "Installer smoke failed: $($p.ExitCode)"}
$p=Start-Process (Join-Path $installed "$app.exe") -ArgumentList '--distribution-smoke' -WindowStyle Hidden -PassThru
if(!$p.WaitForExit(120000) -or $p.ExitCode -ne 0){throw 'Installed application smoke failed'}
Compress-Archive -Path "$out/*" -DestinationPath "build/$app-Windows-x64.zip" -Force
Get-ChildItem build -File | Where-Object Extension -in '.exe','.zip' | ForEach-Object {"$((Get-FileHash $_.FullName).Hash)  $($_.Name)"} | Set-Content build/DOWNLOAD_SHA256.txt
Write-Output 'PASS: Setup installation and installed EXE database/UI smoke.'
