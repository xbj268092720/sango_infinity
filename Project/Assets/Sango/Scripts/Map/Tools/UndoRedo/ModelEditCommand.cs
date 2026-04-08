using RTEditor;
using Sango.Render;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Tools.UndoRedo
{
    /// <summary>
    /// 模型编辑命令
    /// </summary>
    public class ModelEditCommand : IUndoableCommand
    {
        public class ObjectCreateAction : IUndoableAndRedoableAction, IAction
        {
            MapObject _mapObject;
            public ObjectCreateAction(MapObject sourceObject)
            {
                _mapObject = sourceObject;
            }

            public void Execute()
            {
                if (RuntimeEditorApplication.Instance.EnableUndoRedo) EditorUndoRedoSystem.Instance.RegisterAction(this);
                _mapObject.AddToMap(true);
            }

            public void Undo()
            {
                _mapObject.gameObject.SetActive(false);
            }

            public void Redo()
            {
                _mapObject.gameObject.SetActive(true);

            }

            public void OnRemovedFromUndoRedoStack()
            {
                _mapObject.Clear();
                GameObject.Destroy(_mapObject.gameObject);
            }
        }

        MapEditor editor;
        IUndoableAndRedoableAction urAction;
        string _desc;
        public string Description => _desc;

        /// <summary>
        /// 构造函数（添加模型）
        /// </summary>
        /// <param name="editor">地图编辑器</param>
        /// <param name="mapObject">模型对象</param>
        /// <param name="description">命令描述</param>
        public ModelEditCommand(MapEditor editor, IUndoableAndRedoableAction urAction, string description)
        {
            this.editor = editor;
            this.urAction = urAction;
            this._desc = description;
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public void Execute()
        {
            IAction action = urAction as IAction;
            if (action != null)
            {
                action.Execute();
            }
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public void Undo()
        {
            urAction?.Undo();
        }
        
        /// <summary>
        /// 重做命令
        /// </summary>
        public void Redo()
        {
            urAction?.Redo();
        }

        public void Destroy()
        {
            // 释放资源
            urAction.OnRemovedFromUndoRedoStack();
            editor = null;
            urAction = null;
        }

    }
}