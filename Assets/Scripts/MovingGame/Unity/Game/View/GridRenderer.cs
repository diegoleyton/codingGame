using System.Collections.Generic;
using UnityEngine;
using Flowbit.MovingGame.Core;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Renders a 2D grid using tile prefabs and provides grid-to-world conversion.
    /// </summary>
    public sealed class GridRenderer : MonoBehaviour
    {
        [Header("Grid Visuals")]
        [SerializeField] private GameObject cellPrefab_;
        [SerializeField] private GameObject visitedCellPrefab_;
        [SerializeField] private GameObject blockedCellPrefab_;
        [SerializeField] private GameObject breakableBlockedCellPrefab_;
        [SerializeField] private Transform cellsRoot_;

        [Header("Layout")]
        [SerializeField] private float cellSize_ = 1f;
        [SerializeField] private Vector2 origin_ = Vector2.zero;
        [SerializeField] private bool centerGrid_ = false;

        private readonly List<GameObject> spawnedCells_ = new List<GameObject>();
        private int width_;
        private int height_;

        public void RenderGrid(int width, int height)
        {
            RenderGrid(width, height, null, null, null);
        }

        public void RenderGrid(
            int width,
            int height,
            IReadOnlyCollection<GridPosition> blockedPositions)
        {
            RenderGrid(width, height, blockedPositions, null, null);
        }

        public void RenderGrid(
            int width,
            int height,
            IReadOnlyCollection<GridPosition> blockedPositions,
            IReadOnlyCollection<GridPosition> breakableBlockedPositions)
        {
            RenderGrid(width, height, blockedPositions, breakableBlockedPositions, null);
        }

        /// <summary>
        /// Renders a grid with solid obstacles, breakable obstacles, and visited cells.
        /// </summary>
        public void RenderGrid(
            int width,
            int height,
            IReadOnlyCollection<GridPosition> blockedPositions,
            IReadOnlyCollection<GridPosition> breakableBlockedPositions,
            IReadOnlyCollection<GridPosition> visitedPositions)
        {
            width_ = width;
            height_ = height;

            ClearGrid();

            if (cellPrefab_ == null)
            {
                return;
            }

            HashSet<GridPosition> blockedLookup = blockedPositions != null
                ? new HashSet<GridPosition>(blockedPositions)
                : null;

            HashSet<GridPosition> breakableLookup = breakableBlockedPositions != null
                ? new HashSet<GridPosition>(breakableBlockedPositions)
                : null;

            HashSet<GridPosition> visitedLookup = visitedPositions != null
                ? new HashSet<GridPosition>(visitedPositions)
                : null;

            Transform parent = cellsRoot_ != null ? cellsRoot_ : transform;

            for (int y = 0; y < height_; y++)
            {
                for (int x = 0; x < width_; x++)
                {
                    GridPosition gridPosition = new GridPosition(x, y);
                    Vector3 worldPosition = GridToWorld(gridPosition);

                    bool isBlocked = blockedLookup != null && blockedLookup.Contains(gridPosition);
                    bool isBreakableBlocked = breakableLookup != null && breakableLookup.Contains(gridPosition);
                    bool isVisited = visitedLookup != null && visitedLookup.Contains(gridPosition);

                    GameObject prefabToUse = cellPrefab_;

                    if (isBreakableBlocked && breakableBlockedCellPrefab_ != null)
                    {
                        prefabToUse = breakableBlockedCellPrefab_;
                    }
                    else if (isBlocked && blockedCellPrefab_ != null)
                    {
                        prefabToUse = blockedCellPrefab_;
                    }
                    else if (isVisited && visitedCellPrefab_ != null)
                    {
                        prefabToUse = visitedCellPrefab_;
                    }

                    GameObject cell = Instantiate(prefabToUse, worldPosition, Quaternion.identity, parent);

                    if (isBreakableBlocked)
                    {
                        cell.name = $"BreakableBlockedCell_{x}_{y}";
                    }
                    else if (isBlocked)
                    {
                        cell.name = $"BlockedCell_{x}_{y}";
                    }
                    else if (isVisited)
                    {
                        cell.name = $"VisitedCell_{x}_{y}";
                    }
                    else
                    {
                        cell.name = $"Cell_{x}_{y}";
                    }

                    cell.transform.localScale = new Vector3(cellSize_, cellSize_, 1f);
                    spawnedCells_.Add(cell);
                }
            }
        }

        public Vector3 GridToWorld(GridPosition gridPosition)
        {
            Vector2 offset = GetGridOffset();
            float x = origin_.x + offset.x + (gridPosition.GetX() * cellSize_);
            float y = origin_.y + offset.y + (gridPosition.GetY() * cellSize_);
            return new Vector3(x, y, 0f);
        }

        public float GetCellSize()
        {
            return cellSize_;
        }

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