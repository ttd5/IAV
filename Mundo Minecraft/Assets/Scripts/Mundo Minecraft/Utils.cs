using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    static float smooth = 0.002f;
    static float smooth3D = 10f * smooth;
    static int maxHeigth = 150;
    static int octaves = 6;
    static float persistence = 0.7f;
    static float offset = 32000f;

    public static int GenerateHeight(float x, float z) 
    {
        return (int)Map(0, maxHeigth, 0, 1, fBM(x * smooth, z * smooth, octaves, persistence));
    }

    public static int GenerateStoneHeight(float x, float z) 
    {
        return (int)Map(0, maxHeigth - 30, 0, 1, fBM(x * 3 * smooth, z * 3 * smooth, octaves - 1, 1.2f * persistence));
    }

    static float Map(float newmin, float newmax, float orimin, float orimax, float val)
    {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(orimin, orimax, val));
    }

    public static float fBM3D(float x, float y, float z, int octaves, float persistence)
    {
        float xy = fBM(x * smooth3D, y * smooth, octaves, persistence);
        float yx = fBM(y * smooth3D, x * smooth, octaves, persistence);
        float xz = fBM(x * smooth3D, z * smooth, octaves, persistence);
        float zx = fBM(z * smooth3D, x * smooth, octaves, persistence);
        float yz = fBM(y * smooth3D, z * smooth, octaves, persistence);
        float zy = fBM(z * smooth3D, y * smooth, octaves, persistence);

        return (xy + yx + xz + zx + yz + zy) / 6;
    }

    static float fBM(float x, float z, int octaves, float persistence) 
    {
        float total = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;
        for(int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }
        return total / maxValue;
    }
}
