    using Sango.Tools.UndoRedo;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Tools
{
    /// <summary>
    /// 操作历史记录窗口
    /// </summary>
    public class OperationHistoryWindow
    {
        private MapEditor editor;
        private EditorWindow window;
        private UnityEngine.Rect windowRect = new UnityEngine.Rect(850, 100, 150, 400);
        private Vector2 scrollPosition = Vector2.zero;
        private int selectedIndex = -1;
        private bool showWindow = false;
        
        /// <summary>
        /// 历史记录列表（从旧到新）
        /// </summary>
        private List<IUndoableCommand> historyList = new List<IUndoableCommand>();
        
        /// <summary>
        /// 当前位置索引
        /// </summary>
        private int currentIndex = -1;
        
        public OperationHistoryWindow(MapEditor editor)
        {
            this.editor = editor;
            window = EditorWindow.AddWindow(2, windowRect, DrawWindow, "操作历史");
            window.visible = showWindow;
            
            // 订阅事件
            editor.undoRedoManager.CommandAdded += OnCommandAdded;
            editor.undoRedoManager.HistoryChanged += OnHistoryChanged;
        }
        
        /// <summary>
        /// 显示/隐藏窗口
        /// </summary>
        public void ToggleWindow()
        {
            showWindow = !showWindow;
            window.visible = showWindow;
            if (showWindow)
            {
                UpdateHistoryList();
            }
        }
        
        /// <summary>
        /// 更新历史记录列表
        /// </summary>
        public void UpdateHistoryList()
        {
            historyList.Clear();
            
            // 从撤销栈获取历史记录（需要反转顺序）
            Stack<IUndoableCommand> tempUndoStack = new Stack<IUndoableCommand>();
            foreach (var command in editor.undoRedoManager.UndoStack)
            {
                tempUndoStack.Push(command);
            }
            
            while (tempUndoStack.Count > 0)
            {
                historyList.Add(tempUndoStack.Pop());
            }
            
            // 只有在初始化时才设置currentIndex
            if (currentIndex < 0)
            {
                currentIndex = historyList.Count - 1;
                selectedIndex = currentIndex;
            }
            else
            {
                // 确保currentIndex在有效范围内
                currentIndex = Mathf.Clamp(currentIndex, 0, historyList.Count - 1);
                selectedIndex = currentIndex;
            }
        }
        
        /// <summary>
        /// 绘制窗口内容
        /// </summary>
        /// <param name="windowID">窗口ID</param>
        /// <param name="window">窗口对象</param>
        private void DrawWindow(int windowID, EditorWindow window)
        {
            GUILayout.BeginVertical();
            
            // 标题
            GUILayout.Label("操作历史记录", GUILayout.ExpandWidth(true));
            GUILayout.Space(10);
            
            // 历史记录列表（最新的操作显示在最下面）
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            
            // 保存原始颜色值
            Color originalBackgroundColor = GUI.backgroundColor;
            Color originalColor = GUI.color;
            
            for (int actualIndex = 0; actualIndex < historyList.Count; actualIndex++)
            {
                string description = historyList[actualIndex].Description;
                
                // 设置样式
                if (actualIndex == currentIndex)
                {
                    // 当前步骤高亮显示（天蓝色）
                    GUI.backgroundColor = new Color(0.0f, 0.75f, 1.0f); // 天蓝色
                    GUI.color = Color.white; // 白色文字
                }
                else if (actualIndex > currentIndex)
                {
                    // 当前索引之后的记录虚化显示
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
                    GUI.color = new Color(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    // 普通记录
                    GUI.backgroundColor = Color.white;
                    GUI.color = Color.black;
                }
                
                if (GUILayout.Button(description, GUILayout.Height(30)))
                {
                    // 点击跳转到指定步骤
                    JumpToStep(actualIndex);
                }
                
                // 恢复原始颜色值
                GUI.backgroundColor = originalBackgroundColor;
                GUI.color = originalColor;
            }
            
            GUILayout.EndScrollView();
            
            // 统计信息
            GUILayout.Space(10);
            GUILayout.Label($"总操作数: {historyList.Count}");
            GUILayout.Label($"当前步骤: {currentIndex + 1}/{historyList.Count}");
            
            // 按钮组
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("刷新列表", GUILayout.ExpandWidth(true)))
            {
                UpdateHistoryList();
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
        
        /// <summary>
        /// 跳转到指定步骤
        /// </summary>
        /// <param name="targetIndex">目标步骤索引</param>
        private void JumpToStep(int targetIndex)
        {
            if (targetIndex < 0 || targetIndex >= historyList.Count)
                return;
            
            if (targetIndex == currentIndex)
                return;
            
            // 暂时移除事件监听，避免执行Undo/Redo时触发事件
            editor.undoRedoManager.HistoryChanged -= OnHistoryChanged;
            
            try
            {
                // 向前跳（撤销操作）
                while (currentIndex > targetIndex)
                {
                    editor.undoRedoManager.Undo();
                    currentIndex--;
                }
                
                // 向后跳（重做操作）
                while (currentIndex < targetIndex)
                {
                    editor.undoRedoManager.Redo();
                    currentIndex++;
                }
                
                selectedIndex = currentIndex;
            }
            finally
            {
                // 重新添加事件监听
                editor.undoRedoManager.HistoryChanged += OnHistoryChanged;
            }
        }
        

        
        /// <summary>
        /// 添加新的操作记录
        /// </summary>
        /// <param name="command">操作命令</param>
        public void AddOperation(IUndoableCommand command)
        {
            // 如果当前不是在最新的操作位置，需要清除当前索引之后的所有记录
            if (currentIndex < historyList.Count - 1)
            {
                // 清除当前索引之后的所有记录
                historyList.RemoveRange(currentIndex + 1, historyList.Count - (currentIndex + 1));
            }
            
            // 添加新的操作记录
            historyList.Add(command);
            currentIndex = historyList.Count - 1;
            selectedIndex = currentIndex;
            
            // 滚动到最底部
            scrollPosition.y = float.MaxValue;
        }
        
        /// <summary>
        /// 命令添加事件处理
        /// </summary>
        /// <param name="command">添加的命令</param>
        private void OnCommandAdded(IUndoableCommand command)
        {
            // 添加新操作时，会自动处理当前索引之后的记录清除
            AddOperation(command);
        }
        
        /// <summary>
        /// 历史记录变化事件处理
        /// </summary>
        private void OnHistoryChanged()
        {
            // 重新更新历史记录列表，但保持currentIndex不变
            List<IUndoableCommand> oldHistory = new List<IUndoableCommand>(historyList);
            int oldCurrentIndex = currentIndex;
            
            UpdateHistoryList();
            
            // 保持currentIndex不变，确保状态同步
            currentIndex = Mathf.Clamp(oldCurrentIndex, 0, historyList.Count - 1);
            selectedIndex = currentIndex;
        }
    }
}