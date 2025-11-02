using UnityEngine;
using Photon.Pun;
using PV.Multiplayer;

public class CarDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("Dano causado ao jogador ao colidir")]
    public int damageAmount = 200;
    
    [Header("Collision Settings")]
    [Tooltip("Tag do player para identificar colisão")]
    public string playerTag = "Player";
    
    [Tooltip("Cooldown entre danos (segundos) - evita múltiplos hits")]
    public float damageCooldown = 0.5f;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    // Controla o cooldown para evitar múltiplos danos
    private float _lastDamageTime = 0f;
    
    /// <summary>
    /// Detecta colisão com o player e aplica dano
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se colidiu com um objeto com a tag de player
        if (collision.gameObject.CompareTag(playerTag))
        {
            ApplyDamageToPlayer(collision.gameObject);
        }
    }
    
    /// <summary>
    /// Versão alternativa usando Trigger (caso o collider do carro seja trigger)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se colidiu com um objeto com a tag de player
        if (other.CompareTag(playerTag))
        {
            ApplyDamageToPlayer(other.gameObject);
        }
    }
    
    /// <summary>
    /// Aplica dano ao player de forma segura
    /// </summary>
    private void ApplyDamageToPlayer(GameObject playerObject)
    {
        // Verifica se passou o tempo de cooldown
        if (Time.time - _lastDamageTime < damageCooldown)
        {
            return;
        }
        
        // Verifica se o objeto tem o componente PlayerController
        if (playerObject == null)
        {
            if (showDebugInfo)
                Debug.LogWarning("Player object é null!");
            return;
        }
        
        PlayerController player = playerObject.GetComponent<PlayerController>();
        
        // VERIFICAÇÃO CRÍTICA: Confirma que todos os componentes existem
        if (player == null)
        {
            if (showDebugInfo)
                Debug.LogWarning("PlayerController não encontrado no objeto!");
            return;
        }
        
        if (player.photonView == null)
        {
            if (showDebugInfo)
                Debug.LogWarning("PhotonView não encontrado no player!");
            return;
        }
        
        // Só aplica dano se for o player local (owner)
        if (player.photonView.IsMine)
        {
            // USA O PRÓPRIO ActorNumber DO PLAYER como atacante
            // Isso evita o problema de não ter um atacante válido
            int attackerID = player.photonView.Owner.ActorNumber;
            
            // Aplica dano usando o RPC do PlayerController
            player.photonView.RPC("TakeDamage", RpcTarget.All, damageAmount, attackerID);
            
            _lastDamageTime = Time.time;
            
            if (showDebugInfo)
            {
                Debug.Log($"Carro causou {damageAmount} de dano ao player {player.photonView.Owner.NickName}");
            }
        }
    }
}
