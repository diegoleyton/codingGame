using System;
using System.Collections.Generic;
using System.Linq;

using Flowbit.GameBase.GamesSettings;
using Flowbit.MovingGame.Core.Levels;

using UnityEditor;
using UnityEngine;

namespace Flowbit.MovingGame.Editor
{
    public sealed class MovingGameLevelEditorWindow : EditorWindow
    {
        private enum CellBrush
        {
            Erase,
            Start,
            Food,
            Blocked,
            Breakable,
            Hole,
            ToggleObstacle,
            ToggleSwitch
        }

        private const float LevelsPanelWidth = 220f;
        private const float CellButtonSize = 52f;
        private const float CellPadding = 4f;
        private const string MovingGameSettingsAssetPath = "Assets/GameSettings/MovingGameSettings.asset";
        private const string CellFrameTexturePath = "Assets/Images/cartoon/cartoon_square_frame.png";
        private const string FoodTexturePath = "Assets/Images/dog/cartoon_bone_2.png";
        private const string BlockedTexturePath = "Assets/Images/cartoon/cartoon_block_2.png";
        private const string BreakableTexturePath = "Assets/Images/cartoon/cartoon_brakable.png";
        private const string HoleTexturePath = "Assets/Images/cartoon/cartoon_hole_torn.png";
        private const string SwitchTexturePath = "Assets/Images/cartoon/cartoon_button_switch.png";
        private const string SwitchBaseTexturePath = "Assets/Images/cartoon/cartoon_button_switch_2.png";
        private const string StartTexturePath = "Assets/Images/dog/cartoon_dog_top_head.png";

        private static readonly Color EmptyCellTint = new(0.98f, 0.98f, 0.96f, 1f);
        private static readonly Color ToggleOffTint = new(1f, 1f, 1f, 0.22f);

        private readonly GUIContent[] brushLabels_ =
        {
            new("Erase"),
            new("Start"),
            new("Food"),
            new("Blocked"),
            new("Breakable"),
            new("Hole"),
            new("Toggle Obstacle"),
            new("Toggle Switch")
        };

        private MovingGameLevelsFileData fileData_;
        private Vector2 levelsScroll_;
        private Vector2 detailsScroll_;
        private int selectedLevelIndex_;
        private CellBrush selectedBrush_;
        private int selectedGroupId_ = 1;
        private bool selectedToggleIsOn_ = true;
        private string levelsAssetPath_ = MovingGameLevelEditorSerializer.DefaultLevelsAssetPath;
        private string statusMessage_ = string.Empty;
        private MessageType statusMessageType_ = MessageType.Info;
        private GUIStyle cellOverlayLabelStyle_;
        private GUIStyle cellDirectionLabelStyle_;
        private GUIStyle cellCoordinateLabelStyle_;
        private GUIStyle cellBadgeLabelStyle_;
        private Texture2D cellFrameTexture_;
        private Texture2D foodTexture_;
        private Texture2D blockedTexture_;
        private Texture2D breakableTexture_;
        private Texture2D holeTexture_;
        private Texture2D switchTexture_;
        private Texture2D switchBaseTexture_;
        private Texture2D startTexture_;
        private MovingGameSettings movingGameSettings_;

        [MenuItem("Tools/Moving Game/Level Editor")]
        public static void OpenWindow()
        {
            MovingGameLevelEditorWindow window = GetWindow<MovingGameLevelEditorWindow>();
            window.titleContent = new GUIContent("MovingGame Levels");
            window.minSize = new Vector2(1024f, 640f);
            window.Show();
        }

        private void OnEnable()
        {
            EnsureEditorAssetsLoaded();

            if (fileData_ == null)
            {
                ReloadLevels();
            }
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (!string.IsNullOrWhiteSpace(statusMessage_))
            {
                EditorGUILayout.HelpBox(statusMessage_, statusMessageType_);
            }

            if (fileData_ == null || fileData_.levels == null)
            {
                EditorGUILayout.HelpBox("No level file loaded.", MessageType.Warning);
                return;
            }

            EnsureSelectedLevelIsValid();
            MovingGameLevelData level = fileData_.levels[selectedLevelIndex_];
            MovingGameLevelEditorValidator.EnsureCollections(level);

            EditorGUILayout.BeginHorizontal();
            DrawLevelsPanel();
            DrawEditorPanel(level);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label(levelsAssetPath_, EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Reload", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                ReloadLevels();
            }

            if (GUILayout.Button("Validate", EditorStyles.toolbarButton, GUILayout.Width(70f)))
            {
                ValidateFile();
            }

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                SaveLevels();
            }

            if (GUILayout.Button("Ping JSON", EditorStyles.toolbarButton, GUILayout.Width(80f)))
            {
                PingLevelsAsset();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawLevelsPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(LevelsPanelWidth));
            EditorGUILayout.LabelField("Levels", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("New"))
            {
                AddNewLevel();
            }

            if (GUILayout.Button("Duplicate"))
            {
                DuplicateSelectedLevel();
            }

            if (GUILayout.Button("Delete"))
            {
                DeleteSelectedLevel();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Move Up"))
            {
                MoveSelectedLevel(-1);
            }

            if (GUILayout.Button("Move Down"))
            {
                MoveSelectedLevel(1);
            }

            EditorGUILayout.EndHorizontal();

            levelsScroll_ = EditorGUILayout.BeginScrollView(levelsScroll_);

            for (int i = 0; i < fileData_.levels.Count; i++)
            {
                MovingGameLevelData level = fileData_.levels[i];
                string label = level == null
                    ? $"[{i}] <null>"
                    : $"{level.id:00}  {level.name}";

                GUIStyle style = i == selectedLevelIndex_
                    ? EditorStyles.toolbarButton
                    : EditorStyles.miniButton;

                if (GUILayout.Button(label, style))
                {
                    selectedLevelIndex_ = i;
                    ClearStatus();
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawEditorPanel(MovingGameLevelData level)
        {
            detailsScroll_ = EditorGUILayout.BeginScrollView(detailsScroll_);

            EditorGUILayout.LabelField("Level Details", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Id", level.id.ToString());
            level.name = EditorGUILayout.TextField("Name", level.name);
            level.hint = EditorGUILayout.TextField("Hint", level.hint);
            level.difficulty = EditorGUILayout.IntSlider("Difficulty", level.difficulty, 1, 6);

            int width = Mathf.Clamp(EditorGUILayout.IntField("Width", level.width), 1, 5);
            int height = Mathf.Clamp(EditorGUILayout.IntField("Height", level.height), 1, 5);

            if (EditorGUI.EndChangeCheck())
            {
                level.width = width;
                level.height = height;
                ClampAndPruneLevel(level);
            }

            DrawStartSettings(level);

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Brush", EditorStyles.boldLabel);
            selectedBrush_ = (CellBrush)GUILayout.Toolbar((int)selectedBrush_, brushLabels_);

            if (selectedBrush_ == CellBrush.ToggleObstacle || selectedBrush_ == CellBrush.ToggleSwitch)
            {
                selectedGroupId_ = Mathf.Max(1, EditorGUILayout.IntField("Group Id", selectedGroupId_));
            }

            if (selectedBrush_ == CellBrush.ToggleObstacle)
            {
                selectedToggleIsOn_ = EditorGUILayout.Toggle("Starts On", selectedToggleIsOn_);
            }

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
            DrawGrid(level);

            EditorGUILayout.Space(10f);
            DrawLegend(level);

            EditorGUILayout.EndScrollView();
        }

        private void DrawStartSettings(MovingGameLevelData level)
        {
            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("Start", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            int startX = EditorGUILayout.IntField("Start X", level.startPosition.x);
            int startY = EditorGUILayout.IntField("Start Y", level.startPosition.y);
            string direction = EditorGUILayout.Popup(
                "Start Direction",
                DirectionIndex(level.startDirection),
                new[] { "Up", "Right", "Down", "Left" }) switch
            {
                0 => "Up",
                1 => "Right",
                2 => "Down",
                3 => "Left",
                _ => "Right"
            };

            if (EditorGUI.EndChangeCheck())
            {
                level.startPosition.x = Mathf.Clamp(startX, 0, Mathf.Max(0, level.width - 1));
                level.startPosition.y = Mathf.Clamp(startY, 0, Mathf.Max(0, level.height - 1));
                level.startDirection = direction;
                RemoveContentAt(level, level.startPosition.x, level.startPosition.y);
            }
        }

        private void DrawGrid(MovingGameLevelData level)
        {
            for (int y = level.height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < level.width; x++)
                {
                    Rect rect = GUILayoutUtility.GetRect(CellButtonSize, CellButtonSize, GUILayout.Width(CellButtonSize), GUILayout.Height(CellButtonSize));

                    if (GUI.Button(rect, GUIContent.none))
                    {
                        ApplyBrush(level, x, y);
                    }

                    DrawCellVisual(level, x, y, rect);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawLegend(MovingGameLevelData level)
        {
            EditorGUILayout.LabelField("Summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Foods: {level.foodPositions.Count}");
            EditorGUILayout.LabelField($"Blocked: {level.blockedPositions.Count}");
            EditorGUILayout.LabelField($"Breakable: {level.breakableBlockedPositions.Count}");
            EditorGUILayout.LabelField($"Holes: {level.holePositions.Count}");
            EditorGUILayout.LabelField($"Toggle Obstacles: {level.toggleBlockedTiles.Count}");
            EditorGUILayout.LabelField($"Toggle Switches: {level.toggleSwitchTiles.Count}");
        }

        private void ApplyBrush(MovingGameLevelData level, int x, int y)
        {
            MovingGameLevelEditorValidator.EnsureCollections(level);

            switch (selectedBrush_)
            {
                case CellBrush.Erase:
                    RemoveContentAt(level, x, y);
                    if (level.startPosition.x == x && level.startPosition.y == y)
                    {
                        level.startPosition.x = 0;
                        level.startPosition.y = 0;
                        RemoveContentAt(level, 0, 0);
                    }
                    SetStatus($"Cleared cell ({x}, {y}).", MessageType.Info);
                    break;

                case CellBrush.Start:
                    level.startPosition.x = x;
                    level.startPosition.y = y;
                    RemoveContentAt(level, x, y);
                    SetStatus($"Moved start to ({x}, {y}).", MessageType.Info);
                    break;

                case CellBrush.Food:
                    if (IsStart(level, x, y))
                    {
                        SetStatus("Food cannot be placed on the start cell.", MessageType.Warning);
                        return;
                    }

                    RemoveObstacleContentAt(level, x, y);
                    AddPosition(level.foodPositions, x, y);
                    SetStatus($"Placed food at ({x}, {y}).", MessageType.Info);
                    break;

                case CellBrush.Blocked:
                    if (IsStart(level, x, y))
                    {
                        SetStatus("Blocked cells cannot be placed on the start cell.", MessageType.Warning);
                        return;
                    }

                    RemoveContentAt(level, x, y);
                    AddPosition(level.blockedPositions, x, y);
                    SetStatus($"Placed blocked cell at ({x}, {y}).", MessageType.Info);
                    break;

                case CellBrush.Breakable:
                    if (IsStart(level, x, y))
                    {
                        SetStatus("Breakable cells cannot be placed on the start cell.", MessageType.Warning);
                        return;
                    }

                    RemoveContentAt(level, x, y);
                    AddPosition(level.breakableBlockedPositions, x, y);
                    SetStatus($"Placed breakable cell at ({x}, {y}).", MessageType.Info);
                    break;

                case CellBrush.Hole:
                    if (IsStart(level, x, y))
                    {
                        SetStatus("Holes cannot be placed on the start cell.", MessageType.Warning);
                        return;
                    }

                    RemoveContentAt(level, x, y);
                    AddPosition(level.holePositions, x, y);
                    SetStatus($"Placed hole at ({x}, {y}).", MessageType.Info);
                    break;

                case CellBrush.ToggleObstacle:
                    if (IsStart(level, x, y))
                    {
                        SetStatus("Toggle obstacles cannot be placed on the start cell.", MessageType.Warning);
                        return;
                    }

                    RemoveContentAt(level, x, y);
                    level.toggleBlockedTiles.Add(new ToggleBlockedTileData
                    {
                        x = x,
                        y = y,
                        groupId = selectedGroupId_,
                        isOn = selectedToggleIsOn_
                    });
                    SetStatus($"Placed toggle obstacle at ({x}, {y}) in group {selectedGroupId_}.", MessageType.Info);
                    break;

                case CellBrush.ToggleSwitch:
                    if (IsStart(level, x, y))
                    {
                        SetStatus("Toggle switches cannot be placed on the start cell.", MessageType.Warning);
                        return;
                    }

                    RemoveContentAt(level, x, y);
                    level.toggleSwitchTiles.Add(new ToggleSwitchTileData
                    {
                        x = x,
                        y = y,
                        groupId = selectedGroupId_
                    });
                    SetStatus($"Placed toggle switch at ({x}, {y}) in group {selectedGroupId_}.", MessageType.Info);
                    break;
            }
        }

        private void AddNewLevel()
        {
            int insertIndex = Mathf.Clamp(selectedLevelIndex_ + 1, 0, fileData_.levels.Count);
            fileData_.levels.Insert(insertIndex, CreateDefaultLevel(insertIndex + 1));
            selectedLevelIndex_ = insertIndex;
            ResequenceLevelIds();
            SetStatus("Added a new level.", MessageType.Info);
        }

        private void DuplicateSelectedLevel()
        {
            MovingGameLevelData source = fileData_.levels[selectedLevelIndex_];
            MovingGameLevelData clone = CloneLevel(source);
            clone.name = $"{clone.name} Copy";
            fileData_.levels.Insert(selectedLevelIndex_ + 1, clone);
            selectedLevelIndex_++;
            ResequenceLevelIds();
            SetStatus("Duplicated the selected level.", MessageType.Info);
        }

        private void DeleteSelectedLevel()
        {
            if (fileData_.levels.Count <= 1)
            {
                SetStatus("At least one level must remain.", MessageType.Warning);
                return;
            }

            fileData_.levels.RemoveAt(selectedLevelIndex_);
            selectedLevelIndex_ = Mathf.Clamp(selectedLevelIndex_, 0, fileData_.levels.Count - 1);
            ResequenceLevelIds();
            SetStatus("Deleted the selected level.", MessageType.Info);
        }

        private void MoveSelectedLevel(int direction)
        {
            int targetIndex = selectedLevelIndex_ + direction;
            if (targetIndex < 0 || targetIndex >= fileData_.levels.Count)
            {
                return;
            }

            (fileData_.levels[selectedLevelIndex_], fileData_.levels[targetIndex]) =
                (fileData_.levels[targetIndex], fileData_.levels[selectedLevelIndex_]);

            selectedLevelIndex_ = targetIndex;
            ResequenceLevelIds();
            SetStatus(direction < 0 ? "Moved level up." : "Moved level down.", MessageType.Info);
        }

        private void SaveLevels()
        {
            NormalizeFile();
            List<string> errors = MovingGameLevelEditorValidator.ValidateFile(fileData_);

            if (errors.Count > 0)
            {
                SetStatus(string.Join("\n", errors.Take(6)), MessageType.Error);
                return;
            }

            MovingGameLevelEditorSerializer.Save(levelsAssetPath_, fileData_);
            SetStatus("Saved levels JSON successfully.", MessageType.Info);
        }

        private void ReloadLevels()
        {
            try
            {
                fileData_ = MovingGameLevelEditorSerializer.Load(levelsAssetPath_);
                NormalizeFile();
                selectedLevelIndex_ = Mathf.Clamp(selectedLevelIndex_, 0, Mathf.Max(0, fileData_.levels.Count - 1));
                SetStatus("Loaded levels JSON.", MessageType.Info);
            }
            catch (Exception e)
            {
                SetStatus(e.Message, MessageType.Error);
            }
        }

        private void ValidateFile()
        {
            NormalizeFile();
            List<string> errors = MovingGameLevelEditorValidator.ValidateFile(fileData_);

            if (errors.Count == 0)
            {
                SetStatus("Validation passed.", MessageType.Info);
                return;
            }

            SetStatus(string.Join("\n", errors.Take(8)), MessageType.Error);
        }

        private void PingLevelsAsset()
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(levelsAssetPath_);
            if (asset != null)
            {
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }
        }

        private void NormalizeFile()
        {
            fileData_.gameType ??= "moving_game";
            if (fileData_.version <= 0)
            {
                fileData_.version = 1;
            }

            fileData_.levels ??= new List<MovingGameLevelData>();

            foreach (MovingGameLevelData level in fileData_.levels)
            {
                if (level == null)
                {
                    continue;
                }

                MovingGameLevelEditorValidator.EnsureCollections(level);
                ClampAndPruneLevel(level);
                DeduplicateLevel(level);
            }

            ResequenceLevelIds();
        }

        private void ClampAndPruneLevel(MovingGameLevelData level)
        {
            level.width = Mathf.Clamp(level.width, 1, 5);
            level.height = Mathf.Clamp(level.height, 1, 5);
            level.startPosition.x = Mathf.Clamp(level.startPosition.x, 0, level.width - 1);
            level.startPosition.y = Mathf.Clamp(level.startPosition.y, 0, level.height - 1);

            level.foodPositions = level.foodPositions.Where(position => InBounds(level, position)).ToList();
            level.blockedPositions = level.blockedPositions.Where(position => InBounds(level, position)).ToList();
            level.breakableBlockedPositions = level.breakableBlockedPositions.Where(position => InBounds(level, position)).ToList();
            level.holePositions = level.holePositions.Where(position => InBounds(level, position)).ToList();
            level.toggleBlockedTiles = level.toggleBlockedTiles.Where(tile => InBounds(level, tile.x, tile.y)).ToList();
            level.toggleSwitchTiles = level.toggleSwitchTiles.Where(tile => InBounds(level, tile.x, tile.y)).ToList();

            RemoveContentAt(level, level.startPosition.x, level.startPosition.y);
        }

        private void DeduplicateLevel(MovingGameLevelData level)
        {
            level.foodPositions = DeduplicatePositions(level.foodPositions);
            level.blockedPositions = DeduplicatePositions(level.blockedPositions);
            level.breakableBlockedPositions = DeduplicatePositions(level.breakableBlockedPositions);
            level.holePositions = DeduplicatePositions(level.holePositions);
            level.toggleBlockedTiles = level.toggleBlockedTiles
                .GroupBy(tile => $"{tile.x}:{tile.y}")
                .Select(group => group.Last())
                .ToList();
            level.toggleSwitchTiles = level.toggleSwitchTiles
                .GroupBy(tile => $"{tile.x}:{tile.y}")
                .Select(group => group.Last())
                .ToList();
        }

        private static List<PositionData> DeduplicatePositions(List<PositionData> positions)
        {
            return positions
                .GroupBy(position => $"{position.x}:{position.y}")
                .Select(group => group.Last())
                .ToList();
        }

        private static bool InBounds(MovingGameLevelData level, PositionData position)
        {
            return position != null && InBounds(level, position.x, position.y);
        }

        private static bool InBounds(MovingGameLevelData level, int x, int y)
        {
            return x >= 0 && x < level.width && y >= 0 && y < level.height;
        }

        private static void AddPosition(List<PositionData> positions, int x, int y)
        {
            if (positions.Any(position => position.x == x && position.y == y))
            {
                return;
            }

            positions.Add(new PositionData { x = x, y = y });
        }

        private static void RemoveContentAt(MovingGameLevelData level, int x, int y)
        {
            RemoveObstacleContentAt(level, x, y);
            level.foodPositions.RemoveAll(position => position.x == x && position.y == y);
        }

        private static void RemoveObstacleContentAt(MovingGameLevelData level, int x, int y)
        {
            level.blockedPositions.RemoveAll(position => position.x == x && position.y == y);
            level.breakableBlockedPositions.RemoveAll(position => position.x == x && position.y == y);
            level.holePositions.RemoveAll(position => position.x == x && position.y == y);
            level.toggleBlockedTiles.RemoveAll(tile => tile.x == x && tile.y == y);
            level.toggleSwitchTiles.RemoveAll(tile => tile.x == x && tile.y == y);
        }

        private bool IsStart(MovingGameLevelData level, int x, int y)
        {
            return level.startPosition.x == x && level.startPosition.y == y;
        }

        private void DrawCellVisual(MovingGameLevelData level, int x, int y, Rect rect)
        {
            EnsureEditorAssetsLoaded();

            Rect backgroundRect = Shrink(rect, 1f);
            EditorGUI.DrawRect(backgroundRect, EmptyCellTint);
            DrawTexture(backgroundRect, cellFrameTexture_, Color.white);

            if (IsStart(level, x, y))
            {
                DrawTexture(Shrink(rect, 10f), startTexture_, Color.white);
                DrawLabel(
                    new Rect(rect.x + 4f, rect.y + 4f, rect.width - 8f, 16f),
                    "START",
                    cellBadgeLabelStyle_,
                    new Color(0.18f, 0.52f, 0.22f, 0.9f));
                DrawLabel(
                    new Rect(rect.x, rect.yMax - 18f, rect.width, 14f),
                    Arrow(level.startDirection),
                    cellDirectionLabelStyle_);
                return;
            }

            ToggleSwitchTileData toggleSwitch = level.toggleSwitchTiles.FirstOrDefault(tile => tile.x == x && tile.y == y);
            if (toggleSwitch != null)
            {
                Color groupColor = GetGroupColor(toggleSwitch.groupId);
                DrawTexture(Shrink(rect, 8f), switchBaseTexture_, new Color(0.83f, 0.83f, 0.83f, 1f));
                DrawTexture(Shrink(rect, 8f), switchTexture_, groupColor);
                DrawGroupBadge(rect, toggleSwitch.groupId, groupColor, "SW");
                return;
            }

            ToggleBlockedTileData toggleObstacle = level.toggleBlockedTiles.FirstOrDefault(tile => tile.x == x && tile.y == y);
            if (toggleObstacle != null)
            {
                Color groupColor = GetGroupColor(toggleObstacle.groupId);
                DrawTexture(Shrink(rect, 8f), blockedTexture_, groupColor);

                if (!toggleObstacle.isOn)
                {
                    EditorGUI.DrawRect(Shrink(rect, 8f), ToggleOffTint);
                }

                DrawGroupBadge(rect, toggleObstacle.groupId, groupColor, toggleObstacle.isOn ? "TG" : "tg");
                return;
            }

            if (level.foodPositions.Any(position => position.x == x && position.y == y))
            {
                DrawTexture(Shrink(rect, 9f), foodTexture_, Color.white);
                return;
            }

            if (level.blockedPositions.Any(position => position.x == x && position.y == y))
            {
                DrawTexture(Shrink(rect, 8f), blockedTexture_, Color.white);
                return;
            }

            if (level.breakableBlockedPositions.Any(position => position.x == x && position.y == y))
            {
                DrawTexture(Shrink(rect, 8f), breakableTexture_, Color.white);
                return;
            }

            if (level.holePositions.Any(position => position.x == x && position.y == y))
            {
                DrawTexture(Shrink(rect, 6f), holeTexture_, Color.white);
                return;
            }

            DrawLabel(
                new Rect(rect.x, rect.yMax - 16f, rect.width, 14f),
                $"{x},{y}",
                cellCoordinateLabelStyle_);
        }

        private void EnsureSelectedLevelIsValid()
        {
            selectedLevelIndex_ = Mathf.Clamp(selectedLevelIndex_, 0, Mathf.Max(0, fileData_.levels.Count - 1));
        }

        private static MovingGameLevelData CreateDefaultLevel(int id)
        {
            return new MovingGameLevelData
            {
                id = id,
                name = $"Level {id}",
                hint = "Describe the goal for this level.",
                difficulty = 1,
                width = 5,
                height = 5,
                startPosition = new PositionData { x = 0, y = 0 },
                startDirection = "Right",
                foodPositions = new List<PositionData> { new PositionData { x = 1, y = 0 } },
                blockedPositions = new List<PositionData>(),
                breakableBlockedPositions = new List<PositionData>(),
                holePositions = new List<PositionData>(),
                toggleBlockedTiles = new List<ToggleBlockedTileData>(),
                toggleSwitchTiles = new List<ToggleSwitchTileData>()
            };
        }

        private static MovingGameLevelData CloneLevel(MovingGameLevelData source)
        {
            return new MovingGameLevelData
            {
                id = source.id,
                name = source.name,
                hint = source.hint,
                difficulty = source.difficulty,
                width = source.width,
                height = source.height,
                startPosition = new PositionData { x = source.startPosition.x, y = source.startPosition.y },
                startDirection = source.startDirection,
                foodPositions = source.foodPositions.Select(position => new PositionData { x = position.x, y = position.y }).ToList(),
                blockedPositions = source.blockedPositions.Select(position => new PositionData { x = position.x, y = position.y }).ToList(),
                breakableBlockedPositions = source.breakableBlockedPositions.Select(position => new PositionData { x = position.x, y = position.y }).ToList(),
                holePositions = source.holePositions.Select(position => new PositionData { x = position.x, y = position.y }).ToList(),
                toggleBlockedTiles = source.toggleBlockedTiles.Select(tile => new ToggleBlockedTileData
                {
                    x = tile.x,
                    y = tile.y,
                    groupId = tile.groupId,
                    isOn = tile.isOn
                }).ToList(),
                toggleSwitchTiles = source.toggleSwitchTiles.Select(tile => new ToggleSwitchTileData
                {
                    x = tile.x,
                    y = tile.y,
                    groupId = tile.groupId
                }).ToList()
            };
        }

        private static int DirectionIndex(string direction)
        {
            return direction switch
            {
                "Up" => 0,
                "Right" => 1,
                "Down" => 2,
                "Left" => 3,
                _ => 1
            };
        }

        private static string Arrow(string direction)
        {
            return direction switch
            {
                "Up" => "^",
                "Right" => ">",
                "Down" => "v",
                "Left" => "<",
                _ => ">"
            };
        }

        private void ResequenceLevelIds()
        {
            if (fileData_?.levels == null)
            {
                return;
            }

            for (int i = 0; i < fileData_.levels.Count; i++)
            {
                if (fileData_.levels[i] != null)
                {
                    fileData_.levels[i].id = i + 1;
                }
            }
        }

        private void EnsureEditorAssetsLoaded()
        {
            cellFrameTexture_ ??= AssetDatabase.LoadAssetAtPath<Texture2D>(CellFrameTexturePath);
            foodTexture_ ??= AssetDatabase.LoadAssetAtPath<Texture2D>(FoodTexturePath);
            blockedTexture_ ??= AssetDatabase.LoadAssetAtPath<Texture2D>(BlockedTexturePath);
            breakableTexture_ ??= AssetDatabase.LoadAssetAtPath<Texture2D>(BreakableTexturePath);
            holeTexture_ ??= AssetDatabase.LoadAssetAtPath<Texture2D>(HoleTexturePath);
            switchTexture_ ??= AssetDatabase.LoadAssetAtPath<Texture2D>(SwitchTexturePath);
            switchBaseTexture_ ??= AssetDatabase.LoadAssetAtPath<Texture2D>(SwitchBaseTexturePath);
            startTexture_ ??= AssetDatabase.LoadAssetAtPath<Texture2D>(StartTexturePath);
            movingGameSettings_ ??= AssetDatabase.LoadAssetAtPath<MovingGameSettings>(MovingGameSettingsAssetPath);

            cellOverlayLabelStyle_ ??= new GUIStyle(EditorStyles.whiteMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            cellDirectionLabelStyle_ ??= new GUIStyle(EditorStyles.whiteBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                normal = { textColor = new Color(0.14f, 0.14f, 0.14f, 1f) }
            };

            cellCoordinateLabelStyle_ ??= new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.35f, 0.35f, 0.35f, 0.95f) }
            };

            cellBadgeLabelStyle_ ??= new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 8,
                normal = { textColor = Color.white }
            };
        }

        private Color GetGroupColor(int groupId)
        {
            return movingGameSettings_ != null
                ? movingGameSettings_.GetColorForGroupId(groupId)
                : Color.white;
        }

        private void DrawGroupBadge(Rect rect, int groupId, Color badgeColor, string prefix)
        {
            Rect badgeRect = new(rect.x + 3f, rect.y + 3f, 26f, 14f);
            EditorGUI.DrawRect(badgeRect, badgeColor);
            DrawLabel(badgeRect, prefix, cellBadgeLabelStyle_);

            DrawLabel(
                new Rect(rect.x, rect.yMax - 16f, rect.width, 14f),
                $"G{groupId}",
                cellOverlayLabelStyle_);
        }

        private static Rect Shrink(Rect rect, float amount)
        {
            return new Rect(
                rect.x + amount,
                rect.y + amount,
                Mathf.Max(0f, rect.width - (amount * 2f)),
                Mathf.Max(0f, rect.height - (amount * 2f)));
        }

        private static void DrawTexture(Rect rect, Texture2D texture, Color color)
        {
            if (texture == null)
            {
                return;
            }

            Color previousColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
            GUI.color = previousColor;
        }

        private static void DrawLabel(Rect rect, string text, GUIStyle style, Color? background = null)
        {
            if (background.HasValue)
            {
                EditorGUI.DrawRect(rect, background.Value);
            }

            GUI.Label(rect, text, style);
        }

        private void SetStatus(string message, MessageType messageType)
        {
            statusMessage_ = message;
            statusMessageType_ = messageType;
            Repaint();
        }

        private void ClearStatus()
        {
            statusMessage_ = string.Empty;
            statusMessageType_ = MessageType.Info;
        }
    }
}
