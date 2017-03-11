using UnityEngine;
using PickAR.Inputs;
using PickAR.Managers;
using PlayerController;

namespace PickAR.Navigation {

    /// <summary>
    /// Handles waypoint creation while in waypoint placement mode.
    /// </summary>
    public class WaypointCreator : MonoBehaviour {

        /// <summary> The camera attached to the user. </summary>
        private Camera playerCamera;

        /// <summary> The distance in front of the user that a waypoint is created at. </summary>
        [SerializeField]
        [Tooltip("The distance in front of the user that a waypoint is created at.")]
        private float createDistance;

        /// <summary> The prefab for the in-progress connector. </summary>
        [SerializeField]
        [Tooltip("The prefab for the in-progress connector.")]
        private LineRenderer selectedConnectorPrefab;

        [Header("Materials")]
        /// <summary> The material used for unselected waypoints. </summary>
        [SerializeField]
        [Tooltip("The material used for unselected waypoints.")]
        private Material unselectedMaterial;
        /// <summary> The material used for hovered-over waypoints. </summary>
        [SerializeField]
        [Tooltip("The material used for hovered-over waypoints.")]
        private Material hoverMaterial;
        /// <summary> The material used for selected waypoints. </summary>
        [SerializeField]
        [Tooltip("The material used for selected waypoints.")]
        private Material selectedMaterial;
        /// <summary> The material used for dragged waypoints. </summary>
        [SerializeField]
        [Tooltip("The material used for dragged waypoints.")]
        private Material dragMaterial;

        /// <summary> The waypoint currently being hovered over. </summary>
        private Waypoint hoverWaypoint;
        /// <summary> The waypoint currently being selected. </summary>
        private Waypoint selectedWaypoint;
        /// <summary> The waypoint currently being dragged. </summary>
        private Waypoint draggedWaypoint;

        /// <summary> The connector that the user is currently dragging. </summary>
        private LineRenderer selectedConnector;

        /// <summary> Whether a waypoint is currently being dragged. </summary>
        private bool isDragging;

        /// <summary> The singleton instance of the object. </summary>
        public static WaypointCreator instance {
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
            selectedConnector = GameObject.Instantiate(selectedConnectorPrefab);
            selectedConnector.gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            if (ModeSwitcher.isWaypointPlacement) {
                GameObject selectable = Selector.instance.GetLookObject();
                Waypoint waypoint = null;
                if (selectable != null) {
                    waypoint = ((GameObject) selectable).GetComponent<Waypoint>();
                }
                if (Input.GetButtonDown(Buttons.CREATE)) {
                    if (selectable == null) {
                        CreateWaypoint();
                    } else {
                        if (waypoint != null) {
                            waypoint.Delete();
                        }
                    }
                } else {
                    Waypoint currentHover = null;
                    if (waypoint != selectedWaypoint) {
                        currentHover = waypoint;
                    }
                    if (hoverWaypoint != currentHover) {
                        if (!isDragging || hoverWaypoint == null) {
                            if (hoverWaypoint != null && hoverWaypoint != selectedWaypoint) {
                                hoverWaypoint.SetMaterial(unselectedMaterial);
                            }
                            if (currentHover != null) {
                                currentHover.SetMaterial(hoverMaterial);
                            }
                            hoverWaypoint = currentHover;
                        }
                    }
                }

                bool connectorActive = false;
                if (selectedWaypoint != null) {
                    // Move the connector.
                    selectedConnector.SetPosition(0, selectedWaypoint.transform.position);
                    connectorActive = true;
                    if (waypoint == selectedWaypoint) {
                        connectorActive = false;
                    } else {
                        Vector3 targetPosition = waypoint == null ? GetSelectPosition() : waypoint.transform.position;
                        selectedConnector.SetPosition(1, targetPosition);
                    }
                }
                selectedConnector.gameObject.SetActive(connectorActive);

				if (InputSwitcher.isHoloLens && Input.GetButtonDown(Buttons.SELECT)) {
					waypoint.OnSelect();
				}

                if (Input.GetButtonDown(Buttons.DRAG)) {
                    Waypoint dragWaypoint = waypoint;
                    if (!isDragging && dragWaypoint != null) {
                        if (selectedWaypoint != null) {
                            if (connectorActive) {
                                SelectWaypoint(waypoint);
                            }
                            selectedWaypoint = null;
                        }
                        isDragging = true;
                        draggedWaypoint = dragWaypoint;
                        draggedWaypoint.SetMaterial(dragMaterial);
                    } else if (isDragging) {
                        isDragging = false;
                        draggedWaypoint.SetMaterial(hoverMaterial);
                        draggedWaypoint = null;
                    }
                }
                if (draggedWaypoint != null) {
                    draggedWaypoint.MoveWaypoint(GetSelectPosition());
                    draggedWaypoint.SetMaterial(dragMaterial);
                }
            }
        }

        /// <summary>
        /// Resets the creator when switching to order-picking mode.
        /// </summary>
        public void ResetCreator() {
            if (selectedWaypoint != null) {
                selectedWaypoint.SetMaterial(unselectedMaterial);
                selectedWaypoint = null;
            }
            if (hoverWaypoint != null) {
                hoverWaypoint.SetMaterial(unselectedMaterial);
                hoverWaypoint = null;
            }
            if (draggedWaypoint != null) {
                draggedWaypoint.SetMaterial(unselectedMaterial);
                draggedWaypoint = null;
            }
            isDragging = false;
            selectedConnector.gameObject.SetActive(false);
        }

        /// <summary>
        /// Gets the position where new waypoints and connectors are positioned at.
        /// </summary>
        /// <returns>The position where new waypoints and connectors are positioned at.</returns>
        private Vector3 GetSelectPosition() {
            return playerCamera.transform.position + playerCamera.transform.forward * createDistance;
        }

        /// <summary>
        /// Creates a waypoint in front of the user.
        /// </summary>
        private void CreateWaypoint() {
            Vector3 waypointPosition = GetSelectPosition();
            Waypoint waypoint = GameObject.Instantiate(WaypointStorage.instance.waypointPrefab, waypointPosition, Quaternion.identity);
            waypoint.transform.parent = WaypointStorage.instance.waypointParent.transform;
            Navigator.instance.waypoints.Add(waypoint);
            SelectWaypoint(waypoint);
        }

        /// <summary>
        /// Selects the waypoint. Links the current connector to it if possible.
        /// </summary>
        /// <param name="waypoint">Waypoint.</param>
        internal void SelectWaypoint(Waypoint waypoint) {
            if (selectedWaypoint != null) {
                selectedWaypoint.SetMaterial(unselectedMaterial);
            }
            // If the current waypoint is already selected, unselect it.
            if (selectedWaypoint == waypoint) {
                selectedWaypoint = null;
                return;
            }

            // Connect the connector to the new waypoint.
            if (selectedConnector.gameObject.activeSelf) {
                CreateConnection(selectedWaypoint, waypoint);
            }

            selectedWaypoint = waypoint;
            selectedWaypoint.SetMaterial(selectedMaterial);

            isDragging = false;
            draggedWaypoint = null;
        }

        /// <summary>
        /// Removes references to a waypoint about to be deleted.
        /// </summary>
        /// <param name="waypoint">The waypoint about to be deleted.</param>
        internal void OnDelete(Waypoint waypoint) {
            if (selectedWaypoint == waypoint) {
                selectedWaypoint = null;
            }
            if (hoverWaypoint == waypoint) {
                hoverWaypoint = null;
            }
        }

        /// <summary>
        /// Creates a connection between two waypoints. Removes the connection if there is already one.
        /// </summary>
        /// <param name="waypoint1">The first waypoint to create a connection between.</param>
        /// <param name="waypoint2">The second waypoint to create a connection between.</param>
        internal static void CreateConnection(Waypoint waypoint1, Waypoint waypoint2) {
            WaypointConnector connector = waypoint1.GetConnectorLink(waypoint2);
            if (connector != null) {
                // Connection already exists; remove it.
                waypoint1.adjacent.Remove(waypoint2);
                waypoint2.adjacent.Remove(waypoint1);
                waypoint1.connectors.Remove(connector);
                waypoint2.connectors.Remove(connector);
                Destroy(connector.gameObject);
                return;
            }

            connector = GameObject.Instantiate(WaypointStorage.instance.waypointConnectorPrefab);
            connector.transform.parent = WaypointStorage.instance.connectorParent.transform;
            connector.name = "Waypoint Connector";
            connector.SetWaypoints(waypoint1, waypoint2);
            waypoint1.adjacent.Add(waypoint2);
            waypoint2.adjacent.Add(waypoint1);
            waypoint1.connectors.Add(connector);
            waypoint2.connectors.Add(connector);
        }
    }
}