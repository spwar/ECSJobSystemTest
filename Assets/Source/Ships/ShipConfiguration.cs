using UnityEngine;

namespace Ships
{
    public enum ShipManagerType
    {
        ShipManager,
        ShipManagerJobs,
        ShipManagerECS
    }

    [CreateAssetMenu(fileName = "Assets/Configuration/ShipConfiguration", menuName = "GL/Configuration/Ships")]
    public class ShipConfiguration : ScriptableObject
    {
        [SerializeField]
        private ShipManagerType shipManagerType;
        public ShipManagerType ShipManagerType => shipManagerType;
        
        [SerializeField]
        private GameObject shipPrefab;
        public GameObject ShipPrefab => shipPrefab;
        
        [SerializeField]
        private float shipLength;
        public float ShipLength => shipLength;

        [SerializeField]
        private int shipCount;
        public int ShipCount => shipCount;
    }
}