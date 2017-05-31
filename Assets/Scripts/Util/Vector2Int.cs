using UnityEngine;

namespace PickAR.Util {

    /// <summary>
    /// A 2D vector of ints.
    /// </summary>
    public struct Vector2Int {
        /// <summary> The x coordinate of the vector. </summary>
        public int x;
        /// <summary> The y coordinate of the vector. </summary>
        public int y;

        /// <summary>
        /// Initializes a 2D vector.
        /// </summary>
        /// <param name="x">The x coordinate of the vector.</param>
        /// <param name="y">The y coordinate of the vector.</param>
        public Vector2Int(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
}