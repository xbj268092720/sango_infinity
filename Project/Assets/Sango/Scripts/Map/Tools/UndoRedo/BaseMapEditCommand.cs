using Sango.Render;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Tools.UndoRedo
{
    /// <summary>
    /// BaseMap编辑命令
    /// </summary>
    public class BaseMapEditCommand : IUndoableCommand
    {
        /// <summary>
        /// 编辑类型
        /// </summary>
        public enum EditType
        {
            Brush,      // 画笔
            Eraser,     // 橡皮擦
        }

        private MapEditor editor;
        private EditType editType;
        private int season;
        private Texture2D oldTexture; // 操作前的纹理
        private Texture2D newTexture; // 操作后的纹理
        private Rect editArea; // 编辑区域
        
        /// <summary>
        /// 命令描述
        /// </summary>
        public string Description { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="editor">地图编辑器</param>
        /// <param name="editType">编辑类型</param>
        /// <param name="season">季节</param>
        /// <param name="oldTexture">操作前的纹理</param>
        /// <param name="newTexture">操作后的纹理</param>
        /// <param name="editArea">编辑区域</param>
        /// <param name="description">命令描述</param>
        public BaseMapEditCommand(MapEditor editor, EditType editType, int season, Texture2D oldTexture, Texture2D newTexture, Rect editArea, string description)
        {
            this.editor = editor;
            this.editType = editType;
            this.season = season;
            this.oldTexture = oldTexture;
            this.newTexture = newTexture;
            this.editArea = editArea;
            this.Description = description;
        }
        
        /// <summary>
        /// 执行命令
        /// </summary>
        public void Execute()
        {
            ApplyTexture(newTexture);
        }
        
        /// <summary>
        /// 撤销命令
        /// </summary>
        public void Undo()
        {
            ApplyTexture(oldTexture);
        }
        
        /// <summary>
        /// 重做命令
        /// </summary>
        public void Redo()
        {
            ApplyTexture(newTexture);
        }
        
        /// <summary>
        /// 应用纹理
        /// </summary>
        /// <param name="texture">要应用的纹理</param>
        private void ApplyTexture(Texture2D texture)
        {
            if (texture == null) return;
            
            // 将Texture2D转换为RenderTexture
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32, RenderTextureFormat.ARGB32, 0);
            Graphics.Blit(texture, renderTexture);
            
            // 更新BaseMap
            // 直接访问MapEditor中的terrain_brush实例
            if (editor != null && editor.terrain_brush != null && editor.terrain_brush.baseMap != null && season >= 0 && season < editor.terrain_brush.baseMap.Length)
            {
                editor.terrain_brush.baseMap[season] = renderTexture;
                Shader.SetGlobalTexture("_BaseTex", renderTexture);
            }
        }
        
        /// <summary>
        /// 销毁命令，在命令被舍弃时调用
        /// </summary>
        public void Destroy()
        {
            // 释放Texture2D资源
            if (oldTexture != null)
            {
                UnityEngine.Object.Destroy(oldTexture);
                oldTexture = null;
            }
            if (newTexture != null)
            {
                UnityEngine.Object.Destroy(newTexture);
                newTexture = null;
            }
            
            // 释放其他资源
            editor = null;
        }
    }
}