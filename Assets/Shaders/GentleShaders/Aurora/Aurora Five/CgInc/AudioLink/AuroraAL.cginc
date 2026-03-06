    /*

    Stripped down AudioLink CG Include

    AudioLink by llealloo, cnlohr, lox9973, pema99, and float3
    Thank you!
    https://github.com/llealloo/audiolink

    Copyright 2024 llealloo, cnlohr, lox9973, pema99, float3

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
    and associated documentation files (the "Software"), to deal in the Software without restriction, 
    including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
    and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial 
    portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
    LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

    You may freely use this code in any CC 3.0+ licensed projects.

    */
    
    
    #ifndef AUDIOLINK_CGINC_INCLUDED
        #define AUDIOLINK_CGINC_INCLUDED
    #endif
    
    #define ALPASS_AUDIOLINK                uint2(0,0)  //Size: 128, 4
    #define ALPASS_GENERALVU_INSTANCE_TIME  uint2(2,22)
    #define ALPASS_THEME_COLOR0             uint2(0,23)
    #define ALPASS_THEME_COLOR1             uint2(1,23)
    #define ALPASS_THEME_COLOR2             uint2(2,23)
    #define ALPASS_THEME_COLOR3             uint2(3,23)
    #define ALPASS_CHRONOTENSITY            uint2(16,28) //Size: 8, 4

    uniform float4              _AudioTexture_TexelSize;

    #ifdef SHADER_TARGET_SURFACE_ANALYSIS
        #define AUDIOLINK_STANDARD_INDEXING
    #endif

    // Mechanism to index into texture.
    #ifdef AUDIOLINK_STANDARD_INDEXING
        sampler2D _AudioTexture;
        #define AudioLinkData(xycoord) tex2Dlod(_AudioTexture, float4(uint2(xycoord) * _AudioTexture_TexelSize.xy, 0, 0))
    #else
        uniform Texture2D<float4>   _AudioTexture;
        #define AudioLinkData(xycoord) _AudioTexture[uint2(xycoord)]
    #endif
    
    // Extra utility functions for time.
    uint AudioLinkDecodeDataAsUInt(uint2 indexloc)
    {
        uint4 rpx = AudioLinkData(indexloc);
        return rpx.x + rpx.y*1024 + rpx.z * 1048576 + rpx.w * 1073741824;
    }

    // Get a reasonable drop-in replacement time value for _Time.y with the
    // given chronotensity index [0; 7] and AudioLink band [0; 3].
    float AudioLinkGetChronoTime(uint index, uint band)
    {
        return (AudioLinkDecodeDataAsUInt(ALPASS_CHRONOTENSITY + uint2(index, band))) / 100000.0;
    }