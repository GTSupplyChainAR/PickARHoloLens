using UnityEngine;
using PickAR.Managers;

namespace PickAR.Objects {

    /// <summary>
    /// The user of the application.
    /// </summary>
    class User : MonoBehaviour {

        /// <summary> The distance away from the start point that the user can be to complete the job. </summary>
        [SerializeField]
        [Tooltip("The distance away from the start point that the user can be to complete the job.")]
        private float endDistance;

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            JobManager jobManager = JobManager.instance;
            if (jobManager.jobFinished && Vector3.Distance(transform.position, jobManager.startPoint) < endDistance) {
                jobManager.CompleteJob();
            }
        }
    }
}