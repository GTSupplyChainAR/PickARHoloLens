using UnityEngine;
using System.Collections.Generic;
using PickAR.Managers;
using PickAR.UI;

namespace PickAR.Navigation {

    /// <summary>
    /// Finds a path to a goal and highlights it.
    /// </summary>
    public class Navigator : MonoBehaviour {

        /// <summary> All waypoints in the area. </summary>
        [HideInInspector]
        public Waypoint[] waypoints;

        /// <summary> Whether to load waypoints from a file. </summary>
        [SerializeField]
        [Tooltip("Whether to load waypoints from a file.")]
        private bool useWaypointFile;

        /// <summary> The next waypoint in the shortest path between the first and second waypoints. </summary>
        private Waypoint[,] next;

        /// <summary> The object pool to get path line objects from. </summary>
        private PathRenderer pathRenderer;

        /// <summary> The next point that the user has to go to in the path. </summary>
        public Vector3 nextPoint {
            get;
            private set;
        }

        /// <summary> The singleton instance of the object. </summary>
        public static Navigator instance {
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
            pathRenderer = GetComponent<PathRenderer>();

            if (useWaypointFile) {
                waypoints = WaypointStorage.instance.LoadWaypoints();
            } else {
                waypoints = FindObjectsOfType<Waypoint>();

                int indexCounter = 0;
                foreach (Waypoint waypoint in waypoints) {
                    waypoint.index = indexCounter++;
                }
            }

            findShortestPaths();
        }

        /// <summary>
        /// Finds all-pair shortest paths between waypoints.
        /// </summary>
        private void findShortestPaths() {
            int numWaypoints = waypoints.Length;

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
        public void DrawShortestPath(Vector3 start, Vector3 end) {
            Waypoint startPoint = GetClosestWaypoint(start);
            Waypoint endPoint = GetClosestWaypoint(end);
            if (startPoint != endPoint) {
                List<Waypoint> pathWaypoints = GetShortestPath(startPoint, endPoint);
                List<Vector3> path = new List<Vector3>();
                path.Add(start);
                foreach (Waypoint waypoint in pathWaypoints) {
                    path.Add(waypoint.transform.position);
                }
                path.Add(end);
                pathRenderer.DrawPath(path);
                nextPoint = startPoint.transform.position;
                Vector3 firstDirection = startPoint.transform.position - start;
                firstDirection.y = 0;
                firstDirection.Normalize();
                if (firstDirection != Vector3.zero) {
                    Vector3 secondDirection = waypoints[1].transform.position - startPoint.transform.position;
                    secondDirection.y = 0;
                    secondDirection.Normalize();
                    if (Vector3.Angle(firstDirection, secondDirection) > 115) {
                        nextPoint = waypoints[1].transform.position;
                    }

                }

            } else {
                List<Vector3> path = new List<Vector3>();
                path.Add(start);
                path.Add(end);
                pathRenderer.DrawPath(path);
                nextPoint = end;
            }
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
        /// Finds the shortest path between two points.
        /// </summary>
        /// <returns>The shortest path between two points, or an empty list if there isn't a path.</returns>
        /// <param name="start">The starting point of the path.</param>
        /// <param name="end">The ending point of the path.</param>
        public float GetPathDistance(Vector3 start, Vector3 end) {
            Waypoint startPoint = GetClosestWaypoint(start);
            Waypoint endPoint = GetClosestWaypoint(end);
            float distance = 0;
            if (startPoint == endPoint) {
                distance = Vector3.Distance(start, end);
            } else {
                List<Waypoint> shortestPath = GetShortestPath(startPoint, endPoint);
                for (int i = 0; i < shortestPath.Count - 1; i++) {
                    distance += Waypoint.GetDistance(shortestPath[i], shortestPath[i + 1]);
                }
                distance += startPoint.GetDistance(start);
                distance += endPoint.GetDistance(end);
            }
            return distance;
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
            pathRenderer.RemoveLines();
        }
    }
}