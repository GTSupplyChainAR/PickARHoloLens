using UnityEngine;
using System.Collections.Generic;
using PickAR.UI;

namespace PickAR.Managers {

    /// <summary>
    /// Pools path line objects for efficiency.
    /// </summary>
    public class PathLinePool : MonoBehaviour {

        /// <summary> The line renderer prefab to make lines with. </summary>
        [Tooltip("The line renderer prefab to make lines with.")]
        public PathLine lineRenderer;

        /// <summary> Parent object for all path lines. </summary>
        private GameObject pathLines;

        /// <summary> All path lines that have been instantiated. </summary>
        private List<PathLine> pathLineList = new List<PathLine>();
        /// <summary> The index of the first free path line. </summary>
        private int freeIndex;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            pathLines = new GameObject();
            pathLines.name = "Path Lines";
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="start">The starting points of the line.</param>
        /// <param name="end">The ending points of the line.</param>
        public void DrawLine(Vector3 start, Vector3 end) {
            PathLine line;
            if (freeIndex >= pathLineList.Count) {
                line = Instantiate(lineRenderer) as PathLine;
                line.transform.parent = pathLines.transform;
                pathLineList.Add(line);
            } else {
                line = pathLineList[freeIndex];
                if (!line.gameObject.activeSelf) {
                    line.gameObject.SetActive(true);
                }
            }
            freeIndex++;

            line.SetEndpoints(start, end);
        }

        /// <summary>
        /// Removes all drawn lines.
        /// </summary>
        public void RemoveLines() {
            for (int i = 0; i < pathLineList.Count; i++) {
                pathLineList[i].gameObject.SetActive(false);
            }
            ResetFreeIndex();
        }

        /// <summary>
        /// Resets the index of free lines to 0.
        /// </summary>
        public void ResetFreeIndex() {
            freeIndex = 0;
        }

        /// <summary>
        /// Removes all lines past the free index;
        /// </summary>
        public void RemoveExtraLines() {
            for (int i = freeIndex; i < pathLineList.Count; i++) {
                pathLineList[i].gameObject.SetActive(false);
            }
        }
    }
}