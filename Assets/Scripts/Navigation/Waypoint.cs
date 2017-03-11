using UnityEngine;
using System.Collections.Generic;
using PlayerController;
using PickAR.Managers;

namespace PickAR.Navigation {
    /// <summary>
    /// A junction point in the graph of the area.
    /// </summary>
	public class Waypoint : MonoBehaviour, Selectable {

        /// <summary> The waypoints adjacent to this point. </summary>
        [Tooltip("The waypoints adjacent to this point.")]
        public List<Waypoint> adjacent;

        /// <summary> The unique index of the waypoint. </summary>
        [HideInInspector]
        public int index;

        /// <summary> Lines connecting the waypoint to its adjacencies. </summary>
        internal List<WaypointConnector> connectors = new List<WaypointConnector>();

        /// <summary>
        /// Gets the distance between the waypoint and a position.
        /// </summary>
        /// <returns>The distance between the waypoint and the position.</returns>
        /// <param name="position">The position to get the distance from the waypoint.</param>
        public float GetDistance(Vector3 position) {
            return Vector3.Distance(position, transform.position);
        }

        /// <summary>
        /// Gets the distance between two waypoints.
        /// </summary>
        /// <returns>The distance between the two waypoints.</returns>
        /// <param name="waypoint1">The first waypoint to get a distance between.</param>
        /// <param name="waypoint2">The second waypoint to get a distance between.</param>
        public static float GetDistance(Waypoint waypoint1, Waypoint waypoint2) {
            return Vector3.Distance(waypoint1.transform.position, waypoint2.transform.position);
        }

        /// <summary>
        /// Gets the waypoint connector linking this waypoint to another waypoint.
        /// </summary>
        /// <returns>The waypoint connector linking this waypoint to another waypoint, or null if there isn't one.</returns>
        /// <param name="other">The other waypoint to get a waypoint connector for.</param>
        internal WaypointConnector GetConnectorLink(Waypoint other) {
            foreach (WaypointConnector connector in connectors) {
                if (connector.ConnectsWaypoint(other)) {
                    return connector;
                }
            }
            return null;
        }

		/// <summary>
		/// Triggers when the object is selected.
		/// </summary>
		public void OnSelect() {
			WaypointCreator.instance.SelectWaypoint(this);
		}

        /// <summary>
        /// Deletes the waypoint.
        /// </summary>
        public void Delete() {
            foreach (Waypoint waypoint in adjacent) {
                waypoint.adjacent.Remove(this);
            }
            foreach (WaypointConnector connector in connectors) {
                connector.RemoveFromOther(this);
                Destroy(connector.gameObject);
            }
            Navigator.instance.waypoints.Remove(this);
            WaypointCreator.instance.OnDelete(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Sets the waypoint's material.
        /// </summary>
        /// <param name="material">The material to set the waypoint to.</param>
        internal void SetMaterial(Material material) {
            GetComponent<Renderer>().material = material;
        }

        /// <summary>
        /// Moves the waypoint connectors along with the waypoint.
        /// </summary>
        /// <param name="position">The position to move the waypoint to.</param>
        internal void MoveWaypoint(Vector3 position) {
            transform.position = position;
            foreach (WaypointConnector connector in connectors) {
                connector.MoveConnector(this);
            }
        }
    }
}