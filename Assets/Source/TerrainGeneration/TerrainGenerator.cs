using UnityEngine;

namespace TerrainGeneration
{
    public class TerrainGenerator : ITerrainGenerator
    {
        private readonly TerrainSettings terrainSettings;

        public TerrainSettings TerrainSettings => terrainSettings;

        public TerrainGenerator(TerrainSettings terrainSettings)
        {
            this.terrainSettings = terrainSettings;
        }
        
        public Vector3 GetTerrainPosition(Vector3 position)
        {
            var time = Time.time;
            var y = terrainSettings.Amplitude * MathfExt.PerlinNoiseHarmonic(position.x * terrainSettings.Scale, position.z * terrainSettings.Scale, time, terrainSettings.Harmonics);
            position.y = y;
            return position;
        }

        public void GenerateMesh(MeshFilter meshFilter, MeshCollider meshCollider)
        {
            GenerateMesh(meshFilter.sharedMesh, terrainSettings.Resolution);
            meshCollider.sharedMesh = GenerateMesh(meshCollider.sharedMesh, terrainSettings.ColliderResolution);
            
        }

        public Mesh GenerateMesh(Mesh mesh, int resolution)
        {
            float step = terrainSettings.Rect.width / (resolution - 1);
            int vertexCount = (resolution + 1) * (resolution + 1);
            bool rebuildMesh = false;
            int[] triangles = null;
            if (mesh.vertexCount != vertexCount)
            {
                mesh.Clear();
                Debug.Log($"Mesh vertexCount:{mesh.vertexCount} vertices.Count:{mesh.vertices.Length} vertexCountNew: {vertexCount}");

                mesh.vertices = new Vector3[vertexCount];
                mesh.triangles = new int[resolution * resolution * 6];
                triangles = mesh.triangles;
                rebuildMesh = true;
            }

            var vertices = mesh.vertices;

            var tc = 0;
            var vc = 0;
            var time = Time.time;
            for (int i = 0; i < resolution; i++)
            {
                var ishift = i * resolution;
                var ishiftLast = (i - 1) * resolution;
                for (int j = 0; j < resolution; j++)
                {
                    var x = i * step;
                    var z = j * step;
                    var y = terrainSettings.Amplitude * MathfExt.PerlinNoiseHarmonic(x * terrainSettings.Scale, z * terrainSettings.Scale, time, terrainSettings.Harmonics);
                    vertices[ishift + j] = new Vector3() {x = x, y = y, z = z};
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

            mesh.vertices = vertices;
            if (triangles != null)
            {
                mesh.triangles = triangles;
            }
    
            mesh.RecalculateNormals();
            if (rebuildMesh)
            {
                mesh.RecalculateBounds();
            }

            return mesh;
        }

        public void Dispose()
        {
        }
    }
}