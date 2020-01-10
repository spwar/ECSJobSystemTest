using System.Threading.Tasks;
using UnityEngine;

namespace TerrainGeneration
{
    public class VertexTask
    {
        public const int Harmonics = 5;
        public int resolution;
        public Vector3[] vertices;
        public Rect rect;
        public float time;
        public float amplitude;
        public float scale;
        public Task task;
        public int startIndex;
        public int endIndex;

        public bool Execute()
        {
            float step = rect.width / (resolution - 1);
            var vc = 0;
            for (int index = startIndex; index < endIndex; index++)
            {
                int i = index / resolution;
                int j = index % resolution;
                {
                    var x = i * step;
                    var z = j * step;
                    var y = amplitude * MathfExt.PerlinNoiseHarmonic(x * scale, z * scale, time, Harmonics);
                    vertices[vc++] = new Vector3() {x = x, y = y, z = z};
                }
            }
            return true;
        }
    }
}