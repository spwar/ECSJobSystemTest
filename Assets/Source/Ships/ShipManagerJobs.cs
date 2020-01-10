using System.Collections.Generic;
using Ships.ECS;
using TerrainGeneration;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Ships
{
    public class ShipManagerJobs : MonoBehaviour, IShipManager
    {
        [SerializeField] private ShipConfiguration shipConfiguration;

        private ITerrainGenerator terrainGenerator;

        private List<GameObject> ships;

        private TransformAccessArray shipTransforms;

        private FloatingJob floatingJob;
        private JobHandle floatingHandle;

        public void Setup(ITerrainGenerator terrainGenerator, ShipConfiguration shipConfiguration)
        {
            this.terrainGenerator = terrainGenerator;
            this.shipConfiguration = shipConfiguration;
        }

        public void GenerateShips()
        {
            ships = new List<GameObject>();
            shipTransforms = new TransformAccessArray(shipConfiguration.ShipCount);
            for (int i = 0; i < shipConfiguration.ShipCount; i++)
            {
                var ship = CreateShip();
                ships.Add(ship);
                shipTransforms.Add(ship.transform);
            }
        }

        private GameObject CreateShip()
        {
            var TerrainSettings = terrainGenerator.TerrainSettings;
            var ship = Instantiate(shipConfiguration.ShipPrefab,
                new Vector3(
                    Random.Range(TerrainSettings.Rect.xMin, TerrainSettings.Rect.xMax),
                    5f,
                    Random.Range(TerrainSettings.Rect.yMin, TerrainSettings.Rect.yMax)),
                Quaternion.Euler(0, Random.Range(-180f, 180f), 0f));
            //ship.GetComponent<Floating>().Setup(terrainGenerator);
            return ship;
        }

        void Update()
        {
            //floatingHandle.Complete();

            float time = Time.time;

            floatingJob = new FloatingJob()
            {
                Time = time,
                TerrainSettings = terrainGenerator.TerrainSettings
            };

            floatingHandle = floatingJob.Schedule(shipTransforms);
            
            //JobHandle.ScheduleBatchedJobs();
        }

        private void OnDisable()
        {
            floatingHandle.Complete();
            shipTransforms.Dispose();
        }
    }
}