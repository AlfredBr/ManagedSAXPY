namespace Test_App;

public static class LinqDemo
{
	internal static float[] Saxpy(int N, float a, float[] x, float[] y)
	{
		return x.Select((x, i) => (a * x) + y[i]).ToArray();
	}
}
