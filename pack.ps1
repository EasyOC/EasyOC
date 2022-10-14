#进入脚本所在目录
$x = Split-Path -Parent $MyInvocation.MyCommand.Definition
cd $x 

# if($build){
	dotnet build --configuration Release --framework net6.0
# }
$Version="0.0.1-Preview"
Get-ChildItem ./src/ -recurse *.nupkg | Remove-Item
dotnet pack -c Release --no-restore --no-build --include-symbols --include-source -p:PackageVersion=$Version
$pkgs = Get-ChildItem ./src/ -recurse *.nupkg
foreach($pkg in $pkgs){
     if($pkg.FullName - contanins 'EasyOC.'){
	dotnet nuget push $pkg.FullName -n true --skip-duplicate
   }
      
}
