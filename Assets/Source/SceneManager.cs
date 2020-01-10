using TerrainGeneration;
using UnityEngine;
using Ships;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private Transform terrainContainer;
    private int size = 0;
    public Material material;
    public TerrainSettings TerrainSettings;

    private GameObject[] terrain;
    
    private ITerrainGenerator terrainGenerator;

    [SerializeField]
    private TerrainGeneratorType terrainGeneratorType;

    public TerrainGeneratorType TerrainGeneratorType => terrainGeneratorType;
    

    private IShipManager shipManager;

    [SerializeField]
    private ShipConfiguration shipConfiguration;
    
    // Start is called before the first frame update
    void Start()
    {

        CreateTerrainGenerator();
        
        CreateTerrain();

        CreateShips();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTerrain();
    }

    private void CreateShips()
    {
        switch (shipConfiguration.ShipManagerType)
        {
            case ShipManagerType.ShipManager:
                shipManager = new GameObject("ShipManager").AddComponent<ShipManager>();
                break;
            case ShipManagerType.ShipManagerJobs:
                shipManager = new GameObject("ShipManagerJobs").AddComponent<ShipManagerJobs>();
                break;
            case ShipManagerType.ShipManagerECS:
                shipManager = new GameObject("ShipManagerECS").AddComponent<ShipManagerECS>();
                break;
        }
        shipManager.Setup(terrainGenerator, shipConfiguration);
        shipManager.GenerateShips();
    }

    private void CreateTerrain()
    {
        
        terrain = new GameObject[1];
        terrain[0] = new GameObject("terrain00");
        terrain[0].transform.parent = terrainContainer;
        var meshFilter = terrain[0].AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh();
        var meshCollider = terrain[0].AddComponent<MeshCollider>();
        meshCollider.sharedMesh = new Mesh();
        var meshRenderer = terrain[0].AddComponent<MeshRenderer>();

        meshRenderer.sharedMaterial = material;
        
        UpdateTerrain();
    }

    private void CreateTerrainGenerator()
    {
        switch (terrainGeneratorType)
        {
            case TerrainGeneratorType.TerrainGenerator:
                terrainGenerator = new TerrainGenerator(TerrainSettings);
                break;
            case TerrainGeneratorType.TerrainGeneratorJobs:
                terrainGenerator = new TerrainGeneratorJobs(TerrainSettings);
                break;
            case TerrainGeneratorType.TerrainGeneratorThreads:
                terrainGenerator = new TerrainGeneratorThreads(TerrainSettings);
                break;
        }
    }

    private void UpdateTerrain()
    {
        if (TerrainSettings.Resolution != size)
        {
            size = TerrainSettings.Resolution;
            terrainGenerator?.Dispose();
            CreateTerrainGenerator();
        }

        var meshFilter = terrain[0].GetComponent<MeshFilter>();
        var meshCollider = terrain[0].GetComponent<MeshCollider>();

        terrainGenerator.GenerateMesh(meshFilter, meshCollider);
    }

    private void OnDestroy()
    {
        terrainGenerator?.Dispose();
    }
}
