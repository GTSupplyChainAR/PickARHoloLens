using UnityEngine;
using PickAR.Inputs;
using PickAR.Navigation;

namespace PickAR.Managers {

    /// <summary>
    /// Switches between job and waypoint placement modes.
    /// </summary>
    public class ModeSwitcher : MonoBehaviour {

        /// <summary>
        /// Modes that the application can be in.
        /// </summary>
        public enum Mode {
            OrderPicking,
            WaypointPlacement
        }

        /// <summary> The current mode that the application is in. </summary>
        private Mode mode;

        /// <summary> Whether the application is in order picking mode. </summary>
        public static bool isOrderPicking {
            get { return instance.mode == Mode.OrderPicking; }
        }
        /// <summary> Whether the application is in waypoint placement mode. </summary>
        public static bool isWaypointPlacement {
            get { return instance.mode == Mode.WaypointPlacement; }
        }

        /// <summary> The singleton instance of the object. </summary>
        public static ModeSwitcher instance {
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
        /// Updates the object.
        /// </summary>
        private void Update() {
            bool switchModes = false;
            if (InputSwitcher.isDesktop) {
                if (Input.GetButtonDown(Buttons.SWITCH)) {
                    switchModes = true;
                }
            }
            if (switchModes) {
                SwitchMode();
            }
        }

        /// <summary>
        /// Switches the mode that the application is in.
        /// </summary>
        private void SwitchMode() {
            GameObject navigationUI = InputSwitcher.instance.GetNavigationUI();
            bool changeToNode = mode == Mode.OrderPicking;
            if (changeToNode) {
                mode = Mode.WaypointPlacement;
                JobManager.instance.CancelJob();
            } else {
                mode = Mode.OrderPicking;
                WaypointCreator.instance.ResetCreator();
                Navigator.instance.ReindexWaypoints();
                WaypointStorage.instance.StoreWaypoints(Navigator.instance.waypoints);
                Navigator.instance.FindShortestPaths();
                JobManager.instance.StartDefaultJob();
            }
            navigationUI.SetActive(!changeToNode);
            WaypointStorage.instance.waypointParent.SetActive(changeToNode);
        }
    }
}