using UnityEngine;
using System.IO;
using System;
using UnityEngine.EventSystems;

namespace Sango.Tools
{

    public class BrushBase
    {
        protected MapEditor editor;
        protected bool isDragging = false;
        protected float dragThreshold = 0.1f;
        protected float lastClickTime = 0;
        protected Vector2 initialMousePosition;

        public BrushBase(MapEditor e)
        {
            editor = e;
        }

        public virtual void Modify(Vector3 center, MapEditor map)
        {

        }
        public virtual void OnSeasonChanged(int curSeason)
        {

        }
        public virtual void DrawGizmos(Vector3 center)
        {
        }
        public virtual void OnEnter()
        {

        }
        public virtual void SuckValue(Vector3 center, MapEditor editor)
        { 
        }

        protected Vector3 lastCenter = Vector3.zero;

  
        public virtual bool IsPointerOverUI()
        {
            return EditorWindow.IsPointOverUI() || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject());
        }

        UnityEngine.Rect labRect = new UnityEngine.Rect();


        public virtual void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, editor.map.showLimitLength + 2000, editor.rayCastLayer))
            {
                if (hit.point != lastCenter)
                {
                    if (!IsPointerOverUI())
                    {
                        // 鼠标按下记录初始位置
                        if (Input.GetMouseButtonDown(0))
                        {
                            initialMousePosition = Input.mousePosition;
                            lastClickTime = Time.time;
                            // 先不立即进入拖拽模式，等待位置变化
                        }
                        // 鼠标弹起结束拖拽或处理点击
                        else if (Input.GetMouseButtonUp(0))
                        {
                            if (isDragging)
                            {
                                // 结束拖拽
                                isDragging = false;
                                OnDragEnd(hit.point);
                            }
                            else
                            {
                                // 判断是快速点击
                                float clickDuration = Time.time - lastClickTime;
                                if (clickDuration < dragThreshold)
                                {
                                    // 快速点击，执行单次操作
                                    Modify(hit.point, editor);
                                }
                            }
                        }
                        // 检查是否应该进入拖拽模式
                        else if (Input.GetMouseButton(0) && !isDragging)
                        {
                            // 计算鼠标位置变化距离
                            float distance = Vector2.Distance(Input.mousePosition, initialMousePosition);
                            // 如果移动距离超过阈值，进入拖拽模式
                            if (distance > 5f) // 使用5像素作为拖拽阈值
                            {
                                isDragging = true;
                                OnDragStart(hit.point);
                            }
                        }
                        // 拖拽过程中
                        else if (Input.GetMouseButton(0) && isDragging)
                        {
                            OnDrag(hit.point);
                            lastCenter = hit.point;
                        }
                        
                    }
                    DrawGizmos(hit.point);
                }
            }
        }
        
        /// <summary>
        /// 拖拽开始
        /// </summary>
        /// <param name="center">中心点</param>
        public virtual void OnDragStart(Vector3 center)
        {
            
        }
        
        /// <summary>
        /// 拖拽过程
        /// </summary>
        /// <param name="center">中心点</param>
        public virtual void OnDrag(Vector3 center)
        {
            
        }
        
        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="center">中心点</param>
        public virtual void OnDragEnd(Vector3 center)
        {
            
        }

        public virtual void OnBrushTypeChange() { }
        public virtual void OnBrushSizeChange() { }
        public virtual void OnBrushOpacityChange() { }
        public virtual void Clear()
        {

        }
        public virtual void OnGUI()
        {

        }

    }
}