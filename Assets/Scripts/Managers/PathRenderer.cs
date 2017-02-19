using UnityEngine;
using System.Collections.Generic;
using PickAR.UI;

namespace PickAR.Managers {

    /// <summary>
    /// Renders the path to the goal.
    /// </summary>
    public class PathRenderer : MonoBehaviour {

        /// <summary> The path mesh prefab to make lines with. </summary>
        [SerializeField]
        [Tooltip("The path mesh prefab to make lines with.")]
        private GameObject pathMeshPrefab;
        /// <summary> The thickness of path lines. </summary>
        [SerializeField]
        [Tooltip("The thickness of path lines.")]
        private float lineThickness;
        /// <summary> The height of path lines. </summary>
        [SerializeField]
        [Tooltip("The height of path lines.")]
        private float lineHeight;
        /// <summary> The length of curves in the path. </summary>
        [SerializeField]
        [Tooltip("The length of curves in the path.")]
        private float curveLength;

        /// <summary> The path object currently being rendered </summary>
        private GameObject pathObject;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            pathObject = GameObject.Instantiate(pathMeshPrefab);
            pathObject.SetActive(false);
            pathObject.name = "Path Line";

            List<Vector3> path = new List<Vector3>();
            path.Add(new Vector3(0, 0, 0));
            path.Add(new Vector3(1f, 0, 0));
            path.Add(new Vector3(1f, 1f, 1f));
            DrawPath(path);
        }

        /// <summary>
        /// Draws a path defined by points.
        /// </summary>
        /// <param name="path">The points in the path to draw.</param>
        public void DrawPath(List<Vector3> path) {
            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            List<Vector3> newPath = new List<Vector3>();

            for (int i = 0; i < path.Count - 1; i++) {
                Vector3 node0 = path[i];
                Vector3 node1 = path[i + 1];
                Vector3 difference = node1 - node0;
                Vector3 truncate = difference.normalized * curveLength;
                if (i != 0) {
                    node0 += truncate;
                }
                if (i < path.Count - 2) {
                    node1 -= truncate;
                }
                difference = node1 - node0;
                Vector3 side = Quaternion.AngleAxis(90, Vector3.up) * difference;
                side.y = 0;
                side = side.normalized * lineThickness / 2;
                List<Vector3> corners = new List<Vector3>();
                corners.Add(node0 + side);
                corners.Add(node0 - side);
                corners.Add(node1 - side);
                corners.Add(node1 + side);

                int prevOff = verts.Count;
                if (i != 0) {
                    // Curve.
                    verts.Add(verts[prevOff - 5]);
                    verts.Add(verts[prevOff - 6]);
                    verts.Add(corners[1]);
                    verts.Add(corners[0]);

                    verts.Add(verts[prevOff - 1]);
                    verts.Add(verts[prevOff - 2]);
                    verts.Add(corners[1] + Vector3.down * lineHeight);
                    verts.Add(corners[0] + Vector3.down * lineHeight);
                }

                int off = verts.Count;
                foreach (Vector3 corner in corners) {
                    verts.Add(corner);
                }
                foreach (Vector3 corner in corners) {
                    verts.Add(corner + Vector3.down * lineHeight);
                }

                foreach (int j in new int[] { prevOff, off }) {
                    // Top and bottom.
                    tris.Add(j);
                    tris.Add(j + 1);
                    tris.Add(j + 2);

                    tris.Add(j);
                    tris.Add(j + 2);
                    tris.Add(j + 3);

                    tris.Add(j + 6);
                    tris.Add(j + 5);
                    tris.Add(j + 4);

                    tris.Add(j + 7);
                    tris.Add(j + 6);
                    tris.Add(j + 4);

                    // Sides.
                    for (int k = j; k < j + 4; k++) {
                        bool last = k == j + 3;
                        tris.Add(k);
                        tris.Add(k + 4);
                        tris.Add(last ? j : k + 1);

                        tris.Add(last ? j : k + 1);
                        tris.Add(k + 4);
                        tris.Add(last ? j + 4 : k + 5);
                    }
                }

                newPath.Add(node0);
                newPath.Add(node1);
            }

            int numLines = newPath.Count - 1;
            float[] cumulativeLength = new float[numLines];
            float totalLength = 0;
            for (int i = 0; i < numLines; i++) {
                float length = Vector3.Distance(newPath[i + 1], newPath[i]);
                totalLength += length;
                cumulativeLength[i] = totalLength;
            }
            float prevUV = 0;
            for (int i = 0; i < numLines; i++) {
                float currentUV = cumulativeLength[i] / totalLength;
                for (int j = 0; j < 2; j++) {
                    uvs.Add(new Vector2(1, prevUV));
                    uvs.Add(new Vector2(0, prevUV));
                    uvs.Add(new Vector2(0, currentUV));
                    uvs.Add(new Vector2(1, currentUV));
                }
                prevUV = currentUV;
            }

            Mesh mesh = new Mesh();
            mesh.hideFlags = HideFlags.HideAndDontSave;
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();

            MeshFilter meshFilter = pathObject.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            meshFilter.mesh = mesh;
            SpriteSheet spriteSheet = pathObject.GetComponent<SpriteSheet>();
            spriteSheet._scale.y = -0.1f / totalLength;
            spriteSheet.CalcTextureSize();
            pathObject.SetActive(true);
        }

        /// <summary>
        /// Removes all drawn lines.
        /// </summary>
        public void RemoveLines() {
            pathObject.SetActive(false);
        }
    }
}