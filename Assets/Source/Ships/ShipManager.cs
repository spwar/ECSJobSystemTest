using System.Collections.Generic;
using TerrainGeneration;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ships
{
    public class ShipManager : MonoBehaviour, IShipManager
    {
        [SerializeField] private ShipConfiguration shipConfiguration;

        private ITerrainGenerator terrainGenerator;

        private List<GameObject> ships;

        public void Setup(ITerrainGenerator terrainGenerator, ShipConfiguration shipConfiguration)
        {
            this.terrainGenerator = terrainGenerator;
            this.shipConfiguration = shipConfiguration;
        }

        public void GenerateShips()
        {
            ships = new List<GameObject>();
            for (int i = 0; i < shipConfiguration.ShipCount; i++)
            {
                var ship = CreateShip();
                ships.Add(ship);
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
            ship.GetComponent<Floating>().Setup(terrainGenerator);
            return ship;
        }
    }
}