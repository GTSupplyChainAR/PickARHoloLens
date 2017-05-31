using UnityEngine;

namespace PlayerController {

    /// <summary>
    /// An object that can be selected.
    /// </summary>
    public interface Selectable {

        /// <summary> Triggers when the object is selected. </summary>
        void OnSelect();
    }
}