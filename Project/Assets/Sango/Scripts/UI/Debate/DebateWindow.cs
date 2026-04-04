/*
 * 文件名：DebateWindow.cs
 * 描述：舌战窗口，用于显示舌战的主界面
 * 创建日期：2026-03-27
 * 最后修改：2026-03-27
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sango.Core.Debate;

namespace Sango.UI.Debate
{
    /// <summary>
    /// 舌战窗口
    /// </summary>
    public class DebateWindow : Sango.UGUIWindow
    {
        /// <summary>
        /// 参与者1名称
        /// </summary>
        public Text Participant1Name;

        /// <summary>
        /// 参与者1士气条
        /// </summary>
        public Slider Participant1MoraleSlider;

        /// <summary>
        /// 参与者1愤怒条
        /// </summary>
        public Slider Participant1AngerSlider;

        /// <summary>
        /// 参与者2名称
        /// </summary>
        public Text Participant2Name;

        /// <summary>
        /// 参与者2士气条
        /// </summary>
        public Slider Participant2MoraleSlider;

        /// <summary>
        /// 参与者2愤怒条
        /// </summary>
        public Slider Participant2AngerSlider;

        /// <summary>
        /// 技能卡片容器
        /// </summary>
        public Transform SkillCardContainer;

        /// <summary>
        /// 技能卡片预制体
        /// </summary>
        public GameObject SkillCardPrefab;

        /// <summary>
        /// 当前舌战实例
        /// </summary>
        private DebateInstance _currentDebate;

        /// <summary>
        /// 技能卡片列表
        /// </summary>
        private List<SkillCardUI> _skillCards = new List<SkillCardUI>();

        /// <summary>
        /// 初始化窗口
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            
            // 注册事件
            DebateManager.Instance.GetCurrentDebate().OnDebateStart += OnDebateStart;
            DebateManager.Instance.GetCurrentDebate().OnDebateEnd += OnDebateEnd;
            DebateManager.Instance.GetCurrentDebate().OnRoundStart += OnRoundStart;
            DebateManager.Instance.GetCurrentDebate().OnSkillUse += OnSkillUse;
            DebateManager.Instance.GetCurrentDebate().OnTurnStart += OnTurnStart;
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public override void Open()
        {
            base.Open();
            
            // 获取当前舌战实例
            _currentDebate = DebateManager.Instance.GetCurrentDebate();
            if (_currentDebate != null)
            {
                // 注册事件
                _currentDebate.OnDebateStart += OnDebateStart;
                _currentDebate.OnDebateEnd += OnDebateEnd;
                _currentDebate.OnRoundStart += OnRoundStart;
                _currentDebate.OnSkillUse += OnSkillUse;
                _currentDebate.OnTurnStart += OnTurnStart;
                
                // 初始化UI
                UpdateParticipantInfo();
                UpdateSkillCards();
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public override void Close()
        {
            // 取消注册事件
            if (_currentDebate != null)
            {
                _currentDebate.OnDebateStart -= OnDebateStart;
                _currentDebate.OnDebateEnd -= OnDebateEnd;
                _currentDebate.OnRoundStart -= OnRoundStart;
                _currentDebate.OnSkillUse -= OnSkillUse;
                _currentDebate.OnTurnStart -= OnTurnStart;
            }
            
            // 清空技能卡片
            ClearSkillCards();
            
            base.Close();
        }

        /// <summary>
        /// 更新窗口
        /// </summary>
        private void Update()
        {
            // 更新参与者信息
            if (_currentDebate != null)
            {
                UpdateParticipantInfo();
            }
        }

        /// <summary>
        /// 舌战开始事件
        /// </summary>
        /// <param name="debate">舌战实例</param>
        private void OnDebateStart(DebateInstance debate)
        {
            UpdateParticipantInfo();
            UpdateSkillCards();
        }

        /// <summary>
        /// 舌战结束事件
        /// </summary>
        /// <param name="debate">舌战实例</param>
        private void OnDebateEnd(DebateInstance debate)
        {
            // 显示结果
            string resultText = "";
            switch (debate.Result)
            {
                case DebateResult.Participant1Win:
                    resultText = $"{debate.Participant1.Name} 胜利！";
                    break;
                case DebateResult.Participant2Win:
                    resultText = $"{debate.Participant2.Name} 胜利！";
                    break;
                case DebateResult.Draw:
                    resultText = "平局！";
                    break;
            }
            
            // 显示结果对话框
            // 这里可以添加显示结果的逻辑
            Debug.Log(resultText);
            
            // 关闭窗口
            Close();
        }

        /// <summary>
        /// 回合开始事件
        /// </summary>
        /// <param name="debate">舌战实例</param>
        /// <param name="round">回合数</param>
        private void OnRoundStart(DebateInstance debate, int round)
        {
            // 显示回合数
            Debug.Log($"第 {round} 回合开始");
        }

        /// <summary>
        /// 技能使用事件
        /// </summary>
        /// <param name="debate">舌战实例</param>
        /// <param name="participant">技能使用者</param>
        /// <param name="skill">使用的技能</param>
        private void OnSkillUse(DebateInstance debate, DebateParticipant participant, DebateSkill skill)
        {
            // 显示技能使用信息
            Debug.Log($"{participant.Name} 使用了 {skill.Name}");
            
            // 更新技能卡片
            UpdateSkillCards();
        }

        /// <summary>
        /// 回合开始事件
        /// </summary>
        /// <param name="debate">舌战实例</param>
        /// <param name="participant">当前行动的参与者</param>
        private void OnTurnStart(DebateInstance debate, DebateParticipant participant)
        {
            // 显示当前行动的参与者
            Debug.Log($"{participant.Name} 的回合");
            
            // 更新技能卡片
            UpdateSkillCards();
        }

        /// <summary>
        /// 更新参与者信息
        /// </summary>
        private void UpdateParticipantInfo()
        {
            if (_currentDebate == null)
                return;

            // 更新参与者1信息
            Participant1Name.text = _currentDebate.Participant1.Name;
            Participant1MoraleSlider.value = _currentDebate.Participant1.Morale;
            Participant1AngerSlider.value = _currentDebate.Participant1.Anger;

            // 更新参与者2信息
            Participant2Name.text = _currentDebate.Participant2.Name;
            Participant2MoraleSlider.value = _currentDebate.Participant2.Morale;
            Participant2AngerSlider.value = _currentDebate.Participant2.Anger;
        }

        /// <summary>
        /// 更新技能卡片
        /// </summary>
        private void UpdateSkillCards()
        {
            if (_currentDebate == null)
                return;

            // 清空现有技能卡片
            ClearSkillCards();

            // 获取当前行动的参与者
            DebateParticipant currentParticipant = _currentDebate.CurrentTurnParticipant;
            if (currentParticipant.Type == ParticipantType.Player)
            {
                // 显示玩家的技能卡片
                foreach (DebateSkill skill in currentParticipant.Skills)
                {
                    GameObject cardObj = Instantiate(SkillCardPrefab, SkillCardContainer);
                    SkillCardUI skillCard = cardObj.GetComponent<SkillCardUI>();
                    if (skillCard != null)
                    {
                        skillCard.Init(skill, OnSkillCardClick);
                        _skillCards.Add(skillCard);
                    }
                }
            }
        }

        /// <summary>
        /// 清空技能卡片
        /// </summary>
        private void ClearSkillCards()
        {
            foreach (SkillCardUI card in _skillCards)
            {
                Destroy(card.gameObject);
            }
            _skillCards.Clear();
        }

        /// <summary>
        /// 技能卡片点击事件
        /// </summary>
        /// <param name="skill">技能</param>
        private void OnSkillCardClick(DebateSkill skill)
        {
            if (_currentDebate == null)
                return;

            // 获取当前行动的参与者
            DebateParticipant currentParticipant = _currentDebate.CurrentTurnParticipant;
            if (currentParticipant.Type == ParticipantType.Player)
            {
                // 查找技能索引
                int skillIndex = currentParticipant.Skills.FindIndex(s => s.ID == skill.ID);
                if (skillIndex >= 0)
                {
                    // 使用技能
                    _currentDebate.UseSkill(currentParticipant, skillIndex);
                }
            }
        }
    }
}
