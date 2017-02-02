using UnityEngine;
using UnityEngine.UI;
using System;
using PickAR.Managers;

namespace PickAR.UI {

    /// <summary>
    /// Updates the progress of the job on the UI.
    /// </summary>
    class ProgressText : JobText {

        /// <summary>
        /// Gets the text to be displayed when the job is in progress.
        /// </summary>
        /// <returns>The text to be displayed when the job is in progress.</returns>
        protected override string getText() {
            int remainingItems = jobManager.remainingItems;
            return remainingItems.ToString();
        }
    }
}