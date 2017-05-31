using UnityEngine;
using UnityEngine.UI;

namespace PickAR.UI {

    /// <summary>
    /// Displays "items left" text.
    /// </summary>
    class ItemsText : MonoBehaviour {

        /// <summary> The text labeling the number of items remaining. </summary>
        [SerializeField]
        [Tooltip("The text labeling the number of items remaining.")]
        private Text progressText;
        /// <summary> The text labeling "items left". </summary>
        private Text itemsText;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            itemsText = GetComponent<Text>();
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            string remainingItems = progressText.text;
            string newText;
            if (remainingItems == "1") {
                newText = "item left";
            } else if (remainingItems == "-") {
                newText = "";
            } else {
                newText = "items left";
            }
            itemsText.text = newText;
        }
    }
}