namespace Test_Library;

public static class Linq
{
	public static float[] Saxpy(int N, float a, float[] x, float[] y)
	{
		return x.Select((x, i) => (a * x) + y[i]).ToArray();
	}
}
