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
        [SerializeField] private GameObject holeCellPrefab_;
        [SerializeField] private GameObject groupedBlockedCellPrefab_;
        [SerializeField] private GameObject toggleSwitchCellPrefab_;
        [SerializeField] private Transform cellsRoot_;

        [Header("Layout")]
        [SerializeField] private float cellSize_ = 1f;
        [SerializeField] private Vector2 origin_ = Vector2.zero;
        [SerializeField] private bool centerGrid_ = false;

        private readonly List<GameObject> spawnedCells_ = new List<GameObject>();
        private readonly Dictionary<GridPosition, GameObject> cellObjectsByPosition_ = new();

        private int width_;
        private int height_;

        public void RenderGrid(int width, int height)
        {
            RenderGrid(width, height, null, null, null, null, null, null);
        }

        public void RenderGrid(
            int width,
            int height,
            IReadOnlyCollection<GridPosition> blockedPositions)
        {
            RenderGrid(width, height, blockedPositions, null, null, null, null, null);
        }

        public void RenderGrid(
            int width,
            int height,
            IReadOnlyCollection<GridPosition> blockedPositions,
            IReadOnlyCollection<GridPosition> breakableBlockedPositions)
        {
            RenderGrid(width, height, blockedPositions, breakableBlockedPositions, null, null, null, null);
        }

        /// <summary>
        /// Renders a grid with solid obstacles, breakable obstacles, visited cells,
        /// grouped blocked cells, and toggle switches.
        /// </summary>
        public void RenderGrid(
            int width,
            int height,
            IReadOnlyCollection<GridPosition> blockedPositions,
            IReadOnlyCollection<GridPosition> breakableBlockedPositions,
            IReadOnlyCollection<GridPosition> holePositions,
            IReadOnlyCollection<GridPosition> visitedPositions,
            IReadOnlyCollection<ToggleBlockedObstacleState> toggleBlockedObstacles,
            IReadOnlyCollection<ToggleSwitchTileState> toggleSwitchTiles)
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
                : new HashSet<GridPosition>();

            HashSet<GridPosition> breakableLookup = breakableBlockedPositions != null
                ? new HashSet<GridPosition>(breakableBlockedPositions)
                : new HashSet<GridPosition>();

            HashSet<GridPosition> visitedLookup = visitedPositions != null
                ? new HashSet<GridPosition>(visitedPositions)
                : new HashSet<GridPosition>();

            HashSet<GridPosition> holeLookup = holePositions != null
                ? new HashSet<GridPosition>(holePositions)
                : new HashSet<GridPosition>();

            Dictionary<GridPosition, ToggleBlockedObstacleState> toggleBlockedLookup = new();
            if (toggleBlockedObstacles != null)
            {
                foreach (ToggleBlockedObstacleState obstacle in toggleBlockedObstacles)
                {
                    toggleBlockedLookup[obstacle.Position] = obstacle;
                }
            }

            Dictionary<GridPosition, ToggleSwitchTileState> toggleSwitchLookup = new();
            if (toggleSwitchTiles != null)
            {
                foreach (ToggleSwitchTileState toggle in toggleSwitchTiles)
                {
                    toggleSwitchLookup[toggle.Position] = toggle;
                }
            }

            Transform parent = cellsRoot_ != null ? cellsRoot_ : transform;

            for (int y = 0; y < height_; y++)
            {
                for (int x = 0; x < width_; x++)
                {
                    GridPosition gridPosition = new GridPosition(x, y);
                    Vector3 worldPosition = GridToWorld(gridPosition);

                    SpawnCell(cellPrefab_, worldPosition, parent, $"Cell_{x}_{y}", gridPosition);

                    if (visitedLookup.Contains(gridPosition) && visitedCellPrefab_ != null)
                    {
                        SpawnOverlay(
                            visitedCellPrefab_,
                            worldPosition,
                            parent,
                            $"VisitedCell_{x}_{y}");
                    }

                    if (toggleSwitchLookup.TryGetValue(gridPosition, out ToggleSwitchTileState toggleSwitch) &&
                        toggleSwitchCellPrefab_ != null)
                    {
                        GameObject toggleSwitchObject = SpawnOverlay(
                            toggleSwitchCellPrefab_,
                            worldPosition,
                            parent,
                            $"ToggleSwitch_{x}_{y}");

                        ToggleSwitchCellView view = toggleSwitchObject.GetComponent<ToggleSwitchCellView>();
                        if (view != null)
                        {
                            view.Initialize(toggleSwitch.GroupId);
                        }
                    }

                    if (blockedLookup.Contains(gridPosition) && blockedCellPrefab_ != null)
                    {
                        SpawnOverlay(
                            blockedCellPrefab_,
                            worldPosition,
                            parent,
                            $"BlockedCell_{x}_{y}");
                    }

                    if (breakableLookup.Contains(gridPosition) && breakableBlockedCellPrefab_ != null)
                    {
                        SpawnOverlay(
                            breakableBlockedCellPrefab_,
                            worldPosition,
                            parent,
                            $"BreakableBlockedCell_{x}_{y}");
                    }

                    if (holeLookup.Contains(gridPosition) && holeCellPrefab_ != null)
                    {
                        SpawnOverlay(
                            holeCellPrefab_,
                            worldPosition,
                            parent,
                            $"HoleCell_{x}_{y}");
                    }

                    if (toggleBlockedLookup.TryGetValue(gridPosition, out ToggleBlockedObstacleState groupedObstacle) &&
                        groupedBlockedCellPrefab_ != null)
                    {
                        GameObject groupedBlockedObject = SpawnOverlay(
                            groupedBlockedCellPrefab_,
                            worldPosition,
                            parent,
                            $"GroupedBlockedCell_{x}_{y}");

                        GroupedBlockedCellView view =
                            groupedBlockedObject.GetComponent<GroupedBlockedCellView>();

                        if (view != null)
                        {
                            view.Initialize(groupedObstacle.GroupId, groupedObstacle.IsOn);
                        }
                    }
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
            cellObjectsByPosition_.Clear();
        }

        /// <summary>
        /// Refreshes a single classic cell visual using the given state.
        /// This method is kept for compatibility with existing callers.
        /// </summary>
        public void RefreshCell(
            GridPosition gridPosition,
            bool isBlocked,
            bool isBreakableBlocked,
            bool isVisited)
        {
            if (cellObjectsByPosition_.TryGetValue(gridPosition, out GameObject existingCell))
            {
                spawnedCells_.Remove(existingCell);

                if (existingCell != null)
                {
                    Destroy(existingCell);
                }

                cellObjectsByPosition_.Remove(gridPosition);
            }

            Transform parent = cellsRoot_ != null ? cellsRoot_ : transform;
            Vector3 worldPosition = GridToWorld(gridPosition);

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
                cell.name = $"BreakableBlockedCell_{gridPosition.GetX()}_{gridPosition.GetY()}";
            }
            else if (isBlocked)
            {
                cell.name = $"BlockedCell_{gridPosition.GetX()}_{gridPosition.GetY()}";
            }
            else if (isVisited)
            {
                cell.name = $"VisitedCell_{gridPosition.GetX()}_{gridPosition.GetY()}";
            }
            else
            {
                cell.name = $"Cell_{gridPosition.GetX()}_{gridPosition.GetY()}";
            }

            cell.transform.localScale = new Vector3(cellSize_, cellSize_, 1f);

            spawnedCells_.Add(cell);
            cellObjectsByPosition_[gridPosition] = cell;
        }

        private GameObject SpawnCell(
            GameObject prefab,
            Vector3 worldPosition,
            Transform parent,
            string objectName,
            GridPosition gridPosition)
        {
            GameObject cell = Instantiate(prefab, worldPosition, Quaternion.identity, parent);
            cell.name = objectName;
            cell.transform.localScale = new Vector3(cellSize_, cellSize_, 1f);

            spawnedCells_.Add(cell);
            cellObjectsByPosition_[gridPosition] = cell;

            return cell;
        }

        private GameObject SpawnOverlay(
            GameObject prefab,
            Vector3 worldPosition,
            Transform parent,
            string objectName)
        {
            GameObject cell = Instantiate(prefab, worldPosition, Quaternion.identity, parent);
            cell.name = objectName;
            cell.transform.localScale = new Vector3(cellSize_, cellSize_, 1f);

            spawnedCells_.Add(cell);
            return cell;
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
