using UnityEngine;
using UnityEngine.UI;
using PickAR.Managers;

namespace PickAR.UI {

    /// <summary>
    /// Updates job information text in the UI.
    /// </summary>
    abstract class JobText : MonoBehaviour {

        /// <summary> The text displaying the next aisle. </summary>
        protected Text text;
        /// <summary> The manager keeping track of the current job. </summary>
        protected JobManager jobManager;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            text = GetComponent<Text>();
            jobManager = JobManager.instance;
            Initialize();
        }

        /// <summary>
        /// Does subclass initialization after Start().
        /// </summary>
        protected virtual void Initialize() {
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            string newText;
            if (jobManager.jobActive) {
                newText = getText();
            } else {
                newText = "-";
            }
            text.text = newText;
        }

        /// <summary>
        /// Gets the text to be displayed when the job is in progress.
        /// </summary>
        /// <returns>The text to be displayed when the job is in progress.</returns>
        protected abstract string getText();
    }
}