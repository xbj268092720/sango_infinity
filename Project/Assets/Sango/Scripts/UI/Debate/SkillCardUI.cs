/*
 * 文件名：SkillCardUI.cs
 * 描述：技能卡片UI，用于显示舌战中的技能卡片
 * 创建日期：2026-03-27
 * 最后修改：2026-03-27
 */

using UnityEngine;
using UnityEngine.UI;
using Sango.Game.Debate;

namespace Sango.UI.Debate
{
    /// <summary>
    /// 技能卡片UI
    /// </summary>
    public class SkillCardUI : MonoBehaviour
    {
        /// <summary>
        /// 技能名称
        /// </summary>
        public Text SkillName;

        /// <summary>
        /// 技能描述
        /// </summary>
        public Text SkillDescription;

        /// <summary>
        /// 技能等级
        /// </summary>
        public Text SkillLevel;

        /// <summary>
        /// 技能图标
        /// </summary>
        public Image SkillIcon;

        /// <summary>
        /// 技能卡片按钮
        /// </summary>
        public Button SkillButton;

        /// <summary>
        /// 当前技能
        /// </summary>
        private DebateSkill _skill;

        /// <summary>
        /// 点击回调
        /// </summary>
        private System.Action<DebateSkill> _onClick;

        /// <summary>
        /// 初始化技能卡片
        /// </summary>
        /// <param name="skill">技能</param>
        /// <param name="onClick">点击回调</param>
        public void Init(DebateSkill skill, System.Action<DebateSkill> onClick)
        {
            _skill = skill;
            _onClick = onClick;

            // 设置技能信息
            SkillName.text = skill.Name;
            SkillDescription.text = skill.Description;
            SkillLevel.text = skill.Level.ToString();

            // 设置技能图标
            // 这里可以根据技能类型设置不同的图标
            switch (skill.Type)
            {
                case DebateSkillType.Attack:
                    // 设置攻击技能图标
                    break;
                case DebateSkillType.Defense:
                    // 设置防御技能图标
                    break;
                case DebateSkillType.Support:
                    // 设置支援技能图标
                    break;
                case DebateSkillType.Special:
                    // 设置特殊技能图标
                    break;
            }

            // 注册按钮点击事件
            SkillButton.onClick.AddListener(OnButtonClick);
        }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        private void OnButtonClick()
        {
            _onClick?.Invoke(_skill);
        }

        /// <summary>
        /// 销毁时清理
        /// </summary>
        private void OnDestroy()
        {
            // 移除按钮点击事件
            SkillButton.onClick.RemoveListener(OnButtonClick);
        }
    }
}
