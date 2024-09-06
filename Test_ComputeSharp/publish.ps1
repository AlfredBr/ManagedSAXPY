dotnet clean
Remove-Item -Recurse -Force ./publish
Remove-Item -Recurse -Force ./bin
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -o ./publish
Get-ChildItem ./publish
./publish/Test.ComputeSharp.exe