using UnityEngine;

/// <summary>
/// A junction point in the graph of the area.
/// </summary>
public class Waypoint : MonoBehaviour {

    /// <summary> The waypoints adjacent to this point. </summary>
    [Tooltip("The waypoints adjacent to this point.")]
    public Waypoint[] adjacent;

    /// <summary> The unique index of the waypoint. </summary>
    [HideInInspector]
    public int index;

    /// <summary>
    /// Gets the distance between the waypoint and a position.
    /// </summary>
    /// <returns>The distance between the waypoint and the position.</returns>
    /// <param name="position">The position to get the distance from the waypoint.</param>
    public float GetDistance(Vector3 position) {
        return Vector3.Distance(position, transform.position);
    }
}