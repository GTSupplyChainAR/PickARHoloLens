using UnityEngine;
using System.Collections.Generic;
using PickAR.UI;

namespace PickAR.Navigation {

    /// <summary>
    /// Finds a path to a goal and highlights it.
    /// </summary>
    public class Navigator : MonoBehaviour {

        /// <summary> All waypoints in the area. </summary>
        [HideInInspector]
        public Waypoint[] waypoints;

        /// <summary> The line renderer prefab to make lines with. </summary>
        [Tooltip("The line renderer prefab to make lines with.")]
        public PathLine lineRenderer;

        /// <summary> Whether to check if all edges are two-way. </summary>
        [SerializeField]
        [Tooltip("Whether to check if all edges are two-way.")]
        private bool checkTwoWay;

        /// <summary> The next waypoint in the shortest path between the first and second waypoints. </summary>
        private Waypoint[,] next;

        /// <summary> Parent object for all path lines. </summary>
        private GameObject pathLines;

        /// <summary> The singleton instance of the object. </summary>
        public static Navigator instance {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Awake() {
            instance = this;

            waypoints = FindObjectsOfType<Waypoint>();
            pathLines = new GameObject();
            pathLines.name = "Path Lines";

            if (checkTwoWay) {
                foreach (Waypoint waypoint in waypoints) {
                    foreach (Waypoint other in waypoint.adjacent) {
                        bool twoWay = false;
                        foreach (Waypoint check in other.adjacent) {
                            if (check == waypoint) {
                                twoWay = true;
                            }
                        }
                        if (twoWay) {
                            DrawLine(waypoint, other);
                        } else {
                            Debug.Log("Missing edge between " + other.name + " and " + waypoint.name + ".");
                        }
                    }
                }
            }

            int numWaypoints = waypoints.Length;
            int indexCounter = 0;
            foreach (Waypoint waypoint in waypoints) {
                waypoint.index = indexCounter++;
            }

            // Floyd-Warshall.
            float[,] dist = new float[numWaypoints, numWaypoints];
            next = new Waypoint[numWaypoints, numWaypoints];
            for (int i = 0; i < numWaypoints; i++) {
                for (int j = 0; j < numWaypoints; j++) {
                    dist[i, j] = i == j ? 0 : Mathf.Infinity;
                    next[i, j] = waypoints[j];
                }
            }
            foreach (Waypoint waypoint in waypoints) {
                foreach (Waypoint other in waypoint.adjacent) {
                    dist[waypoint.index, other.index] = GetDistance(waypoint, other);
                }
                waypoint.gameObject.SetActive(false);
            }

            for (int k = 0; k < numWaypoints; k++) {
                for (int i = 0; i < numWaypoints; i++) {
                    for (int j = 0; j < numWaypoints; j++) {
                        if (dist[i, j] > dist[i, k] + dist[k, j]) {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the raw distance between two waypoints.
        /// </summary>
        /// <returns>The raw distance between two waypoints.</returns>
        /// <param name="start">The starting waypoint.</param>
        /// <param name="end">The ending waypoint.</param>
        private float GetDistance(Waypoint start, Waypoint end) {
            return Vector3.Distance(start.transform.position, end.transform.position);
        }

        /// <summary>
        /// Draws the shortest path between two points
        /// </summary>
        /// <returns>The shortest path between two points, or an empty list if there isn't a path.</returns>
        /// <param name="start">The starting point of the path.</param>
        /// <param name="end">The ending point of the path.</param>
        public List<Waypoint> DrawShortestPath(Waypoint start, Waypoint end) {
            List<Waypoint> path = GetShortestPath(start, end);
            for (int i = 0; i < path.Count - 1; i++) {
                DrawLine(path[i], path[i + 1]);
            }
            return path;
        }

        /// <summary>
        /// Draws the shortest path between two points
        /// </summary>
        /// <returns>The shortest path between two points, or an empty list if there isn't a path.</returns>
        /// <param name="start">The starting point of the path.</param>
        /// <param name="end">The ending point of the path.</param>
        public void DrawShortestPath(Vector3 start, Vector3 end) {
            RemoveLines();
            Waypoint startPoint = GetClosestWaypoint(start);
            Waypoint endPoint = GetClosestWaypoint(end);
            DrawLine(start, startPoint.transform.position);
            if (startPoint != endPoint) {
                DrawShortestPath(startPoint, endPoint);
            }
            DrawLine(endPoint.transform.position, end);
        }

        /// <summary>
        /// Finds the shortest path between two points.
        /// </summary>
        /// <returns>The shortest path between two points, or an empty list if there isn't a path.</returns>
        /// <param name="start">The starting point of the path.</param>
        /// <param name="end">The ending point of the path.</param>
        public List<Waypoint> GetShortestPath(Waypoint start, Waypoint end) {
            List<Waypoint> path = new List<Waypoint>();
            Waypoint current = start;
            if (next[start.index, end.index] != null) {
                path.Add(start);
                while (current != end) {
                    current = next[current.index, end.index];
                    path.Add(current);
                }
            }
            return path;
        }

        /// <summary>
        /// Draws a line between two waypoints.
        /// </summary>
        /// <param name="start">The starting waypoint of the line.</param>
        /// <param name="end">The ending waypoint of the line.</param>
        private void DrawLine(Waypoint start, Waypoint end) {
            DrawLine(start.transform.position, end.transform.position);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="start">The starting points of the line.</param>
        /// <param name="end">The ending points of the line.</param>
        private void DrawLine(Vector3 start, Vector3 end) {
            PathLine line = Instantiate(lineRenderer) as PathLine;
            line.SetEndpoints(start, end);
            line.transform.parent = pathLines.transform;
        }

        /// <summary>
        /// Gets the closest waypoint to a position.
        /// </summary>
        /// <returns>The closest waypoint to the position.</returns>
        /// <param name="position">The position to get the closest waypoint from.</param>
        public Waypoint GetClosestWaypoint(Vector3 position) {
            float closestDistance = Mathf.Infinity;
            Waypoint closestWaypoint = null;
            float currentDistance;
            foreach (Waypoint waypoint in waypoints) {
                currentDistance = waypoint.GetDistance(position);
                if (closestDistance > currentDistance) {
                    closestDistance = currentDistance;
                    closestWaypoint = waypoint;
                }
            }
            return closestWaypoint;
        }

        /// <summary>
        /// Removes all drawn lines.
        /// </summary>
        public void RemoveLines() {
            foreach (Transform child in pathLines.transform) {
                Destroy(child.gameObject);
            }
        }
    }
}