using UnityEngine;

namespace PickAR.Navigation {

    /// <summary>
    /// Connects two nodes together.
    /// </summary>
    class WaypointConnector : MonoBehaviour {

        /// <summary> The two waypoints that the node is connecting. </summary>
        internal Waypoint[] connections;
        /// <summary> Renders the line between the two waypoints. </summary>
        private LineRenderer line;

        /// <summary>
        /// Sets the two waypoints that the node is connecting.
        /// </summary>
        /// <param name="waypoint1">The first waypoint that the node is connecting.</param>
        /// <param name="waypoint2">The second waypoint that the node is connecting.</param>
        internal void SetWaypoints(Waypoint waypoint1, Waypoint waypoint2) {
            connections = new Waypoint[] { waypoint1, waypoint2 };
            line = GetComponent<LineRenderer>();
            line.SetPosition(0, waypoint1.transform.position);
            line.SetPosition(1, waypoint2.transform.position);
        }

        /// <summary>
        /// Checks if the node connector is connected to a waypoint.
        /// </summary>
        /// <returns>Whether the node connector is connected to the specified waypoint.</returns>
        /// <param name="search">The waypoint to check a connection for.</param>
        internal bool ConnectsWaypoint(Waypoint search) {
            foreach (Waypoint waypoint in connections) {
                if (waypoint == search) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the node from the waypoint connected to the specified waypoint.
        /// </summary>
        /// <param name="waypoint">The waypoint to get the other waypoint from.</param>
        internal void RemoveFromOther(Waypoint waypoint) {
            if (ConnectsWaypoint(waypoint)) {
                foreach (Waypoint other in connections) {
                    if (other != waypoint) {
                        other.connectors.Remove(this);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Moves an end of the connector with a waypoint.
        /// </summary>
        /// <param name="end">The waypoint endpoint to move the connector with.</param>
        internal void MoveConnector(Waypoint end) {
            for (int i = 0; i < connections.Length; i++) {
                if (connections[i] == end) {
                    line.SetPosition(i, end.transform.position);
                    break;
                }
            }
        }
    }
}