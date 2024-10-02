namespace Test_Library;

public static class ParallelFor
{
    public static float[] Saxpy(int N, float a, float[] x, float[] y)
    {
        var result = new float[N];
        Parallel.For(0, N, i =>
        {
            result[i] = (a * x[i]) + y[i];
        });
        return result;
    }
}