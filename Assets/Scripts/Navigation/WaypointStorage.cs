using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace PickAR.Navigation {

    /// <summary>
    /// Stores waypoint data in a file.
    /// </summary>
    public class WaypointStorage : MonoBehaviour {

        /// <summary> The default file to get waypoint data from. </summary>
        [SerializeField]
        [Tooltip("The file to store waypoints in.")]
        private string _defaultFile;

        /// <summary> The default file to get waypoint data from. </summary>
        private string defaultFile {
            get { return _defaultFile == "" ? "" : Path.Combine(Application.streamingAssetsPath, _defaultFile); }
        }

        /// <summary> Whether to load the default JSON file regardless of whether there is waypoint data or not. </summary>
        [SerializeField]
        [Tooltip("Whether to load the default JSON file regardless of whether there is waypoint data or not.")]
        private bool loadDefaultFile;
        [Header("Prefabs")]
		/// <summary> The waypoint prefab to instantiate. </summary>
        [SerializeField]
        [Tooltip("The waypoint prefab to instantiate.")]
        private Waypoint _waypointPrefab;

        /// <summary> The waypoint prefab to instantiate. </summary>
        public Waypoint waypointPrefab {
            get { return _waypointPrefab; }
        }

        /// <summary> The waypoint connector prefab to instantiate from. </summary>
        [SerializeField]
        [Tooltip("The waypoint connector prefab to instantiate from.")]
        private WaypointConnector _waypointConnectorPrefab;

        /// <summary> The waypoint connector prefab to instantiate from. </summary>
        internal WaypointConnector waypointConnectorPrefab {
            get { return _waypointConnectorPrefab; }
        }

        /// <summary> The parent object of waypoint objects. </summary>
        public GameObject waypointParent {
            get;
            private set;
        }

        /// <summary> The parent object of connector objects. </summary>
        public GameObject connectorParent {
            get;
            private set;
        }

        /// <summary> The singleton instance of the object. </summary>
        public static WaypointStorage instance {
            get;
            private set;
        }

        /// <summary> PlayerPrefs key for waypoint JSON data. </summary>
        private const string WAYPOINT_KEY = "Waypoints";

        /// <summary>
        /// Initializes the singleton instance of the object.
        /// </summary>
        private void Awake() {
            instance = this;
        }

        /// <summary>
        /// Stores waypoint data in a file.
        /// </summary>
        /// <param name="waypoints">The waypoints to store.</param>
        public void StoreWaypoints(List<Waypoint> waypoints) {
            WaypointContainer container = new WaypointContainer(waypoints);
            string json = JsonUtility.ToJson(container);
            PlayerPrefs.SetString(WAYPOINT_KEY, json);
        }

        /// <summary>
        /// Loads waypoint data from a file.
        /// </summary>
        public List<Waypoint> LoadWaypoints() {
            waypointParent = new GameObject();
            waypointParent.name = "Waypoints";
            waypointParent.SetActive(false);

            connectorParent = new GameObject();
            connectorParent.transform.parent = waypointParent.transform;
            List<Waypoint> waypoints = new List<Waypoint>();

            string json = PlayerPrefs.GetString(WAYPOINT_KEY);
            if (loadDefaultFile || json == "") {
                Debug.Log("No waypoint data found.");
                if (defaultFile == "") {
                    return waypoints;
                } else {
                    Debug.Log("Using default waypoint file.");
					#if UNITY_EDITOR_WIN
                    byte[] bytes = UnityEngine.Windows.File.ReadAllBytes(defaultFile);
                    #else
                    byte[] bytes = File.ReadAllBytes(defaultFile);
                    #endif
                    json = System.Text.Encoding.ASCII.GetString(bytes);
                }
            }
            WaypointContainer container = JsonUtility.FromJson<WaypointContainer>(json);
            int numWaypoints = container.waypoints.Length;

            // Load waypoints from the file.
            for (int i = 0; i < numWaypoints; i++) {
                Waypoint waypoint = GameObject.Instantiate(waypointPrefab);
                WaypointData data = container.waypoints[i];
                waypoint.index = data.index;
                waypoint.transform.position = data.position;
                waypoint.name = "Waypoint " + waypoint.index.ToString();
                waypoint.transform.parent = waypointParent.transform;

                waypoints.Add(waypoint);
            }

            // Load adjacencies from the file.
            for (int i = 0; i < numWaypoints; i++) {
                Waypoint waypoint = waypoints[i];
                WaypointData data = container.waypoints[i];
                int numAdjacent = data.adjacent.Length;
                waypoint.adjacent = new List<Waypoint>();
                for (int j = 0; j < numAdjacent; j++) {
                    Waypoint other = waypoints[data.adjacent[j]];
                    waypoint.adjacent.Add(other);
                    WaypointConnector line;
                    if (waypoint.index < other.index) {
                        // Create a line between the two waypoints.
                        line = GameObject.Instantiate(waypointConnectorPrefab);
                        line.name = "Waypoint Connector";
                        line.transform.parent = connectorParent.transform;
                        line.SetWaypoints(waypoint, other);
                    } else {
                        line = other.GetConnectorLink(waypoint);
                    }
                    waypoint.connectors.Add(line);
                }
            }

            return waypoints;
        }
    }

    /// <summary>
    /// A serializable container of waypoint data.
    /// </summary>
    [Serializable]
    public struct WaypointContainer {
        /// <summary> Data about waypoints in the network. </summary>
        public WaypointData[] waypoints;

        /// <summary>
        /// Initializes a waypoint container
        /// </summary>
        /// <param name="waypointObjects">Waypoints in the network.</param>
        internal WaypointContainer(List<Waypoint> waypointObjects) {
            int numWaypoints = waypointObjects.Count;
            waypoints = new WaypointData[numWaypoints];
            for (int i = 0; i < numWaypoints; i++) {
                waypoints[i] = new WaypointData(waypointObjects[i]);
            }
        }

        /// <summary>
        /// Initializes a waypoint container
        /// </summary>
        /// <param name="waypoints">Data about waypoints in the network.</param>
        public WaypointContainer(WaypointData[] waypoints) {
            this.waypoints = waypoints;
        }
    }

    /// <summary>
    /// Data for a single waypoint.
    /// </summary>
    [Serializable]
    public struct WaypointData {
        /// <summary> The position of the waypoint. </summary>
        public Vector3 position;
        /// <summary> The unique index of the waypoint. </summary>
        public int index;
        /// <summary> The indices of waypoints adjacent to this waypoint. </summary>
        public int[] adjacent;

        /// <summary>
        /// Initializes a waypoint data object.
        /// </summary>
        /// <param name="waypoint">The waypoint to get data from.</param>
        internal WaypointData(Waypoint waypoint) {
            position = waypoint.transform.position;
            index = waypoint.index;
            int numAdjacent = waypoint.adjacent.Count;
            adjacent = new int[numAdjacent];
            for (int i = 0; i < numAdjacent; i++) {
                adjacent[i] = waypoint.adjacent[i].index;
            }
        }

        /// <summary>
        /// Initializes a waypoint data object.
        /// </summary>
        /// <param name="position">The position of the waypoint.</param>
        /// <param name="index">The unique index of the waypoint.</param>
        /// <param name="adjacent">The indices of waypoints adjacent to this waypoint.</param>
        public WaypointData(Vector3 position, int index, int[] adjacent) {
            this.position = position;
            this.index = index;
            this.adjacent = adjacent;
        }
    }
}