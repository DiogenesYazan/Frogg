using Photon.Pun;
using UnityEngine;


namespace PV.Multiplayer
{
    public class PlayerController : Movement
    {
        [Header("UI")]
        [Tooltip("Reference to the player's UI.")]
        public PlayerUI playerUI;


        [Header("Camera")]
        [Tooltip("Target Transform for the camera to follow.")]
        public Transform followTarget;


        [Header("Stats")]
        [Tooltip("The player's current health.")]
        public int health = 100;

        [Header("Debug")]
        [Tooltip("Show debug messages in console.")]
        public bool showDebugInfo = false;


        // Reference to the PhotonView component for network synchronization.
        internal PhotonView photonView;
        // Used to store the player stats (e.g., kills, deaths, score, etc.).
        internal Stats stats;


        // Manages the player's weapon state and actions.
        private WeaponManager _weaponManager;
        // Tracks the name of the last player who dealt damage.
        private PhotonView _lastAttacker;
        // Tracks the last attacker's ID.
        private int _lastAttackerID;
        // Tracks the last aiming input.
        private bool _wasAiming;


        protected override void Awake()
        {
            base.Awake();


            photonView = GetComponent<PhotonView>();
            _weaponManager = GetComponent<WeaponManager>();


            // Assign the PlayerUI component if not already set.
            if (playerUI == null)
            {
                playerUI = GetComponentInChildren<PlayerUI>(true);
            }


            if (playerUI != null)
            {
                playerUI.EnableReticle(false);
            }


            // Disable components if this instance does not belong to the local player.
            if (!photonView.IsMine)
            {
                enabled = false;
                if (playerUI != null)
                {
                    playerUI.enabled = false;
                    playerUI.gameObject.SetActive(false);
                }
            }


            // Initialize stats and store data 
            stats = new(photonView.Owner.ActorNumber);
            GameUIManager.Instance.SetStats(this);
        }


        private void FixedUpdate()
        {
            if (photonView == null)
            {
                return;
            }


            // Update movement based on player input.
            UpdateMovement();


            if (_weaponManager != null)
            {
                // Update weapon-related logic.
                _weaponManager.DoUpdate();
            }


            if (playerUI != null && _wasAiming != Input.isAiming)
            {
                _wasAiming = Input.isAiming;
                // Update the reticle state based on aiming input.
                playerUI.EnableReticle(Input.isAiming);
            }
        }


        /// <summary>
        /// Handles damage taken by the player and updates health and UI.
        /// </summary>
        /// <param name="damage">The amount of damage dealt.</param>
        /// <param name="attackerID">The ID of the player who dealt the damage.</param>
        [PunRPC]
        public void TakeDamage(int damage, int attackerID)
        {
            health -= damage; // Reduce the player's health.


            // Change last attacker on first time or when attacker changes.
            if (_lastAttacker == null || _lastAttackerID != attackerID)
            {
                _lastAttackerID = attackerID;
                _lastAttacker = PhotonView.Find(attackerID); // Record the last attacker.
            }


            if (playerUI != null)
            {
                playerUI.SetHealth(health);
            }


            // Handle player death if health drops to zero or below.
            if (health <= 0)
            {
                Die();
            }
        }


        /// <summary>
        /// Handles player death, updates stats, and triggers respawning.
        /// </summary>
        private void Die()
        {
            // Reset health for respawning.
            health = 100;


            if (playerUI != null)
            {
                playerUI.SetHealth(health);
            }


            // VERIFICAÇÃO CRÍTICA: Confirma que o atacante existe antes de usá-lo
            if (_lastAttacker != null && _lastAttacker.TryGetComponent(out PlayerController attacker))
            {
                // Atacante válido (outro player) - incrementa kills
                attacker.stats.AddKill();
                
                if (showDebugInfo)
                {
                    Debug.Log($"Player {photonView.Owner.NickName} foi morto por {_lastAttacker.Owner.NickName}");
                }
            }
            else
            {
                // Morte por ambiente (carro, queda, etc) - sem atacante válido
                if (showDebugInfo)
                {
                    Debug.Log($"Player {photonView.Owner.NickName} morreu por causa ambiental (carro, queda, etc)");
                }
            }
            
            // Incrementa as mortes do jogador
            stats.AddDeaths();


            // Log the kill in the game UI - verifica se há atacante válido
            if (_lastAttacker != null && _lastAttacker.Owner != null)
            {
                // Morte por outro player
                GameUIManager.Instance.LogKilled(_lastAttacker.Owner.NickName, photonView.Owner.NickName);
            }
            else
            {
                // Morte por ambiente/carro - mostra "Ambiente" como atacante
                GameUIManager.Instance.LogKilled("Ambiente", photonView.Owner.NickName);
            }
            
            // Notify the game manager to respawn the player.
            GameManager.Instance.ReSpawn(this);
        }
    }


    /// <summary>
    /// Manages player statistics such as kills, deaths, and score.
    /// </summary>
    [System.Serializable]
    public class Stats
    {
        public const string KillsKey = "Kills";
        public const string DeathsKey = "Deaths";
        public const string ScoreKey = "Score";


        public int Kills { get; private set; }
        public int Deaths { get; private set; }
        public int Score { get; private set; }


        // The player's unique number (ActorNumber).
        private int _playerNumber = -1;


        public Stats(int playerNumber)
        {
            _playerNumber = playerNumber;
            Kills = 0;
            Deaths = 0;
            Score = 0;
        }


        /// <summary>
        /// Increments the player's kill count and updates the score.
        /// </summary>
        public void AddKill()
        {
            if (_playerNumber < 0)
            {
                return;
            }


            Kills++;
            UpdateStat();
        }


        /// <summary>
        /// Increments the player's death count and updates the score.
        /// </summary>
        public void AddDeaths()
        {
            if (_playerNumber < 0)
            {
                return;
            }


            Deaths++;
            UpdateStat();
        }


        /// <summary>
        /// Updates the player's score and informs the Game UI Manager.
        /// </summary>
        private void UpdateStat()
        {
            Score = Kills - Deaths;
            GameUIManager.Instance.UpdateStats(_playerNumber);
        }
    }
}
