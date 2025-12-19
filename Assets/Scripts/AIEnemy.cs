using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    /// <summary>
    /// IA ennemie qui poursuit le joueur après 20 secondes.
    /// Sa vitesse augmente progressivement au fil du temps.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIEnemy : MonoBehaviour
    {
        [Header("Cible")]
        [Tooltip("Le joueur à poursuivre")]
        public Transform player;

        [Header("Timing")]
        [Tooltip("Temps avant de commencer la poursuite (secondes)")]
        public float startDelay = 20f;

        [Header("Vitesse")]
        [Tooltip("Vitesse de départ (très lente)")]
        public float startSpeed = 2f;

        [Tooltip("Vitesse maximale atteinte à la fin")]
        public float maxSpeed = 15f;

        [Tooltip("Temps pour atteindre la vitesse maximale (secondes)")]
        public float speedIncreaseTime = 100f; // 120s - 20s de délai

        [Header("Détection")]
        [Tooltip("Distance minimale pour toucher le joueur")]
        public float touchDistance = 1.5f;

        // Composants
        private NavMeshAgent agent;
        private GameManager gameManager;
        private GameAudioManager audioManager;
        private Animator animator;

        // Variables
        private bool isChasing = false;
        private float chaseStartTime = 0f;
        private bool hasAnimator = false;
        private bool hasTouchedPlayer = false;

        // Hash des paramètres d'animation
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDMotionSpeed;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            gameManager = FindFirstObjectByType<GameManager>();
            audioManager = FindFirstObjectByType<GameAudioManager>();
            
            // Vérifier l'animator
            if (TryGetComponent(out animator))
            {
                hasAnimator = true;
                AssignAnimationIDs();
            }

            if (agent == null)
            {
                Debug.LogError("AIEnemy nécessite un NavMeshAgent! Ajoutez le composant NavMeshAgent.");
                enabled = false;
                return;
            }

            // Vérifier le GameManager
            if (gameManager == null)
            {
                Debug.LogError("⚠️ GameManager introuvable ! Le Game Over ne fonctionnera pas !");
            }
            else
            {
                Debug.Log($"✓ GameManager trouvé: {gameManager.gameObject.name}");
            }

            // IMPORTANT : Supprimer le CharacterController si présent (conflit avec NavMeshAgent)
            CharacterController characterController = GetComponent<CharacterController>();
            if (characterController != null)
            {
                Debug.LogWarning("⚠️ CharacterController détecté sur l'IA ! Suppression (conflit avec NavMeshAgent)...");
                Destroy(characterController);
            }

            // Configurer le NavMeshAgent pour contrôler le Transform
            agent.updatePosition = true;  // Le NavMeshAgent contrôle la position
            agent.updateRotation = true;  // Le NavMeshAgent contrôle la rotation
            agent.enabled = true;

            // Vérifier si on est sur un NavMesh
            UnityEngine.AI.NavMeshHit hit;
            if (!UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
            {
                Debug.LogError($"AIEnemy '{gameObject.name}' n'est PAS sur un NavMesh! Créez un NavMesh: Window → AI → Navigation → Bake");
            }
            else
            {
                Debug.Log($"AIEnemy '{gameObject.name}' est correctement placé sur le NavMesh ✓");
            }

            if (player == null)
            {
                Debug.LogWarning("AIEnemy: Aucun joueur assigné, recherche automatique...");
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                    Debug.Log($"Joueur trouvé: {playerObj.name} ✓");
                }
                else
                {
                    Debug.LogError("Aucun objet avec le tag 'Player' trouvé! Ajoutez le tag 'Player' à votre personnage.");
                }
            }

            // Configuration initiale du NavMeshAgent
            agent.speed = 0f; // Immobile au début
            agent.angularSpeed = 120f;
            agent.acceleration = 8f;
            agent.stoppingDistance = touchDistance;
            agent.autoBraking = true;
            agent.updateRotation = true;
            agent.updateUpAxis = true;

            Debug.Log($"AIEnemy initialisé. Démarrage dans {startDelay} secondes.");
        }

        private void Update()
        {
            if (agent == null) return;

            if (gameManager != null && (gameManager.IsGameOver || !gameManager.IsGameRunning))
            {
                // Arrêter le mouvement si le jeu est terminé
                agent.isStopped = true;
                return;
            }

            if (player == null) 
            {
                Debug.LogWarning("AIEnemy: Pas de joueur assigné!");
                return;
            }

            // Démarrer la poursuite après le délai
            if (!isChasing)
            {
                // Sans GameManager, démarrer après startDelay secondes depuis le début
                float currentTime = gameManager != null ? gameManager.GameTime : Time.time;
                
                if (currentTime >= startDelay)
                {
                    StartChasing();
                }
            }
            else
            {
                UpdateChasing();
            }

            // Mettre à jour les animations
            if (hasAnimator)
            {
                UpdateAnimations();
            }
        }

        private void StartChasing()
        {
            if (agent == null) return;

            isChasing = true;
            chaseStartTime = Time.time;
            
            // S'assurer que l'agent peut bouger
            agent.isStopped = false;
            agent.enabled = true;
            agent.updatePosition = true;
            agent.updateRotation = true;
            
            // Forcer la vitesse initiale
            agent.speed = startSpeed;
            agent.acceleration = 8f; // Accélération
            agent.angularSpeed = 120f; // Vitesse de rotation
            
            // Jouer le son de poursuite
            if (audioManager != null)
            {
                audioManager.PlayChaseMusic();
            }
            else
            {
                Debug.LogWarning("⚠️ GameAudioManager introuvable ! Le son ne sera pas joué.");
            }
            
            Debug.Log($"🔴 AI Enemy commence la poursuite !");
            Debug.Log($"  - Speed: {agent.speed} m/s (target: {startSpeed} → {maxSpeed})");
            Debug.Log($"  - IsStopped: {agent.isStopped}");
            Debug.Log($"  - UpdatePosition: {agent.updatePosition}");
            Debug.Log($"  - Enabled: {agent.enabled}");
        }

        private void UpdateChasing()
        {
            if (agent == null || player == null) return;

            // Calculer la vitesse progressive
            float chaseTime = Time.time - chaseStartTime;
            float speedProgress = Mathf.Clamp01(chaseTime / speedIncreaseTime);
            float currentSpeed = Mathf.Lerp(startSpeed, maxSpeed, speedProgress);

            // Appliquer la vitesse
            agent.speed = currentSpeed;

            // Toujours mettre à jour la destination
            agent.SetDestination(player.position);

            // Debug: afficher les infos toutes les 5 secondes
            if (Time.frameCount % 300 == 0)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                Debug.Log($"[IA] Vitesse: {currentSpeed:F1} m/s ({speedProgress*100:F0}%), Distance: {distance:F1}m");
                Debug.Log($"     HasPath: {agent.hasPath}, PathStatus: {agent.pathStatus}, IsStopped: {agent.isStopped}");
                Debug.Log($"     Velocity: {agent.velocity.magnitude:F2}, RemainingDistance: {agent.remainingDistance:F1}");
            }

            // Vérifier si on touche le joueur
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= touchDistance)
            {
                OnTouchPlayer();
            }
        }

        private void OnTouchPlayer()
        {
            if (hasTouchedPlayer) return; // Éviter les appels multiples
            hasTouchedPlayer = true;

            Debug.Log("💀 L'IA a touché le joueur !");
            
            if (gameManager == null)
            {
                Debug.LogError("❌ GameManager est NULL ! Impossible de déclencher Game Over !");
                gameManager = FindFirstObjectByType<GameManager>();
                
                if (gameManager == null)
                {
                    Debug.LogError("❌ GameManager toujours introuvable même après recherche !");
                    return;
                }
            }

            Debug.Log($"✓ Appel de GameManager.GameOver() sur {gameManager.gameObject.name}");
            gameManager.GameOver();
            agent.isStopped = true;
            
            // Arrêter le son
            if (audioManager != null)
            {
                audioManager.StopAllSounds();
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void UpdateAnimations()
        {
            if (!hasAnimator || animator == null) return;

            // Calculer la vitesse normalisée pour l'animation
            float speed = agent.velocity.magnitude;
            float normalizedSpeed = 0f;

            if (speed > 0.1f)
            {
                if (speed > 5f)
                    normalizedSpeed = 2f; // Run
                else
                    normalizedSpeed = 1f; // Walk
            }

            animator.SetFloat(_animIDSpeed, normalizedSpeed);
            animator.SetBool(_animIDGrounded, true);
            animator.SetFloat(_animIDMotionSpeed, 1f);
        }

        // Visualisation dans l'éditeur
        private void OnDrawGizmosSelected()
        {
            if (player == null) return;

            // Ligne vers le joueur
            Gizmos.color = isChasing ? Color.red : Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);

            // Sphère de détection
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, touchDistance);
        }

        /// <summary>
        /// Obtenir la vitesse actuelle de l'ennemi
        /// </summary>
        public float CurrentSpeed => agent != null ? agent.speed : 0f;

        /// <summary>
        /// Est-ce que l'ennemi poursuit activement ?
        /// </summary>
        public bool IsChasing => isChasing;

        /// <summary>
        /// Progression de la vitesse (0 à 1)
        /// </summary>
        public float SpeedProgress
        {
            get
            {
                if (!isChasing) return 0f;
                float chaseTime = Time.time - chaseStartTime;
                return Mathf.Clamp01(chaseTime / speedIncreaseTime);
            }
        }
    }
}
