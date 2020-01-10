using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TerrainGeneration
{
    public struct VertexJob : IJobParallelFor
    {
        public const int Harmonics = 5;
        public int resolution;
        public NativeArray<Vector3> vertices;
        public Rect rect;
        public float time;
        public float amplitude;
        public float scale;

        public void Execute(int index)
        {
            float step = rect.width / (resolution - 1);
            var tc = 0;
            var vc = 0;
            //for (int i = 0; i < resolution; i++)
            int i = index / resolution;
            {
                var ishift = i * resolution;
                //for (int j = 0; j < resolution; j++)
                int j = index % resolution; 
                {
                    var x = i * step;
                    var z = j * step;
                    var y = amplitude * MathfExt.PerlinNoiseHarmonic(x * scale, z * scale, time, Harmonics);
                    vertices[ishift + j] = new Vector3() {x = x, y = y, z = z};
                }
            }
        }
    }
}