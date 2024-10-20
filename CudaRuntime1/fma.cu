#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>

extern "C"
{
    // CUDA device code
    __global__ void Fma(int N, float A, float* S, const float* X, const float* Y)
    {
        int i = blockIdx.x * blockDim.x + threadIdx.x;
        if (i < N)
        {
            S[i] = __fmaf_rn(A, X[i], Y[i]);
        }
    }
}