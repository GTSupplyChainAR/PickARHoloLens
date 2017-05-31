using UnityEngine;
using UnityEngine.UI;
using PickAR.Managers;

namespace PickAR.UI {

    /// <summary>
    /// Updates the next aisle text on the UI.
    /// </summary>
    class AisleText : JobText {

        /// <summary>
        /// Gets the text to be displayed when the job is in progress.
        /// </summary>
        /// <returns>The text to be displayed when the job is in progress.</returns>
        protected override string getText() {
            return jobManager.jobFinished ? "-" : jobManager.itemAisle;
        }
    }
}