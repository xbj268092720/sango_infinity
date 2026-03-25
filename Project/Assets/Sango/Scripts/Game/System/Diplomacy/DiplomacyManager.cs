using System.Collections.Generic;
using TKNewtonsoft.Json;
using UnityEngine;

namespace Sango.Game
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

            switch (actionType)
            {
                case DiplomacyActionType.Alliance:
                    return relation >= 2000 && !sender.IsAlliance(receiver);
                case DiplomacyActionType.Truce:
                    return relation >= -1000 && !sender.IsAlliance(receiver);
                case DiplomacyActionType.DeclareWar:
                    return !sender.IsAlliance(receiver);
                case DiplomacyActionType.SendGift:
                    return true;
                case DiplomacyActionType.RequestTechnique:
                    return relation >= 1000;
                case DiplomacyActionType.RequestTroops:
                    return relation >= 1500;
                case DiplomacyActionType.Trade:
                    return relation >= -500;
                case DiplomacyActionType.Marriage:
                    return relation >= 1500;
                case DiplomacyActionType.AllianceRequest:
                    return relation >= 1500 && !sender.IsAlliance(receiver);
                case DiplomacyActionType.TruceRequest:
                    return relation >= -1500 && !sender.IsAlliance(receiver);
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

            switch (actionType)
            {
                case DiplomacyActionType.Alliance:
                    // 关系值越高，成功率越高，最高90%
                    baseSuccessRate = (int)Mathf.Clamp(50 + (relation - 2000) / 50, 10, 90);
                    break;
                case DiplomacyActionType.Truce:
                    // 关系值越高，成功率越高，最高80%
                    baseSuccessRate = (int)Mathf.Clamp(40 + (relation + 1000) / 37.5f, 10, 80);
                    break;
                case DiplomacyActionType.DeclareWar:
                    // 宣战总是成功
                    return 100;
                case DiplomacyActionType.SendGift:
                    // 送礼总是成功
                    return 100;
                case DiplomacyActionType.RequestTechnique:
                    // 关系值越高，成功率越高，最高85%
                    baseSuccessRate = (int)Mathf.Clamp(30 + (relation - 1000) / 47.06f, 10, 85);
                    break;
                case DiplomacyActionType.RequestTroops:
                    // 关系值越高，成功率越高，最高80%
                    baseSuccessRate = (int)Mathf.Clamp(20 + (relation - 1500) / 43.75f, 5, 80);
                    break;
                case DiplomacyActionType.Trade:
                    // 关系值越高，成功率越高，最高95%
                    baseSuccessRate = (int)Mathf.Clamp(50 + (relation + 500) / 55.56f, 10, 95);
                    break;
                case DiplomacyActionType.Marriage:
                    // 关系值越高，成功率越高，最高95%
                    baseSuccessRate = (int)Mathf.Clamp(40 + (relation - 1500) / 36.84f, 10, 95);
                    break;
                case DiplomacyActionType.AllianceRequest:
                    // 关系值越高，成功率越高，最高85%
                    baseSuccessRate = (int)Mathf.Clamp(30 + (relation - 1500) / 41.18f, 10, 85);
                    break;
                case DiplomacyActionType.TruceRequest:
                    // 关系值越高，成功率越高，最高75%
                    baseSuccessRate = (int)Mathf.Clamp(20 + (relation + 1500) / 40f, 5, 75);
                    break;
                default:
                    return 0;
            }

            // 使者能力加成
            if (diplomat != null)
            {
                int diplomacyAbility = diplomat.Politics + diplomat.Glamour / 2;
                int abilityBonus = Mathf.Min(diplomacyAbility / 10, 20);
                baseSuccessRate += abilityBonus;
            }

            // 金钱和道具价值加成
            if (resourceValue > 0)
            {
                int resourceBonus = Mathf.Min(resourceValue / 100, 30); // 每100金增加1%成功率，最高30%
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
        /// <returns>行动是否成功</returns>
        public bool PerformDiplomacyAction(DiplomacyActionType actionType, Force sender, Force receiver, Person diplomat = null, object param = null)
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
            diplomat.SetMission(MissionType.PersonDiplomacy, targetCity, distance, receiver.Id, (int)actionType, paramValue);

            // 将武将从首都的空闲武将列表中移除
            diplomat.BelongCity.freePersons.Remove(diplomat);

#if SANGO_DEBUG
            Sango.Log.Print($"@外交@{sender.Name} 对 {receiver.Name} 派遣了使者 {diplomat.Name} 执行{GetActionName(actionType)}行动！");
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
            // 使用ForceAI中的外交推荐方法选择合适的武将
            Person[] recommendedDiplomats = ForceAI.CounsellorRecommendDiplomacy(force.Governor.BelongCity.freePersons);
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
            if (forceA.IsAlliance(forceB))
                return false;

            // 创建联盟
            Alliance alliance = new Alliance()
            {
                ForceList = new SangoObjectList<Force>(),
                leftCount = 36, // 12个月
                allianceType = 1,
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
            AddRelation(forceA, forceB, 500);

#if SANGO_DEBUG
            Sango.Log.Print($"@外交@{forceA.Name} 与 {forceB.Name} 达成了12个月的结盟 Id={alliance.Id}!!");
#endif

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
            // 创建停战协议
            Alliance truce = new Alliance()
            {
                ForceList = new SangoObjectList<Force>(),
                leftCount = 18, // 6个月
                allianceType = 2, // 停战类型
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
            AddRelation(forceA, forceB, 300);

#if SANGO_DEBUG
            Sango.Log.Print($"@外交@{forceA.Name} 与 {forceB.Name} 达成了6个月的停战协议!!");
#endif

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
            ReduceRelation(forceA, forceB, 1000);

#if SANGO_DEBUG
            Sango.Log.Print($"@外交@{forceA.Name} 向 {forceB.Name} 宣战!!");
#endif

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
            // 检查发送方是否有足够的资金
            if (sender.Governor?.BelongCity?.gold < giftValue)
                return false;

            // 扣除资金
            sender.Governor.BelongCity.gold -= giftValue;

            // 计算关系增加量
            int relationIncrease = giftValue / 10; // 每10金增加1点关系

            // 增加关系
            AddRelation(sender, receiver, relationIncrease);

#if SANGO_DEBUG
            Sango.Log.Print($"@外交@{sender.Name} 向 {receiver.Name} 赠送了 {giftValue} 金，关系增加了 {relationIncrease}!!");
#endif

            return true;
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
            ReduceRelation(sender, receiver, 200);

#if SANGO_DEBUG
            Technique tech = Scenario.Cur.GetObject<Technique>(techId);
            Sango.Log.Print($"@外交@{sender.Name} 从 {receiver.Name} 处学到了技术 {tech?.Name}!!");
#endif

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
                sender.Governor.BelongCity.troops += troopCount;
            }

            // 减少关系
            ReduceRelation(sender, receiver, 300);

#if SANGO_DEBUG
            Sango.Log.Print($"@外交@{receiver.Name} 向 {sender.Name} 提供了 {troopCount} 兵力!!");
#endif

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

            // 增加双方的黄金收入
            sender.ForEachCity(city => city.baseGainGold += 50);
            receiver.ForEachCity(city => city.baseGainGold += 50);

            // 增加关系
            AddRelation(sender, receiver, 100);

#if SANGO_DEBUG
            Sango.Log.Print($"@外交@{sender.Name} 与 {receiver.Name} 达成了通商协议，双方城市的黄金收入增加了!!");
#endif

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
            // 增加关系
            AddRelation(sender, receiver, 500);

            // 提高双方的同盟概率
            if (!sender.IsAlliance(receiver))
            {
                // 和亲后更容易结盟
                AddRelation(sender, receiver, 200);
            }

#if SANGO_DEBUG
            Sango.Log.Print($"@外交@{sender.Name} 与 {receiver.Name} 达成了和亲，关系大幅提升!!");
#endif

            return true;
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
            if (relation < 800)
                return false;

            // 有一定概率成功
            if (GameRandom.Chance(relation, 2000))
            {
                return PerformAlliance(sender, receiver);
            }
            else
            {
                // 失败但关系略有提升
                AddRelation(sender, receiver, 50);
                
#if SANGO_DEBUG
                Sango.Log.Print($"@外交@{sender.Name} 请求与 {receiver.Name} 结盟，但被拒绝了，不过关系有所改善!!");
#endif

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
            if (relation < -800)
                return false;

            // 有一定概率成功
            if (GameRandom.Chance(relation + 1000, 1500))
            {
                return PerformTruce(sender, receiver);
            }
            else
            {
                // 失败但关系略有提升
                AddRelation(sender, receiver, 30);
                
#if SANGO_DEBUG
                Sango.Log.Print($"@外交@{sender.Name} 请求与 {receiver.Name} 停战，但被拒绝了，不过关系有所改善!!");
#endif

                return false;
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
        TruceRequest = 10     // 请求停战
    }
}
