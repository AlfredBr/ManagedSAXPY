# dotnet clean
dotnet build -c Release
Copy-Item E:\Test.GPU\CudaRuntime1\x64\Debug\saxpy.ptx E:\Test.GPU\Test_App\bin\Debug\net8.0-windows\saxpy.ptx
dotnet run -c Release