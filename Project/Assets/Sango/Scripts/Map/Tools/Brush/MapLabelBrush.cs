using Sango.Render;
using Sango.Tools.UndoRedo;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Tools
{
    public class MapLabelBrush : BrushBase
    {
        public enum LabelType
        {
            Blue,
            Red,
            Green,
            Yellow
        }

        public class MapLabelEditCommand : IUndoableCommand
        {
            private MapEditor editor;
            private MapLabel label;
            private ActionType actionType;
            private string actionName;

            public enum ActionType
            {
                Create,
                Delete
            }

            public MapLabelEditCommand(MapEditor editor, MapLabel label, ActionType actionType, string actionName)
            {
                this.editor = editor;
                this.label = label;
                this.actionType = actionType;
                this.actionName = actionName;
            }

            public string Description
            {
                get { return actionName; }
            }

            public void Execute()
            {
                if (actionType == ActionType.Create)
                {
                    editor.map.mapLabelSet.AddLabel(label.labelText, label.position, label.textColor, label.fontSize);
                }
                else if (actionType == ActionType.Delete)
                {
                    editor.map.mapLabelSet.RemoveLabel(label);
                }
            }

            public void Undo()
            {
                if (actionType == ActionType.Create)
                {
                    editor.map.mapLabelSet.RemoveLabel(label);
                }
                else if (actionType == ActionType.Delete)
                {
                    editor.map.mapLabelSet.AddLabel(label.labelText, label.position, label.textColor, label.fontSize);
                }
            }

            public void Redo()
            {
                Execute();
            }

            public void Destroy()
            {
                // 清理资源
            }
        }

        private LabelType currentLabelType = LabelType.Blue;
        private string labelText = "标记";
        private int fontSize = 24;
        private MapLabel previewLabel = null;
        private bool anchorByGrid = true;

        private string[] labelTypeNames = { "蓝色", "红色", "绿色", "黄色" };
        private Color32[] labelColors = 
        {
            new Color32(0, 122, 255, 255), // 蓝色
            new Color32(255, 0, 0, 255),     // 红色
            new Color32(0, 255, 0, 255),     // 绿色
            new Color32(255, 255, 0, 255)    // 黄色
        };

        public MapLabelBrush(MapEditor e) : base(e)
        {
        }

        public override void OnBrushTypeChange()
        {
        }

        public override void OnSeasonChanged(int curSeason)
        {
        }

        public override void Clear()
        {
            ClearPreviewLabel();
        }

        public override void Modify(Vector3 center, MapEditor editor)
        {
            Color32 color = labelColors[(int)currentLabelType];

            MapLabel label = editor.map.mapLabelSet.AddLabel(labelText, previewLabel.position, color, fontSize);

            // 创建添加标注命令并执行
            MapLabelEditCommand command = new MapLabelEditCommand(editor, label, MapLabelEditCommand.ActionType.Create, "添加地图标记");
            editor.undoRedoManager.AddCommand(command);

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                ClearPreviewLabel();
            }
        }

        public override void OnGUI()
        {
            GUILayout.Label("地图标记设置");
            GUILayout.Space(10);

            // 标记类型选择
            GUILayout.Label("标记类型:");
            int typeIndex = GUILayout.SelectionGrid((int)currentLabelType, labelTypeNames, 2);
            if (typeIndex != (int)currentLabelType)
            {
                currentLabelType = (LabelType)typeIndex;
                UpdatePreviewLabel();
            }
            GUILayout.Space(10);

            // 标记文本输入
            GUILayout.Label("标记文本:");
            string newText = GUILayout.TextField(labelText, GUILayout.Width(200));
            if (newText != labelText)
            {
                labelText = newText;
                UpdatePreviewLabel();
            }
            GUILayout.Space(10);

            // 字号大小设置
            GUILayout.Label("字号大小:");
            int newFontSize = (int)GUILayout.HorizontalSlider(fontSize, 12, 48, GUILayout.Width(200));
            if (newFontSize != fontSize)
            {
                fontSize = newFontSize;
                UpdatePreviewLabel();
            }
            GUILayout.Label(fontSize.ToString(), GUILayout.Width(40));
            GUILayout.Space(10);

            // 贴合格子中心选项
            anchorByGrid = GUILayout.Toggle(anchorByGrid, "贴合格子中心");
        }

        public override void OnEnter()
        {
        }

        public override void DrawGizmos(Vector3 center)
        {
            if (previewLabel == null)
            {
                CreatePreviewLabel();
            }

            Vector3 pos = center;
            if (anchorByGrid)
            {
                Sango.Hexagon.Hex hex = editor.map.mapGrid.hexWorld.PositionToHex(center);
                Sango.Hexagon.Coord offset = Sango.Hexagon.Coord.OffsetFromCube(hex);
                pos = editor.map.mapGrid.hexWorld.CoordsToPosition(offset.col, offset.row);
                pos.y = editor.map.mapGrid.GetGridHeight(offset.col, offset.row) + 5f; // 稍微抬高一点，确保在地形上方
            }

            if (previewLabel != null)
            {
                previewLabel.position = pos;
            }
        }

        private void CreatePreviewLabel()
        {
            if (previewLabel == null)
            {
                GameObject labelObj = new GameObject("PreviewMapLabel");
                labelObj.transform.parent = null;
                labelObj.SetActive(true);

                MapLabel label = labelObj.AddComponent<MapLabel>();
                Color32 color = labelColors[(int)currentLabelType];
                label.Initialize(editor.map, labelText, Vector3.zero, color, fontSize);
                label.visible = true;

                previewLabel = label;
            }
        }

        private void UpdatePreviewLabel()
        {
            if (previewLabel != null)
            {
                Color32 color = labelColors[(int)currentLabelType];
                previewLabel.UpdateLabel(labelText, color, fontSize);
            }
        }

        private void ClearPreviewLabel()
        {
            if (previewLabel != null)
            {
                previewLabel.Destroy();
                previewLabel = null;
            }
        }

        public override void Update()
        {
            if (previewLabel == null && Input.GetKeyDown(KeyCode.Space))
            {
                CreatePreviewLabel();
            }

            if (previewLabel != null)
            {
                // 右键或者Esc取消预览
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    ClearPreviewLabel();
                    return;
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, editor.map.showLimitLength + 2000, editor.rayCastLayer))
                {
                    if (hit.point != lastCenter)
                    {
                        if (!IsPointerOverUI() && Input.GetMouseButtonDown(0))
                        {
                            Modify(hit.point, editor);
                            lastCenter = hit.point;
                        }
                        DrawGizmos(hit.point);
                    }
                }
            }
        }

        /// <summary>
        /// 拖拽开始（地图标记放置不支持拖拽）
        /// </summary>
        /// <param name="center">中心点</param>
        public override void OnDragStart(Vector3 center)
        {
            // 地图标记放置只支持点击操作，不支持拖拽
        }

        /// <summary>
        /// 拖拽过程（地图标记放置不支持拖拽）
        /// </summary>
        /// <param name="center">中心点</param>
        public override void OnDrag(Vector3 center)
        {
            // 地图标记放置只支持点击操作，不支持拖拽
        }

        /// <summary>
        /// 拖拽结束（地图标记放置不支持拖拽）
        /// </summary>
        /// <param name="center">中心点</param>
        public override void OnDragEnd(Vector3 center)
        {
            // 地图标记放置只支持点击操作，不支持拖拽
        }
    }
}
