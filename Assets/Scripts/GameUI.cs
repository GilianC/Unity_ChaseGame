using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game
{
    /// <summary>
    /// Gère l'affichage de l'interface utilisateur du jeu
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("UI Principale")]
        [Tooltip("Panel du HUD de jeu")]
        public GameObject gamePanel;

        [Tooltip("Texte du chronomètre")]
        public TextMeshProUGUI timerText;

        [Tooltip("Texte d'avertissement AI")]
        public TextMeshProUGUI warningText;

        [Header("UI Game Over")]
        [Tooltip("Panel de Game Over")]
        public GameObject gameOverPanel;

        [Tooltip("Texte Game Over")]
        public TextMeshProUGUI gameOverText;

        [Tooltip("Bouton Recommencer")]
        public Button restartButton;

        [Header("UI Victoire")]
        [Tooltip("Panel de Victoire")]
        public GameObject victoryPanel;

        [Tooltip("Texte de Victoire")]
        public TextMeshProUGUI victoryText;

        [Tooltip("Bouton Quitter")]
        public Button quitButton;

        [Header("Couleurs")]
        [Tooltip("Couleur normale du timer")]
        public Color normalTimerColor = Color.white;

        [Tooltip("Couleur du timer en danger")]
        public Color dangerTimerColor = Color.red;

        [Tooltip("Temps restant pour passer en rouge (secondes)")]
        public float dangerThreshold = 30f;

        [Header("Configuration UI")]
        [Tooltip("Configurer automatiquement le timer en haut à droite")]
        public bool autoSetupTimer = true;

        [Tooltip("Taille de police du timer")]
        public float timerFontSize = 48f;

        // Référence au GameManager
        private GameManager gameManager;
        private AIEnemy aiEnemy;

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            aiEnemy = FindObjectOfType<AIEnemy>();

            // Configurer automatiquement le timer
            if (autoSetupTimer)
            {
                SetupTimer();
            }

            // Cacher tous les panels au départ
            if (gamePanel != null) gamePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (warningText != null) warningText.gameObject.SetActive(false);

            // Configurer les boutons
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
        }

        /// <summary>
        /// Configure automatiquement le timer en haut à droite
        /// </summary>
        private void SetupTimer()
        {
            if (timerText == null) return;

            RectTransform timerRect = timerText.GetComponent<RectTransform>();
            if (timerRect == null) return;

            // Ancrer en haut à droite
            timerRect.anchorMin = new Vector2(1, 1); // Coin haut droit
            timerRect.anchorMax = new Vector2(1, 1); // Coin haut droit
            timerRect.pivot = new Vector2(1, 1);     // Pivot en haut à droite

            // Position
            timerRect.anchoredPosition = new Vector2(-30, -30); // 30 pixels du bord

            // Taille
            timerRect.sizeDelta = new Vector2(200, 80);

            // Style du texte
            timerText.fontSize = timerFontSize;
            timerText.fontStyle = FontStyles.Bold;
            timerText.alignment = TextAlignmentOptions.TopRight;
            timerText.color = normalTimerColor;

            // Ajouter un outline pour la lisibilité
            if (!timerText.TryGetComponent<Outline>(out _))
            {
                Outline outline = timerText.gameObject.AddComponent<Outline>();
                outline.effectColor = Color.black;
                outline.effectDistance = new Vector2(2, -2);
            }

            Debug.Log("Timer configuré en haut à droite !");
        }

        private void Update()
        {
            // Configuration automatique du warning text au premier affichage
            if (warningText != null && !warningText.gameObject.activeSelf && autoSetupTimer)
            {
                SetupWarningText();
            }

            // Afficher l'avertissement quand l'AI commence à poursuivre
            if (aiEnemy != null && warningText != null && gameManager != null)
            {
                if (aiEnemy.IsChasing && !gameManager.IsGameOver)
                {
                    warningText.gameObject.SetActive(true);
                    float speedPercent = aiEnemy.SpeedProgress * 100f;
                    warningText.text = $"! DANGER ! Vitesse IA: {speedPercent:F0}%";
                    
                    // Faire clignoter le texte
                    float alpha = Mathf.PingPong(Time.time * 2f, 1f);
                    Color color = warningText.color;
                    color.a = 0.5f + (alpha * 0.5f);
                    warningText.color = color;
                }
                else
                {
                    warningText.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Configure le texte d'avertissement en haut au centre
        /// </summary>
        private void SetupWarningText()
        {
            if (warningText == null) return;

            RectTransform warningRect = warningText.GetComponent<RectTransform>();
            if (warningRect == null) return;

            // Ancrer en haut au centre
            warningRect.anchorMin = new Vector2(0.5f, 1); // Haut centre
            warningRect.anchorMax = new Vector2(0.5f, 1); // Haut centre
            warningRect.pivot = new Vector2(0.5f, 1);     // Pivot en haut centre

            // Position
            warningRect.anchoredPosition = new Vector2(0, -120); // En dessous du timer

            // Taille
            warningRect.sizeDelta = new Vector2(400, 60);

            // Style du texte
            warningText.fontSize = 32f;
            warningText.fontStyle = FontStyles.Bold;
            warningText.alignment = TextAlignmentOptions.Center;
            warningText.color = Color.red;

            // Ajouter un outline
            if (!warningText.TryGetComponent<Outline>(out _))
            {
                Outline outline = warningText.gameObject.AddComponent<Outline>();
                outline.effectColor = Color.black;
                outline.effectDistance = new Vector2(3, -3);
            }
        }

        /// <summary>
        /// Afficher l'UI de jeu
        /// </summary>
        public void ShowGameUI()
        {
            if (gamePanel != null) gamePanel.SetActive(true);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
        }

        /// <summary>
        /// Mettre à jour le chronomètre
        /// </summary>
        public void UpdateTimer(float remainingTime)
        {
            if (timerText == null) return;

            // Formater le temps
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Changer la couleur si danger
            if (remainingTime <= dangerThreshold)
            {
                timerText.color = dangerTimerColor;
                
                // Faire pulser le texte en danger
                float scale = 1f + Mathf.Sin(Time.time * 10f) * 0.1f;
                timerText.transform.localScale = Vector3.one * scale;
            }
            else
            {
                timerText.color = normalTimerColor;
                timerText.transform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Afficher l'écran de Game Over
        /// </summary>
        public void ShowGameOver()
        {
            if (gamePanel != null) gamePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            
            if (gameOverText != null)
            {
                gameOverText.text = "GAME OVER\n\nL'ennemi vous a attrapé !";
            }
        }

        /// <summary>
        /// Afficher l'écran de Victoire
        /// </summary>
        public void ShowVictory()
        {
            if (gamePanel != null) gamePanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(true);
            
            if (victoryText != null)
            {
                victoryText.text = "VICTOIRE !\n\nVous avez survécu 2 minutes !";
            }
        }

        private void OnRestartClicked()
        {
            if (gameManager != null)
            {
                gameManager.RestartGame();
            }
        }

        private void OnQuitClicked()
        {
            if (gameManager != null)
            {
                gameManager.QuitGame();
            }
        }
    }
}
