#pragma once

#include <vector>
#include <stdexcept>

using namespace System;
using namespace System::Collections::Generic;

namespace CppClassLibrary1
{
    public ref class CppClass1
    {
    public:
        static array<float>^ Saxpy1(int N, float a, array<float>^ x, array<float>^ y)
        {
            // Check input arguments
            if (x->Length != N || y->Length != N)
            {
                throw gcnew ArgumentException("Size of x and y must be equal to N");
            }

            // Create space for vectors
            std::vector<float> xVec(N);
            std::vector<float> yVec(N);

            // Copy array to vector
            for (int i = 0; i < N; ++i)
            {
                xVec[i] = x[i];
                yVec[i] = y[i];
            }

            // Call native kernel implementation
            SaxpyImpl(N, a, xVec, yVec);

            // Copy vector to array
            for (int i = 0; i < N; ++i)
            {
                y[i] = yVec[i];
            }

            // Return managed array
            return y;
        }

        static array<float>^ Saxpy2(int N, float a, array<float>^ x, array<float>^ y)
        {
            // Check input arguments
            if (x->Length != N || y->Length != N)
            {
                throw gcnew ArgumentException("Size of x and y must be equal to N");
            }

            // Perform SAXPY operation
            for (int i = 0; i < N; ++i)
            {
                y[i] = a * x[i] + y[i];
            }

            // Return managed array
            return y;
        }

    private:
        static std::vector<float>& SaxpyImpl(int N, float a, const std::vector<float>& x, std::vector<float>& y)
        {
            // Check input arguments
            if (x.size() != N || y.size() != N)
            {
                throw std::invalid_argument("Size of x and y must be equal to N");
            }

            // Perform SAXPY operation
            for (int i = 0; i < N; ++i)
            {
                y[i] = a * x[i] + y[i];
            }

            // Return reference to y
            return y;
        }
    };
}
