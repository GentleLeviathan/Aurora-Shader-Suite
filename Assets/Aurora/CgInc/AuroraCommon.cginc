struct AuroraOutput
{
    fixed3 Diffuse;
    fixed3 Normal;
    fixed3 Emission;
    fixed Occlusion;
    fixed Roughness;
    fixed Metal;
};

//Packs two float values into a single float, with 4 bits per float
float pack_floats_4bpf(float v1, float v2)
{
    float a = round(v1 * 15.0);
    float b = round(v2 * 15.0);

    float2 bitShiftVector = float2(1.0 / ( 255.0/16.0 ), 1.0 / 255.0);

    return dot(float2(a, b), bitShiftVector);
}

//Unpacks a packed float into a float2
float2 unpack_floats_4bpf(float input)
{
    float temp = input * 15.9375;

    float a = floor(temp) / 15.0;
    float b = frac(temp) * 1.0667;

    return float2(a, b);
}

//Thanks to RetroGEO - https://github.com/RetroGEO/reroStandard
half3 getVertexLightsDir(float3 worldPos)
{
    half3 toLightX = half3(unity_4LightPosX0.x, unity_4LightPosY0.x, unity_4LightPosZ0.x);
    half3 toLightY = half3(unity_4LightPosX0.y, unity_4LightPosY0.y, unity_4LightPosZ0.y);
    half3 toLightZ = half3(unity_4LightPosX0.z, unity_4LightPosY0.z, unity_4LightPosZ0.z);
    half3 toLightW = half3(unity_4LightPosX0.w, unity_4LightPosY0.w, unity_4LightPosZ0.w);

    half3 dirX = toLightX - worldPos;
    half3 dirY = toLightY - worldPos;
    half3 dirZ = toLightZ - worldPos;
    half3 dirW = toLightW - worldPos;

    dirX *= length(toLightX);
    dirY *= length(toLightY);
    dirZ *= length(toLightZ);
    dirW *= length(toLightW);

    half3 dir = (dirX + dirY + dirZ + dirW);
    return normalize(dir);
}