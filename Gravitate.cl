float2 expandSpaceByReflection(float2 pos, float2 area)
{
    float x = pos.x;
    float y = pos.y;

    if (x < 0)
    {
        while (x < 0)
        {
            x += area.x;
        }
    }
    else if (x > area.x)
    {
        while (x > area.x)
        {
            x -= area.x;
        }
    }

    if (y < 0)
    {
        while (y < 0)
        {
            y += area.y;
        }
    }
    else if (y > area.y)
    {
        while (y > area.y)
        {
            y -= area.y;
        }
    }

    return (float2)(x, y);
}

bool onTheScreen(float2 pos, float2 area)
{
    return !(pos.x <= 0
        || pos.y <= 0
        || pos.x >= area.x
        || pos.y >= area.y);
}

__kernel void gravitate(
    __global float *posArg,
    __global float *accArg,
    __global const float *maps,
    __global const float *props)
{
    int mapIndex = get_global_id(0) * 4;
    int i = (int)maps[mapIndex + 0] * 6;
    int j = (int)maps[mapIndex + 1] * 6;
    float force = maps[mapIndex + 2];
    float areaOfInfluence = maps[mapIndex + 3];
    float2 area = (float2)(2, 2);
    float attenuation = props[2];

    float2 srcPos = (float2)(posArg[i], posArg[i + 1]);
    float2 dstPos = (float2)(posArg[j], posArg[j + 1]);

    float widthThreshold = area.x - areaOfInfluence;
    float heightThreshold = area.y - areaOfInfluence;

    float2 difference = dstPos - srcPos;
    if (difference.x >= widthThreshold)
        difference.x -= area.x;
    else if (difference.x <= -widthThreshold)
        difference.x += area.x;

    if (difference.y >= heightThreshold)
        difference.y -= area.y;
    else if (difference.y <= -heightThreshold)
        difference.y += area.y;

    float distance = length(difference);
    if (distance > 0 && distance <= areaOfInfluence)
    {
        float2 summaryForce = (force / distance * difference);

        float2 srcAcc = (float2)(accArg[i], accArg[i + 1]);
        float2 tmpAcc = (srcAcc + summaryForce) * attenuation;
        float2 tmpPos = (srcPos + srcAcc);

        if (!onTheScreen(tmpPos, area))
        {
            tmpPos = expandSpaceByReflection(tmpPos, area);
        }

        posArg[i] = tmpPos.x;
        posArg[i + 1] = tmpPos.y;

        accArg[i] = tmpAcc.x;
        accArg[i + 1] = tmpAcc.y;
    }
}