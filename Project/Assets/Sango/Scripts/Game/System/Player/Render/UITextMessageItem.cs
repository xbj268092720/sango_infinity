using Sango.Game.Player;
using Sango.Loader;
using Sango.Render;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    /// <summary>
    /// 文本消息项UI组件
    /// 负责显示文本消息的内容、时间、势力信息，并处理点击事件
    /// </summary>
    public class UITextMessageItem : MonoBehaviour
    {
        /// <summary>
        /// 布局元素组件
        /// 用于控制消息项的高度
        /// </summary>
        public LayoutElement layoutElement;
        
        /// <summary>
        /// 根变换组件
        /// 用于调整消息项的大小
        /// </summary>
        public RectTransform root;
        
        /// <summary>
        /// 内容文本组件
        /// 用于显示消息的文本内容
        /// </summary>
        public Text contentText;
        
        /// <summary>
        /// 时间文本组件
        /// 用于显示消息的日期信息
        /// </summary>
        public Text timeText;

        /// <summary>
        /// time节点
        /// </summary>
        public GameObject timeNode;
        
        /// <summary>
        /// 势力图标组件
        /// 用于显示消息关联势力的颜色
        /// </summary>
        public Image forceImg;
        
        /// <summary>
        /// 关联的文本消息对象
        /// </summary>
        public PlayerMessage.TextMessage textMessage;
        
        /// <summary>
        /// 点击事件回调
        /// 当点击消息项时触发
        /// </summary>
        public Action<PlayerMessage.TextMessage> onClickItem;

        /// <summary>
        /// 设置消息项的高度
        /// </summary>
        /// <param name="height">高度值</param>
        /// <returns>返回自身实例，用于链式调用</returns>
        public UITextMessageItem SetHeight(float height)
        {
            //layoutElement.preferredHeight = 42;
            //Vector2 size = root.sizeDelta;
            //size.y = height;
            //root.sizeDelta = size;
            return this;
        }

        /// <summary>
        /// 设置消息的日期信息
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="day">日期</param>
        /// <returns>返回自身实例，用于链式调用</returns>
        public UITextMessageItem SetDate(int year, int month, int day)
        {
            if (year > 0)
            {
                timeText.text = $"《{year}年{month}月{day}日》";
                timeNode.SetActive(true);
            }
            else
            {
                timeText.text = "";
                timeNode.SetActive(false);
            }
            return this;
        }

        /// <summary>
        /// 设置消息关联的势力
        /// </summary>
        /// <param name="force">势力对象</param>
        /// <returns>返回自身实例，用于链式调用</returns>
        public UITextMessageItem SetForce(Force force)
        {
            forceImg.color = force != null ? force.Color : Color.white;
            return this;
        }

        /// <summary>
        /// 设置消息的文本内容
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns>返回自身实例，用于链式调用</returns>
        public UITextMessageItem SetContent(string text)
        {
            contentText.text = text;
            return this;
        }

        /// <summary>
        /// 点击事件处理方法
        /// 触发点击事件回调
        /// </summary>
        public void OnClick()
        {
            onClickItem?.Invoke(textMessage);
        }
    }
}
