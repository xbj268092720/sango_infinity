using System.Collections.Generic;
using UnityEngine;

namespace Sango.Tools
{
    public class MapEditorAutoSave
    {
        private MapEditor editor;
        
        private float autoSaveInterval = 5 * 60f;
        private float autoSaveTimer = 0f;
        private bool autoSaveEnabled = true;
        private int autoSaveLimit = 20;
        private List<string> autoSavePaths = new List<string>();
        
        private float saveNotificationTimer = 0f;
        private string saveNotificationMessage = "";
        private bool showSaveNotification = false;

        public bool AutoSaveEnabled { get => autoSaveEnabled; set => autoSaveEnabled = value; }
        public float AutoSaveInterval { get => autoSaveInterval; set => autoSaveInterval = value; }
        public int AutoSaveLimit { get => autoSaveLimit; set => autoSaveLimit = value; }

        public MapEditorAutoSave(MapEditor editor)
        {
            this.editor = editor;
        }

        public void Update()
        {
            if (autoSaveEnabled)
            {
                autoSaveTimer += Time.deltaTime;
                if (autoSaveTimer >= autoSaveInterval)
                {
                    AutoSave();
                    autoSaveTimer = 0f;
                }
            }

            if (showSaveNotification)
            {
                saveNotificationTimer += Time.deltaTime;
                if (saveNotificationTimer >= 3f)
                {
                    showSaveNotification = false;
                    saveNotificationMessage = "";
                    saveNotificationTimer = 0f;
                }
            }
        }

        public void QuickSave()
        {
            if (!string.IsNullOrEmpty(editor.lastSavedPath))
            {
                editor.map.SaveMap(editor.lastSavedPath);
                SaveBaseMap(editor.lastSavedPath);
                
                if (editor.scenario != null && !string.IsNullOrEmpty(editor.scenario.FilePath))
                {
                    editor.CorrectCityPositions();
                    editor.scenario.Export(editor.scenario.FilePath);
                    Sango.Log.Info("剧本数据已同步保存");
                }
                
                string message = $"地图已快捷保存到: {System.IO.Path.GetFileName(editor.lastSavedPath)}";
                Sango.Log.Info(message);
                ShowSaveNotification(message);
            }
            else
            {
                string path = WindowDialog.SaveFileDialog("map.bin", "地图文件(*.bin)\0*.bin;\0\0");
                if (path != null)
                {
                    editor.lastSavedPath = path;
                    editor.map.SaveMap(path);
                    SaveBaseMap(path);
                    
                    if (editor.scenario != null && !string.IsNullOrEmpty(editor.scenario.FilePath))
                    {
                        editor.CorrectCityPositions();
                        editor.scenario.Export(editor.scenario.FilePath);
                        Sango.Log.Info("剧本数据已同步保存");
                    }
                    
                    string message = $"地图已保存到: {System.IO.Path.GetFileName(path)}";
                    Sango.Log.Info(message);
                    ShowSaveNotification(message);
                }
            }
        }

        public void AutoSave()
        {
            if (!string.IsNullOrEmpty(editor.lastSavedPath))
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(editor.lastSavedPath);
                string extension = System.IO.Path.GetExtension(editor.lastSavedPath);
                string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

                string mapDirectory = System.IO.Path.GetDirectoryName(editor.lastSavedPath);
                string contentDirectory = System.IO.Path.GetDirectoryName(mapDirectory);
                string autoSaveDirectory = System.IO.Path.Combine(contentDirectory, "AutoSaveMap");

                if (!System.IO.Directory.Exists(autoSaveDirectory))
                {
                    System.IO.Directory.CreateDirectory(autoSaveDirectory);
                }

                string autoSavePath = System.IO.Path.Combine(autoSaveDirectory, $"{fileName}_auto_save_{timestamp}{extension}");

                editor.map.SaveMap(autoSavePath);
                SaveBaseMap(autoSavePath);
                
                if (editor.scenario != null)
                {
                    editor.CorrectCityPositions();
                    string scenarioAutoSavePath = System.IO.Path.Combine(autoSaveDirectory, $"{fileName}_auto_save_{timestamp}.json");
                    editor.scenario.Export(scenarioAutoSavePath);
                    Sango.Log.Info("剧本数据已同步自动保存");
                }
                
                string message = $"地图已自动保存到: {System.IO.Path.GetFileName(autoSavePath)}";
                Sango.Log.Info(message);
                ShowSaveNotification(message);

                autoSavePaths.Add(autoSavePath);

                while (autoSavePaths.Count > autoSaveLimit)
                {
                    string oldestPath = autoSavePaths[0];
                    autoSavePaths.RemoveAt(0);

                    if (System.IO.File.Exists(oldestPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldestPath);
                            Sango.Log.Info($"已删除旧的自动保存文件: {oldestPath}");
                        }
                        catch (System.Exception e)
                        {
                            Sango.Log.Error($"删除旧自动保存文件失败: {e.Message}");
                        }
                    }

                    try
                    {
                        string oldDir = System.IO.Path.GetDirectoryName(oldestPath);
                        string oldFileName = System.IO.Path.GetFileNameWithoutExtension(oldestPath);
                        int autoSaveIndex = oldFileName.LastIndexOf("_auto_save_");
                        if (autoSaveIndex > 0)
                        {
                            string baseMapName = oldFileName.Substring(0, autoSaveIndex);
                            string baseTexPath = System.IO.Path.Combine(oldDir, "..", "Assets", "Map", baseMapName, "BaseTex");
                            if (System.IO.Directory.Exists(baseTexPath))
                            {
                                System.IO.Directory.Delete(baseTexPath, true);
                                Sango.Log.Info($"已删除旧的自动保存底图文件夹: {baseTexPath}");
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Sango.Log.Error($"删除旧自动保存底图失败: {e.Message}");
                    }
                }
            }
        }

        public void InitializeAutoSavePaths()
        {
            autoSavePaths.Clear();

            if (!string.IsNullOrEmpty(editor.lastSavedPath))
            {
                string mapDirectory = System.IO.Path.GetDirectoryName(editor.lastSavedPath);
                string contentDirectory = System.IO.Path.GetDirectoryName(mapDirectory);
                string autoSaveDirectory = System.IO.Path.Combine(contentDirectory, "AutoSaveMap");

                if (System.IO.Directory.Exists(autoSaveDirectory))
                {
                    string[] autoSaveFiles = System.IO.Directory.GetFiles(autoSaveDirectory, "*_auto_save_*.bin");

                    if (autoSaveFiles.Length > 0)
                    {
                        System.Array.Sort(autoSaveFiles, (a, b) =>
                            System.IO.File.GetLastWriteTime(a).CompareTo(System.IO.File.GetLastWriteTime(b)));

                        autoSavePaths.AddRange(autoSaveFiles);

                        while (autoSavePaths.Count > autoSaveLimit)
                        {
                            string oldestPath = autoSavePaths[0];
                            autoSavePaths.RemoveAt(0);

                            if (System.IO.File.Exists(oldestPath))
                            {
                                try
                                {
                                    System.IO.File.Delete(oldestPath);
                                    Sango.Log.Info($"已删除旧的自动保存文件: {oldestPath}");
                                }
                                catch (System.Exception e)
                                {
                                    Sango.Log.Error($"删除旧自动保存文件失败: {e.Message}");
                                }
                            }

                            try
                            {
                                string oldDir = System.IO.Path.GetDirectoryName(oldestPath);
                                string oldFileName = System.IO.Path.GetFileNameWithoutExtension(oldestPath);
                                int autoSaveIndex = oldFileName.LastIndexOf("_auto_save_");
                                if (autoSaveIndex > 0)
                                {
                                    string baseMapName = oldFileName.Substring(0, autoSaveIndex);
                                    string baseTexPath = System.IO.Path.Combine(oldDir, "..", "Assets", "Map", baseMapName, "BaseTex");
                                    if (System.IO.Directory.Exists(baseTexPath))
                                    {
                                        System.IO.Directory.Delete(baseTexPath, true);
                                        Sango.Log.Info($"已删除旧的自动保存底图文件夹: {baseTexPath}");
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {
                                Sango.Log.Error($"删除旧自动保存底图失败: {e.Message}");
                            }
                        }

                        Sango.Log.Info($"已加载 {autoSavePaths.Count} 个自动保存文件");
                    }
                }
            }
        }

        private void SaveBaseMap(string mapPath)
        {
            int maxRetries = 3;
            int retryCount = 0;
            bool saveSuccess = false;

            while (!saveSuccess && retryCount < maxRetries)
            {
                try
                {
                    string mapDirectory = System.IO.Path.GetDirectoryName(mapPath);
                    string mapName = System.IO.Path.GetFileNameWithoutExtension(mapPath);
                    if (mapName.Contains("_auto_save"))
                    {
                        mapName = mapName.Substring(0, mapName.IndexOf("_auto_save"));
                    }
                    string baseMapPath = System.IO.Path.Combine(mapDirectory, "..", "Assets", "Map", mapName, "BaseTex");

                    System.IO.Directory.CreateDirectory(baseMapPath);

                    for (int season = 0; season < 4; season++)
                    {
                        if (editor.terrain_brush != null && editor.terrain_brush.baseMap != null && editor.terrain_brush.baseMap[season] != null)
                        {
                            string baseMapFileName = System.IO.Path.Combine(baseMapPath, $"BaseMap{season}.png");
                            editor.terrain_brush.SaveBaseTexture(baseMapFileName, season);
                        }
                    }

                    Sango.Log.Info($"底图已保存到: {baseMapPath}");
                    saveSuccess = true;
                }
                catch (System.Exception e)
                {
                    retryCount++;
                    Sango.Log.Error($"保存底图失败 (尝试 {retryCount}/{maxRetries}): {e.Message}");

                    if (retryCount >= maxRetries)
                    {
                        Sango.Log.Error("保存底图失败，已达到最大重试次数");
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                }
            }
        }

        public void ShowSaveNotification(string message)
        {
            saveNotificationMessage = message;
            showSaveNotification = true;
            saveNotificationTimer = 0f;
        }

        public void DrawSaveNotification()
        {
            if (showSaveNotification)
            {
                GUILayout.BeginArea(new UnityEngine.Rect(10, 10, 400, 60), GUI.skin.box);
                GUILayout.Label(saveNotificationMessage, GUILayout.ExpandWidth(true));
                GUILayout.EndArea();
            }
        }
    }
}
