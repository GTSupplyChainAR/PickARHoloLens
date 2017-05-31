using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PickAR.Managers;
using PickAR.Objects;
using PickAR.Util;

namespace PickAR.UI {

    /// <summary>
    /// Controls the display of the minimap.
    /// </summary>
    class Minimap : MonoBehaviour {

        /// <summary> Maps shelf IDs to shelf displays. </summary>
        private Dictionary<string, MinimapShelf> shelfDict = new Dictionary<string, MinimapShelf>();
        /// <summary> The entry of the item currently selected. </summary>
        private AisleEntry currentEntry;

        /// <summary> The color of an unhighlighted shelf. </summary>
        private Color shelfColor;
        /// <summary> The color of a highlighted shelf. </summary>
        [SerializeField]
        [Tooltip("The color of a highlighted shelf.")]
        private Color shelfHighlightColor;

        /// <summary> The object containing all shelf icons. </summary>
        private GameObject shelfContainer;
        /// <summary> The grid on the minimap. </summary>
        private MinimapGrid grid;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            LoadShelfDict();
            grid = GetComponentInChildren<MinimapGrid>();
        }

        /// <summary>
        /// Registers all shelf displays.
        /// </summary>
        private void LoadShelfDict() {
            MinimapShelf[] shelves = GetComponentsInChildren<MinimapShelf>();
            bool first = true;
            foreach (MinimapShelf shelf in shelves) {
                if (first) {
                    first = false;
                    shelfColor = shelf.GetComponent<Image>().color;
                    shelfContainer = shelf.transform.parent.gameObject;
                }
                shelfDict.Add(shelf.shelfID, shelf);
            }
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            AisleEntry newEntry = JobManager.instance.itemAisleEntry;
            if (newEntry != null && (JobManager.instance.refreshItems || !newEntry.Equals(currentEntry))) {
                JobManager.instance.refreshItems = false;
                string shelfID = newEntry == null ? "" : newEntry.shelf;
                string currentShelf = currentEntry == null ? "" : currentEntry.shelf;
                if (shelfID != currentShelf) {
                    if (currentShelf != "") {
                        shelfDict[currentShelf].SetShelfColor(shelfColor);
                    }
                    if (shelfID != "") {
                        shelfDict[shelfID].SetShelfColor(shelfHighlightColor);
                    }
                    currentShelf = shelfID;
                }

                grid.ResetGrid();
                HighlightGridSquare(newEntry, true);
                JobManager jobManager = JobManager.instance;
                List<Item> items = jobManager.targetItems;
                foreach (Item item in items) {
                    if (item != jobManager.currentItem) {
                        AisleEntry itemEntry = InventoryManager.instance.GetAisleEntry(item);
                        if (itemEntry.shelf == currentShelf) {
                            HighlightGridSquare(itemEntry, false);
                        }
                    }
                }

                currentEntry = newEntry;
            }

            bool active = newEntry != null;
            shelfContainer.SetActive(active);
            grid.SetVisible(active);
        }

        /// <summary>
        /// Highlights a minimap grid square.
        /// </summary>
        /// <param name="entry">Aisle entry data to highlight a grid square with..</param>
        /// <param name="selected">Whether the grid square is the selected item.</param>
        private void HighlightGridSquare(AisleEntry entry, bool selected) {
            int x = entry.column - 1;
            int y = entry.row - 1;
            Shelf shelf = InventoryManager.instance.GetShelfData(entry.shelf);
            if (shelf.mirrored) {
                x = shelf.dimensions.x - 1 - x;
            }

            grid.HighlightSquare(x, y, selected);
        }
    }
}