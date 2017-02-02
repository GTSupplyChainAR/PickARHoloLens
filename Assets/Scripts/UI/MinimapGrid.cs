using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PickAR.Util;

namespace PickAR.UI {

    /// <summary>
    /// The shelf grid portion of the minimap.
    /// </summary>
    class MinimapGrid : MonoBehaviour {

        /// <summary> The thickness of grid lines. </summary>
        [SerializeField]
        [Tooltip("The thickness of grid lines.")]
        private float lineThickness;
        /// <summary> The dimensions of the grid. </summary>
        [SerializeField]
        [Tooltip("The dimensions of the grid.")]
        private Vector2 gridSize;

        /// <summary> The color of the currently selected item. </summary>
        [SerializeField]
        [Tooltip("The color of the currently selected item.")]
        private Color currentColor;
        /// <summary> The color of other needed items. </summary>
        [SerializeField]
        [Tooltip("The color of other needed items.")]
        private Color neededColor;

        /// <summary> The prefab for a grid square. </summary>
        [SerializeField]
        [Tooltip("The prefab for a grid square.")]
        private Image gridSquare;

        /// <summary> The grid of compartments. </summary>
        private Image[,] grid;

        /// <summary> The grid squares that are highlighted. </summary>
        private List<Vector2Int> highlightSquares = new List<Vector2Int>();

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            int sizeX = (int) gridSize.x;
            int sizeY = (int) gridSize.y;
            grid = new Image[sizeX, sizeY];
            float totalLineX = lineThickness * (sizeX + 1);
            float totalLineY = lineThickness * (sizeY + 1);
            RectTransform rectTransform = GetComponent<RectTransform>();
            float boxWidth = rectTransform.rect.width;
            float boxHeight = rectTransform.rect.height;
            float squareWidth = (boxWidth - totalLineX) / sizeX;
            float squareHeight = (boxHeight - totalLineY) / sizeY;
            float totalWidth = totalLineX + squareWidth * sizeX;
            float totalHeight = totalLineY + squareHeight * sizeY;
            float xLeft = -boxWidth / 2;
            float yTop = -boxHeight / 2;

            int i;
            float position = xLeft;
            for (i = 0; i < sizeX + 1; i++) {
                CreateImage("Grid Line", lineThickness, totalHeight, position, yTop);
                position += lineThickness + squareWidth;
            }
            position = yTop;
            for (i = 0; i < sizeY + 1; i++) {
                CreateImage("Grid Line", totalWidth, lineThickness, xLeft, position);
                position += lineThickness + squareHeight;
            }
            int j;
            float positionX = xLeft + lineThickness;
            for (i = 0; i < sizeX; i++) {
                float positionY = yTop + lineThickness;
                for (j = 0; j < sizeY; j++) {
                    grid[i, j] = CreateImage("Grid Square", squareWidth, squareHeight, positionX, positionY);
                    grid[i, j].gameObject.SetActive(false);
                    positionY += lineThickness + squareHeight;
                }
                positionX += lineThickness + squareWidth;
            }
        }

        /// <summary>
        /// Creates an image on the grid.
        /// </summary>
        /// <returns>The image that was created.</returns>
        /// <param name="name">The name of the image.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="x">The x coordinate of the top-left corner of the image.</param>
        /// <param name="y">The y coordinate of the top-left corner of the image.</param>
        private Image CreateImage(string name, float width, float height, float x, float y) {
            Image imageObject = GameObject.Instantiate(gridSquare) as Image;
            imageObject.name = name;
            imageObject.transform.SetParent(transform);
            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.localPosition = new Vector3(x + width / 2, y + height / 2, 0);
            rectTransform.localScale = Vector3.one;
            rectTransform.GetComponent<Image>().color = Color.black;
            return imageObject;
        }

        /// <summary>
        /// Resets the highlighted squares in the grid.
        /// </summary>
        internal void ResetGrid() {
            foreach (Vector2Int c in highlightSquares) {
                grid[c.x, c.y].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Highlights a grid square.
        /// </summary>
        /// <param name="x">The x coordinate of the grid square.</param>
        /// <param name="y">The y coordinate of the grid square.</param>
        /// <param name="selected">Whether the square is the object currently selected.</param>
        internal void HighlightSquare(int x, int y, bool selected) {
            Color color = selected ? currentColor : neededColor;
            Image image = grid[x, y];
            image.color = color;
            image.gameObject.SetActive(true);
            highlightSquares.Add(new Vector2Int(x, y));
        }

        /// <summary>
        /// Sets whether the grid is visible.
        /// </summary>
        /// <param name="visible">Whether the grid is visible.</param>
        internal void SetVisible(bool visible) {
            gameObject.SetActive(visible);
        }
    }
}