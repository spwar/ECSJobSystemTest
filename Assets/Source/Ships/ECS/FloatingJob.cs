using TerrainGeneration;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;

namespace Ships.ECS
{
    [BurstCompile]
    public struct FloatingJob : IJobParallelForTransform
    {
        public const float ShipLength = 0.2f;
        public TerrainSettings TerrainSettings;
        public float Time;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 position = transform.position;
            Vector3 forward = transform.rotation * Vector3.forward;
            forward.y = 0;
            forward.Normalize();
            forward = forward * ShipLength;
            Vector3 positionBack = GetTerrainPosition(position - forward);
            Vector3 positionForward = GetTerrainPosition(position + forward);
            transform.position = (positionBack + positionForward) / 2f;
            transform.rotation = Quaternion.LookRotation(positionForward - positionBack);
        }

        private Vector3 GetTerrainPosition(Vector3 position)
        {
            var y = TerrainSettings.Amplitude * MathfExt.PerlinNoiseHarmonic(position.x * TerrainSettings.Scale, position.z * TerrainSettings.Scale, Time, TerrainSettings.Harmonics);
            position.y = y;
            return position;
        }
    }
}
