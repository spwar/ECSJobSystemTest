using System.Linq;
using Ships.ECS;
using TerrainGeneration;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Ships
{
    public class ShipManagerECS : MonoBehaviour, IShipManager
    {
        [SerializeField] private ShipConfiguration shipConfiguration;

        private ITerrainGenerator terrainGenerator;

        private NativeArray<Entity> shipEntities;

        private EntityManager entityManager;
        
        public void Setup(ITerrainGenerator terrainGenerator, ShipConfiguration shipConfiguration)
        {
            this.terrainGenerator = terrainGenerator;
            this.shipConfiguration = shipConfiguration;
            
            entityManager = World.Active.EntityManager;
            SetupFloatingSystem();
        }

        public void GenerateShips()
        {
            shipEntities = new NativeArray<Entity>(shipConfiguration.ShipCount, Allocator.Temp);
            
            //Convert GameObject prefab to an Entity
            Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(shipConfiguration.ShipPrefab, new GameObjectConversionSettings(World.Active, GameObjectConversionUtility.ConversionFlags.AddEntityGUID));
            entityManager.Instantiate(entity, shipEntities);
            
            //Add Tranlsation and Rotation components and setting initial values
            var TerrainSettings = terrainGenerator.TerrainSettings;
            for (int index = 0; index < shipConfiguration.ShipCount; index++)
            {
                var position = new Vector3(
                    Random.Range(TerrainSettings.Rect.xMin, TerrainSettings.Rect.xMax),
                    5f,
                    Random.Range(TerrainSettings.Rect.yMin, TerrainSettings.Rect.yMax));
                var rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0f);
                entityManager.SetComponentData(shipEntities[index], new Translation(){Value = position});
                entityManager.SetComponentData(shipEntities[index], new Rotation(){Value = rotation});
            }
            
            shipEntities.Dispose();
        }

        private void SetupFloatingSystem()
        {
            var TerrainSettings = terrainGenerator.TerrainSettings;
            var floatingSystem = entityManager.World.Systems.First((x) => x.GetType() == typeof(FloatingSystem)) as FloatingSystem;
            floatingSystem.Setup(TerrainSettings);
        }
    }
}