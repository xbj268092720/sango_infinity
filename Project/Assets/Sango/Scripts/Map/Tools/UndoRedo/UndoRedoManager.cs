using System.Collections.Generic;
using UnityEngine;

namespace Sango.Tools.UndoRedo
{
    /// <summary>
    /// 撤销/重做管理器
    /// </summary>
    public class UndoRedoManager
    {
        /// <summary>
        /// 撤销历史记录
        /// </summary>
        private Stack<IUndoableCommand> undoStack = new Stack<IUndoableCommand>();
        
        /// <summary>
        /// 重做历史记录
        /// </summary>
        private Stack<IUndoableCommand> redoStack = new Stack<IUndoableCommand>();
        
        /// <summary>
        /// 最大历史记录数量
        /// </summary>
        public int MaxHistoryCount { get; set; } = 100;
        
        /// <summary>
        /// 是否可以撤销
        /// </summary>
        public bool CanUndo => undoStack.Count > 0;
        
        /// <summary>
        /// 是否可以重做
        /// </summary>
        public bool CanRedo => redoStack.Count > 0;
        
        /// <summary>
        /// 添加命令到历史记录
        /// </summary>
        /// <param name="command">要添加的命令</param>
        public void AddCommand(IUndoableCommand command, bool execute = false)
        {
            // 执行命令
            if(execute)
                command.Execute();
            
            // 添加到撤销栈
            undoStack.Push(command);
            
            // 清空重做栈（新操作后无法重做之前的操作）
            redoStack.Clear();
            
            // 限制历史记录数量
            if (undoStack.Count > MaxHistoryCount)
            {
                // 创建新栈并保留最近的记录
                Stack<IUndoableCommand> tempStack = new Stack<IUndoableCommand>();
                int count = 0;
                
                while (undoStack.Count > 0)
                {
                    tempStack.Push(undoStack.Pop());
                    count++;
                    
                    if (count >= MaxHistoryCount)
                    {
                        break;
                    }
                }
                
                undoStack.Clear();
                while (tempStack.Count > 0)
                {
                    undoStack.Push(tempStack.Pop());
                }
            }
        }
        
        /// <summary>
        /// 撤销操作
        /// </summary>
        public bool Undo()
        {
            if (CanUndo)
            {
                IUndoableCommand command = undoStack.Pop();
                command.Undo();
                redoStack.Push(command);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 重做操作
        /// </summary>
        public bool Redo()
        {
            if (CanRedo)
            {
                IUndoableCommand command = redoStack.Pop();
                command.Redo();
                undoStack.Push(command);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 清空历史记录
        /// </summary>
        public void ClearHistory()
        {
            undoStack.Clear();
            redoStack.Clear();
        }
        
        /// <summary>
        /// 获取下一个可撤销命令的描述
        /// </summary>
        public string GetNextUndoDescription()
        {
            if (CanUndo)
            {
                return undoStack.Peek().Description;
            }
            return "无法撤销";
        }
        
        /// <summary>
        /// 获取下一个可重做命令的描述
        /// </summary>
        public string GetNextRedoDescription()
        {
            if (CanRedo)
            {
                return redoStack.Peek().Description;
            }
            return "无法重做";
        }
    }
}