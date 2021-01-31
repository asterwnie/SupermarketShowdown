using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
    // simple perlin
    public static float GetPerlinValueFromPos(int x, int z, float scale)
    {
        float sampleX = ((float)x) / scale;
        float sampleZ = ((float)z) / scale;
        float perlinSample = Mathf.PerlinNoise(sampleX, sampleZ);
        return perlinSample;
    }

    public static float GetCurvedPerlinValueFromPos(int x, int z, int maxX, int maxZ, float scale)
    {
        //calculate the perlin value for that point
        float sampleX = ((float)x) / scale;
        float sampleZ = ((float)z) / scale;

        float perlinSample = Mathf.PerlinNoise(sampleX, sampleZ);

        // curve it along a cos curve
        //-(cos(x*pi))/2 + 1 (x is a value from 0 to 1, based on how far along x is
        float cosCurveX = (-1f * Mathf.Cos( ((float)x / (float)maxX) * Mathf.PI * 2f) / 2f) + 0.5f;
        float cosCurveZ = (-1f * Mathf.Cos( ((float)z / (float)maxZ) * Mathf.PI * 2f) / 2f) + 0.5f;

        return perlinSample * cosCurveX * cosCurveZ;
    }
}
