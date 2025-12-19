using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    /// <summary>
    /// Script de caméra première personne.
    /// Attachez ce script à votre personnage et assignez la caméra.
    /// </summary>
    public class FirstPersonCamera : MonoBehaviour
    {
        [Header("Références")]
        [Tooltip("La caméra qui suivra le joueur (créez une caméra enfant du personnage)")]
        public Camera playerCamera;
        
        [Tooltip("Le point où la caméra sera positionnée (généralement la tête du personnage)")]
        public Transform cameraHolder;

        [Header("Sensibilité")]
        [Tooltip("Sensibilité horizontale de la souris")]
        public float mouseSensitivityX = 2f;
        
        [Tooltip("Sensibilité verticale de la souris")]
        public float mouseSensitivityY = 2f;

        [Header("Limites de rotation")]
        [Tooltip("Angle maximum vers le haut")]
        public float maxLookUpAngle = 80f;
        
        [Tooltip("Angle maximum vers le bas")]
        public float maxLookDownAngle = 80f;

        [Header("Options")]
        [Tooltip("Inverser l'axe Y")]
        public bool invertY = false;
        
        [Tooltip("Verrouiller le curseur au centre de l'écran")]
        public bool lockCursor = true;

        // Variables privées
        private float rotationX = 0f;
        private float rotationY = 0f;

        private void Start()
        {
            // Verrouiller le curseur si demandé
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            // Initialiser la rotation avec la rotation actuelle
            rotationY = transform.eulerAngles.y;
        }

        private void Update()
        {
            HandleMouseInput();
            HandleCursorLock();
        }

        private void LateUpdate()
        {
            // Positionner la caméra au point de vue si défini
            if (playerCamera != null && cameraHolder != null)
            {
                // Si la caméra n'est PAS déjà enfant du cameraHolder, la déplacer
                if (playerCamera.transform.parent != cameraHolder)
                {
                    playerCamera.transform.position = cameraHolder.position;
                }
            }
        }

        private void HandleMouseInput()
        {
            // Récupérer les entrées de la souris avec le nouveau Input System
            Vector2 mouseDelta = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;
            
            float mouseX = mouseDelta.x * mouseSensitivityX * 0.1f; // Ajustement de sensibilité
            float mouseY = mouseDelta.y * mouseSensitivityY * 0.1f;

            // Inverser Y si demandé
            if (invertY)
                mouseY = -mouseY;

            // Rotation horizontale (tourner le personnage)
            rotationY += mouseX;
            transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

            // Rotation verticale (regarder haut/bas - seulement la caméra)
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -maxLookUpAngle, maxLookDownAngle);

            if (playerCamera != null)
            {
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            }
        }

        private void HandleCursorLock()
        {
            // Appuyer sur Escape pour libérer le curseur
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // Cliquer pour verrouiller à nouveau le curseur
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Définir la sensibilité de la souris
        /// </summary>
        public void SetSensitivity(float sensitivity)
        {
            mouseSensitivityX = sensitivity;
            mouseSensitivityY = sensitivity;
        }
    }
}
