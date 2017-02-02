using UnityEngine;
using UnityEngine.UI;

namespace PickAR.UI {

    /// <summary>
    /// A shelf icon in the minimap.
    /// </summary>
    class MinimapShelf : MonoBehaviour {
        
        /// <summary> The ID of the shelf. </summary>
        [SerializeField]
        [Tooltip("The ID of the shelf.")]
        private string ShelfID;
        /// <summary> The ID of the shelf. </summary>
        internal string shelfID {
            get { return ShelfID; }
        }

        /// <summary>
        /// Sets the color of the shelf icon.
        /// </summary>
        /// <param name="color">The color of the shelf icon.</param>
        internal void SetShelfColor(Color color) {
            GetComponent<Image>().color = color;
        }
    }
}