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
        public enum InputType {
            Desktop,
            Hololens
        };

        /// <summary> The source of input that the application should take. </summary>
        [SerializeField]
        [Tooltip("The source of input that the application should take.")]
        private InputType _inputType;
        /// <summary> The source of input that the application should take. </summary>
        public InputType inputType {
            get { return _inputType; }
        }
        /// <summary> Whether the application is currently in HoloLens mode. </summary>
        public static bool isHoloLens {
            get { return instance.inputType == InputType.Hololens; }
        }
        /// <summary> Whether the application is currently in desktop mode. </summary>
        public static bool isDesktop {
            get { return instance.inputType == InputType.Desktop; }
        }

        /// <summary> Objects to enable in desktop mode only. </summary>
        [SerializeField]
        [Tooltip("Objects to enable in desktop mode only.")]
        private GameObject[] desktopObjects;
        /// <summary> Objects to enable in HoloLens mode only. </summary>
        [SerializeField]
        [Tooltip("Objects to enable in HoloLens mode only.")]
        private GameObject[] hololensObjects;

        /// <summary> The navigation UIs for each input. </summary>
        [SerializeField]
        [Tooltip("The navigation UIs for each input.")]
        private GameObject[] navigationScreens;

        /// <summary> Virtual objects to make invisible in HoloLens mode. </summary>
        [SerializeField]
        [Tooltip("Virtual objects to make invisible in HoloLens mode.")]
        private GameObject[] virtualObjects;

        /// <summary> The camera keeping track of the user. </summary>
        public Camera playerCamera {
            get;
            private set;
        }

        /// <summary> The singleton instance of the object. </summary>
        public static InputSwitcher instance {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Awake() {
            instance = this;

            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

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
                playerCamera.transform.parent = player.transform;
            }

            if (isHoloLens) {
                foreach (GameObject virtualObject in virtualObjects) {
                    foreach (Renderer renderer in virtualObject.GetComponentsInChildren<Renderer>()) {
                        renderer.enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the navigation UI in use.
        /// </summary>
        /// <returns>The navigation UI in use.</returns>
        public GameObject GetNavigationUI() {
            return navigationScreens[(int) inputType];
        }
    }
}