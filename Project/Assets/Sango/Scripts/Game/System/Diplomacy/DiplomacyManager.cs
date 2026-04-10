using System.Collections.Generic;
using TKNewtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

namespace Sango.Core
{
    /// <summary>
    /// 外交管理器
    /// </summary>
    public class DiplomacyManager : Singleton<DiplomacyManager>
    {
        /// <summary>
        /// 势力关系字典
        /// </summary>
        private Dictionary<string, ForceRelation> _forceRelations;

        /// <summary>
        /// 初始化外交管理器
        /// </summary>
        public void Init()
        {
            _forceRelations = new Dictionary<string, ForceRelation>();

            // 注册每月开始事件
            GameEvent.OnMonthStart += HandleMonthlyRelationChanges;
        }

        /// <summary>
        /// 获取势力间的关系
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <returns>关系值</returns>
        public int GetRelation(Force forceA, Force forceB)
        {
            if (forceA == null || forceB == null || forceA == forceB)
                return 0;

            // 使用Scenario中的关系数据
            if (Scenario.Cur != null)
            {
                return Scenario.Cur.GetRelation(forceA, forceB);
            }

            // 备用：使用本地关系数据
            string key = GetForceRelationKey(forceA, forceB);
            if (_forceRelations.TryGetValue(key, out ForceRelation relation))
            {
                return relation.relation;
            }

            // 默认为0
            return 0;
        }

        /// <summary>
        /// 设置势力间的关系
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <param name="relation">关系值</param>
        public void SetRelation(Force forceA, Force forceB, int relation)
        {
            if (forceA == null || forceB == null || forceA == forceB)
                return;

            // 使用Scenario中的关系数据
            if (Scenario.Cur != null)
            {
                // 计算当前关系与目标关系的差值
                int currentRelation = Scenario.Cur.GetRelation(forceA, forceB);
                int delta = relation - currentRelation;
                Scenario.Cur.AddRelation(forceA, forceB, delta);
                return;
            }

            // 备用：使用本地关系数据
            string key = GetForceRelationKey(forceA, forceB);
            ForceRelation forceRelation;

            if (!_forceRelations.TryGetValue(key, out forceRelation))
            {
                forceRelation = new ForceRelation();
                _forceRelations[key] = forceRelation;
            }

            forceRelation.relation = relation;
        }

        /// <summary>
        /// 增加势力间的关系
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <param name="value">增加的值</param>
        public void AddRelation(Force forceA, Force forceB, int value)
        {
            int currentRelation = GetRelation(forceA, forceB);
            SetRelation(forceA, forceB, currentRelation + value);
            Player.PlayerMessage.AddTextMessage($"{forceA.ColorName}与{forceB.ColorName}的关系得到了提升!!", forceA, forceB.CapitalCity.x, forceB.CapitalCity.y);
        }

        /// <summary>
        /// 减少势力间的关系
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <param name="value">减少的值</param>
        public void ReduceRelation(Force forceA, Force forceB, int value)
        {
            int currentRelation = GetRelation(forceA, forceB);
            SetRelation(forceA, forceB, currentRelation - value);
        }

        /// <summary>
        /// 获取势力关系的键
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <returns>关系键</returns>
        private string GetForceRelationKey(Force forceA, Force forceB)
        {
            // 确保键的唯一性，使用较小的ID在前
            if (forceA.Id < forceB.Id)
            {
                return $"{forceA.Id}_{forceB.Id}";
            }
            else
            {
                return $"{forceB.Id}_{forceA.Id}";
            }
        }

        /// <summary>
        /// 检查是否可以执行外交行动
        /// </summary>
        /// <param name="actionType">外交行动类型</param>
        /// <param name="sender">发送方势力</param>
        /// <param name="receiver">接收方势力</param>
        /// <returns>是否可以执行</returns>
        public bool CanPerformDiplomacyAction(DiplomacyActionType actionType, Force sender, Force receiver)
        {
            if (sender == null || receiver == null || sender == receiver)
                return false;

            int relation = GetRelation(sender, receiver);
            ScenarioVariables variables = Scenario.Cur.Variables;

            switch (actionType)
            {
                case DiplomacyActionType.Alliance:
                    return relation >= variables.diplomacyAllianceRelationThreshold && !sender.HasActiveAgreement(receiver);
                case DiplomacyActionType.Truce:
                    return relation >= variables.diplomacyTruceRelationThreshold && !sender.HasActiveAgreement(receiver);
                case DiplomacyActionType.DeclareWar:
                    return !sender.HasActiveAgreement(receiver);
                case DiplomacyActionType.SendGift:
                    return true;
                case DiplomacyActionType.RequestTechnique:
                    return relation >= variables.diplomacyRequestTechniqueRelationThreshold;
                case DiplomacyActionType.RequestTroops:
                    return relation >= variables.diplomacyRequestTroopsRelationThreshold;
                case DiplomacyActionType.Trade:
                    return relation >= variables.diplomacyTradeRelationThreshold && !sender.HasActiveAgreement(receiver);
                case DiplomacyActionType.Marriage:
                    return relation >= variables.diplomacyMarriageRelationThreshold;
                case DiplomacyActionType.AllianceRequest:
                    return relation >= variables.diplomacyAllianceRequestRelationThreshold && !sender.HasActiveAgreement(receiver);
                case DiplomacyActionType.TruceRequest:
                    return relation >= variables.diplomacyTruceRequestRelationThreshold && !sender.HasActiveAgreement(receiver);
                case DiplomacyActionType.Ransom:
                    return true; // 赎回俘虏总是可以尝试
                default:
                    return false;
            }
        }

        /// <summary>
        /// 计算外交行动的成功率
        /// </summary>
        /// <param name="actionType">外交行动类型</param>
        /// <param name="sender">发送方势力</param>
        /// <param name="receiver">接收方势力</param>
        /// <param name="diplomat">执行外交的武将</param>
        /// <param name="resourceValue">金钱或道具的价值</param>
        /// <returns>成功率（0-100）</returns>
        public int CalculateDiplomacySuccessRate(DiplomacyActionType actionType, Force sender, Force receiver, Person diplomat = null, int resourceValue = 0)
        {
            if (sender == null || receiver == null || sender == receiver)
                return 0;

            int relation = GetRelation(sender, receiver);
            int baseSuccessRate = 0;
            ScenarioVariables variables = Scenario.Cur.Variables;

            switch (actionType)
            {
                case DiplomacyActionType.Alliance:
                    // 关系值越高，成功率越高，最高90%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyAllianceBaseSuccessRate + (relation - variables.diplomacyAllianceRelationThreshold) / variables.diplomacyAllianceRelationFactor, variables.diplomacyAllianceMinSuccessRate, variables.diplomacyAllianceMaxSuccessRate);
                    break;
                case DiplomacyActionType.Truce:
                    // 关系值越高，成功率越高，最高80%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyTruceBaseSuccessRate + (relation - variables.diplomacyTruceRelationThreshold) / variables.diplomacyTruceRelationFactor, variables.diplomacyTruceMinSuccessRate, variables.diplomacyTruceMaxSuccessRate);
                    break;
                case DiplomacyActionType.DeclareWar:
                    // 宣战总是成功
                    return 100;
                case DiplomacyActionType.SendGift:
                    // 送礼总是成功
                    return 100;
                case DiplomacyActionType.RequestTechnique:
                    // 关系值越高，成功率越高，最高85%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyRequestTechniqueBaseSuccessRate + (relation - variables.diplomacyRequestTechniqueRelationThreshold) / variables.diplomacyRequestTechniqueRelationFactor, variables.diplomacyRequestTechniqueMinSuccessRate, variables.diplomacyRequestTechniqueMaxSuccessRate);
                    break;
                case DiplomacyActionType.RequestTroops:
                    // 关系值越高，成功率越高，最高80%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyRequestTroopsBaseSuccessRate + (relation - variables.diplomacyRequestTroopsRelationThreshold) / variables.diplomacyRequestTroopsRelationFactor, variables.diplomacyRequestTroopsMinSuccessRate, variables.diplomacyRequestTroopsMaxSuccessRate);
                    break;
                case DiplomacyActionType.Trade:
                    // 关系值越高，成功率越高，最高95%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyTradeBaseSuccessRate + (relation - variables.diplomacyTradeRelationThreshold) / variables.diplomacyTradeRelationFactor, variables.diplomacyTradeMinSuccessRate, variables.diplomacyTradeMaxSuccessRate);
                    break;
                case DiplomacyActionType.Marriage:
                    // 关系值越高，成功率越高，最高95%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyMarriageBaseSuccessRate + (relation - variables.diplomacyMarriageRelationThreshold) / variables.diplomacyMarriageRelationFactor, variables.diplomacyMarriageMinSuccessRate, variables.diplomacyMarriageMaxSuccessRate);
                    break;
                case DiplomacyActionType.AllianceRequest:
                    // 关系值越高，成功率越高，最高85%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyAllianceRequestBaseSuccessRate + (relation - variables.diplomacyAllianceRequestRelationThreshold) / variables.diplomacyAllianceRequestRelationFactor, variables.diplomacyAllianceRequestMinSuccessRate, variables.diplomacyAllianceRequestMaxSuccessRate);
                    break;
                case DiplomacyActionType.TruceRequest:
                    // 关系值越高，成功率越高，最高75%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyTruceRequestBaseSuccessRate + (relation - variables.diplomacyTruceRequestRelationThreshold) / variables.diplomacyTruceRequestRelationFactor, variables.diplomacyTruceRequestMinSuccessRate, variables.diplomacyTruceRequestMaxSuccessRate);
                    break;
                case DiplomacyActionType.Ransom:
                    // 关系值越高，成功率越高，最高90%
                    baseSuccessRate = (int)Mathf.Clamp(variables.diplomacyRansomBaseSuccessRate + (relation + 1000) / variables.diplomacyRansomSuccessRelationFactor, variables.diplomacyRansomMinSuccessRate, variables.diplomacyRansomMaxSuccessRate);
                    break;
                default:
                    return 0;
            }

            // 使者能力加成
            if (diplomat != null)
            {
                int diplomacyAbility = diplomat.Politics + diplomat.Glamour / 2;
                int abilityBonus = Mathf.Min(diplomacyAbility / 10, Scenario.Cur.Variables.diplomacyAbilityBonusMax);
                baseSuccessRate += abilityBonus;
            }

            // 金钱和道具价值加成
            if (resourceValue > 0)
            {
                int resourceBonus = Mathf.Min(resourceValue / Scenario.Cur.Variables.diplomacyResourceBonusFactor, Scenario.Cur.Variables.diplomacyResourceBonusMax);
                baseSuccessRate += resourceBonus;
            }

            // 确保成功率在合理范围内
            return Mathf.Clamp(baseSuccessRate, 0, 100);
        }

        /// <summary>
        /// 执行外交行动
        /// </summary>
        /// <param name="actionType">外交行动类型</param>
        /// <param name="sender">发送方势力</param>
        /// <param name="receiver">接收方势力</param>
        /// <param name="diplomat">执行外交的武将</param>
        /// <param name="param">行动参数</param>
        /// <param name="captiveId">俘虏ID（仅用于赎回俘虏）</param>
        /// <returns>行动是否成功</returns>
        public bool PerformDiplomacyAction(DiplomacyActionType actionType, Force sender, Force receiver, Person diplomat = null, object param = null, int captiveId = 0)
        {
            if (!CanPerformDiplomacyAction(actionType, sender, receiver))
                return false;

            // 如果没有指定武将，自动选择合适的武将
            if (diplomat == null || !diplomat.IsFree)
            {
                diplomat = FindSuitableDiplomat(sender);
                if (diplomat == null)
                    return false;
            }

            // 处理附加参数
            int paramValue = 0;
            if (param != null)
            {
                if (param is int intParam)
                {
                    paramValue = intParam;
                }
            }

            // 获取目标势力主公所在的城市
            City targetCity = receiver.Governor?.BelongCity;
            if (targetCity == null)
                return false;

            // 计算移动所需时间
            int distance = diplomat.DistanceDays(targetCity);
            if (distance <= 0)
                distance = 1;

            // 设置外交任务，使用新的重载函数传递参数
            if (actionType == DiplomacyActionType.Ransom)
            {
                // 对于赎回俘虏，传递俘虏ID
                diplomat.SetMission(MissionType.PersonDiplomacy, targetCity, distance, receiver.Id, (int)actionType, paramValue, captiveId);
            }
            else
            {
                diplomat.SetMission(MissionType.PersonDiplomacy, targetCity, distance, receiver.Id, (int)actionType, paramValue);
            }
            if (receiver.Id == diplomat.BelongForce.Id)
            {
                Sango.Log.Error($"@外交@{sender.Name} 对 {receiver.Name} 派遣了使者 {diplomat.Name} 执行{GetActionName(actionType)}行动！");
            }
            // 将武将从首都的空闲武将列表中移除
            diplomat.BelongCity.freePersons.Remove(diplomat);

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{sender.Name} 对 {receiver.Name} 派遣了使者 {diplomat.Name} 执行{GetActionName(actionType)}行动！");
#endif
            return true;
        }

        /// <summary>
        /// 获取外交行动的名称
        /// </summary>
        /// <param name="actionType">外交行动类型</param>
        /// <returns>行动名称</returns>
        public string GetActionName(DiplomacyActionType actionType)
        {
            switch (actionType)
            {
                case DiplomacyActionType.Alliance:
                    return "结盟";
                case DiplomacyActionType.Truce:
                    return "停战";
                case DiplomacyActionType.DeclareWar:
                    return "宣战";
                case DiplomacyActionType.SendGift:
                    return "送礼";
                case DiplomacyActionType.RequestTechnique:
                    return "请求技术";
                case DiplomacyActionType.RequestTroops:
                    return "请求兵力";
                case DiplomacyActionType.Trade:
                    return "通商";
                case DiplomacyActionType.Marriage:
                    return "和亲";
                case DiplomacyActionType.AllianceRequest:
                    return "请求结盟";
                case DiplomacyActionType.TruceRequest:
                    return "请求停战";
                case DiplomacyActionType.Ransom:
                    return "赎回俘虏";
                default:
                    return "未知行动";
            }
        }

        /// <summary>
        /// 寻找合适的外交使者
        /// </summary>
        /// <param name="force">势力</param>
        /// <returns>合适的武将</returns>
        private Person FindSuitableDiplomat(Force force)
        {
            if (force == null || force.Governor == null || force.CapitalCity == null)
            {
                return null;
            }
            // 使用ForceAI中的外交推荐方法选择合适的武将
            Person[] recommendedDiplomats = ForceAI.CounsellorRecommendDiplomacy(force.CapitalCity.freePersons);
            if (recommendedDiplomats != null && recommendedDiplomats.Length > 0)
            {
                return recommendedDiplomats[0];
            }

            return null;
        }

        /// <summary>
        /// 执行结盟
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <returns>是否成功</returns>
        public bool PerformAlliance(Force forceA, Force forceB)
        {
            // 检查是否已经结盟
            if (forceA.CheckAlliance(forceB, AllianceType.Alliance) != null)
                return false;

            ScenarioVariables variables = Scenario.Cur.Variables;

            // 创建联盟
            Alliance alliance = new Alliance()
            {
                ForceList = new SangoObjectList<Force>(),
                leftCount = variables.diplomacyAllianceDuration, // 12个月
                allianceType = AllianceType.Alliance,
                IsAlive = true
            };

            alliance.ForceList.Add(forceA);
            alliance.ForceList.Add(forceB);

            // 添加到场景
            Scenario.Cur.Add(alliance);

            // 添加到势力的联盟列表
            forceA.AllianceList.Add(alliance);
            forceB.AllianceList.Add(alliance);

            // 增加关系
            AddRelation(forceA, forceB, variables.diplomacyAllianceRelationIncrease);

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{forceA.Name} 与 {forceB.Name} 达成了{variables.diplomacyAllianceDuration / 3}个月的结盟 Id={alliance.Id}!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacyAlliance?.Invoke(forceA, forceB, true);

            return true;
        }

        /// <summary>
        /// 执行停战
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <returns>是否成功</returns>
        public bool PerformTruce(Force forceA, Force forceB)
        {
            // 检查是否已经有停战协议
            if (forceA.CheckAlliance(forceB, AllianceType.Truce) != null)
                return false;

            ScenarioVariables variables = Scenario.Cur.Variables;

            // 创建停战协议
            Alliance truce = new Alliance()
            {
                ForceList = new SangoObjectList<Force>(),
                leftCount = variables.diplomacyTruceDuration, // 6个月
                allianceType = AllianceType.Truce, // 停战类型
                IsAlive = true
            };

            truce.ForceList.Add(forceA);
            truce.ForceList.Add(forceB);

            // 添加到场景
            Scenario.Cur.Add(truce);

            // 添加到势力的联盟列表
            forceA.AllianceList.Add(truce);
            forceB.AllianceList.Add(truce);

            // 增加关系
            AddRelation(forceA, forceB, variables.diplomacyTruceRelationIncrease);

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{forceA.Name} 与 {forceB.Name} 达成了{variables.diplomacyTruceDuration / 3}个月的停战协议!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacyTruce?.Invoke(forceA, forceB, true);

            return true;
        }

        /// <summary>
        /// 执行宣战
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <returns>是否成功</returns>
        public bool PerformDeclareWar(Force forceA, Force forceB)
        {
            // 解除联盟
            Alliance alliance = forceA.CheckAlliance(forceB);
            if (alliance != null)
            {
                alliance.IsAlive = false;
                forceA.AllianceList.Remove(alliance);
                forceB.AllianceList.Remove(alliance);
            }

            // 减少关系
            ReduceRelation(forceA, forceB, Scenario.Cur.Variables.diplomacyDeclareWarRelationDecrease);

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{forceA.Name} 向 {forceB.Name} 宣战!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacyDeclareWar?.Invoke(forceA, forceB, true);

            return true;
        }

        /// <summary>
        /// 执行送礼
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="receiver">接收方</param>
        /// <param name="giftValue">礼物价值</param>
        /// <returns>是否成功</returns>
        public bool PerformSendGift(Force sender, Force receiver, int giftValue)
        {
            // 计算关系增加量（考虑多种因素）
            int relationIncrease = CalculateDynamicRelationIncrease(sender, receiver, giftValue);

            // 增加关系
            AddRelation(sender, receiver, relationIncrease);

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{sender.Name} 向 {receiver.Name} 赠送了 {giftValue} 金，关系增加了 {relationIncrease}!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacySendGift?.Invoke(sender, receiver, giftValue, true);

            return true;
        }

        /// <summary>
        /// 动态计算送礼的关系增加量
        /// 考虑因素：对象势力的金钱、势力对比、主公喜好、性格等
        /// </summary>
        /// <param name="sender">发送方势力</param>
        /// <param name="receiver">接收方势力</param>
        /// <param name="giftValue">礼物价值</param>
        /// <returns>关系增加量</returns>
        private int CalculateDynamicRelationIncrease(Force sender, Force receiver, int giftValue)
        {
            ScenarioVariables variables = Scenario.Cur.Variables;

            // 基础关系增加量（每10金增加1点关系）
            int baseIncrease = giftValue / variables.diplomacySendGiftRelationFactor;

            // 计算各种因素的修正系数
            float factor = 1.0f;

            // 1. 对象势力的金钱因素：越穷的势力越看重礼物
            int receiverTotalGold = 0;
            receiver.ForEachCity(city =>
            {
                receiverTotalGold += city.gold;
            });

            // 如果接收方很穷，增加礼物效果
            if (receiverTotalGold < 5000)
            {
                factor += (5000 - receiverTotalGold) / 10000.0f; // 最多增加0.5倍效果
            }
            // 如果接收方很富，减少礼物效果
            else if (receiverTotalGold > 50000)
            {
                factor -= (receiverTotalGold - 50000) / 200000.0f; // 最多减少0.25倍效果
                factor = Mathf.Max(factor, 0.75f); // 最低保持0.75倍效果
            }

            // 2. 势力对比因素：弱小势力向强大势力送礼效果更好
            int senderPower = sender.FightPower > 0 ? sender.FightPower : 1;
            int receiverPower = receiver.FightPower > 0 ? receiver.FightPower : 1;
            float powerRatio = (float)senderPower / receiverPower;

            // 当发送方势力较弱时，增加礼物效果
            if (powerRatio < 0.5f)
            {
                factor += (0.5f - powerRatio) * 0.4f; // 最多增加0.2倍效果
            }
            // 当发送方势力较强时，减少礼物效果
            else if (powerRatio > 2.0f)
            {
                factor -= (powerRatio - 2.0f) * 0.1f; // 最多减少0.2倍效果
                factor = Mathf.Max(factor, 0.8f); // 最低保持0.8倍效果
            }

            // 3. 主公喜好和性格因素
            if (receiver.Governor != null)
            {
                Person governor = receiver.Governor;

                // 性格影响：使用Personality类中的送礼效果加成参数
                if (governor.personality != null)
                {
                    // 获取性格的送礼效果加成（百分比）
                    int giftEffectBonus = governor.personality.giftEffectAdd;
                    // 将百分比转换为系数（例如：10表示+10%，即0.1）
                    float bonusFactor = giftEffectBonus / 100.0f;
                    factor += bonusFactor;
                }

                // 相性影响：相性好的话效果更好
                if (sender.Governor != null)
                {
                    int compatibilityDiff = System.Math.Abs(sender.Governor.compatibility - governor.compatibility);
                    if (compatibilityDiff < 30)
                    {
                        factor += (30 - compatibilityDiff) / 300.0f; // 最多增加0.1倍效果
                    }
                }

                // 政治属性影响：政治高的主公更看重外交
                factor += governor.Politics / 1000.0f; // 最多增加0.1倍效果
            }

            // 4. 关系基础影响：关系越差，送礼效果越好
            int currentRelation = GetRelation(sender, receiver);
            if (currentRelation < 0)
            {
                factor += Mathf.Abs(currentRelation) / 2000.0f; // 最多增加0.5倍效果
            }
            else if (currentRelation > 1000)
            {
                factor -= currentRelation / 4000.0f; // 最多减少0.25倍效果
                factor = Mathf.Max(factor, 0.75f); // 最低保持0.75倍效果
            }

            // 计算最终关系增加量
            int finalIncrease = Mathf.RoundToInt(baseIncrease * factor);

            // 确保关系增加量至少为1
            return Mathf.Max(finalIncrease, 1);
        }

        /// <summary>
        /// 执行请求技术
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="receiver">接收方</param>
        /// <param name="techId">技术ID</param>
        /// <returns>是否成功</returns>
        public bool PerformRequestTechnique(Force sender, Force receiver, int techId)
        {
            // 检查接收方是否拥有该技术
            if (!receiver.HasTechnique(techId))
                return false;

            // 检查发送方是否已经拥有该技术
            if (sender.HasTechnique(techId))
                return false;

            // 学习技术
            sender.AddTechnique(techId);

            // 减少关系
            ReduceRelation(sender, receiver, Scenario.Cur.Variables.diplomacyRequestTechniqueRelationDecrease);

#if SANGO_DEBUG
            Technique tech = Scenario.Cur.GetObject<Technique>(techId);
            Sango.Log.Info($"@外交@{sender.Name} 从 {receiver.Name} 处学到了技术 {tech?.Name}!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacyRequestTechnique?.Invoke(sender, receiver, techId, true);

            return true;
        }

        /// <summary>
        /// 执行请求兵力
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="receiver">接收方</param>
        /// <param name="troopCount">请求兵力</param>
        /// <returns>是否成功</returns>
        public bool PerformRequestTroops(Force sender, Force receiver, int troopCount)
        {
            // 检查接收方是否有足够的兵力
            int totalTroops = 0;
            receiver.ForEachCity(city =>
            {
                totalTroops += city.troops;
            });

            if (totalTroops < troopCount)
                return false;

            // 从接收方的城市中抽调兵力
            int remainingTroops = troopCount;
            receiver.ForEachCity(city =>
            {
                if (remainingTroops <= 0)
                    return;

                int troopsToTake = Mathf.Min(city.troops, remainingTroops);
                city.troops -= troopsToTake;
                remainingTroops -= troopsToTake;
            });

            // 增加到发送方的首都
            if (sender.Governor?.BelongCity != null)
            {
                sender.CapitalCity.troops += troopCount;
            }

            // 减少关系
            ReduceRelation(sender, receiver, Scenario.Cur.Variables.diplomacyRequestTroopsRelationDecrease);

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{receiver.Name} 向 {sender.Name} 提供了 {troopCount} 兵力!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacyRequestTroops?.Invoke(sender, receiver, troopCount, true);

            return true;
        }

        /// <summary>
        /// 执行通商
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="receiver">接收方</param>
        /// <returns>是否成功</returns>
        public bool PerformTrade(Force sender, Force receiver)
        {
            // 检查双方是否有足够的城市
            int senderCityCount = 0;
            sender.ForEachCity(city => senderCityCount++);
            int receiverCityCount = 0;
            receiver.ForEachCity(city => receiverCityCount++);

            if (senderCityCount == 0 || receiverCityCount == 0)
                return false;

            // 检查是否已经有协议
            if (sender.HasActiveAgreement(receiver))
                return false;

            ScenarioVariables variables = Scenario.Cur.Variables;

            // 创建通商协议
            Alliance tradeAlliance = new Alliance()
            {
                ForceList = new SangoObjectList<Force>(),
                leftCount = variables.diplomacyTradeDuration, // 8个月
                allianceType = AllianceType.Trade, // 通商类型
                IsAlive = true
            };

            tradeAlliance.ForceList.Add(sender);
            tradeAlliance.ForceList.Add(receiver);

            // 添加到场景
            Scenario.Cur.Add(tradeAlliance);

            // 添加到势力的联盟列表
            sender.AllianceList.Add(tradeAlliance);
            receiver.AllianceList.Add(tradeAlliance);

            // 增加双方的黄金收入
            sender.ForEachCity(city => city.baseGainGold += variables.diplomacyTradeGoldIncrease);
            receiver.ForEachCity(city => city.baseGainGold += variables.diplomacyTradeGoldIncrease);

            // 增加关系
            AddRelation(sender, receiver, variables.diplomacyTradeRelationIncrease);

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{sender.Name} 与 {receiver.Name} 达成了{variables.diplomacyTradeDuration / 3}个月的通商协议，双方城市的黄金收入增加了!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacyTrade?.Invoke(sender, receiver, true);

            return true;
        }

        /// <summary>
        /// 执行和亲
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="receiver">接收方</param>
        /// <returns>是否成功</returns>
        public bool PerformMarriage(Force sender, Force receiver)
        {
            ScenarioVariables variables = Scenario.Cur.Variables;

            // 增加关系
            AddRelation(sender, receiver, variables.diplomacyMarriageRelationIncrease);

            // 提高双方的同盟概率
            if (!sender.IsAlliance(receiver))
            {
                // 和亲后更容易结盟
                AddRelation(sender, receiver, variables.diplomacyMarriageExtraRelationIncrease);
            }

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{sender.Name} 与 {receiver.Name} 达成了和亲，关系大幅提升!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacyMarriage?.Invoke(sender, receiver, true);

            return true;
        }

        /// <summary>
        /// 执行赎回俘虏
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="receiver">接收方</param>
        /// <param name="ransomValue">赎金</param>
        /// <param name="captiveId">俘虏ID</param>
        /// <returns>是否成功</returns>
        public bool PerformRansom(Person sender, Force receiver, int ransomValue, int captiveId)
        {
            // 检查发送方是否有足够的资金
            ransomValue = System.Math.Min(ransomValue, sender.BelongCity.gold);

            // 查找指定的俘虏
            Person captive = null;
            City captiveCity = null;
            Troop captiveTroop = null;

            for (int i = 0; i < sender.BelongForce.BeCaptiveList.Count; i++)
            {
                Person person = sender.BelongForce.BeCaptiveList[i];
                if (person.Id == captiveId)
                {
                    captive = person;
                    captiveCity = person.CurrentCity;
                    captiveTroop = person.BelongTroop;
                    break;
                }
            }

            if (captive == null)
                return false;

            // 扣除资金
            sender.BelongCity.gold -= ransomValue;

            // 释放俘虏
            captive.Escape(EscapeType.Released, receiver);

            // 增加关系
            int relationIncrease = ransomValue / Scenario.Cur.Variables.diplomacyRansomRelationFactor; // 每20金增加1点关系
            AddRelation(sender.BelongForce, receiver, relationIncrease);

#if SANGO_DEBUG
            Sango.Log.Info($"@外交@{sender.Name} 向 {receiver.Name} 支付了 {ransomValue} 金赎回了俘虏 {captive.Name}，关系增加了 {relationIncrease}!!");
#endif

            // 触发事件
            GameEvent.OnDiplomacyRansom?.Invoke(sender.BelongForce, receiver, ransomValue, true);

            return true;
        }

        /// <summary>
        /// 计算赎回俘虏的额外费用
        /// </summary>
        /// <param name="captive">俘虏</param>
        /// <returns>额外费用</returns>
        public int CalculateRansomExtraCost(Person captive)
        {
            if (captive == null)
                return 0;

            ScenarioVariables variables = Scenario.Cur.Variables;
            int extraCost = 0;

            // 基于等级的额外费用
            extraCost += captive.Level.Id * variables.diplomacyRansomLevelCostFactor;

            // 基于功绩的额外费用
            extraCost += captive.merit / variables.diplomacyRansomMeritCostFactor;

            // 基于官职的额外费用
            if (captive.Official != null)
            {
                extraCost += captive.Official.level * variables.diplomacyRansomOfficialCostFactor;
            }

            // 基于属性的额外费用（统率、武力、智力、政治、魅力的平均值）
            int avgAttribute = (captive.Command + captive.Strength + captive.Intelligence + captive.Politics + captive.Glamour) / 5;
            extraCost += avgAttribute * variables.diplomacyRansomAttributeCostFactor;

            return extraCost;
        }

        /// <summary>
        /// 执行请求结盟
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="receiver">接收方</param>
        /// <returns>是否成功</returns>
        public bool PerformAllianceRequest(Force sender, Force receiver)
        {
            // 检查关系是否足够
            int relation = GetRelation(sender, receiver);
            ScenarioVariables variables = Scenario.Cur.Variables;
            if (relation < variables.diplomacyAllianceRequestSuccessRelationThreshold)
                return false;

            // 检查是否已经有协议
            if (sender.HasActiveAgreement(receiver))
                return false;

            // 有一定概率成功
            if (GameRandom.Chance(relation, variables.diplomacyAllianceRequestChanceDenominator))
            {
                bool success = PerformAlliance(sender, receiver);
                // 触发事件
                GameEvent.OnDiplomacyAllianceRequest?.Invoke(sender, receiver, success);
                return success;
            }
            else
            {
                // 失败但关系略有提升
                AddRelation(sender, receiver, variables.diplomacyAllianceRequestRelationIncrease);

#if SANGO_DEBUG
                Sango.Log.Info($"@外交@{sender.Name} 请求与 {receiver.Name} 结盟，但被拒绝了，不过关系有所改善!!");
#endif

                // 触发事件
                GameEvent.OnDiplomacyAllianceRequest?.Invoke(sender, receiver, false);
                return false;
            }
        }

        /// <summary>
        /// 执行请求停战
        /// </summary>
        /// <param name="sender">发送方</param>
        /// <param name="receiver">接收方</param>
        /// <returns>是否成功</returns>
        public bool PerformTruceRequest(Force sender, Force receiver)
        {
            // 检查关系是否足够
            int relation = GetRelation(sender, receiver);
            ScenarioVariables variables = Scenario.Cur.Variables;
            if (relation < variables.diplomacyTruceRequestSuccessRelationThreshold)
                return false;

            // 检查是否已经有协议
            if (sender.HasActiveAgreement(receiver))
                return false;

            // 有一定概率成功
            if (GameRandom.Chance(relation + variables.diplomacyTruceRequestChanceOffset, variables.diplomacyTruceRequestChanceDenominator))
            {
                bool success = PerformTruce(sender, receiver);
                // 触发事件
                GameEvent.OnDiplomacyTruceRequest?.Invoke(sender, receiver, success);
                return success;
            }
            else
            {
                // 失败但关系略有提升
                AddRelation(sender, receiver, variables.diplomacyTruceRequestRelationIncrease);

#if SANGO_DEBUG
                Sango.Log.Info($"@外交@{sender.Name} 请求与 {receiver.Name} 停战，但被拒绝了，不过关系有所改善!!");
#endif

                // 触发事件
                GameEvent.OnDiplomacyTruceRequest?.Invoke(sender, receiver, false);
                return false;
            }
        }

        /// <summary>
        /// 撕毁条约
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        /// <returns>是否成功</returns>
        public bool BreakAlliance(Force forceA, Force forceB)
        {
            // 检查是否有同盟或停战协议
            Alliance alliance = forceA.CheckAlliance(forceB);
            if (alliance == null)
                return false;

            // 标记条约为无效
            alliance.IsAlive = false;

            // 从双方的同盟列表中移除
            forceA.AllianceList.Remove(alliance);
            forceB.AllianceList.Remove(alliance);

            ScenarioVariables variables = Scenario.Cur.Variables;

            // 如果是通商协议，还原黄金收入
            if (alliance.allianceType == AllianceType.Trade)
            {
                forceA.ForEachCity(city => city.baseGainGold -= variables.diplomacyTradeGoldIncrease);
                forceB.ForEachCity(city => city.baseGainGold -= variables.diplomacyTradeGoldIncrease);
            }

            // 减少关系
            int relationDecrease = 0;
            switch (alliance.allianceType)
            {
                case AllianceType.Alliance: // 同盟
                    relationDecrease = variables.diplomacyBreakAllianceRelationDecrease;
                    break;
                case AllianceType.Truce: // 停战
                    relationDecrease = variables.diplomacyBreakTruceRelationDecrease;
                    break;
                case AllianceType.Trade: // 通商
                    relationDecrease = variables.diplomacyBreakTradeRelationDecrease;
                    break;
            }
            ReduceRelation(forceA, forceB, relationDecrease);

#if SANGO_DEBUG
            string treatyType = alliance.allianceType == AllianceType.Alliance ? "同盟" : (alliance.allianceType == AllianceType.Truce ? "停战协议" : "通商协议");
            Sango.Log.Info($"@外交@{forceA.Name} 撕毁了与 {forceB.Name} 的{treatyType}，关系减少了 {relationDecrease}！");
#endif

            // 触发事件
            GameEvent.OnDiplomacyBreakAlliance?.Invoke(forceA, forceB, true);

            return true;
        }

        /// <summary>
        /// 处理每月关系变化
        /// </summary>
        /// <param name="scenario">场景</param>
        private void HandleMonthlyRelationChanges(Scenario scenario)
        {
            if (scenario == null || scenario.forceSet.Count < 2)
                return;

            int forceCount = scenario.forceSet.Count;
            ScenarioVariables variables = scenario.Variables;

            for (int i = 0; i < forceCount; ++i)
            {
                Force forceA = scenario.forceSet[i];
                if (forceA == null || !forceA.IsAlive)
                    continue;

                for (int j = i + 1; j < forceCount; ++j)
                {
                    Force forceB = scenario.forceSet[j];
                    if (forceB == null || !forceB.IsAlive)
                        continue;

                    // 检查是否有同盟或停战协议
                    bool hasAlliance = forceA.IsAlliance(forceB);

                    // 关系变化概率
                    int chance = variables.relationChangeChance;

                    // 同盟关系变化概率降低，且变化值为正
                    if (hasAlliance)
                    {
                        chance = chance / 2;
                        if (GameRandom.Chance(chance))
                        {
                            // 同盟关系缓慢增长
                            AddRelation(forceA, forceB, variables.diplomacyMonthlyAllianceRelationIncrease);
                        }
                    }
                    else
                    {
                        // 普通关系有一定概率下降
                        if (GameRandom.Chance(chance))
                        {
                            // 关系自然衰减
                            ReduceRelation(forceA, forceB, variables.diplomacyMonthlyNormalRelationDecrease);
                        }
                    }
                }
            }
        }

        public void DoPersonDiplomacyAction(Person person, DiplomacyActionType actionType, Force receiverForce, int resourceValue, int captiveId)
        {
            // 计算成功率
            int successRate = CalculateDiplomacySuccessRate(actionType, person.BelongForce, receiverForce, person, resourceValue);
            bool success = false;

            // 根据成功率判断是否执行成功
            if (GameRandom.Chance(successRate))
            {
                switch (actionType)
                {
                    case DiplomacyActionType.Alliance:
                        success = PerformAlliance(person.BelongForce, receiverForce);
                        break;
                    case DiplomacyActionType.Truce:
                        success = PerformTruce(person.BelongForce, receiverForce);
                        break;
                    case DiplomacyActionType.DeclareWar:
                        success = PerformDeclareWar(person.BelongForce, receiverForce);
                        break;
                    case DiplomacyActionType.SendGift:
                        // 使用missionParams3存储礼物价值
                        success = PerformSendGift(person.BelongForce, receiverForce, resourceValue);
                        break;
                    case DiplomacyActionType.RequestTechnique:
                        // 使用missionParams3存储技术ID
                        success = PerformRequestTechnique(person.BelongForce, receiverForce, resourceValue);
                        break;
                    case DiplomacyActionType.RequestTroops:
                        // 使用missionParams3存储兵力数量
                        success = PerformRequestTroops(person.BelongForce, receiverForce, resourceValue);
                        break;
                    case DiplomacyActionType.Trade:
                        success = PerformTrade(person.BelongForce, receiverForce);
                        break;
                    case DiplomacyActionType.Marriage:
                        success = PerformMarriage(person.BelongForce, receiverForce);
                        break;
                    case DiplomacyActionType.AllianceRequest:
                        success = PerformAllianceRequest(person.BelongForce, receiverForce);
                        break;
                    case DiplomacyActionType.TruceRequest:
                        success = PerformTruceRequest(person.BelongForce, receiverForce);
                        break;
                    case DiplomacyActionType.Ransom:

                        // 使用missionParams3存储赎金，missionParams4存储俘虏ID
                        success = PerformRansom(person, receiverForce, resourceValue, captiveId);
                        break;
                }
            }

            // 输出调试信息
            if (success)
            {
#if SANGO_DEBUG
                Sango.Log.Info($"@外交@{person.BelongForce.Name} 对 {receiverForce.Name} 的{GetActionName(actionType)}行动成功了！成功率: {successRate}%");
#endif
            }
            else
            {
#if SANGO_DEBUG
                Sango.Log.Info($"@外交@{person.BelongForce.Name} 对 {receiverForce.Name} 的{GetActionName(actionType)}行动失败了！成功率: {successRate}%");
#endif
                // 外交失败减少关系
                int relationDecrease = 0;
                switch (actionType)
                {
                    case DiplomacyActionType.Alliance:
                    case DiplomacyActionType.AllianceRequest:
                        relationDecrease = 50;
                        break;
                    case DiplomacyActionType.Truce:
                    case DiplomacyActionType.TruceRequest:
                        relationDecrease = 30;
                        break;
                    case DiplomacyActionType.RequestTechnique:
                        relationDecrease = 100;
                        break;
                    case DiplomacyActionType.RequestTroops:
                        relationDecrease = 150;
                        break;
                    case DiplomacyActionType.Trade:
                        relationDecrease = 40;
                        break;
                    case DiplomacyActionType.Marriage:
                        relationDecrease = 80;
                        break;
                    case DiplomacyActionType.Ransom:
                        relationDecrease = 60;
                        break;
                    default:
                        relationDecrease = 20;
                        break;
                }

                ReduceRelation(person.BelongForce, receiverForce, relationDecrease);

#if SANGO_DEBUG
                Sango.Log.Info($"@外交@{person.BelongForce.Name} 与 {receiverForce.Name} 的关系减少了 {relationDecrease}！");
#endif
            }
        }

        /// <summary>
        /// 玩家发起外交行动（直接执行，无判断）
        /// </summary>
        /// <param name="actionType">外交行动类型</param>
        /// <param name="diplomat">执行外交的武将</param>
        /// <param name="receiver">接收方势力</param>
        /// <param name="cost">消耗资金</param>
        /// <param name="param">行动参数</param>
        /// <param name="captiveId">俘虏ID（仅用于赎回俘虏）</param>
        /// <returns>是否成功</returns>
        public bool PlayerInitiateDiplomacyAction(DiplomacyActionType actionType, Person diplomat, Force receiver, int cost, object param = null, int captiveId = 0)
        {
            City senderCity = diplomat.BelongCity;
            if (senderCity == null || senderCity.BelongForce == null || receiver == null || senderCity.BelongForce == receiver || diplomat == null)
                return false;

            // 扣除资金
            if (senderCity.gold >= cost)
            {
                senderCity.gold -= cost;
                // 刷新城市Render
                senderCity.Render?.UpdateRender();
            }
            else
            {
                return false; // 资金不足
            }

            // 处理附加参数
            int paramValue = cost;

            // 获取目标势力主公所在的城市
            City targetCity = receiver.Governor?.BelongCity;
            if (targetCity == null)
                return false;

            // 计算移动所需时间
            int distance = diplomat.DistanceDays(targetCity);
            if (distance <= 0)
                distance = 1;

            // 设置外交任务
            if (actionType == DiplomacyActionType.Ransom)
            {
                // 对于赎回俘虏，传递俘虏ID
                diplomat.SetMission(MissionType.PersonDiplomacy, targetCity, distance, receiver.Id, (int)actionType, paramValue, captiveId);
            }
            else
            {
                diplomat.SetMission(MissionType.PersonDiplomacy, targetCity, distance, receiver.Id, (int)actionType, paramValue);
            }

            // 将武将从城市的空闲武将列表中移除
            if (diplomat.BelongCity != null)
            {
                diplomat.BelongCity.freePersons.Remove(diplomat);
            }

            // 设置武将为行动过了
            diplomat.ActionOver = true;

#if SANGO_DEBUG
            Sango.Log.Info($"@玩家外交@{senderCity.BelongForce.Name} 对 {receiver.Name} 派遣了使者 {diplomat.Name} 执行{GetActionName(actionType)}行动，消耗了 {cost} 金！");
#endif
            return true;
        }

        public void DoPersonDiplomacyActionNoCheck(bool success, Person person, DiplomacyActionType actionType, Force receiverForce, int resourceValue, int captiveId)
        {
            switch (actionType)
            {
                case DiplomacyActionType.Alliance:
                    PerformAlliance(person.BelongForce, receiverForce);
                    break;
                case DiplomacyActionType.Truce:
                    PerformTruce(person.BelongForce, receiverForce);
                    break;
                case DiplomacyActionType.DeclareWar:
                    PerformDeclareWar(person.BelongForce, receiverForce);
                    break;
                case DiplomacyActionType.SendGift:
                    // 使用missionParams3存储礼物价值
                    PerformSendGift(person.BelongForce, receiverForce, resourceValue);
                    break;
                case DiplomacyActionType.RequestTechnique:
                    // 使用missionParams3存储技术ID
                    PerformRequestTechnique(person.BelongForce, receiverForce, resourceValue);
                    break;
                case DiplomacyActionType.RequestTroops:
                    // 使用missionParams3存储兵力数量
                    PerformRequestTroops(person.BelongForce, receiverForce, resourceValue);
                    break;
                case DiplomacyActionType.Trade:
                    PerformTrade(person.BelongForce, receiverForce);
                    break;
                case DiplomacyActionType.Marriage:
                    PerformMarriage(person.BelongForce, receiverForce);
                    break;
                case DiplomacyActionType.AllianceRequest:
                    PerformAllianceRequest(person.BelongForce, receiverForce);
                    break;
                case DiplomacyActionType.TruceRequest:
                    PerformTruceRequest(person.BelongForce, receiverForce);
                    break;
                case DiplomacyActionType.Ransom:

                    // 使用missionParams3存储赎金，missionParams4存储俘虏ID
                    PerformRansom(person, receiverForce, resourceValue, captiveId);
                    break;
            }

            // 输出调试信息
            if (success)
            {
#if SANGO_DEBUG
                Sango.Log.Info($"@外交@{person.BelongForce.Name} 对 {receiverForce.Name} 的{GetActionName(actionType)}行动成功了！");
#endif
            }
            else
            {
#if SANGO_DEBUG
                Sango.Log.Info($"@外交@{person.BelongForce.Name} 对 {receiverForce.Name} 的{GetActionName(actionType)}行动失败了！");
#endif
                // 外交失败减少关系
                int relationDecrease = 0;
                switch (actionType)
                {
                    case DiplomacyActionType.Alliance:
                    case DiplomacyActionType.AllianceRequest:
                        relationDecrease = 50;
                        break;
                    case DiplomacyActionType.Truce:
                    case DiplomacyActionType.TruceRequest:
                        relationDecrease = 30;
                        break;
                    case DiplomacyActionType.RequestTechnique:
                        relationDecrease = 100;
                        break;
                    case DiplomacyActionType.RequestTroops:
                        relationDecrease = 150;
                        break;
                    case DiplomacyActionType.Trade:
                        relationDecrease = 40;
                        break;
                    case DiplomacyActionType.Marriage:
                        relationDecrease = 80;
                        break;
                    case DiplomacyActionType.Ransom:
                        relationDecrease = 60;
                        break;
                    default:
                        relationDecrease = 20;
                        break;
                }

                ReduceRelation(person.BelongForce, receiverForce, relationDecrease);

#if SANGO_DEBUG
                Sango.Log.Info($"@外交@{person.BelongForce.Name} 与 {receiverForce.Name} 的关系减少了 {relationDecrease}！");
#endif
            }
        }

    }

    /// <summary>
    /// 外交行动类型
    /// </summary>
    public enum DiplomacyActionType
    {
        Alliance = 1,        // 结盟
        Truce = 2,          // 停战
        DeclareWar = 3,      // 宣战
        SendGift = 4,        // 送礼
        RequestTechnique = 5, // 请求技术
        RequestTroops = 6,    // 请求兵力
        Trade = 7,           // 通商
        Marriage = 8,        // 和亲
        AllianceRequest = 9,  // 请求结盟
        TruceRequest = 10,    // 请求停战
        Ransom = 11           // 赎回俘虏
    }
}
