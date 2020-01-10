using System;
using System.Linq;
using TerrainGeneration;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Ships.ECS
{
    //[UpdateAfter()]
    public class FloatingSystem : JobComponentSystem
    {
        private TerrainSettings terrainSettings;
        
        [BurstCompile]
        struct FloatingJob : IJobForEach<Translation, Rotation>
        {
            public const float ShipLength = 0.2f;
            public TerrainSettings TerrainSettings;
            public float Time;

            public void Execute(ref Translation pos, ref Rotation rot)
            {
                float3 forward = math.mul(rot.Value.value,  math.forward(quaternion.identity));
                forward = ShipLength * math.normalize(forward);
                //Debug.Log($"Forward: {forward} rotation: {rot.Value.value.xyz}");
                float3 positionForward = GetTerrainPosition(pos.Value + forward);
                float3 positionBackward = GetTerrainPosition(pos.Value - forward);
                pos.Value = (positionForward + positionBackward) / 2;
                rot.Value = quaternion.LookRotation(positionForward - positionBackward, math.up());
                return;
                //float3 forward = rot.Value.value.xyz * math.forward(quaternion.identity);
//                forward.y = 0;
//                forward = ShipLength * forward / math.abs(forward);
//                float3 positionBack = GetTerrainPosition(position - forward);
//                float3 positionForward = GetTerrainPosition(position + forward);
//                pos.Value = (positionBack + positionForward) / 2f;
//                rot.Value = quaternion.LookRotation(positionForward - positionBack, math.up());
            }

            private float3 GetTerrainPosition(float3 position)
            {
                var y = TerrainSettings.Amplitude * MathfExt.PerlinNoiseHarmonic(position.x * TerrainSettings.Scale,
                            position.z * TerrainSettings.Scale, Time, TerrainSettings.Harmonics);
                position.y = y;
                return position;
            }
        }
        
        public void Setup(TerrainSettings terrainSettings)
        {
            this.terrainSettings = terrainSettings;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float time = Time.time;
            var job = new FloatingJob
            {
                Time = time,
                TerrainSettings = terrainSettings
            };
            return job.Schedule(this, inputDeps);
        }
    }
}