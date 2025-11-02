using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    [Header("Configurações do Carro")]
    [Tooltip("Prefab do carro que vai spawnar")]
    public GameObject carPrefab;
    
    [Header("Spawn Configuration")]
    [Tooltip("Ponto onde os carros vão spawnar")]
    public Transform spawnPoint;
    
    [Tooltip("Collider que detecta quando o carro deve ser destruído")]
    public BoxCollider endCollider;
    
    [Header("Timing")]
    [Tooltip("Tempo mínimo entre spawns (segundos)")]
    public float minSpawnInterval = 2f;
    
    [Tooltip("Tempo máximo entre spawns (segundos)")]
    public float maxSpawnInterval = 5f;
    
    [Header("Movimento")]
    [Tooltip("Velocidade do carro")]
    public float carSpeed = 5f;
    
    [Tooltip("Direção do movimento (Forward, Back, Left, Right)")]
    public MovementDirection direction = MovementDirection.Forward;
    
    [Header("Espaçamento")]
    [Tooltip("Distância mínima entre carros")]
    public float minDistanceBetweenCars = 3f;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private Transform lastSpawnedCar;
    private float nextSpawnTime;
    
    public enum MovementDirection
    {
        Forward,
        Back,
        Left,
        Right
    }
    
    void Start()
    {
        // Configura o primeiro spawn
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        
        // Configura o collider como trigger se não estiver
        if (endCollider != null)
        {
            endCollider.isTrigger = true;
        }
    }
    
    void Update()
    {
        // Verifica se é hora de spawnar um novo carro
        if (Time.time >= nextSpawnTime)
        {
            // Verifica se há espaço suficiente para spawnar
            if (CanSpawn())
            {
                SpawnCar();
            }
            
            // Agenda o próximo spawn
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }
    
    bool CanSpawn()
    {
        // Se não há último carro spawnado, pode spawnar
        if (lastSpawnedCar == null)
            return true;
        
        // Calcula a distância do último carro spawnado
        float distance = Vector3.Distance(spawnPoint.position, lastSpawnedCar.position);
        
        // Só spawna se houver distância suficiente
        return distance >= minDistanceBetweenCars;
    }
    
    void SpawnCar()
    {
        if (carPrefab == null || spawnPoint == null)
        {
            Debug.LogError("CarSpawner: Prefab ou SpawnPoint não configurado!");
            return;
        }
        
        // Spawna o carro na posição do spawn point
        GameObject newCar = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Adiciona o componente de movimento ao carro
        CarMover mover = newCar.AddComponent<CarMover>();
        mover.speed = carSpeed;
        mover.direction = direction;
        mover.spawner = this;
        
        // Guarda referência do último carro
        lastSpawnedCar = newCar.transform;
        
        if (showDebugInfo)
            Debug.Log($"Carro spawnado em {spawnPoint.position}");
    }
    
    // Chamado quando um carro atinge o collider final
    public void OnCarReachedEnd(GameObject car)
    {
        if (showDebugInfo)
            Debug.Log($"Carro {car.name} atingiu o final e será destruído");
        
        Destroy(car);
    }
}
