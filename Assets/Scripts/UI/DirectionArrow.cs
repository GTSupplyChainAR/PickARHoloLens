using UnityEngine;
using UnityEngine.UI;
using PickAR.Objects;

namespace PickAR.UI {

    /// <summary>
    /// Points in the direction of the path if the user is facing the wrong way.
    /// </summary>
    class DirectionArrow : MonoBehaviour {
        /// <summary> The angle where an arrow will start showing. </summary>
        [SerializeField]
        [Tooltip("The angle where an arrow will start showing.")]
        private float angleThreshold;

        /// <summary> The image to render arrows on. </summary>
        private Image image;
        /// <summary> Arrow to turn around. </summary>
        [SerializeField]
        [Tooltip("Arrow to turn around.")]
        private Sprite reverseArrow;
        /// <summary> Arrow to turn right. </summary>
        [SerializeField]
        [Tooltip("Arrow to turn right.")]
        private Sprite rightArrow;
        /// <summary> Arrow to turn left. </summary>
        [SerializeField]
        [Tooltip("Arrow to turn left.")]
        private Sprite leftArrow;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            image = GetComponent<Image>();
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            float angle = User.instance.angleAway;
            float absAngle = Mathf.Abs(angle);
            if (absAngle > angleThreshold) {
                image.enabled = true;
                if (absAngle > 180 - ((180 - angleThreshold) / 3)) {
                    image.sprite = reverseArrow;
                } else if (angle < 0) {
                    image.sprite = rightArrow;
                } else {
                    image.sprite = leftArrow;
                }
            } else {
                image.enabled = false;
            }
        }
    }
}