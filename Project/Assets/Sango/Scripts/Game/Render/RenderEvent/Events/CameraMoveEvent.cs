/*
 * 文件名：CameraMoveEvent.cs
 * 描述：相机移动事件类，用于处理相机移动和对话框显示
 * 创建日期：2026-03-27
 * 最后修改：2026-03-27
 */

using Sango.Render;
using UnityEngine;
using Sango.Game.Render.UI;

namespace Sango.Game.Render
{
    /// <summary>
    /// 相机移动事件类，用于处理相机移动和对话框显示
    /// </summary>
    public class CameraMoveEvent : RenderEventBase
    {
        /// <summary>
        /// 目标位置
        /// </summary>
        public Vector3 targetPosition;

        /// <summary>
        /// 移动持续时间
        /// </summary>
        public float moveDuration = 0.5f;

        /// <summary>
        /// 起始位置
        /// </summary>
        private Vector3 startPosition;

        /// <summary>
        /// 是否可以堆叠
        /// </summary>
        public override bool IsStack => true;

        /// <summary>
        /// 对话框样式
        /// </summary>
        public UIDialog.DialogStyle dialogStyle;

        /// <summary>
        /// 对话框内容
        /// </summary>
        public string content;

        /// <summary>
        /// 人物对象
        /// </summary>
        public Person person;

        /// <summary>
        /// 确认按钮回调
        /// </summary>
        public System.Action sureAction;

        /// <summary>
        /// 取消按钮回调
        /// </summary>
        public System.Action cancelAction;

        /// <summary>
        /// 对话框实例
        /// </summary>
        UIDialog dialog;

        /// <summary>
        /// 进入事件处理
        /// </summary>
        /// <param name="scenario">场景实例</param>
        public override void Enter(Scenario scenario)
        {
            base.Enter(scenario);
            startPosition = MapRender.Instance.GetCameraPos();
            MapRender.Instance.MoveCameraTo(targetPosition, moveDuration);
            dialog = UIDialog.Open(dialogStyle, content, () =>
            {
                sureAction?.Invoke();
                UIDialog.Close();
                IsDone = true;
            });
            if (person != null)
                dialog.SetPerson(person);
            dialog.cancelAction = () =>
            {
                UIDialog.Close();
                IsDone = true;
                cancelAction?.Invoke();
            };
        }

        /// <summary>
        /// 退出事件处理
        /// </summary>
        /// <param name="scenario">场景实例</param>
        public override void Exit(Scenario scenario) 
        {
            MapRender.Instance.MoveCameraTo(startPosition, moveDuration);
        }

    }
}