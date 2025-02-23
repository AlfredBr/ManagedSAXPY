# What is SAXPY?
![SAXPY Image](https://developer-blogs.nvidia.com/wp-content/uploads/2021/03/SAXPY.png)

What is SAXPY?  Single precision [A * X + Y.](https://en.wikipedia.org/wiki/Basic_Linear_Algebra_Subprograms#Level_1)  We do this [all the time.](https://en.wikipedia.org/wiki/Multiply%E2%80%93accumulate_operation)  There is even [hardware support for it](https://en.wikipedia.org/wiki/FMA_instruction_set).

Here are some great articles from the folks at nVidia.
- [N Ways to SAXPY: Demonstrating the Breadth of GPU Programming Options](https://developer.nvidia.com/blog/n-ways-to-saxpy-demonstrating-the-breadth-of-gpu-programming-options/)
- [Six Ways to SAXPY](https://developer.nvidia.com/blog/six-ways-saxpy/)

## You can SAXPY quickly via Managed C# GPU Programming
I want to add to this discussion and this repo demonstrates a few more ways to SAXPY in C# / .NET / Managed Code.

### Libraries / Frameworks
- [ILGPU](https://ilgpu.net/)
- [ComputeSharp](https://github.com/Sergio0694/ComputeSharp)
- [ManagedCuda](https://github.com/kunzmi/managedCuda)
- [MathNet.Numerics](https://numerics.mathdotnet.com/)
### Docs
- [ILGPU](https://ilgpu.net/docs/)
- [ComputeSharp WIKI](https://github.com/Sergio0694/ComputeSharp/wiki/3.-Getting-started-%F0%9F%93%96)
- [HLSL Wiki](https://en.wikibooks.org/wiki/Cg_Programming/Unity/Compute_Shaders)
- [HLSL @ WikiPedia](https://en.wikipedia.org/wiki/High-Level_Shader_Language)
- [HLSL @ Microsoft Learn](https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl)
### Articles
- [HackerNews](https://news.ycombinator.com/item?id=40393873)
- [CUDA is a giant MOAT](https://weightythoughts.com/p/cuda-is-still-a-giant-moat-for-nvidia)
- [GPUs for Deep Learning](https://timdettmers.com/2023/01/30/which-gpu-for-deep-learning/comment-page-1/)
### Tutorials
- [GPU Puzzles & Tutorial](https://www.youtube.com/watch?v=K4T-YwsOxrM)
- [WebGPU Fundamentals](https://webgpufundamentals.org/webgpu/lessons/webgpu-fundamentals.html)
