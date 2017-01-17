using UnityEngine;
using PlayerController;
using PickAR.Managers;

namespace PickAR.Objects {

    /// <summary>
    /// An item that is part of the warehouse inventory.
    /// </summary>
    public class Item : MonoBehaviour, Selectable {

        /// <summary> The aisle number of the item. </summary>
        [SerializeField]
        [Tooltip("The aisle number of the item.")]
        private string AisleNumber;
        /// <summary> The aisle number of the item. </summary>
        public string aisleNumber {
            get { return AisleNumber; }
        }
        /// <summary> The unique ID of the item. </summary>
        [SerializeField]
        [Tooltip("The unique ID of the item.")]
        private int ItemID;
        /// <summary> The unique ID of the item. </summary>
        public int itemID {
            get { return ItemID; }
        }

        /// <summary>
        /// Triggers when the object is selected.
        /// </summary>
        public void OnSelect() {
            JobManager.instance.SelectItem(itemID);
        }
    }
}