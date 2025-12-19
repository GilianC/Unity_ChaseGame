using UnityEngine;

namespace Game
{
    /// <summary>
    /// Gère les effets sonores du jeu
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class GameAudioManager : MonoBehaviour
    {
        [Header("Sons du jeu")]
        [Tooltip("Son à jouer quand l'IA commence à poursuivre")]
        public AudioClip chaseMusic;

        [Header("Configuration")]
        [Tooltip("Volume du son (0-1)")]
        [Range(0f, 1f)]
        public float volume = 0.7f;

        [Tooltip("Le son doit-il boucler ?")]
        public bool loop = true;

        private AudioSource audioSource;
        private bool isPlaying = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.playOnAwake = false;
        }

        /// <summary>
        /// Jouer le son de poursuite
        /// </summary>
        public void PlayChaseMusic()
        {
            if (isPlaying || chaseMusic == null) return;

            audioSource.clip = chaseMusic;
            audioSource.Play();
            isPlaying = true;

            Debug.Log("🎵 Son de poursuite lancé !");
        }

        /// <summary>
        /// Arrêter tous les sons
        /// </summary>
        public void StopAllSounds()
        {
            audioSource.Stop();
            isPlaying = false;
        }

        /// <summary>
        /// Mettre en pause
        /// </summary>
        public void Pause()
        {
            audioSource.Pause();
        }

        /// <summary>
        /// Reprendre
        /// </summary>
        public void Resume()
        {
            audioSource.UnPause();
        }
    }
}
