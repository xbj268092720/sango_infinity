using System.Collections.Generic;
using UnityEngine;

namespace Sango.Game
{
    /// <summary>
    /// 外交事件管理器
    /// </summary>
    public class DiplomacyEventManager : Singleton<DiplomacyEventManager>
    {
        /// <summary>
        /// 外交事件列表
        /// </summary>
        private List<DiplomacyEvent> _diplomacyEvents;

        /// <summary>
        /// 初始化外交事件管理器
        /// </summary>
        public void Init()
        {
            _diplomacyEvents = new List<DiplomacyEvent>();
            // 初始化默认外交事件
            InitDefaultEvents();
        }

        /// <summary>
        /// 初始化默认外交事件
        /// </summary>
        private void InitDefaultEvents()
        {
            // 添加默认外交事件
            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 1,
                Name = "使者来访",
                Description = "对方派遣使者来访，带来了友好的问候。",
                MinRelation = 0,
                MaxRelation = 500,
                Probability = 5,
                Effect = (sender, receiver) =>
                {
                    // 增加关系
                    DiplomacyManager.Instance.AddRelation(sender, receiver, 50);
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{receiver.Name} 的使者访问了 {sender.Name}，关系增加了50点！");
#endif
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 2,
                Name = "边境冲突",
                Description = "双方在边境发生了小规模冲突。",
                MinRelation = -500,
                MaxRelation = 0,
                Probability = 8,
                Effect = (sender, receiver) =>
                {
                    // 减少关系
                    DiplomacyManager.Instance.ReduceRelation(sender, receiver, 100);
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{sender.Name} 与 {receiver.Name} 在边境发生了冲突，关系减少了100点！");
#endif
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 3,
                Name = "贸易提议",
                Description = "对方提出了贸易合作的提议。",
                MinRelation = 300,
                MaxRelation = 800,
                Probability = 10,
                Effect = (sender, receiver) =>
                {
                    // 尝试通商
                    DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.Trade, sender, receiver);
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 4,
                Name = "结盟邀请",
                Description = "对方邀请你结成同盟。",
                MinRelation = 800,
                MaxRelation = 1500,
                Probability = 5,
                Effect = (sender, receiver) =>
                {
                    // 尝试结盟
                    DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.AllianceRequest, receiver, sender);
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 5,
                Name = "停战请求",
                Description = "对方请求停战。",
                MinRelation = -800,
                MaxRelation = -300,
                Probability = 7,
                Effect = (sender, receiver) =>
                {
                    // 尝试停战
                    DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.TruceRequest, receiver, sender);
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 6,
                Name = "技术交流",
                Description = "对方提议进行技术交流。",
                MinRelation = 500,
                MaxRelation = 1000,
                Probability = 6,
                Effect = (sender, receiver) =>
                {
                    // 随机选择一个技术进行交流
                    if (receiver.Techniques.Count > 0)
                    {
                        int randomIndex = Random.Range(0, receiver.Techniques.Count);
                        Technique tech = receiver.Techniques[randomIndex];
                        if (tech != null && !sender.HasTechnique(tech.Id))
                        {
                            sender.AddTechnique(tech.Id);
                            DiplomacyManager.Instance.ReduceRelation(sender, receiver, 100);
#if SANGO_DEBUG
                            Sango.Log.Print($"@外交事件@{receiver.Name} 与 {sender.Name} 进行了技术交流，{sender.Name} 获得了技术 {tech.Name}！");
#endif
                        }
                    }
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 7,
                Name = "军事援助请求",
                Description = "对方请求军事援助。",
                MinRelation = 600,
                MaxRelation = 900,
                Probability = 4,
                Effect = (sender, receiver) =>
                {
                    // 尝试请求兵力
                    int troopCount = Random.Range(1000, 5000);
                    DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.RequestTroops, receiver, sender, null, troopCount);
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 8,
                Name = "和亲提议",
                Description = "对方提议通过和亲来加强关系。",
                MinRelation = 700,
                MaxRelation = 1200,
                Probability = 5,
                Effect = (sender, receiver) =>
                {
                    // 尝试和亲
                    DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.Marriage, sender, receiver);
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 9,
                Name = "领土争端",
                Description = "双方因领土问题产生了争端。",
                MinRelation = -800,
                MaxRelation = -400,
                Probability = 6,
                Effect = (sender, receiver) =>
                {
                    // 减少关系
                    DiplomacyManager.Instance.ReduceRelation(sender, receiver, 150);
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{sender.Name} 与 {receiver.Name} 发生了领土争端，关系减少了150点！");
#endif
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 10,
                Name = "共同敌人",
                Description = "双方发现了共同的敌人。",
                MinRelation = -300,
                MaxRelation = 300,
                Probability = 7,
                Effect = (sender, receiver) =>
                {
                    // 增加关系
                    DiplomacyManager.Instance.AddRelation(sender, receiver, 100);
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{sender.Name} 与 {receiver.Name} 发现了共同的敌人，关系增加了100点！");
#endif
                }
            });

            // 新增外交事件
            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 11,
                Name = "文化交流",
                Description = "对方提议进行文化交流，促进相互了解。",
                MinRelation = 200,
                MaxRelation = 700,
                Probability = 6,
                Effect = (sender, receiver) =>
                {
                    // 增加关系
                    DiplomacyManager.Instance.AddRelation(sender, receiver, 80);
                    // 有一定概率获得技术
                    if (GameRandom.Chance(20) && receiver.Techniques.Count > 0)
                    {
                        int randomIndex = Random.Range(0, receiver.Techniques.Count);
                        Technique tech = receiver.Techniques[randomIndex];
                        if (tech != null && !sender.HasTechnique(tech.Id))
                        {
                            sender.AddTechnique(tech.Id);
#if SANGO_DEBUG
                            Sango.Log.Print($"@外交事件@{receiver.Name} 与 {sender.Name} 进行了文化交流，{sender.Name} 获得了技术 {tech.Name}！");
#endif
                        }
                    }
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{receiver.Name} 与 {sender.Name} 进行了文化交流，关系增加了80点！");
#endif
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 12,
                Name = "使者被扣留",
                Description = "你的使者被对方扣留，引发外交危机。",
                MinRelation = -700,
                MaxRelation = -200,
                Probability = 5,
                Effect = (sender, receiver) =>
                {
                    // 减少关系
                    DiplomacyManager.Instance.ReduceRelation(sender, receiver, 200);
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{sender.Name} 的使者被 {receiver.Name} 扣留，关系减少了200点！");
#endif
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 13,
                Name = "共同庆典",
                Description = "双方共同举办庆典，增进友谊。",
                MinRelation = 400,
                MaxRelation = 900,
                Probability = 4,
                Effect = (sender, receiver) =>
                {
                    // 增加关系
                    DiplomacyManager.Instance.AddRelation(sender, receiver, 100);
                    // 增加双方的黄金
                    if (sender.Governor?.BelongCity != null)
                    {
                        sender.Governor.BelongCity.gold += 1000;
                    }
                    if (receiver.Governor?.BelongCity != null)
                    {
                        receiver.Governor.BelongCity.gold += 1000;
                    }
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{sender.Name} 与 {receiver.Name} 共同举办了庆典，关系增加了100点，双方各获得了1000金！");
#endif
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 14,
                Name = "边境贸易",
                Description = "双方在边境开展贸易活动，促进经济发展。",
                MinRelation = -100,
                MaxRelation = 600,
                Probability = 8,
                Effect = (sender, receiver) =>
                {
                    // 增加关系
                    DiplomacyManager.Instance.AddRelation(sender, receiver, 60);
                    // 增加双方的黄金
                    if (sender.Governor?.BelongCity != null)
                    {
                        sender.Governor.BelongCity.gold += 500;
                    }
                    if (receiver.Governor?.BelongCity != null)
                    {
                        receiver.Governor.BelongCity.gold += 500;
                    }
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{sender.Name} 与 {receiver.Name} 在边境开展了贸易活动，关系增加了60点，双方各获得了500金！");
#endif
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 15,
                Name = "军事威胁",
                Description = "对方在边境集结军队，对你构成威胁。",
                MinRelation = -900,
                MaxRelation = -300,
                Probability = 6,
                Effect = (sender, receiver) =>
                {
                    // 减少关系
                    DiplomacyManager.Instance.ReduceRelation(sender, receiver, 150);
                    // 增加双方的军事紧张度
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{receiver.Name} 在边境集结军队，对 {sender.Name} 构成威胁，关系减少了150点！");
#endif
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 16,
                Name = "技术援助",
                Description = "对方提供技术援助，帮助你发展。",
                MinRelation = 600,
                MaxRelation = 1200,
                Probability = 5,
                Effect = (sender, receiver) =>
                {
                    // 增加关系
                    DiplomacyManager.Instance.AddRelation(sender, receiver, 80);
                    // 提供技术
                    if (receiver.Techniques.Count > 0)
                    {
                        int randomIndex = Random.Range(0, receiver.Techniques.Count);
                        Technique tech = receiver.Techniques[randomIndex];
                        if (tech != null && !sender.HasTechnique(tech.Id))
                        {
                            sender.AddTechnique(tech.Id);
#if SANGO_DEBUG
                            Sango.Log.Print($"@外交事件@{receiver.Name} 向 {sender.Name} 提供了技术援助，{sender.Name} 获得了技术 {tech.Name}！");
#endif
                        }
                    }
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 17,
                Name = "经济援助",
                Description = "对方提供经济援助，帮助你度过难关。",
                MinRelation = 500,
                MaxRelation = 1000,
                Probability = 4,
                Effect = (sender, receiver) =>
                {
                    // 增加关系
                    DiplomacyManager.Instance.AddRelation(sender, receiver, 100);
                    // 提供资金
                    if (sender.Governor?.BelongCity != null)
                    {
                        int aidAmount = Random.Range(1000, 3000);
                        sender.Governor.BelongCity.gold += aidAmount;
#if SANGO_DEBUG
                        Sango.Log.Print($"@外交事件@{receiver.Name} 向 {sender.Name} 提供了 {aidAmount} 金的经济援助，关系增加了100点！");
#endif
                    }
                }
            });

            _diplomacyEvents.Add(new DiplomacyEvent
            {
                Id = 18,
                Name = "间谍事件",
                Description = "发现对方在你的领土上进行间谍活动。",
                MinRelation = -600,
                MaxRelation = 100,
                Probability = 5,
                Effect = (sender, receiver) =>
                {
                    // 减少关系
                    DiplomacyManager.Instance.ReduceRelation(sender, receiver, 180);
#if SANGO_DEBUG
                    Sango.Log.Print($"@外交事件@{sender.Name} 发现 {receiver.Name} 在其领土上进行间谍活动，关系减少了180点！");
#endif
                }
            });
        }

        /// <summary>
        /// 检查并触发外交事件
        /// </summary>
        /// <param name="forceA">势力A</param>
        /// <param name="forceB">势力B</param>
        public void CheckAndTriggerEvents(Force forceA, Force forceB)
        {
            if (forceA == null || forceB == null || forceA == forceB)
                return;

            int relation = DiplomacyManager.Instance.GetRelation(forceA, forceB);

            foreach (DiplomacyEvent e in _diplomacyEvents)
            {
                // 检查关系是否符合条件
                if (relation >= e.MinRelation && relation <= e.MaxRelation)
                {
                    // 检查概率
                    if (GameRandom.Chance(e.Probability))
                    {
                        // 触发事件
                        e.Effect(forceA, forceB);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 为所有势力检查外交事件
        /// </summary>
        /// <param name="scenario">游戏场景</param>
        public void CheckEventsForAllForces(Scenario scenario)
        {
            // 遍历所有势力对
            for (int i = 0; i < scenario.forceSet.Count; i++)
            {
                Force forceA = scenario.forceSet[i];
                if (!forceA.IsAlive) continue;

                for (int j = i + 1; j < scenario.forceSet.Count; j++)
                {
                    Force forceB = scenario.forceSet[j];
                    if (!forceB.IsAlive) continue;

                    // 检查并触发事件
                    CheckAndTriggerEvents(forceA, forceB);
                }
            }
        }
    }

    /// <summary>
    /// 外交事件
    /// </summary>
    public class DiplomacyEvent
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 事件描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 最小关系要求
        /// </summary>
        public int MinRelation { get; set; }

        /// <summary>
        /// 最大关系要求
        /// </summary>
        public int MaxRelation { get; set; }

        /// <summary>
        /// 触发概率
        /// </summary>
        public int Probability { get; set; }

        /// <summary>
        /// 事件效果
        /// </summary>
        public System.Action<Force, Force> Effect { get; set; }
    }
}