using System;
using UnityEngine;

namespace TerrainGeneration
{
    public enum TerrainGeneratorType
    {
        TerrainGenerator,
        TerrainGeneratorJobs,
        TerrainGeneratorThreads
    }

    public interface ITerrainGenerator: IDisposable
    {
        void GenerateMesh(MeshFilter meshFilter, MeshCollider meshCollider);
        Vector3 GetTerrainPosition(Vector3 position);

        TerrainSettings TerrainSettings { get; }
    }
}