using System.Collections.Generic;
using UnityEngine;
using CodingGame.Runtime.Games.Moving;

namespace CodingGame.Presentation.Games.Moving
{
    /// <summary>
    /// Renders a 2D grid using tile prefabs and provides grid-to-world conversion.
    /// </summary>
    public sealed class GridRenderer : MonoBehaviour
    {
        [Header("Grid Visuals")]
        [SerializeField] private GameObject cellPrefab_;
        [SerializeField] private Transform cellsRoot_;

        [Header("Layout")]
        [SerializeField] private float cellSize_ = 1f;
        [SerializeField] private Vector2 origin_ = Vector2.zero;
        [SerializeField] private bool centerGrid_ = false;

        private readonly List<GameObject> spawnedCells_ = new List<GameObject>();
        private int width_;
        private int height_;

        /// <summary>
        /// Renders a grid with the given dimensions.
        /// </summary>
        public void RenderGrid(int width, int height)
        {
            width_ = width;
            height_ = height;

            ClearGrid();

            if (cellPrefab_ == null)
            {
                return;
            }

            Transform parent = cellsRoot_ != null ? cellsRoot_ : transform;

            for (int y = 0; y < height_; y++)
            {
                for (int x = 0; x < width_; x++)
                {
                    Vector3 worldPosition = GridToWorld(new GridPosition(x, y));
                    GameObject cell = Instantiate(cellPrefab_, worldPosition, Quaternion.identity, parent);
                    cell.name = $"Cell_{x}_{y}";
                    cell.transform.localScale = new Vector3(cellSize_, cellSize_, 1f);
                    spawnedCells_.Add(cell);
                }
            }
        }

        /// <summary>
        /// Converts a grid position to a world position.
        /// </summary>
        public Vector3 GridToWorld(GridPosition gridPosition)
        {
            Vector2 offset = GetGridOffset();

            float x = origin_.x + offset.x + (gridPosition.GetX() * cellSize_);
            float y = origin_.y + offset.y + (gridPosition.GetY() * cellSize_);

            return new Vector3(x, y, 0f);
        }

        /// <summary>
        /// Returns the configured cell size.
        /// </summary>
        public float GetCellSize()
        {
            return cellSize_;
        }

        /// <summary>
        /// Clears all currently spawned grid cells.
        /// </summary>
        public void ClearGrid()
        {
            for (int i = 0; i < spawnedCells_.Count; i++)
            {
                if (spawnedCells_[i] != null)
                {
                    Destroy(spawnedCells_[i]);
                }
            }

            spawnedCells_.Clear();
        }

        private Vector2 GetGridOffset()
        {
            if (!centerGrid_)
            {
                return Vector2.zero;
            }

            float widthOffset = -((width_ - 1) * cellSize_) * 0.5f;
            float heightOffset = -((height_ - 1) * cellSize_) * 0.5f;

            return new Vector2(widthOffset, heightOffset);
        }
    }
}