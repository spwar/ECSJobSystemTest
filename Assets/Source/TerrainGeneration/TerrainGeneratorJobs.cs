using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TerrainGeneration
{
    public class TerrainGeneratorJobs : ITerrainGenerator, IDisposable
    {
        private readonly NativeArray<Vector3> meshVertices;
        private readonly NativeArray<Vector3> colliderVertices;
        
        private readonly TerrainSettings terrainSettings;

        private VertexJob vertexJob;

        private JobHandle vertexHandle;
        public TerrainSettings TerrainSettings => terrainSettings;

        public TerrainGeneratorJobs(TerrainSettings terrainSettings)
        {
            meshVertices = new NativeArray<Vector3>((terrainSettings.Resolution + 1) * (terrainSettings.Resolution + 1), Allocator.Persistent);
            colliderVertices = new NativeArray<Vector3>((terrainSettings.ColliderResolution + 1) * (terrainSettings.ColliderResolution + 1), Allocator.Persistent);
            this.terrainSettings = terrainSettings;
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
            GenerateMesh(meshVertices, meshFilter.sharedMesh, terrainSettings.Resolution);
            GenerateMesh(colliderVertices, meshCollider.sharedMesh, terrainSettings.ColliderResolution);
        }

        private void GenerateMesh(NativeArray<Vector3> nVertices, Mesh mesh, int resolution)
        {
            var vertexCount = (resolution + 1) * (resolution + 1);
            var triangleCount = resolution * resolution * 6;
            bool rebuildMesh = false;
            int[] triangles = null;
            if (mesh.vertexCount != vertexCount)
            {
                mesh.Clear();

                rebuildMesh = true;
            }

            int chunkSize = 5;
           
            vertexJob = new VertexJob();
            vertexJob.resolution = resolution;
            vertexJob.rect = terrainSettings.Rect;
            vertexJob.vertices = nVertices;
            vertexJob.time = Time.time;
            vertexJob.amplitude = terrainSettings.Amplitude;
            vertexJob.scale = terrainSettings.Scale;
            vertexJob.resolution = resolution;
            vertexHandle = vertexJob.Schedule(nVertices.Length, 1);
            
            JobHandle.ScheduleBatchedJobs();

            vertexHandle.Complete();
            
            mesh.vertices = nVertices.ToArray();
            
            if (rebuildMesh)
            {
                var nTriangles = new NativeArray<int>(triangleCount, Allocator.TempJob);
                TrianglesJob tJob = new TrianglesJob();
                tJob.resolution = resolution;
                tJob.triangles = nTriangles;
                var tHandle = tJob.Schedule();
                tHandle.Complete();
                mesh.triangles = tJob.triangles.ToArray();
                nTriangles.Dispose();
            }

            mesh.RecalculateNormals();
            if (rebuildMesh)
            {
                mesh.RecalculateBounds();
            }
        }
        public void Dispose()
        {
            vertexHandle.Complete();
            meshVertices.Dispose();
            colliderVertices.Dispose();
        }
    }
}