using UnityEngine;
using System.Collections.Generic;
using PickAR.Objects;
using PickAR.Util;

namespace PickAR.Managers {

    /// <summary>
    /// Keeps track of the locations of items in the warehouse.
    /// </summary>
    public class InventoryManager : MonoBehaviour {

        /// <summary> Maps item IDs to aisle entries. </summary>
        private Dictionary<int, AisleEntry> aisleDict = new Dictionary<int, AisleEntry>();
        /// <summary> Maps shelf IDs to shelf data. </summary>
        private Dictionary<string, Shelf> shelfDict = new Dictionary<string, Shelf>();

        /// <summary> The singleton instance of the object. </summary>
        public static InventoryManager instance {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the object.
        /// </summary>
        private void Awake() {
            instance = this;
            LoadAisleDict();
            LoadShelves();
        }

        /// <summary>
        /// Registers the locations of items on shelves.
        /// </summary>
        private void LoadAisleDict() {
            aisleDict.Add(0, new AisleEntry("A", 1, 1));
            aisleDict.Add(1, new AisleEntry("A", 3, 2));
            aisleDict.Add(2, new AisleEntry("A", 4, 4));
            aisleDict.Add(3, new AisleEntry("B", 2, 3));
            aisleDict.Add(4, new AisleEntry("B", 3, 4));
            aisleDict.Add(5, new AisleEntry("B", 5, 2));
        }

        /// <summary>
        /// Loads the shelves that need to be mirrored.
        /// </summary>
        private void LoadShelves() {
            shelfDict.Add("A", new Shelf(new Vector2Int(5, 4), false));
            shelfDict.Add("B", new Shelf(new Vector2Int(5, 4), true));
            shelfDict.Add("C", new Shelf(new Vector2Int(5, 4), false));
            shelfDict.Add("D", new Shelf(new Vector2Int(5, 4), true));
        }

        /// <summary>
        /// Gets the aisle entry of an item.
        /// </summary>
        /// <returns>The aisle entry of the given item.</returns>
        /// <param name="itemID">The item to get an aisle entry for.</param>
        public AisleEntry GetAisleEntry(Item item) {
            return GetAisleEntry(item.itemID);
        }

        /// <summary>
        /// Gets the aisle entry of an item.
        /// </summary>
        /// <returns>The aisle entry of the given item.</returns>
        /// <param name="itemID">The item ID to get an aisle entry for.</param>
        public AisleEntry GetAisleEntry(int itemID) {
            return aisleDict[itemID];
        }

        /// <summary>
        /// Gets the aisle number of an item.
        /// </summary>
        /// <returns>The aisle number of the given item.</returns>
        /// <param name="itemID">The item ID to get an aisle number for.</param>
        public string GetAisleNumber(int itemID) {
            return GetAisleEntry(itemID).ToString();
        }

        /// <summary>
        /// Gets data about a certain shelf.
        /// </summary>
        /// <returns>Data about the specified shelf.</returns>
        /// <param name="shelf">The shelf ID.</param>
        public Shelf GetShelfData(string shelf) {
            return shelfDict[shelf];
        }
    }
}