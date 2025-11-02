using UnityEngine;
using System.Collections.Generic;

public class CarSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnLane
    {
        public GameObject carPrefab;
        public Transform spawnPoint;
        public float carSpeed = 5f;
        public CarSpawner.MovementDirection direction = CarSpawner.MovementDirection.Forward;
        public float minSpawnInterval = 2f;
        public float maxSpawnInterval = 5f;
        public float minDistanceBetweenCars = 3f;
    }
    
    [Header("Configuração das Faixas")]
    public List<SpawnLane> lanes = new List<SpawnLane>();
    
    [Header("Collider Compartilhado")]
    public BoxCollider endCollider;
    
    private List<CarSpawner> spawners = new List<CarSpawner>();
    
    void Start()
    {
        // Cria um spawner para cada faixa
        foreach (SpawnLane lane in lanes)
        {
            GameObject spawnerObj = new GameObject($"Spawner_{lane.spawnPoint.name}");
            spawnerObj.transform.SetParent(transform);
            
            CarSpawner spawner = spawnerObj.AddComponent<CarSpawner>();
            spawner.carPrefab = lane.carPrefab;
            spawner.spawnPoint = lane.spawnPoint;
            spawner.endCollider = endCollider;
            spawner.minSpawnInterval = lane.minSpawnInterval;
            spawner.maxSpawnInterval = lane.maxSpawnInterval;
            spawner.carSpeed = lane.carSpeed;
            spawner.direction = lane.direction;
            spawner.minDistanceBetweenCars = lane.minDistanceBetweenCars;
            
            spawners.Add(spawner);
        }
    }
}
