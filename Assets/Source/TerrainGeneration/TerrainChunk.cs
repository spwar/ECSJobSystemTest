using UnityEngine;

namespace TerrainGeneration
{
    struct TerrainChunk
    {
        public Rect Borders;
        public Vector3[] Vertices;
        public int[] Faces;

        public TerrainChunk(Rect borders, Vector3[] vertices, int[] faces)
        {
            Borders = borders;
            Vertices = vertices;
            Faces = faces;
        }
    }
}