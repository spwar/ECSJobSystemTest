using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace TerrainGeneration
{
    public class TerrainGeneratorThreads : ITerrainGenerator, IDisposable
    {
        private readonly TerrainSettings terrainSettings;

        private Dictionary<Mesh, bool> meshGenerationLock;
        private Dictionary<Mesh, List<VertexTask>> meshTasks;
        private Dictionary<Mesh, Vector3[]> meshVertices;

        public TerrainSettings TerrainSettings => terrainSettings;

        public TerrainGeneratorThreads(TerrainSettings terrainSettings)
        {
            this.terrainSettings = terrainSettings;
            meshGenerationLock = new Dictionary<Mesh, bool>();
            meshTasks = new Dictionary<Mesh, List<VertexTask>>();
            meshVertices = new Dictionary<Mesh, Vector3[]>();
        }
        
        public Vector3 GetTerrainPosition(Vector3 position)
        {
            var time = Time.time;
            var y = terrainSettings.Amplitude * MathfExt.PerlinNoiseHarmonic(position.x * terrainSettings.Scale, position.z * terrainSettings.Scale, time, 5);
            position.y = y;
            return position;
        }

        public void GenerateMesh(MeshFilter meshFilter, MeshCollider meshCollider)
        {
            GenerateMesh(meshFilter.sharedMesh, terrainSettings.Resolution);
            GenerateMesh(meshCollider.sharedMesh, terrainSettings.ColliderResolution);
        }

        private async void GenerateMesh( Mesh mesh, int resolution)
        {
            if (meshGenerationLock.ContainsKey(mesh) && meshGenerationLock[mesh])
            {
                //return;
            }

            meshGenerationLock[mesh] = true;

            var vertexCount = (resolution + 1) * (resolution + 1);
            var triangleCount = resolution * resolution * 6;
            bool rebuildMesh = false;
            float time = Time.time;
            if (mesh.vertexCount != vertexCount)
            {
                meshTasks[mesh] = new List<VertexTask>();
                meshVertices[mesh] = new Vector3[vertexCount];

                rebuildMesh = true;
                
                //Setting mesh tasks
                int range = vertexCount;
                int chunkSize = vertexCount / 4;
                
                for (int i = 0; i <= range - chunkSize; i += chunkSize)
                {
                    var vertexTask = new VertexTask();
                    vertexTask.resolution = resolution;
                    vertexTask.rect = terrainSettings.Rect;
                    vertexTask.time = time;
                    vertexTask.amplitude = terrainSettings.Amplitude;
                    vertexTask.scale = terrainSettings.Scale;
                    vertexTask.resolution = resolution;
                    vertexTask.startIndex = i;
                    vertexTask.endIndex = math.min(i + chunkSize, range);
                    int length = vertexTask.endIndex - vertexTask.startIndex;
                    var vertexChunk = new Vector3[length];
                    vertexTask.vertices = vertexChunk;
                    meshTasks[mesh].Add(vertexTask);
                }
            }

            foreach (var vertexTask in meshTasks[mesh])
            {
                vertexTask.time = time;
                vertexTask.task = Task.Run(() => vertexTask.Execute());
            }

            await Task.WhenAll(meshTasks[mesh].ConvertAll((vTask) => vTask.task));
            
            foreach (var task in meshTasks[mesh])
            {
                Array.Copy(task.vertices, 0,  meshVertices[mesh], task.startIndex, task.endIndex - task.startIndex);
            }

            //LogMesh(mesh)

            if (mesh == null)
            {
                return;
            }
            
            mesh.vertices = meshVertices[mesh];

            if (rebuildMesh)
            {
                var triangles = new int[triangleCount];
                var tc = 0;
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
                mesh.triangles = triangles;
                mesh.RecalculateBounds();
            }
            mesh.RecalculateNormals();
            meshGenerationLock[mesh] = false;
        }
        
        public void Dispose()
        {
            
        }

        private void LogMesh(Mesh mesh)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var vertex in meshVertices[mesh])
            {
                builder.Append($" {vertex}");
            }
            Debug.Log(builder.ToString());
        }

        void OnDestroy()
        {
            
        }
    }
}