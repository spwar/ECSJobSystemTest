using System;
using UnityEngine;

namespace TerrainGeneration
{
    [Serializable]
    public struct TerrainSettings
    {
        public Rect Rect;
        public int Resolution;
        public int ColliderResolution;
        public int Harmonics;
        public float Amplitude;
        public float Scale;
    }
}