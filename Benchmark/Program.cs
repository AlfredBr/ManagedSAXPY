using BenchmarkDotNet.Running;

namespace SaxpyBenchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        //BenchmarkRunner.Run<Md5VsSha256>();
        BenchmarkRunner.Run<SaxpyTests>();
    }
}
