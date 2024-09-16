float2 GetUV(int tileCount, int tileNumber, float2 uv)
{
    tileNumber++;
    return float2(uv.x + ((1 / tileCount) * tileNumber), uv.y);
}

fixed GetMultiplier(int tileCount, int tileNumber, float2 uv)
{
    return step(step((1.0 / tileCount) * (tileNumber + 1), uv.x), 0) * step((1.0 / tileCount) * tileNumber, uv.x);
}