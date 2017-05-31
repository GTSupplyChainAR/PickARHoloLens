using UnityEngine;
using PickAR.Util;

namespace PickAR.Objects {

    /// <summary>
    /// Data about a warehouse shelf.
    /// </summary>
    public struct Shelf {

        /// <summary> The dimensions of the shelf. </summary>
        public Vector2Int dimensions;
        /// <summary> Whether to mirror the shelf on the display. </summary>
        public bool mirrored;

        /// <summary>
        /// Initializes a shelf.
        /// </summary>
        /// <param name="dimensions">The dimensions of the shelf.</param>
        /// <param name="mirrored">Whether to mirror the shelf on the display.</param>
        public Shelf(Vector2Int dimensions, bool mirrored) {
            this.dimensions = dimensions;
            this.mirrored = mirrored;
        }
    }
}