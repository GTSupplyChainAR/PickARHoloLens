using UnityEngine;

namespace PlayerController {

    /// <summary>
    /// Allows the player to select objects that are being looked at with the mouse.
    /// </summary>
    class Selector : MonoBehaviour {

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

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            playerCamera = GetComponentInChildren<Camera>();
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            Vector3 forward = playerCamera.transform.forward;
            if (Input.GetButtonDown("Fire1")) {
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.transform.position, forward, out hit, maxSelectDistance, layerMask)) {
                    Selectable selectable = hit.collider.GetComponent<Selectable>();
                    if (selectable != null) {
                        selectable.OnSelect();
                    }
                }
            }
        }
    }
}