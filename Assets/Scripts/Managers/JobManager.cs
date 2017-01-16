using UnityEngine;

namespace PickAR.Managers {
    /// <summary>
    /// Keeps track of the user's current order picking job.
    /// </summary>
    class JobManager : MonoBehaviour {
        
        /// <summary> The items targeted by the user. </summary>
        [SerializeField]
        [Tooltip("The items targeted by the user.")]
        private Item[] targetItems;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            
        }
    }
}