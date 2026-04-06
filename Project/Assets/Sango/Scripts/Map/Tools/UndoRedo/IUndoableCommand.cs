using UnityEngine;

namespace Sango.Tools.UndoRedo
{
    /// <summary>
    /// 可撤销命令接口
    /// </summary>
    public interface IUndoableCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        void Execute();
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        void Undo();
        
        /// <summary>
        /// 重做命令
        /// </summary>
        void Redo();
        
        /// <summary>
        /// 命令描述
        /// </summary>
        string Description { get; }
    }
}