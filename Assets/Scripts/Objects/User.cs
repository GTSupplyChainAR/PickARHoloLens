using UnityEngine;
using PickAR.Managers;
using PickAR.Navigation;

namespace PickAR.Objects {

    /// <summary>
    /// The user of the application.
    /// </summary>
    class User : MonoBehaviour {

        /// <summary> The distance away from the start point that the user can be to complete the job. </summary>
        [SerializeField]
        [Tooltip("The distance away from the start point that the user can be to complete the job.")]
        private float endDistance;
        /// <summary> The distance from the path where the angle away will always be set to 0. </summary>
        [SerializeField]
        [Tooltip("The distance from the path where the angle away will always be set to 0.")]
        private float angleIgnoreDistance;

        /// <summary> The angle between the user's view and the path. </summary>
        public float angleAway {
            get;
            private set;
        }

        /// <summary> The singleton instance of the object. </summary>
        public static User instance {
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
            JobManager jobManager = JobManager.instance;
            if (jobManager.jobActive) {
                if (jobManager.jobFinished && Vector3.Distance(transform.position, jobManager.startPoint) < endDistance) {
                    jobManager.CompleteJob();
                } else {
                    Vector3 nextOffset = Navigator.instance.nextPoint - transform.position;
                    nextOffset.y = 0;
                    if (nextOffset.magnitude < angleIgnoreDistance) {
                        angleAway = 0;
                    } else {
                        nextOffset.Normalize();
                        Vector3 forward = transform.forward;
                        forward.y = 0;
                        forward.Normalize();
                        float angle = Vector3.Angle(nextOffset, forward);
                        if (Vector3.Cross(nextOffset, forward).y < 0) {
                            angle = -angle;
                        }
                        angleAway = angle;
                    }
                }
            } else {
                angleAway = 0;
            }
        }
    }
}