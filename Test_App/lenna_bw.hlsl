// ================================================
//                  AUTO GENERATED
// ================================================
// This shader was created by ComputeSharp.
// See: https://github.com/Sergio0694/ComputeSharp.

#define __GroupSize__get_X 1
#define __GroupSize__get_Y 1
#define __GroupSize__get_Z 1

cbuffer _ : register(b0)
{
    uint __x;
    uint __y;
    uint __z;
}

RWTexture2D<unorm float4> __reserved__texture : register(u0);

static int __get_Dimension1(RWTexture2D<unorm float4> resource);

static int __get_Dimension0(RWTexture2D<unorm float4> resource);

static int __get_Dimension1(RWTexture2D<unorm float4> resource)
{
    uint _0, _1;
    resource.GetDimensions(_0, _1);
    return _1;
}

static int __get_Dimension0(RWTexture2D<unorm float4> resource)
{
    uint _0, _1;
    resource.GetDimensions(_0, _1);
    return _0;
}

[NumThreads(__GroupSize__get_X, __GroupSize__get_Y, __GroupSize__get_Z)]
void Execute(uint3 ThreadIds : SV_DispatchThreadID)
{
    if (ThreadIds.x < __x && ThreadIds.y < __y && ThreadIds.z < __z)
    {
        int radius = __get_Dimension1(__reserved__texture) / 5;
        int midY = __get_Dimension1(__reserved__texture) / 2;
        int midX = __get_Dimension0(__reserved__texture) / 2;
        int x = ThreadIds.x;
        int y = ThreadIds.y;
        double tx = pow(x - midX, 2);
        double ty = pow(y - midY, 2);
        bool outCircle = (tx + ty) > pow(radius, 2);
        if (outCircle)
        {
            float3 rgb = __reserved__texture[ThreadIds.xy].rgb;
            float avg = dot(rgb, float3(0.0722, 0.7152, 0.2126));
            __reserved__texture[ThreadIds.xy].rgb = avg;
        }
    }
}