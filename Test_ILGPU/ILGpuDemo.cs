using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;

using Throw;

namespace Test_ILGPU;

public static partial class ILGpuDemo
{
    static ILGpuDemo()
    {
        //Console.WriteLine("ILGPUDemo()");
    }

    public static string GetInfoString(Accelerator a)
    {
        var infoString = new StringWriter();
        a.PrintInformation(infoString);
        return infoString.ToString();
    }

    public static void Tutorial0()
    {
        //using Context context = Context.Create(builder => builder.AllAccelerators());
        using Context context = Context.CreateDefault();

        // foreach (Device device in context)
        // {
        // 	Console.WriteLine(device);
        // }

        // foreach (Device d in context)
        // {
        // 	using Accelerator accelerator = d.CreateAccelerator(context);
        // 	Console.WriteLine(accelerator);
        // 	Console.WriteLine(GetInfoString(accelerator));
        // }

        // Prints all CPU accelerators.
        // foreach (CPUDevice d in context.GetCPUDevices())
        // {
        // 	using CPUAccelerator accelerator = (CPUAccelerator)d.CreateAccelerator(context);
        // 	Console.WriteLine(accelerator);
        // 	Console.WriteLine(GetInfoString(accelerator));
        // }

        // Prints all Cuda accelerators.
        foreach (Device device in context.GetCudaDevices())
        {
            using Accelerator accelerator = device.CreateAccelerator(context);
            Console.WriteLine(accelerator);
            Console.WriteLine(GetInfoString(accelerator));
        }

        // Print the preferred accelerator.
        // Console.WriteLine("Preferred Accelerator:");
        // foreach (Device d in context.GetPreferredDevices(preferCPU: false, matchingDevicesOnly: true))
        // {
        // 	using Accelerator accelerator = d.CreateAccelerator(context);
        // 	Console.WriteLine(accelerator);
        // 	Console.WriteLine(GetInfoString(accelerator));
        // }
    }

    public static void Tutorial1()
    {
        Console.WriteLine("Tutorial1");

        // We still need the Context and Accelerator boiler plate.
        using Context context = Context.CreateDefault();
        using Accelerator accelerator = context.CreateCPUAccelerator(0);

        // Creates an array of 1000 doubles on host.
        var doubles = new double[1000];

        // Creates a MemoryBuffer on device with same size and contents as doubles.
        MemoryBuffer1D<double, Stride1D.Dense> doublesOnDevice = accelerator.Allocate1D(doubles);

        // What if we change the doubles on the host and need to update the device side memory?
        for (var i = 0; i < doubles.Length; i++)
        {
            doubles[i] = i * Math.PI;
        }

        // We call MemoryBuffer.CopyFrom which copies any linear slice of doubles into the device side memory.
        doublesOnDevice.CopyFromCPU(doubles);

        // What if we change the doublesOnDevice and need to write that data into host memory?
        doublesOnDevice.CopyToCPU(doubles);

        // You can copy data to and from MemoryBuffers into any array / span / memorybuffer that allocates the same
        // type. for example:
        var doubles2 = new double[doublesOnDevice.Length];
        doublesOnDevice.CopyFromCPU(doubles2);

        // There are also helper functions, but be aware of what a function does.
        // As an example this function is shorthand for the above two lines.
        // This completely allocates a new double[] on the host. This is slow.
        var doubles3 = doublesOnDevice.GetAsArray1D();

        // Notice that you cannot access memory in a MemoryBuffer or an ArrayView from host code.
        // If you uncomment the following lines they should crash.
        // doublesOnDevice[1] = 0;
        // double d = doublesOnDevice[1];

        // There is not much we can show with ArrayViews currently, but in the
        // Kernels Tutorial it will go over much more.
        ArrayView1D<double, Stride1D.Dense> doublesArrayView = doublesOnDevice.View;

        // do not forget to dispose of everything in the reverse order you constructed it.
        doublesOnDevice.Dispose();
        // note the doublesArrayView is now invalid, but does not need to be disposed.
        accelerator.Dispose();
        // dispose of the context last.
        context.Dispose();
    }

    public static void Tutorial2()
    {
        static void Kernel(Index1D i, ArrayView<double> data, ArrayView<double> output)
        {
            output[i] = data[i % data.Length] * 3.14d * XMath.Sin(i % 90);
        }

        // create a context with all accelerators and enable all algorithms.
        using Context context = Context.Create(builder =>
            builder.Default()
                   //.Math(MathMode.Fast)
                   .EnableAlgorithms());
        context.ThrowIfNull();
        context.Devices.Length.Throw().IfEquals(0);

        // create a Cuda accelerator
        using Accelerator accelerator = context.CreateCudaAccelerator(0);
        accelerator.ThrowIfNull();

        Console.WriteLine($"{accelerator.AcceleratorType}: {accelerator.Name}");

        // load the data
        using MemoryBuffer1D<double, Stride1D.Dense> deviceData = accelerator.Allocate1D(new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        using MemoryBuffer1D<double, Stride1D.Dense> deviceOutput = accelerator.Allocate1D<double>(10_000);

        // load / precompile the kernel
        var loadedKernel = accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<double>, ArrayView<double>>(Kernel);

        // set the input data view to the device data
        var inputDataView = deviceData.View;

        // create a buffer to copy the output data to the host
        MemoryBuffer1D<double, Stride1D.Dense>? hostOutputCopy = null;

        try
        {
            for (var l = 0; l < 3; l++)
            {
                // finish compiling and tell the accelerator to start computing the kernel
                loadedKernel((int)deviceOutput.Length, inputDataView, deviceOutput.View);
                if (hostOutputCopy is not null)
                {
                    hostOutputCopy.Dispose();
                    hostOutputCopy = null;
                }

                // wait for the accelerator to be finished with whatever it's doing
                // in this case it just waits for the kernel to finish.
                accelerator.Synchronize();

                // moved output data from the GPU to the CPU for output to console
                var hostOutput = deviceOutput.GetAsArray1D();

                // print the first 10 values of the output
                for (var i = 0; i < deviceData.Length * 2; i++)
                {
                    Console.Write($"{hostOutput[i]:N2}");
                    Console.Write(" ");
                }

                Console.WriteLine();

                // copy the first 10 values of the output to a new buffer
                hostOutputCopy = accelerator.Allocate1D(hostOutput.Take(10).ToArray());
                // set the input data view to the new buffer
                inputDataView = hostOutputCopy.View;
            }
        }
        finally
        {
            hostOutputCopy?.Dispose();
        }
    }

    public static double[] Tutorial3()
    {
        static void Kernel(Index1D idx, ArrayView<double> data, ArrayView<double> output)
        {
            double d = data[idx % data.Length];
            double ex1(double d) => d * 3.14d;
            double ex2(double d, int i) => d * Math.Pow(Math.Sin(i % 90), 2);
            output[idx] = ex2(ex1(d), idx);
        }

        // Initialize ILGPU.
        using Context context = Context.Create(builder => builder.Default().EnableAlgorithms());
        //using Accelerator accelerator = context.CreateCudaAccelerator(0);
        using Accelerator accelerator = context.CreateCPUAccelerator(0);

        Console.WriteLine($"{accelerator.AcceleratorType}: {accelerator.Name}");

        // Load the data.
        double[] array = [.. Enumerable.Range(1, 1_000_000)];
        using MemoryBuffer1D<double, Stride1D.Dense> deviceData = accelerator.Allocate1D(array);
        using MemoryBuffer1D<double, Stride1D.Dense> deviceOutput = accelerator.Allocate1D<double>(array.Length);

        // load / precompile the kernel
        Action<Index1D, ArrayView<double>, ArrayView<double>> loadedKernel =
            accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<double>, ArrayView<double>>(Kernel);

        var inputDataView = deviceData.View;

        MemoryBuffer1D<double, Stride1D.Dense>? hostOutputCopy = null;

        try
        {
            for (int l = 0; l < 3; l++)
            {
                // finish compiling and tell the accelerator to start computing the kernel
                loadedKernel((int)deviceOutput.Length, inputDataView, deviceOutput.View);
                if (hostOutputCopy is not null)
                {
                    hostOutputCopy.Dispose();
                    hostOutputCopy = null;
                }

                // wait for the accelerator to be finished with whatever it's doing
                // in this case it just waits for the kernel to finish.
                accelerator.Synchronize();

                // moved output data from the GPU to the CPU for output to console
                double[] hostOutput = deviceOutput.GetAsArray1D();

                hostOutputCopy = accelerator.Allocate1D(hostOutput.ToArray());
                inputDataView = hostOutputCopy.View;
            }

            // copy the final output to the host
            double[] finalOutput = deviceOutput.GetAsArray1D();
            return finalOutput;

            // The accelerator will automatically clean up all memory buffers when it is disposed.

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
        finally
        {
            hostOutputCopy?.Dispose();
        }
    }

    // var sw = Stopwatch.StartNew();
    // var result = Tutorial3();
    // sw.Stop();
    // Console.WriteLine($"Time: {sw.ElapsedMilliseconds}ms");
    // Console.WriteLine(string.Join(", ", result.Take(10).Select(d => d.ToString("N2"))));

    public static void SpecializedKernelTest()
    {
        static void GenericKernel(ArrayView<int> data, int c)
        {
            var globalIndex = Grid.GlobalIndex.X;
            // Generates code that loads c (at runtime) and adds the value 2 (at runtime)
            data[globalIndex] = c + 3;
        }

        static void SpecializedKernel(ArrayView<int> data, SpecializedValue<int> c)
        {
            var globalIndex = Grid.GlobalIndex.X;
            // Generates code that has an inlined constant value
            data[globalIndex] = c + 7; // Will be specialized for every value c
        }

        using Context context = Context.Create(builder => builder.Default().EnableAlgorithms());
        using Accelerator accelerator = context.CreateCudaAccelerator(0);
        const int gridSize = 1;
        var groupSize = accelerator.MaxNumThreadsPerGroup;

        Console.WriteLine($"{accelerator.AcceleratorType}: {accelerator.Name}");

        var buffer = accelerator.Allocate1D<int>(40);

        var genericKernel = accelerator.LoadStreamKernel<ArrayView<int>, int>(GenericKernel);
        genericKernel((gridSize, groupSize), buffer.View, 40);
        Console.WriteLine(string.Join(", ", buffer.GetAsArray1D()));

        var specializedKernel = accelerator.LoadStreamKernel<ArrayView<int>, SpecializedValue<int>>(SpecializedKernel);
        specializedKernel((gridSize, groupSize), buffer.View, SpecializedValue.New(40));
        Console.WriteLine(string.Join(", ", buffer.GetAsArray1D()));
    }

    //SpecializedKernelTest();

    public static void Tutorial5(bool debug = false)
    {
        static void AddKernel(int limit, ArrayView<float> a, ArrayView<float> b, ArrayView<float> c)
        {
            var index = (Grid.IdxX * Group.DimX) + Group.IdxX;
            if (index < limit)
            {
                c[index] = a[index] + b[index];
            }
        }

        using Context context = Context.Create(builder => builder.Default().EnableAlgorithms());
        using Accelerator accelerator = debug ? context.CreateCPUAccelerator(0) : context.CreateCudaAccelerator(0);
        const int gridSize = 1;
        var groupSize = accelerator.MaxNumThreadsPerGroup;
        var warpSize = accelerator.WarpSize;

        Console.WriteLine($"{accelerator.AcceleratorType} : {accelerator.Name} : {accelerator.MaxNumThreadsPerGroup}");
        //Console.WriteLine(GetInfoString(accelerator));

        const int length = 256;
        var a = Enumerable.Range(0, length).Select(i => (float)i).ToArray();
        var b = Enumerable.Range(0, length).Select(i => (float)i).ToArray();
        var c = Enumerable.Repeat(-1.0f, length).ToArray();

        using var bufferA = accelerator.Allocate1D(a);
        using var bufferB = accelerator.Allocate1D(b);
        using var bufferC = accelerator.Allocate1D(c);

        var config = new KernelConfig(gridSize, Math.Clamp(length, warpSize, groupSize));

        var addKernel = accelerator.LoadStreamKernel<int, ArrayView<float>, ArrayView<float>, ArrayView<float>>(AddKernel);
        addKernel(config, length, bufferA.View, bufferB.View, bufferC.View);

        bufferC.CopyToCPU(c);

        Console.WriteLine(string.Join(", ", c.Take(length).Select(f => f.ToString("N2"))));
    }
}