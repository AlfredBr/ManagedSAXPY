#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>

extern "C"
{
    // CUDA device code
    __global__ void Saxpy(int N, float A, float* S, const float* X, const float* Y)
    {
        int i = blockDim.x * blockIdx.x + threadIdx.x;
        if (i < N)
        {
            S[i] = A * X[i] + Y[i];
        }
    }
}