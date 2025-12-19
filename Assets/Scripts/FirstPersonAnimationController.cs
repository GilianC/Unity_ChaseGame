using UnityEngine;

namespace Game
{
    /// <summary>
    /// Script qui gère les animations du personnage en fonction des mouvements.
    /// Attachez ce script sur votre personnage avec un Animator.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonAnimationController : MonoBehaviour
    {
        [Header("Références")]
        [Tooltip("Le script de mouvement FirstPersonMovement")]
        public FirstPersonMovement movementScript;

        [Header("Paramètres d'animation")]
        [Tooltip("Vitesse de transition entre les animations")]
        public float animationSmoothTime = 0.1f;

        [Tooltip("Multiplicateur de vitesse d'animation")]
        public float animationSpeedMultiplier = 1f;

        // Composants
        private Animator animator;
        private CharacterController controller;

        // Variables d'animation
        private float currentSpeed = 0f;
        private float speedVelocity = 0f;
        private float lastJumpTime = -10f; // Temps du dernier saut

        // Hash des paramètres d'animation (performance optimisée)
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        // Propriétés publiques pour accès externe
        public bool IsGrounded => controller != null && controller.isGrounded;
        public bool IsJumping => Time.time - lastJumpTime < 0.5f; // Considéré en saut pendant 0.5s

        private void Start()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();

            if (movementScript == null)
            {
                movementScript = GetComponent<FirstPersonMovement>();
            }

            if (animator == null)
            {
                Debug.LogError("FirstPersonAnimationController nécessite un Animator!");
            }

            if (movementScript == null)
            {
                Debug.LogError("FirstPersonAnimationController nécessite FirstPersonMovement!");
            }

            // Initialiser les hash des paramètres d'animation
            AssignAnimationIDs();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void Update()
        {
            if (animator == null || movementScript == null) return;

            UpdateAnimations();
        }

        private void UpdateAnimations()
        {
            // Calculer la vitesse actuelle du personnage
            Vector3 velocity = controller.velocity;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            float speed = horizontalVelocity.magnitude;

            // Lisser la transition de vitesse
            currentSpeed = Mathf.SmoothDamp(currentSpeed, speed, ref speedVelocity, animationSmoothTime);

            // Normaliser la vitesse pour l'animation
            // 0 = idle (immobile)
            // 1 = walk (marche)
            // 2 = run (course)
            float normalizedSpeed = 0f;
            
            if (currentSpeed > 0.1f)
            {
                // Déterminer si c'est de la marche ou de la course
                // Si la vitesse dépasse la vitesse de marche + 30%, c'est de la course
                float threshold = movementScript.walkSpeed + (movementScript.walkSpeed * 0.3f);
                
                if (currentSpeed > threshold)
                {
                    // Course (run)
                    normalizedSpeed = 2f;
                }
                else
                {
                    // Marche (walk)
                    normalizedSpeed = 1f;
                }
            }
            // else normalizedSpeed reste à 0 (idle)

            // Mettre à jour les paramètres d'animation avec les hash
            if (animator != null)
            {
                animator.SetFloat(_animIDSpeed, normalizedSpeed);
                animator.SetFloat(_animIDMotionSpeed, animationSpeedMultiplier);
                
                // Important: mettre à jour Grounded pour que le jump se termine
                bool isGrounded = controller.isGrounded;
                animator.SetBool(_animIDGrounded, isGrounded);

                // Détection de chute libre (falling)
                // Seulement si on n'est PAS au sol ET qu'on tombe (vélocité Y négative)
                bool isFreeFalling = !isGrounded && velocity.y < -1f;
                animator.SetBool(_animIDFreeFall, isFreeFalling);
            }
        }

        /// <summary>
        /// Déclenche l'animation de saut
        /// </summary>
        public void TriggerJump()
        {
            if (animator != null)
            {
                animator.SetTrigger(_animIDJump);
                lastJumpTime = Time.time; // Enregistrer le moment du saut
            }
        }

        /// <summary>
        /// Définir la vitesse d'animation
        /// </summary>
        public void SetAnimationSpeed(float speed)
        {
            animationSpeedMultiplier = speed;
        }
    }
}
