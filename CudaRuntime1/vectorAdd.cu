#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>

extern "C"
{
    // CUDA device code
    __global__ void VectorAdd(const int* A, const int* B, int* C, int N)
    {
        int i = blockDim.x * blockIdx.x + threadIdx.x;
        if (i < N)
        {
            C[i] = A[i] + B[i];
        }
    }
}