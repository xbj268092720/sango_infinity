using System;
using System.IO;
using UnityEngine;

namespace Sango.Tools
{
    /// <summary>
    /// 布局管理器
    /// </summary>
    public class LayoutManager
    {
        private MapEditor editor;
        
        /// <summary>
        /// 布局文件路径
        /// </summary>
        private string layoutFilePath;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="editor">地图编辑器</param>
        public LayoutManager(MapEditor editor)
        {
            this.editor = editor;
            layoutFilePath = System.IO.Path.Combine(Application.persistentDataPath, "map_editor_layout.json");
        }
        
        /// <summary>
        /// 保存布局
        /// </summary>
        public void SaveLayout()
        {
            try
            {
                LayoutData layoutData = new LayoutData();
                
                // 保存工具栏窗口
                layoutData.toolbarWindowX = editor.editorToolsBarWindow.windowRect.x;
                layoutData.toolbarWindowY = editor.editorToolsBarWindow.windowRect.y;
                
                // 保存属性窗口
                layoutData.contentWindowX = editor.editorContentWindow.windowRect.x;
                layoutData.contentWindowY = editor.editorContentWindow.windowRect.y;
                
                // 保存操作历史窗口
                if (editor.operationHistoryWindow != null)
                {
                    layoutData.historyWindowX = editor.operationHistoryWindow.window.windowRect.x;
                    layoutData.historyWindowY = editor.operationHistoryWindow.window.windowRect.y;
                    layoutData.historyWindowVisible = editor.operationHistoryWindow.window.visible;
                }
                
                // 保存ColorPicker位置
                if (EditorUtility.picker != null)
                {
                    RectTransform rectTransform = EditorUtility.picker.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        layoutData.colorPickerX = rectTransform.anchoredPosition.x;
                        layoutData.colorPickerY = rectTransform.anchoredPosition.y;
                    }
                }
                
                // 保存到文件
                string json = JsonUtility.ToJson(layoutData, true);
                File.WriteAllText(layoutFilePath, json);
                
                Sango.Log.Info($"布局已保存到: {layoutFilePath}");
            }
            catch (Exception e)
            {
                Sango.Log.Error($"保存布局失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 加载布局
        /// </summary>
        public void LoadLayout()
        {
            try
            {
                if (File.Exists(layoutFilePath))
                {
                    string json = File.ReadAllText(layoutFilePath);
                    LayoutData layoutData = JsonUtility.FromJson<LayoutData>(json);
                    
                    // 加载工具栏窗口位置
                    editor.editorToolsBarWindow.windowRect = new UnityEngine.Rect(
                        layoutData.toolbarWindowX, 
                        layoutData.toolbarWindowY,
                        editor.editorToolsBarWindow.windowRect.width,
                        editor.editorToolsBarWindow.windowRect.height
                    );
                    
                    // 加载属性窗口位置
                    editor.editorContentWindow.windowRect = new UnityEngine.Rect(
                        layoutData.contentWindowX, 
                        layoutData.contentWindowY,
                        editor.editorContentWindow.windowRect.width,
                        editor.editorContentWindow.windowRect.height
                    );
                    
                    // 加载操作历史窗口位置
                    if (editor.operationHistoryWindow != null)
                    {
                        editor.operationHistoryWindow.window.windowRect = new UnityEngine.Rect(
                            layoutData.historyWindowX,
                            layoutData.historyWindowY,
                            editor.operationHistoryWindow.window.windowRect.width,
                            editor.operationHistoryWindow.window.windowRect.height
                        );
                        editor.operationHistoryWindow.window.visible = layoutData.historyWindowVisible;
                    }
                    
                    // 加载ColorPicker位置
                    if (EditorUtility.picker != null)
                    {
                        RectTransform rectTransform = EditorUtility.picker.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            rectTransform.anchoredPosition = new Vector2(layoutData.colorPickerX, layoutData.colorPickerY);
                        }
                    }
                    
                    Sango.Log.Info($"布局已从: {layoutFilePath} 加载");
                }
                else
                {
                    Sango.Log.Info("布局文件不存在，使用默认布局");
                }
            }
            catch (Exception e)
            {
                Sango.Log.Error($"加载布局失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 布局数据结构
        /// </summary>
        [Serializable]
        private class LayoutData
        {
            public float toolbarWindowX;
            public float toolbarWindowY;
            public float contentWindowX;
            public float contentWindowY;
            public float historyWindowX;
            public float historyWindowY;
            public bool historyWindowVisible;
            public float colorPickerX;
            public float colorPickerY;
        }
    }
}