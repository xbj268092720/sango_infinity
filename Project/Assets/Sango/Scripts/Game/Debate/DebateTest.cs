/*
 * 文件名：DebateTest.cs
 * 描述：舌战系统测试脚本，用于验证舌战功能的完整性和稳定性
 * 创建日期：2026-03-27
 * 最后修改：2026-03-27
 */

using UnityEngine;
using Sango.Core.Debate;

namespace Sango.Core
{
    /// <summary>
    /// 舌战系统测试脚本
    /// </summary>
    public class DebateTest : MonoBehaviour
    {
        /// <summary>
        /// 测试舌战功能
        /// </summary>
        public void TestDebate()
        {
            // 创建测试参与者
            DebateParticipant player = new DebateParticipant(1, "玩家", ParticipantType.Player, 90, 80);
            DebateParticipant ai = new DebateParticipant(2, "AI", ParticipantType.AI, 70, 60);

            // 初始化技能
            player.InitSkills();
            ai.InitSkills();

            // 开始舌战
            DebateManager.Instance.StartDebate(player, ai);

            // 输出测试信息
            Debug.Log("舌战测试开始");
            Debug.Log($"玩家：{player.Name}，智力：{player.Intelligence}，魅力：{player.Charisma}");
            Debug.Log($"AI：{ai.Name}，智力：{ai.Intelligence}，魅力：{ai.Charisma}");
            Debug.Log($"玩家技能数量：{player.Skills.Count}");
            Debug.Log($"AI技能数量：{ai.Skills.Count}");
        }

        /// <summary>
        /// 测试技能生成
        /// </summary>
        public void TestSkillGeneration()
        {
            // 测试技能工厂
            DebateSkillFactory.Init();

            // 生成不同类型的技能
            for (int i = 0; i < 10; i++)
            {
                DebateSkillType skillType = (DebateSkillType)Random.Range(0, 4);
                int level = Random.Range(1, 4);
                DebateSkill skill = DebateSkillFactory.CreateSkill(skillType, level);
                Debug.Log($"生成技能：{skill.Name}，类型：{skill.Type}，等级：{skill.Level}，效果：{skill.EffectValue}");
            }

            // 生成特殊技能
            for (int i = 0; i < 3; i++)
            {
                DebateSkill specialSkill = DebateSkillFactory.CreateSpecialSkill();
                Debug.Log($"生成特殊技能：{specialSkill.Name}，效果：{specialSkill.EffectValue}");
            }
        }

        /// <summary>
        /// 测试舌战流程
        /// </summary>
        public void TestDebateFlow()
        {
            // 创建测试参与者
            DebateParticipant player = new DebateParticipant(1, "玩家", ParticipantType.Player, 90, 80);
            DebateParticipant ai = new DebateParticipant(2, "AI", ParticipantType.AI, 70, 60);

            // 初始化技能
            player.InitSkills();
            ai.InitSkills();

            // 开始舌战
            DebateInstance debate = new DebateInstance(player, ai);
            debate.OnDebateStart += (d) => Debug.Log("舌战开始");
            debate.OnDebateEnd += (d) => 
            {
                string result = "";
                switch (d.Result)
                {
                    case DebateResult.Participant1Win:
                        result = $"{d.Participant1.Name} 胜利！";
                        break;
                    case DebateResult.Participant2Win:
                        result = $"{d.Participant2.Name} 胜利！";
                        break;
                    case DebateResult.Draw:
                        result = "平局！";
                        break;
                }
                Debug.Log($"舌战结束：{result}");
            };
            debate.OnRoundStart += (d, round) => Debug.Log($"第 {round} 回合开始");
            debate.OnSkillUse += (d, participant, skill) => Debug.Log($"{participant.Name} 使用了 {skill.Name}");
            debate.OnTurnStart += (d, participant) => Debug.Log($"{participant.Name} 的回合");

            // 开始舌战
            debate.Start();

            // 模拟玩家使用技能
            if (player.Skills.Count > 0)
            {
                debate.UseSkill(player, 0);
            }
        }
    }
}
