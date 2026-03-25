using System;
using System.Collections.Generic;
using TKNewtonsoft.Json;
using TKNewtonsoft.Json.Linq;
using UnityEngine;

namespace Sango.Game
{
    /// <summary>
    /// 技能时间轴事件类型
    /// </summary>
    public enum SkillTimelineEventType
    {
        /// <summary>
        /// 播放动画
        /// </summary>
        PlayAnimation,
        /// <summary>
        /// 播放特效
        /// </summary>
        PlayEffect,
        /// <summary>
        /// 播放音效
        /// </summary>
        PlaySound,
        /// <summary>
        /// 执行伤害
        /// </summary>
        ExecuteDamage,
        /// <summary>
        /// 执行位移
        /// </summary>
        ExecuteOffset,
        /// <summary>
        /// 执行效果
        /// </summary>
        ExecuteEffect,
        /// <summary>
        /// 显示文本
        /// </summary>
        ShowText,
        /// <summary>
        /// 相机抖动
        /// </summary>
        CameraShake
    }

    /// <summary>
    /// 技能时间轴事件
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SkillTimelineEvent
    {
        /// <summary>
        /// 事件时间点
        /// </summary>
        [JsonProperty]
        public float time;

        /// <summary>
        /// 事件类型
        /// </summary>
        [JsonProperty]
        public string type;

        /// <summary>
        /// 事件参数
        /// </summary>
        [JsonProperty]
        public JObject parameters;

        /// <summary>
        /// 初始化事件
        /// </summary>
        /// <param name="eventData"></param>
        public void Init(JObject eventData)
        {
            if (eventData == null) return;

            if (eventData.TryGetValue("time", out JToken timeToken))
            {
                time = timeToken.Value<float>();
            }

            if (eventData.TryGetValue("type", out JToken typeToken))
            {
                type = typeToken.Value<string>();
            }

            if (eventData.TryGetValue("parameters", out JToken parametersToken))
            {
                parameters = parametersToken as JObject;
            }
        }

        /// <summary>
        /// 获取事件类型枚举
        /// </summary>
        /// <returns></returns>
        public SkillTimelineEventType GetEventType()
        {
            SkillTimelineEventType eventType;
            if (Enum.TryParse<SkillTimelineEventType>(type, true, out eventType))
            {
                return eventType;
            }
            return SkillTimelineEventType.PlayEffect;
        }
    }
}