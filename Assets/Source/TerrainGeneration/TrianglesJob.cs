using Unity.Collections;
using Unity.Jobs;

namespace TerrainGeneration
{
    public struct TrianglesJob : IJob
    {
        public int resolution;
        public NativeArray<int> triangles;

        public void Execute()
        {
            var tc = 0;
            var vc = 0;
            for (int i = 0; i < resolution; i++)
            {
                var ishift = i * resolution;
                var ishiftLast = (i - 1) * resolution;
                for (int j = 0; j < resolution; j++)
                {
                    if (i > 0 && j > 0 && triangles != null)
                    {
                        triangles[tc++] = ishiftLast + j - 1;
                        triangles[tc++] = ishiftLast + j;
                        triangles[tc++] = ishift + j;
                        triangles[tc++] = ishiftLast + j - 1;
                        triangles[tc++] = ishift + j;
                        triangles[tc++] = ishift + j - 1;
                    }
                }
            }
        }
    }
}