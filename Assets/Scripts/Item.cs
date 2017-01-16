using UnityEngine;

namespace PickAR {

    /// <summary>
    /// An item that is part of the warehouse inventory.
    /// </summary>
    class Item : MonoBehaviour {

        /// <summary> The aisle number of the item. </summary>
        [SerializeField]
        [Tooltip("The aisle number of the item.")]
        private string aisleNumber;
    }
}