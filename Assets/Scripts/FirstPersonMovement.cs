using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{

    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonMovement : MonoBehaviour
    {
        [Header("Mouvement")]
        [Tooltip("Vitesse de marche")]
        public float walkSpeed = 6f;
        
        [Tooltip("Vitesse de course")]
        public float runSpeed = 12f;
        
        [Tooltip("Accélération du mouvement")]
        public float acceleration = 10f;
        
        [Tooltip("Décélération du mouvement")]
        public float deceleration = 10f;

        [Header("Saut")]
        [Tooltip("Force du saut")]
        public float jumpForce = 15f;
        
        [Tooltip("Gravité appliquée au personnage")]
        public float gravity = 30f;

        [Header("Options")]
        [Tooltip("Utiliser les touches AZERTY (ZQSD) au lieu de WASD")]
        public bool useAzerty = true;

        // Composants
        private CharacterController controller;
        private FirstPersonAnimationController animationController;
        
        // Variables de mouvement
        private Vector3 moveDirection = Vector3.zero;
        private Vector3 currentVelocity = Vector3.zero;
        private bool isGrounded;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animationController = GetComponent<FirstPersonAnimationController>();
            
            if (controller == null)
            {
                Debug.LogError("FirstPersonMovement nécessite un CharacterController!");
            }
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            isGrounded = controller.isGrounded;

            // Récupérer les entrées de mouvement
            float horizontal = GetHorizontalInput();
            float vertical = GetVerticalInput();

            // Direction cible du mouvement (normalisée pour un mouvement cohérent dans toutes les directions)
            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
            if (inputDirection.magnitude > 1f)
                inputDirection.Normalize();

            // Calculer la direction de mouvement dans l'espace monde
            Vector3 targetDirection = transform.right * inputDirection.x + transform.forward * inputDirection.z;

            // Déterminer la vitesse cible (course ou marche)
            bool runKeyPressed = false;
            if (Keyboard.current != null)
            {
                if (useAzerty)
                {
                    runKeyPressed = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.eKey.isPressed;
                }
                else
                {
                    runKeyPressed = Keyboard.current.leftShiftKey.isPressed;
                }
            }
            
            float targetSpeed = runKeyPressed ? runSpeed : walkSpeed;

            // Si aucune entrée, ralentir jusqu'à l'arrêt
            if (inputDirection.magnitude < 0.01f)
                targetSpeed = 0f;

            // Interpoler la vélocité actuelle vers la vélocité cible avec accélération/décélération
            float currentHorizontalSpeed = new Vector3(currentVelocity.x, 0f, currentVelocity.z).magnitude;
            float speedOffset = 0.1f;
            
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // Accélération ou décélération progressive
                float speedChangeRate = (currentHorizontalSpeed < targetSpeed) ? acceleration : deceleration;
                currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            }
            else
            {
                currentHorizontalSpeed = targetSpeed;
            }

            // Appliquer la vitesse dans la direction cible
            if (targetDirection.magnitude > 0.01f)
            {
                currentVelocity.x = targetDirection.normalized.x * currentHorizontalSpeed;
                currentVelocity.z = targetDirection.normalized.z * currentHorizontalSpeed;
            }
            else
            {
                // Arrêt progressif
                currentVelocity.x = Mathf.Lerp(currentVelocity.x, 0f, Time.deltaTime * deceleration);
                currentVelocity.z = Mathf.Lerp(currentVelocity.z, 0f, Time.deltaTime * deceleration);
            }

            // Gestion du saut
            bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            
            if (isGrounded)
            {
                // Au sol: réinitialiser la vélocité verticale
                if (moveDirection.y < 0)
                {
                    moveDirection.y = -2f;
                }

                // Saut
                if (jumpPressed)
                {
                    moveDirection.y = Mathf.Sqrt(jumpForce * 2f * gravity);
                    
                    // Déclencher l'animation de saut
                    if (animationController != null)
                    {
                        animationController.TriggerJump();
                    }
                }
            }

            // Appliquer la gravité
            moveDirection.y -= gravity * Time.deltaTime;

            // Combiner mouvement horizontal et vertical
            Vector3 finalMovement = new Vector3(currentVelocity.x, moveDirection.y, currentVelocity.z);
            
            // Appliquer le mouvement au CharacterController
            controller.Move(finalMovement * Time.deltaTime);
        }

        private float GetHorizontalInput()
        {
            float input = 0f;

            if (Keyboard.current != null)
            {
                if (useAzerty)
                {
                    // AZERTY: touches physiques A (Q sur clavier) et D
                    // Sur clavier AZERTY physique: A est à gauche de Z, D est à droite de Z
                    if (Keyboard.current.aKey.isPressed) input -= 1f; // A physique = Q visuel AZERTY
                    if (Keyboard.current.dKey.isPressed) input += 1f; // D physique = D visuel AZERTY
                }
                else
                {
                    // QWERTY: A = gauche, D = droite
                    if (Keyboard.current.aKey.isPressed) input -= 1f;
                    if (Keyboard.current.dKey.isPressed) input += 1f;
                }

                // Flèches directionnelles
                if (Keyboard.current.leftArrowKey.isPressed) input -= 1f;
                if (Keyboard.current.rightArrowKey.isPressed) input += 1f;
            }
            
            return Mathf.Clamp(input, -1f, 1f);
        }

        private float GetVerticalInput()
        {
            float input = 0f;

            if (Keyboard.current != null)
            {
                if (useAzerty)
                {
                    // AZERTY: touches physiques W (Z sur clavier) et S
                    // Sur clavier AZERTY physique: W est au-dessus de A, S est en dessous
                    if (Keyboard.current.wKey.isPressed) input += 1f; // W physique = Z visuel AZERTY
                    if (Keyboard.current.sKey.isPressed) input -= 1f; // S physique = S visuel AZERTY
                }
                else
                {
                    // QWERTY: W = avancer, S = reculer
                    if (Keyboard.current.wKey.isPressed) input += 1f;
                    if (Keyboard.current.sKey.isPressed) input -= 1f;
                }

                // Flèches directionnelles
                if (Keyboard.current.upArrowKey.isPressed) input += 1f;
                if (Keyboard.current.downArrowKey.isPressed) input -= 1f;
            }
            
            return Mathf.Clamp(input, -1f, 1f);
        }

        /// <summary>
        /// Basculer entre AZERTY et QWERTY
        /// </summary>
        public void SetKeyboardLayout(bool azerty)
        {
            useAzerty = azerty;
        }
    }
}
