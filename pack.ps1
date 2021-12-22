#进入脚本所在目录
$x = Split-Path -Parent $MyInvocation.MyCommand.Definition
cd $x 

# if($build){
    # dotnet build --configuration Release --framework net6.0
	dotnet build --configuration Release --framework net5.0
	# dotnet build --configuration Release --framework netcoreapp3.1
# }
$Version="xx"
Get-ChildItem ./src/ -recurse *.nupkg | Remove-Item
dotnet pack -c Release --no-restore --no-build --include-symbols --include-source -p:PackageVersion=$Version
$pkgs = Get-ChildItem ./src/ -recurse *.nupkg
foreach($pkg in $pkgs){
      dotnet nuget push $pkg.FullName -n true --skip-duplicate
}
