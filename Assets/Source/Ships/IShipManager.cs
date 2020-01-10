using TerrainGeneration;

namespace Ships
{
    public interface IShipManager
    {
        void GenerateShips();
        void Setup(ITerrainGenerator terrainGenerator, ShipConfiguration shipConfiguration);
    }
}