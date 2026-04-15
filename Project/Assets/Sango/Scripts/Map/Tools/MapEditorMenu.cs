using UnityEngine;

namespace Sango.Tools
{
    public class MapEditorMenu
    {
        private MapEditor editor;
        
        private bool showFileMenu = false;
        private bool showEditMenu = false;
        private bool showViewMenu = false;
        private bool showRenderMenu = false;
        
        private UnityEngine.Rect fileMenuRect;
        private UnityEngine.Rect editMenuRect;
        private UnityEngine.Rect viewMenuRect;
        private UnityEngine.Rect renderMenuRect;

        private string[] toolbarSeason = { "秋", "春", "夏", "冬" };

        public MapEditorMenu(MapEditor editor)
        {
            this.editor = editor;
        }

        public void Draw()
        {
            DrawTopMenuBar();
        }

        public void DrawTopMenuBar()
        {
            GUILayout.BeginArea(new UnityEngine.Rect(0, 0, Screen.width, 30), GUI.skin.box);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("文件", GUILayout.Width(60), GUILayout.Height(28)))
            {
                showFileMenu = !showFileMenu;
                showEditMenu = false;
                showViewMenu = false;
                showRenderMenu = false;
                fileMenuRect = new UnityEngine.Rect(0, 30, 150, 200);
            }

            if (GUILayout.Button("编辑", GUILayout.Width(60), GUILayout.Height(28)))
            {
                showEditMenu = !showEditMenu;
                showFileMenu = false;
                showViewMenu = false;
                showRenderMenu = false;
                editMenuRect = new UnityEngine.Rect(60, 30, 150, 150);
            }

            if (GUILayout.Button("视图", GUILayout.Width(60), GUILayout.Height(28)))
            {
                showViewMenu = !showViewMenu;
                showFileMenu = false;
                showEditMenu = false;
                showRenderMenu = false;
                viewMenuRect = new UnityEngine.Rect(120, 30, 150, 250);
            }

            if (GUILayout.Button("渲染", GUILayout.Width(60), GUILayout.Height(28)))
            {
                showRenderMenu = !showRenderMenu;
                showFileMenu = false;
                showEditMenu = false;
                showViewMenu = false;
                renderMenuRect = new UnityEngine.Rect(180, 30, 150, 120);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (showFileMenu)
            {
                DrawFileMenu();
            }

            if (showEditMenu)
            {
                DrawEditMenu();
            }

            if (showViewMenu)
            {
                DrawViewMenu();
            }

            if (showRenderMenu)
            {
                DrawRenderMenu();
            }

            if (Event.current.type == EventType.MouseDown &&
                !fileMenuRect.Contains(Event.current.mousePosition) &&
                !editMenuRect.Contains(Event.current.mousePosition) &&
                !viewMenuRect.Contains(Event.current.mousePosition) &&
                !renderMenuRect.Contains(Event.current.mousePosition))
            {
                UnityEngine.Rect menuBarRect = new UnityEngine.Rect(0, 0, Screen.width, 30);
                if (!menuBarRect.Contains(Event.current.mousePosition))
                {
                    showFileMenu = false;
                    showEditMenu = false;
                    showViewMenu = false;
                    showRenderMenu = false;
                }
            }
        }

        private void DrawFileMenu()
        {
            GUILayout.BeginArea(fileMenuRect, GUI.skin.box);

            if (GUILayout.Button("新建地图"))
            {
                editor.windows.ShowNewMapWindow();
                showFileMenu = false;
            }

            if (GUILayout.Button("加载地图"))
            {
                string[] path = WindowDialog.OpenFileDialog("地图文件(*.bin)\0*.bin;\0\0");
                if (path != null)
                {
                    string fName = path[0];
                    editor.lastSavedPath = fName;
                    editor.map.LoadMap(fName);
                    EditorFreeCamera editorfree = Camera.main.gameObject.GetComponent<Sango.Tools.EditorFreeCamera>();
                    if (editorfree != null)
                        editorfree.lookAt = editor.map.mapCamera.GetCenterTransform();
                    BrushBase brush = editor.CheckBrush();
                    if (brush != null)
                        brush.OnEnter();
                    if (editor.ViewIs311Camera)
                        editor.SetCameraControlType(1);
                    else
                        editor.SetCameraControlType(0);
                    Sango.Log.Info($"地图已加载: {fName}");
                }
                showFileMenu = false;
            }

            if (GUILayout.Button("保存地图"))
            {
                string path = WindowDialog.SaveFileDialog("map.bin", "地图文件(*.bin)\0*.bin;\0\0");
                if (path != null)
                {
                    editor.lastSavedPath = path;
                    editor.map.SaveMap(path);
                    editor.autoSave.ShowSaveNotification($"地图已保存到: {System.IO.Path.GetFileName(path)}");
                }
                showFileMenu = false;
            }

            GUILayout.Space(5);
            GUILayout.Label("---");
            GUILayout.Space(5);

            if (GUILayout.Button("设置"))
            {
                editor.windows.ShowSettingsWindow();
                showFileMenu = false;
            }

            if (GUILayout.Button("退出编辑器"))
            {
                Application.Quit();
            }

            GUILayout.EndArea();
        }

        private void DrawEditMenu()
        {
            GUILayout.BeginArea(editMenuRect, GUI.skin.box);

            if (GUILayout.Button("撤销"))
            {
                editor.undoRedoManager.Undo();
                showEditMenu = false;
            }

            if (GUILayout.Button("重做"))
            {
                editor.undoRedoManager.Redo();
                showEditMenu = false;
            }

            GUILayout.Space(5);
            GUILayout.Label("---");
            GUILayout.Space(5);

            if (GUILayout.Button("清空历史"))
            {
                editor.undoRedoManager.ClearHistory();
                showEditMenu = false;
            }

            GUILayout.EndArea();
        }

        private void DrawRenderMenu()
        {
            GUILayout.BeginArea(renderMenuRect, GUI.skin.box);

            if (GUILayout.Button("灯光设置"))
            {
                editor.windows.ShowLightWindow();
                showRenderMenu = false;
            }

            if (GUILayout.Button("雾效设置"))
            {
                editor.windows.ShowFogWindow();
                showRenderMenu = false;
            }

            GUILayout.EndArea();
        }

        private void DrawViewMenu()
        {
            GUILayout.BeginArea(viewMenuRect, GUI.skin.box);

            bool fixedView = GUILayout.Toggle(editor.ViewIs311Camera, "固定视角");
            if (fixedView != editor.ViewIs311Camera)
            {
                editor.ViewIs311Camera = fixedView;
                if (editor.ViewIs311Camera)
                    editor.SetCameraControlType(1);
                else
                    editor.SetCameraControlType(0);
            }

            if (GUILayout.Button("重置相机"))
            {
                editor.map.mapCamera.position = new Vector3(0, 500, 0);
                editor.map.mapCamera.lookRotate = new Vector3(90, -90, 0);
                editor.ViewIs311Camera = false;
                editor.SetCameraControlType(0);
                Camera.main.gameObject.transform.position = editor.map.mapCamera.position;
                Camera.main.gameObject.transform.rotation = Quaternion.Euler(90, -90, 0);
            }

            GUILayout.Space(5);
            GUILayout.Label("---");
            GUILayout.Space(5);

            GUILayout.Label("季节:");
            for (int i = 0; i < toolbarSeason.Length; i++)
            {
                if (GUILayout.Button(toolbarSeason[i]))
                {
                    editor.map.curSeason = i;
                    foreach (BrushBase brush in editor.brushes)
                    {
                        brush.OnSeasonChanged(i);
                    }
                }
            }

            GUILayout.EndArea();
        }
    }
}
