using UnityEngine;
using PickAR.Inputs;
using PickAR.Managers;

namespace PlayerController {

    /// <summary>
    /// Allows the player to select objects that are being looked at with the mouse.
    /// </summary>
    public class Selector : MonoBehaviour {

        /// <summary> The camera attached to the player. </summary>
        private Camera playerCamera;
        /// <summary> The range of selection. </summary>
        [SerializeField]
        [Tooltip("The range of selection.")]
        private float maxSelectDistance;
        /// <summary> The layer mask for selectable objects. </summary>
        [SerializeField]
        [Tooltip("The layer mask for selectable objects.")]
        private LayerMask layerMask;

        /// <summary> The singleton instance of the object. </summary>
        public static Selector instance {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the object.
        /// </summary>
        private void Awake() {
            instance = this;
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            playerCamera = InputSwitcher.instance.playerCamera;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            Vector3 forward = playerCamera.transform.forward;
            if (Input.GetButtonUp(Buttons.SELECT)) {
                Selectable selectable = GetLookSelectable();
                if (selectable != null) {
                    selectable.OnSelect();
                }
            }
        }

        /// <summary>
        /// Gets the selectable object currently being looked at.
        /// </summary>
        /// <returns>The selectable object currently being looked at, or null if there isn't any.</returns>
        public GameObject GetLookObject() {
            Vector3 forward = playerCamera.transform.forward;
            RaycastHit hit;
            GameObject lookObject = null;
            if (Physics.Raycast(playerCamera.transform.position, forward, out hit, maxSelectDistance, layerMask)) {
                lookObject = hit.collider.gameObject;
            }
            Selectable selectable = null;
            if (lookObject != null) {
                selectable = lookObject.GetComponent<Selectable>();
            }
            return selectable == null ? null : lookObject;
        }

        /// <summary>
        /// Gets the selectable currently being looked at.
        /// </summary>
        /// <returns>The selectable currently being looked at, or null if there isn't any.</returns>
        public Selectable GetLookSelectable() {
            GameObject lookObject = GetLookObject();
            if (lookObject != null) {
                return lookObject.GetComponent<Selectable>();
            }
            return null;
        }
    }
}