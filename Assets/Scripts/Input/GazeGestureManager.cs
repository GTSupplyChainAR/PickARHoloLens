using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }

    GestureRecognizer recognizer;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            // Send an OnSelect message to the focused object and its ancestors.
            if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("OnSelect");
            }
        };
        recognizer.StartCapturingGestures();
    }

    // Update is called once per frame
    void Update()
    {
        // Figure out which hologram is focused this frame.
        GameObject oldFocusObject = FocusedObject;

		FocusedObject = GetFocusedObject();

        // If the focused object changed this frame,
        // start detecting fresh gestures again.
        if (FocusedObject != oldFocusObject)
        {
            recognizer.CancelGestures();
            recognizer.StartCapturingGestures();
        }
    }

	/// <summary>
	/// Gets the object being looked at by the camera.
	/// </summary>
	/// <returns>The object being looked at by the camera.</returns>
	public static GameObject GetFocusedObject() {
		// Do a raycast into the world based on the user's
		// head position and orientation.
		var headPosition = Camera.main.transform.position;
		var gazeDirection = Camera.main.transform.forward;

		RaycastHit hitInfo;
		if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
		{
			// If the raycast hit a hologram, use that as the focused object.
			return hitInfo.collider.gameObject;
		}
		else
		{
			// If the raycast did not hit a hologram, clear the focused object.
			return null;
		}
	}
}
