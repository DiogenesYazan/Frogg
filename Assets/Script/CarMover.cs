using UnityEngine;

public class CarMover : MonoBehaviour
{
    [HideInInspector] public float speed = 5f;
    [HideInInspector] public CarSpawner.MovementDirection direction;
    [HideInInspector] public CarSpawner spawner;
    
    void Update()
    {
        // Move o carro na direção configurada
        Vector3 movement = GetMovementVector() * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
    
    Vector3 GetMovementVector()
    {
        switch (direction)
        {
            case CarSpawner.MovementDirection.Forward:
                return Vector3.forward;
            case CarSpawner.MovementDirection.Back:
                return Vector3.back;
            case CarSpawner.MovementDirection.Left:
                return Vector3.left;
            case CarSpawner.MovementDirection.Right:
                return Vector3.right;
            default:
                return Vector3.forward;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Verifica se colidiu com o collider de fim
        if (other.CompareTag("CarEnd") && spawner != null)
        {
            spawner.OnCarReachedEnd(gameObject);
        }
    }
}
