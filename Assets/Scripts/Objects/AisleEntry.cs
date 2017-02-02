using UnityEngine;

namespace PickAR.Objects {

    /// <summary>
    /// 
    /// </summary>
    public class AisleEntry {

        /// <summary> The shelf of the entry. </summary>
        public string shelf {
            get;
            private set;
        }
        /// <summary> The column that the entry is in. </summary>
        public int column {
            get;
            private set;
        }
        /// <summary> The row that the entry is in. </summary>
        public int row {
            get;
            private set;
        }

        /// <summary>
        /// Initializes an aisle number entry.
        /// </summary>
        /// <param name="shelf">The shelf of the entry.</param>
        /// <param name="column">The column that the entry is in.</param>
        /// <param name="row">The row that the entry is in.</param>
        public AisleEntry(string shelf, int column, int row) {
            this.shelf = shelf;
            this.column = column;
            this.row = row;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="PickAR.Objects.AisleNumber"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="PickAR.Objects.AisleNumber"/>.</returns>
        public override string ToString() {
            return shelf + column.ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PickAR.Objects.AisleEntry"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PickAR.Objects.AisleEntry"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="PickAR.Objects.AisleEntry"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) {
            AisleEntry other = obj as AisleEntry;
            if (other != null) {
                return other.shelf == shelf && other.column == column && other.row == row;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return (shelf + column.ToString() + "," + row.ToString()).GetHashCode();
        }
    }
}