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
        /// <summary>
        /// 编辑类型
        /// </summary>
        public enum EditType
        {
            Add,        // 添加模型
            Delete,     // 删除模型
            Move,       // 移动模型
            Rotate,     // 旋转模型
            Scale,      // 缩放模型
        }
        
        private MapEditor editor;
        private EditType editType;
        private MapObject mapObject;
        private ModelState oldState;
        private ModelState newState;
        
        /// <summary>
        /// 模型状态记录
        /// </summary>
        public struct ModelState
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public int objId;
            public int modelId;
            public int objType;
            public int bindId;
        }
        
        /// <summary>
        /// 命令描述
        /// </summary>
        public string Description { get; private set; }
        
        /// <summary>
        /// 构造函数（添加模型）
        /// </summary>
        /// <param name="editor">地图编辑器</param>
        /// <param name="mapObject">模型对象</param>
        /// <param name="description">命令描述</param>
        public ModelEditCommand(MapEditor editor, MapObject mapObject, string description)
        {
            this.editor = editor;
            this.editType = EditType.Add;
            this.mapObject = mapObject;
            this.Description = description;
        }
        
        /// <summary>
        /// 构造函数（修改模型）
        /// </summary>
        /// <param name="editor">地图编辑器</param>
        /// <param name="editType">编辑类型</param>
        /// <param name="mapObject">模型对象</param>
        /// <param name="oldState">旧状态</param>
        /// <param name="newState">新状态</param>
        /// <param name="description">命令描述</param>
        public ModelEditCommand(MapEditor editor, EditType editType, MapObject mapObject, ModelState oldState, ModelState newState, string description)
        {
            this.editor = editor;
            this.editType = editType;
            this.mapObject = mapObject;
            this.oldState = oldState;
            this.newState = newState;
            this.Description = description;
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public void Execute()
        {
            switch (editType)
            {
                case EditType.Add:
                    AddModel();
                    break;
                
                case EditType.Delete:
                    DeleteModel();
                    break;
                
                case EditType.Move:
                case EditType.Rotate:
                case EditType.Scale:
                    ApplyState(newState);
                    break;
            }
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public void Undo()
        {
            switch (editType)
            {
                case EditType.Add:
                    DeleteModel();
                    break;
                
                case EditType.Delete:
                    AddModel();
                    break;
                
                case EditType.Move:
                case EditType.Rotate:
                case EditType.Scale:
                    ApplyState(oldState);
                    break;
            }
        }
        
        /// <summary>
        /// 重做命令
        /// </summary>
        public void Redo()
        {
            Execute();
        }
        
        /// <summary>
        /// 添加模型
        /// </summary>
        private void AddModel()
        {
            if (mapObject != null)
            {
                editor.map.AddStatic(mapObject);
            }
        }
        
        /// <summary>
        /// 删除模型
        /// </summary>
        private void DeleteModel()
        {
            if (mapObject != null)
            {
                editor.map.RemoveStatic(mapObject);
                mapObject.visible = false;
                mapObject.ClearModels();
            }
        }
        
        /// <summary>
        /// 应用模型状态
        /// </summary>
        /// <param name="state">模型状态</param>
        private void ApplyState(ModelState state)
        {
            if (mapObject != null)
            {
                // 更新模型状态
                mapObject.position = state.position;
                mapObject.rotation = state.rotation;
                mapObject.scale = state.scale;
                mapObject.objId = state.objId;
                mapObject.modelId = state.modelId;
                mapObject.objType = state.objType;
                mapObject.bindId = state.bindId;
                
                // 更新模型变换
                GameObject gameObject = mapObject.GetGameObject();
                if (gameObject != null)
                {
                    gameObject.transform.position = state.position;
                    gameObject.transform.rotation = Quaternion.Euler(state.rotation);
                    gameObject.transform.localScale = state.scale;
                }
            }
        }
        
        /// <summary>
        /// 从MapObject创建模型状态
        /// </summary>
        /// <param name="mapObject">模型对象</param>
        /// <returns>模型状态</returns>
        public static ModelState CreateModelState(MapObject mapObject)
        {
            ModelState state = new ModelState();
            state.position = mapObject.position;
            state.rotation = mapObject.rotation;
            state.scale = mapObject.scale;
            state.objId = mapObject.objId;
            state.modelId = mapObject.modelId;
            state.objType = mapObject.objType;
            state.bindId = mapObject.bindId;
            return state;
        }
    }
}