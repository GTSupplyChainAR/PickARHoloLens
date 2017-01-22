using UnityEngine;
using System.Collections.Generic;

namespace PickAR.Managers {

    /// <summary>
    /// Handles playback of sound.
    /// </summary>
    public class SoundManager : MonoBehaviour {
        /// <summary> The audio source to play sound out of. </summary>
        [SerializeField]
        [Tooltip("The audio source to play sound out of.")]
        private AudioSource audioSource;
        /// <summary> The sound for picking a correct item. </summary>
        [SerializeField]
        [Tooltip("The sound for picking a correct item.")]
        public AudioClip correctSound;
        /// <summary> The sound for picking an incorrect item. </summary>
        [SerializeField]
        [Tooltip("The sound for picking an incorrect item.")]
        public AudioClip incorrectSound;
        /// <summary> The sound for collecting all items in a job. </summary>
        [SerializeField]
        [Tooltip("The sound for collecting all items in a job.")]
        public AudioClip collectAllSound;
        /// <summary> The sound for finishing a job. </summary>
        [SerializeField]
        [Tooltip("The sound for finishing a job.")]
        public AudioClip finishSound;

        /// <summary>
        /// Sounds that can be played.
        /// </summary>
        public enum Sound {
            Correct,
            Incorrect,
            CollectAll,
            Finish
        }

        /// <summary>
        /// Maps sound enums to sounds.
        /// </summary>
        private Dictionary<Sound, AudioClip> soundDict = new Dictionary<Sound, AudioClip>();

        /// <summary> The singleton instance of the object. </summary>
        public static SoundManager instance {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the object.
        /// </summary>
        private void Awake() {
            instance = this;
        }

        /// <summary>
        /// Initializes the sound dictionary.
        /// </summary>
        private void Start() {
            soundDict.Add(Sound.Correct, correctSound);
            soundDict.Add(Sound.Incorrect, incorrectSound);
            soundDict.Add(Sound.CollectAll, collectAllSound);
            soundDict.Add(Sound.Finish, finishSound);
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="clip">The sound to play.</param>
        public void PlaySound(Sound sound) {
            AudioClip clip;
            soundDict.TryGetValue(sound, out clip);
            if (clip != null) {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}