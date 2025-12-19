using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    /// <summary>
    /// Script de debug pour tester les inputs.
    /// Attachez-le temporairement à votre personnage pour voir les inputs dans la console.
    /// </summary>
    public class InputDebugger : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current != null)
            {
                // Test touches AZERTY
                if (Keyboard.current.zKey.isPressed)
                    Debug.Log("Z pressé (Avancer)");
                
                if (Keyboard.current.sKey.isPressed)
                    Debug.Log("S pressé (Reculer)");
                
                if (Keyboard.current.qKey.isPressed)
                    Debug.Log("Q pressé (Gauche)");
                
                if (Keyboard.current.dKey.isPressed)
                    Debug.Log("D pressé (Droite)");
                
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    Debug.Log("ESPACE pressé (Saut)");
                
                if (Keyboard.current.leftShiftKey.isPressed)
                    Debug.Log("SHIFT pressé (Course)");
            }

            if (Mouse.current != null)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();
                if (delta.magnitude > 0.1f)
                    Debug.Log($"Souris bouge: {delta}");
            }
        }
    }
}
