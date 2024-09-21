namespace Test_App;

public static class ParallelFor
{
    internal static float[] Saxpy(int N, float a, float[] x, float[] y)
    {
        var result = new float[N];
        Parallel.For(0, N, i =>
        {
            result[i] = (a * x[i]) + y[i];
        });
        return result;
    }
}