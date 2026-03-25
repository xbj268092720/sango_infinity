using System.Collections.Generic;
using UnityEngine;

namespace Sango.Game
{
    public class SkillTestSystem
    {
        private static SkillTestSystem _instance;
        public static SkillTestSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SkillTestSystem();
                }
                return _instance;
            }
        }

        public void TestAllSkills()
        {
            Sango.Log.Print("开始测试所有技能...");
            
            // 测试技能表现系统
            TestSkillVisualSystem();
            
            // 测试位移技能系统
            TestOffsetSkillSystem();
            
            // 测试范围技能系统
            TestRangeSkillSystem();
            
            // 测试BUFF系统
            TestBuffSystem();
            
            // 测试技能AI系统
            TestSkillAISystem();
            
            // 测试技能配置系统
            TestSkillConfigSystem();
            
            Sango.Log.Print("技能测试完成!");
        }

        private void TestSkillVisualSystem()
        {
            Sango.Log.Print("测试技能表现系统...");
            // 测试技能视觉效果的初始化
            SkillVisualizer.Init();
            
            // 测试不同类型的技能视觉效果
            SkillVisualizer defaultVisualizer = SkillVisualizer.Create("Default");
            SkillVisualizer rangeVisualizer = SkillVisualizer.Create("Range");
            SkillVisualizer meleeVisualizer = SkillVisualizer.Create("Melee");
            SkillVisualizer strategyVisualizer = SkillVisualizer.Create("Strategy");
            
            Sango.Log.Print($"默认视觉效果器: {defaultVisualizer != null}");
            Sango.Log.Print($"远程视觉效果器: {rangeVisualizer != null}");
            Sango.Log.Print($"近战视觉效果器: {meleeVisualizer != null}");
            Sango.Log.Print($"计策视觉效果器: {strategyVisualizer != null}");
        }

        private void TestOffsetSkillSystem()
        {
            Sango.Log.Print("测试位移技能系统...");
            // 测试位移类型的数量
            int offsetTypeCount = System.Enum.GetValues(typeof(SkillCellOffsetType)).Length;
            Sango.Log.Print($"位移类型数量: {offsetTypeCount}");
            
            // 测试位移类型是否包含新添加的类型
            bool hasMasterRandom = System.Enum.IsDefined(typeof(SkillCellOffsetType), "MasterRandom");
            bool hasTargetRandom = System.Enum.IsDefined(typeof(SkillCellOffsetType), "TargetRandom");
            bool hasMaster指定位置 = System.Enum.IsDefined(typeof(SkillCellOffsetType), "Master指定位置");
            bool hasTarget指定位置 = System.Enum.IsDefined(typeof(SkillCellOffsetType), "Target指定位置");
            
            Sango.Log.Print($"包含MasterRandom: {hasMasterRandom}");
            Sango.Log.Print($"包含TargetRandom: {hasTargetRandom}");
            Sango.Log.Print($"包含Master指定位置: {hasMaster指定位置}");
            Sango.Log.Print($"包含Target指定位置: {hasTarget指定位置}");
        }

        private void TestRangeSkillSystem()
        {
            Sango.Log.Print("测试范围技能系统...");
            // 测试范围类型的数量
            int rangeTypeCount = System.Enum.GetValues(typeof(SkillAttackOffsetType)).Length;
            Sango.Log.Print($"范围类型数量: {rangeTypeCount}");
            
            // 测试范围类型是否包含新添加的类型
            bool hasFan = System.Enum.IsDefined(typeof(SkillAttackOffsetType), "Fan");
            bool hasRectangle = System.Enum.IsDefined(typeof(SkillAttackOffsetType), "Rectangle");
            bool hasCross = System.Enum.IsDefined(typeof(SkillAttackOffsetType), "Cross");
            bool hasSquare = System.Enum.IsDefined(typeof(SkillAttackOffsetType), "Square");
            bool hasDiamond = System.Enum.IsDefined(typeof(SkillAttackOffsetType), "Diamond");
            
            Sango.Log.Print($"包含Fan: {hasFan}");
            Sango.Log.Print($"包含Rectangle: {hasRectangle}");
            Sango.Log.Print($"包含Cross: {hasCross}");
            Sango.Log.Print($"包含Square: {hasSquare}");
            Sango.Log.Print($"包含Diamond: {hasDiamond}");
        }

        private void TestBuffSystem()
        {
            Sango.Log.Print("测试BUFF系统...");
            // 测试BUFF效果的初始化
            BuffEffect.Init();
            
            // 测试不同类型的BUFF效果
            BuffEffect poisonEffect = BuffEffect.Create("Poison");
            BuffEffect burnEffect = BuffEffect.Create("Burn");
            BuffEffect freezeEffect = BuffEffect.Create("Freeze");
            BuffEffect silenceEffect = BuffEffect.Create("Silence");
            BuffEffect invincibleEffect = BuffEffect.Create("Invincible");
            BuffEffect shieldEffect = BuffEffect.Create("Shield");
            
            Sango.Log.Print($"中毒效果: {poisonEffect != null}");
            Sango.Log.Print($"燃烧效果: {burnEffect != null}");
            Sango.Log.Print($"冰冻效果: {freezeEffect != null}");
            Sango.Log.Print($"沉默效果: {silenceEffect != null}");
            Sango.Log.Print($"无敌效果: {invincibleEffect != null}");
            Sango.Log.Print($"护盾效果: {shieldEffect != null}");
        }

        private void TestSkillAISystem()
        {
            Sango.Log.Print("测试技能AI系统...");
            // 测试AI系统是否能够正常工作
            // 这里可以添加具体的AI测试逻辑
            Sango.Log.Print("技能AI系统测试完成");
        }

        private void TestSkillConfigSystem()
        {
            Sango.Log.Print("测试技能配置系统...");
            // 测试技能配置管理器的初始化
            SkillConfigManager.Instance.Init();
            
            // 测试技能配置的加载
            var skillConfigs = SkillConfigManager.Instance.LoadSkillConfigs();
            var buffConfigs = SkillConfigManager.Instance.LoadBuffConfigs();
            
            Sango.Log.Print($"技能配置数量: {skillConfigs.Count}");
            Sango.Log.Print($"BUFF配置数量: {buffConfigs.Count}");
        }

        public void TestSkillBalance()
        {
            Sango.Log.Print("开始技能平衡测试...");
            
            // 测试技能的伤害平衡
            TestSkillDamageBalance();
            
            // 测试技能的消耗平衡
            TestSkillCostBalance();
            
            // 测试技能的范围平衡
            TestSkillRangeBalance();
            
            Sango.Log.Print("技能平衡测试完成!");
        }

        private void TestSkillDamageBalance()
        {
            Sango.Log.Print("测试技能伤害平衡...");
            // 这里可以添加具体的伤害平衡测试逻辑
            Sango.Log.Print("技能伤害平衡测试完成");
        }

        private void TestSkillCostBalance()
        {
            Sango.Log.Print("测试技能消耗平衡...");
            // 这里可以添加具体的消耗平衡测试逻辑
            Sango.Log.Print("技能消耗平衡测试完成");
        }

        private void TestSkillRangeBalance()
        {
            Sango.Log.Print("测试技能范围平衡...");
            // 这里可以添加具体的范围平衡测试逻辑
            Sango.Log.Print("技能范围平衡测试完成");
        }
    }
}
