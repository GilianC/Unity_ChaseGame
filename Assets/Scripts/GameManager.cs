using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// Gère le déroulement du jeu : chronomètre, victoire et défaite
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Durée du jeu")]
        [Tooltip("Temps total de la partie en secondes")]
        public float gameDuration = 120f; // 2 minutes

        [Header("Références")]
        [Tooltip("L'UI du jeu")]
        public GameUI gameUI;

        // État du jeu
        private float gameTime = 0f;
        private bool isGameRunning = false;
        private bool isGameOver = false;
        private bool hasWon = false;

        // Propriétés publiques
        public float GameTime => gameTime;
        public float RemainingTime => Mathf.Max(0f, gameDuration - gameTime);
        public bool IsGameRunning => isGameRunning;
        public bool IsGameOver => isGameOver;
        public bool HasWon => hasWon;

        private void Start()
        {
            // Démarrer le jeu après un court délai
            Invoke(nameof(StartGame), 0.5f);
        }

        private void Update()
        {
            if (!isGameRunning || isGameOver) return;

            // Incrémenter le chronomètre
            gameTime += Time.deltaTime;

            // Mettre à jour l'UI
            if (gameUI != null)
            {
                gameUI.UpdateTimer(RemainingTime);
            }

            // Vérifier la victoire
            if (gameTime >= gameDuration)
            {
                Victory();
            }
        }

        private void StartGame()
        {
            isGameRunning = true;
            gameTime = 0f;
            isGameOver = false;
            hasWon = false;

            // Déverrouiller le curseur
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (gameUI != null)
            {
                gameUI.ShowGameUI();
            }

            Debug.Log("Jeu démarré ! Survivez 2 minutes !");
        }

        /// <summary>
        /// Appelé quand le joueur est touché par l'IA
        /// </summary>
        public void GameOver()
        {
            Debug.Log($"🔴 GameManager.GameOver() appelé ! (isGameOver={isGameOver})");
            
            if (isGameOver)
            {
                Debug.LogWarning("Game Over déjà déclenché, ignoré.");
                return;
            }

            isGameOver = true;
            hasWon = false;
            isGameRunning = false;

            Debug.Log("✓ État du jeu mis à jour : Game Over");

            // Libérer le curseur
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (gameUI != null)
            {
                Debug.Log($"✓ Appel de gameUI.ShowGameOver() sur {gameUI.gameObject.name}");
                gameUI.ShowGameOver();
            }
            else
            {
                Debug.LogError("❌ GameUI est NULL ! L'écran Game Over ne s'affichera pas !");
            }

            Debug.Log("GAME OVER - L'ennemi vous a attrapé !");
        }

        /// <summary>
        /// Appelé quand le joueur survit 2 minutes
        /// </summary>
        private void Victory()
        {
            if (isGameOver) return;

            isGameOver = true;
            hasWon = true;
            isGameRunning = false;

            // Libérer le curseur
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (gameUI != null)
            {
                gameUI.ShowVictory();
            }

            Debug.Log("VICTOIRE - Vous avez survécu !");
        }

        /// <summary>
        /// Redémarrer le jeu
        /// </summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Quitter le jeu
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Formater le temps restant en MM:SS
        /// </summary>
        public string GetFormattedTime()
        {
            float time = RemainingTime;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
