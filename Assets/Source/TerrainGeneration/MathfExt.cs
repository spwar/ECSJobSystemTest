using UnityEngine;

namespace TerrainGeneration
{
    public static class MathfExt
    {
        public static float PerlinNoiseHarmonic(float x, float z, float time, int harmonics)
        {
            var result = 0f;
            for (var i = 1; i <= harmonics; i++)
            {
                var harmonicCoefficient = 1f / Mathf.Pow(2, i);
                result += harmonicCoefficient * Mathf.PerlinNoise(x / harmonicCoefficient + time * harmonicCoefficient,
                              z / harmonicCoefficient);
            }

            return result;
        }
    }
}