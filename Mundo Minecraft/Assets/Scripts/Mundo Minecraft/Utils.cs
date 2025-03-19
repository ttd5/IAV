using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    static float smooth = 0.005f;
    static int maxHeigth = 40;
    static int octaves = 6;
    static float persistence = 0.7f;

    public static int GenerateHeight(float x, float z) 
    {
        return (int)Map(0, maxHeigth, 0, 1, fBM(x * smooth, z * smooth, octaves, persistence));
    }

    public static int GenerateStoneHeight(float x, float z) 
    {
        return (int)Map(0, maxHeigth - 2, 0, 1, fBM(x * 10 * smooth, z * 2 * smooth, octaves - 1, 1.2f * persistence));
    }

    static float Map(float newmin, float newmax, float orimin, float orimax, float val)
    {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(orimin, orimax, val));
    }

    static float fBM(float x, float z, int octaves, float persistence) 
    {
        float total = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;
        for(int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
            amplitude *= persistence;
            frequency *= 2;
            maxValue += amplitude;
        }
        return total / maxValue;
    }
}
