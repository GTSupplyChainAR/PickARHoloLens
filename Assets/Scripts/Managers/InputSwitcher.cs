using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace PickAR.Managers {

    /// <summary>
    /// Toggles between desktop and HoloLens input.
    /// </summary>
    class InputSwitcher : MonoBehaviour {

        /// <summary>
        /// Types of input that the application can take.
        /// </summary>
        private enum InputType {
            Desktop,
            Hololens
        };

        /// <summary> The source of input that the application should take. </summary>
        [SerializeField]
        [Tooltip("The source of input that the application should take.")]
        private InputType inputType;
        /// <summary> Objects to enable in desktop mode only. </summary>
        [SerializeField]
        [Tooltip("Objects to enable in desktop mode only.")]
        private GameObject[] desktopObjects;
        /// <summary> Objects to enable in HoloLens mode only. </summary>
        [SerializeField]
        [Tooltip("Objects to enable in HoloLens mode only.")]
        private GameObject[] hololensObjects;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Awake() {
            bool isDesktop = inputType == InputType.Desktop;
            GameObject player = null;
            foreach (GameObject desktopObject in desktopObjects) {
                desktopObject.SetActive(isDesktop);
                if (desktopObject.name == "Player") {
                    player = desktopObject;
                }
            }
            foreach (GameObject hololensObject in hololensObjects) {
                hololensObject.SetActive(!isDesktop);
            }
            GetComponent<GazeGestureManager>().enabled = !isDesktop;
            GetComponent<InputManager>().enabled = !isDesktop;

            if (isDesktop) {
                GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
                camera.transform.parent = player.transform;
            }
        }
    }
}