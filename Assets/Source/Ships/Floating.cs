using TerrainGeneration;
using UnityEngine;

namespace Ships
{
    public class Floating : MonoBehaviour
    {
        public const float ShipLength = 0.2f;
        private ITerrainGenerator terrainGenerator;

        public void Setup(ITerrainGenerator terrainGenerator)
        {
            this.terrainGenerator = terrainGenerator;
        }

        void Update()
        {
            Vector3 position = transform.position;
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            forward = forward * ShipLength;
            Vector3 positionBack = terrainGenerator.GetTerrainPosition(position - forward);
            Vector3 positionForward = terrainGenerator.GetTerrainPosition(position + forward);
            transform.position = (positionBack + positionForward) / 2f;
            transform.rotation = Quaternion.LookRotation(positionForward - positionBack);
        }
    }
}