using System.Collections.Generic;
using UnityEngine;
using Sango.Core.Player;
using System.IO;
using TKNewtonsoft.Json;
using Sango.Mod;

namespace Sango.Core
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
        /// 当前季度的事件触发次数
        /// </summary>
        private int _eventTriggerCount;

        /// <summary>
        /// 当前季度标识符
        /// </summary>
        private int _currentQuarter;

        /// <summary>
        /// 初始化外交事件管理器
        /// </summary>
        public void Init()
        {
            _diplomacyEvents = new List<DiplomacyEvent>();
            // 初始化事件计数器
            _eventTriggerCount = 0;
            // 加载外交事件
            LoadDiplomacyEvents();

            //string mainPath = $"{Path.ContentRootPath}/Data/DiplomacyEvent";
            //GenerateDefaultDiplomacyEvents(mainPath);

            // 注册季节更新事件监听
            GameEvent.OnSeasonUpdate += OnSeasonUpdate;
            GameEvent.OnForceTurnStart += OnForceTurnStart;
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
                    // 添加玩家消息
                    string message = $"{receiver.ColorName}派遣使者访问{sender.ColorName}，带来了友好的问候，关系增加了50点！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                    // 添加玩家消息
                    string message = $"{sender.ColorName}与{receiver.ColorName}在边境发生了冲突，关系减少了100点！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                    bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.Trade, sender, receiver);
                    // 添加玩家消息
                    if (success)
                    {
                        string message = $"{receiver.ColorName}向{sender.ColorName}提出了贸易合作的提议，双方达成了通商协议！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    }
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
                    bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.AllianceRequest, receiver, sender);
                    // 添加玩家消息
                    string message = $"{receiver.ColorName}邀请{sender.ColorName}结成同盟！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                    bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.TruceRequest, receiver, sender);
                    // 添加玩家消息
                    string message = $"{receiver.ColorName}请求与{sender.ColorName}停战！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                            // 添加玩家消息
                            string message = $"{sender.ColorName}与{receiver.ColorName}进行了技术交流，{sender.ColorName}获得了技术{tech.Name}！";
                            if (sender.Governor?.BelongCity != null)
                            {
                                PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                            }
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
                    bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.RequestTroops, receiver, sender, null, troopCount);
                    // 添加玩家消息
                    string message = $"{receiver.ColorName}请求{sender.ColorName}提供军事援助，请求兵力{troopCount}！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                    bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.Marriage, sender, receiver);
                    // 添加玩家消息
                    if (success)
                    {
                        string message = $"{sender.ColorName}与{receiver.ColorName}达成了和亲，关系得到了加强！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    }
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
                    // 添加玩家消息
                    string message = $"{sender.ColorName}与{receiver.ColorName}发生了领土争端，关系减少了150点！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                    // 添加玩家消息
                    string message = $"{sender.ColorName}与{receiver.ColorName}发现了共同的敌人，关系增加了100点！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                    // 添加玩家消息
                    string message = $"{sender.ColorName}与{receiver.ColorName}进行了文化交流，促进了相互了解，关系增加了80点！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
                    // 有一定概率获得技术
                    if (GameRandom.Chance(20) && receiver.Techniques.Count > 0)
                    {
                        int randomIndex = Random.Range(0, receiver.Techniques.Count);
                        Technique tech = receiver.Techniques[randomIndex];
                        if (tech != null && !sender.HasTechnique(tech.Id))
                        {
                            sender.AddTechnique(tech.Id);
                            // 添加玩家消息
                            string techMessage = $"{sender.ColorName}与{receiver.ColorName}进行了文化交流，{sender.ColorName}获得了技术{tech.Name}！";
                            if (sender.Governor?.BelongCity != null)
                            {
                                PlayerMessage.AddTextMessage(techMessage, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                            }
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
                    // 添加玩家消息
                    string message = $"{sender.ColorName}的使者被{receiver.ColorName}扣留，引发外交危机，关系减少了200点！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                    // 添加玩家消息
                    string message = $"{sender.ColorName}与{receiver.ColorName}共同举办了庆典，增进了友谊，关系增加了100点，双方各获得了1000金！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
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
                    // 添加玩家消息
                    string message = $"{sender.ColorName}与{receiver.ColorName}在边境开展了贸易活动，促进了经济发展，关系增加了60点，双方各获得了500金！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
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
                    // 添加玩家消息
                    string message = $"{receiver.ColorName}在边境集结军队，对{sender.ColorName}构成威胁，关系减少了150点！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                            // 添加玩家消息
                            string message = $"{receiver.ColorName}向{sender.ColorName}提供了技术援助，{sender.ColorName}获得了技术{tech.Name}，关系增加了80点！";
                            if (sender.Governor?.BelongCity != null)
                            {
                                PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                            }
#if SANGO_DEBUG
                            Sango.Log.Print($"@外交事件@{receiver.Name} 向 {sender.Name} 提供了技术援助，{sender.Name} 获得了技术 {tech.Name}！");
#endif
                        }
                    }
                    else
                    {
                        // 添加玩家消息
                        string message = $"{receiver.ColorName}向{sender.ColorName}提供了技术援助，关系增加了80点！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
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
                        // 添加玩家消息
                        string message = $"{receiver.ColorName}向{sender.ColorName}提供了{aidAmount}金的经济援助，帮助{sender.ColorName}度过难关，关系增加了100点！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
#if SANGO_DEBUG
                        Sango.Log.Print($"@外交事件@{receiver.Name} 向 {sender.Name} 提供了 {aidAmount} 金的经济援助，关系增加了100点！");
#endif
                    }
                    else
                    {
                        // 添加玩家消息
                        string message = $"{receiver.ColorName}向{sender.ColorName}提供了经济援助，关系增加了100点！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
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
                    // 添加玩家消息
                    string message = $"{sender.ColorName}发现{receiver.ColorName}在其领土上进行间谍活动，关系减少了180点！";
                    if (sender.Governor?.BelongCity != null)
                    {
                        PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                    }
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
                        // 增加事件计数器
                        _eventTriggerCount--;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 季节更新事件处理
        /// </summary>
        /// <param name="scenario">游戏场景</param>
        private void OnSeasonUpdate(Scenario scenario)
        {
            // 季节更新时重置事件计数器
            _eventTriggerCount = GameRandom.Range(0, 5);
        }

        /// <summary>
        /// 加载外交事件
        /// </summary>
        private void LoadDiplomacyEvents()
        {
            // 加载主游戏的外交事件
            string mainPath = $"{Path.ContentRootPath}/Data/DiplomacyEvent";
            LoadDiplomacyEventsFromPath(mainPath);

            // 加载激活的Mod的外交事件
            LoadModDiplomacyEvents();
        }

        /// <summary>
        /// 加载激活的Mod的外交事件
        /// </summary>
        private void LoadModDiplomacyEvents()
        {
            // 获取所有激活的Mod
            foreach (var mod in ModManager.Instance.GetEnabledMods())
            {
                string modPath = System.IO.Path.Combine(mod.ModDir, "Data", "DiplomacyEvent");
                LoadDiplomacyEventsFromPath(modPath);
            }
        }

        /// <summary>
        /// 从指定路径加载外交事件
        /// </summary>
        /// <param name="path">事件文件路径</param>
        private void LoadDiplomacyEventsFromPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            Directory.EnumFiles(path, "*.json", SearchOption.AllDirectories, (file) =>
            {
                try
                {
                    // 读取文件内容
                    string jsonContent = File.ReadAllText(file);
                    // 反序列化
                    DiplomacyEvent ev = JsonConvert.DeserializeObject<DiplomacyEvent>(jsonContent);
                    if (ev != null)
                    {
                        // 设置事件效果
                        SetEventEffect(ev);

                        _diplomacyEvents.RemoveAll(x => x.Id == ev.Id);

#if SANGO_DEBUG
                        Sango.Log.Print($"加载外交事件文件: {file}, id: {ev.Id}, name: {ev.Name}");
#endif

                        // 添加到事件列表
                        _diplomacyEvents.Add(ev);
                    }
                }
                catch (System.Exception e)
                {
                    Sango.Log.Error($"加载外交事件文件失败: {file}, 错误: {e.Message}");
                }
            });
        }

        /// <summary>
        /// 生成默认外交事件
        /// </summary>
        /// <param name="path">保存路径</param>
        private void GenerateDefaultDiplomacyEvents(string path)
        {
            // 创建默认事件列表
            List<DiplomacyEvent> defaultEvents = new List<DiplomacyEvent>();

            // 使者来访
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 1,
                Name = "使者来访",
                Description = "对方派遣使者来访，带来了友好的问候。",
                MinRelation = 0,
                MaxRelation = 500,
                Probability = 5,
                EffectType = "AddRelation",
                EffectParams = new Dictionary<string, object> { { "Value", 50 } }
            });

            // 边境冲突
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 2,
                Name = "边境冲突",
                Description = "双方在边境发生了小规模冲突。",
                MinRelation = -500,
                MaxRelation = 0,
                Probability = 8,
                EffectType = "ReduceRelation",
                EffectParams = new Dictionary<string, object> { { "Value", 100 } }
            });

            // 贸易提议
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 3,
                Name = "贸易提议",
                Description = "对方提出了贸易合作的提议。",
                MinRelation = 300,
                MaxRelation = 800,
                Probability = 10,
                EffectType = "Trade",
                EffectParams = new Dictionary<string, object>()
            });

            // 结盟邀请
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 4,
                Name = "结盟邀请",
                Description = "对方邀请你结成同盟。",
                MinRelation = 800,
                MaxRelation = 1500,
                Probability = 5,
                EffectType = "AllianceRequest",
                EffectParams = new Dictionary<string, object>()
            });

            // 停战请求
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 5,
                Name = "停战请求",
                Description = "对方请求停战。",
                MinRelation = -800,
                MaxRelation = -300,
                Probability = 7,
                EffectType = "TruceRequest",
                EffectParams = new Dictionary<string, object>()
            });

            // 技术交流
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 6,
                Name = "技术交流",
                Description = "对方提议进行技术交流。",
                MinRelation = 500,
                MaxRelation = 1000,
                Probability = 6,
                EffectType = "TechniqueExchange",
                EffectParams = new Dictionary<string, object>()
            });

            // 军事援助请求
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 7,
                Name = "军事援助请求",
                Description = "对方请求军事援助。",
                MinRelation = 600,
                MaxRelation = 900,
                Probability = 4,
                EffectType = "RequestTroops",
                EffectParams = new Dictionary<string, object> { { "MinTroops", 1000 }, { "MaxTroops", 5000 } }
            });

            // 和亲提议
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 8,
                Name = "和亲提议",
                Description = "对方提议通过和亲来加强关系。",
                MinRelation = 700,
                MaxRelation = 1200,
                Probability = 5,
                EffectType = "Marriage",
                EffectParams = new Dictionary<string, object>()
            });

            // 领土争端
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 9,
                Name = "领土争端",
                Description = "双方因领土问题产生了争端。",
                MinRelation = -800,
                MaxRelation = -400,
                Probability = 6,
                EffectType = "ReduceRelation",
                EffectParams = new Dictionary<string, object> { { "Value", 150 } }
            });

            // 共同敌人
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 10,
                Name = "共同敌人",
                Description = "双方发现了共同的敌人。",
                MinRelation = -300,
                MaxRelation = 300,
                Probability = 7,
                EffectType = "AddRelation",
                EffectParams = new Dictionary<string, object> { { "Value", 100 } }
            });

            // 文化交流
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 11,
                Name = "文化交流",
                Description = "对方提议进行文化交流，促进相互了解。",
                MinRelation = 200,
                MaxRelation = 700,
                Probability = 6,
                EffectType = "CulturalExchange",
                EffectParams = new Dictionary<string, object> { { "RelationValue", 80 } }
            });

            // 使者被扣留
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 12,
                Name = "使者被扣留",
                Description = "你的使者被对方扣留，引发外交危机。",
                MinRelation = -700,
                MaxRelation = -200,
                Probability = 5,
                EffectType = "ReduceRelation",
                EffectParams = new Dictionary<string, object> { { "Value", 200 } }
            });

            // 共同庆典
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 13,
                Name = "共同庆典",
                Description = "双方共同举办庆典，增进友谊。",
                MinRelation = 400,
                MaxRelation = 900,
                Probability = 4,
                EffectType = "CommonCelebration",
                EffectParams = new Dictionary<string, object> { { "RelationValue", 100 }, { "GoldValue", 1000 } }
            });

            // 边境贸易
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 14,
                Name = "边境贸易",
                Description = "双方在边境开展贸易活动，促进经济发展。",
                MinRelation = -100,
                MaxRelation = 600,
                Probability = 8,
                EffectType = "BorderTrade",
                EffectParams = new Dictionary<string, object> { { "RelationValue", 60 }, { "GoldValue", 500 } }
            });

            // 军事威胁
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 15,
                Name = "军事威胁",
                Description = "对方在边境集结军队，对你构成威胁。",
                MinRelation = -900,
                MaxRelation = -300,
                Probability = 6,
                EffectType = "ReduceRelation",
                EffectParams = new Dictionary<string, object> { { "Value", 150 } }
            });

            // 技术援助
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 16,
                Name = "技术援助",
                Description = "对方提供技术援助，帮助你发展。",
                MinRelation = 600,
                MaxRelation = 1200,
                Probability = 5,
                EffectType = "TechnicalAssistance",
                EffectParams = new Dictionary<string, object> { { "RelationValue", 80 } }
            });

            // 经济援助
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 17,
                Name = "经济援助",
                Description = "对方提供经济援助，帮助你度过难关。",
                MinRelation = 500,
                MaxRelation = 1000,
                Probability = 4,
                EffectType = "EconomicAssistance",
                EffectParams = new Dictionary<string, object> { { "RelationValue", 100 }, { "MinGold", 1000 }, { "MaxGold", 3000 } }
            });

            // 间谍事件
            defaultEvents.Add(new DiplomacyEvent
            {
                Id = 18,
                Name = "间谍事件",
                Description = "发现对方在你的领土上进行间谍活动。",
                MinRelation = -600,
                MaxRelation = 100,
                Probability = 5,
                EffectType = "ReduceRelation",
                EffectParams = new Dictionary<string, object> { { "Value", 180 } }
            });

            // 添加到事件列表
            foreach (DiplomacyEvent ev in defaultEvents)
            {
                // 保存为Json文件
                string jsonContent = JsonConvert.SerializeObject(ev, Formatting.Indented);
                string filePath = System.IO.Path.Combine(path, $"{ev.Id}_{ev.Name}.json");
                File.WriteAllText(filePath, jsonContent);
                //SetEventEffect(ev);
                //_diplomacyEvents.Add(ev);
            }
        }

        /// <summary>
        /// 设置事件效果
        /// </summary>
        /// <param name="ev">外交事件</param>
        private void SetEventEffect(DiplomacyEvent ev)
        {
            switch (ev.EffectType)
            {
                case "AddRelation":
                    ev.Effect = (sender, receiver) =>
                    {
                        int value = 50;
                        if (ev.EffectParams != null && ev.EffectParams.TryGetValue("Value", out object valueObj))
                        {
                            value = System.Convert.ToInt32(valueObj);
                        }
                        DiplomacyManager.Instance.AddRelation(sender, receiver, value);
                        string message = $"{receiver.ColorName}派遣使者访问{sender.ColorName}，带来了友好的问候，关系增加了{value}点！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    };
                    break;

                case "ReduceRelation":
                    ev.Effect = (sender, receiver) =>
                    {
                        int value = 100;
                        if (ev.EffectParams != null && ev.EffectParams.TryGetValue("Value", out object valueObj))
                        {
                            value = System.Convert.ToInt32(valueObj);
                        }
                        DiplomacyManager.Instance.ReduceRelation(sender, receiver, value);
                        string message = $"{sender.ColorName}与{receiver.ColorName}在边境发生了冲突，关系减少了{value}点！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    };
                    break;

                case "Trade":
                    ev.Effect = (sender, receiver) =>
                    {
                        bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.Trade, sender, receiver);
                        if (success)
                        {
                            string message = $"{receiver.ColorName}向{sender.ColorName}提出了贸易合作的提议，双方达成了通商协议！";
                            if (sender.Governor?.BelongCity != null)
                            {
                                PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                            }
                        }
                    };
                    break;

                case "AllianceRequest":
                    ev.Effect = (sender, receiver) =>
                    {
                        bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.AllianceRequest, receiver, sender);
                        string message = $"{receiver.ColorName}邀请{sender.ColorName}结成同盟！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    };
                    break;

                case "TruceRequest":
                    ev.Effect = (sender, receiver) =>
                    {
                        bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.TruceRequest, receiver, sender);
                        string message = $"{receiver.ColorName}请求与{sender.ColorName}停战！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    };
                    break;

                case "TechniqueExchange":
                    ev.Effect = (sender, receiver) =>
                    {
                        if (receiver.Techniques.Count > 0)
                        {
                            int randomIndex = Random.Range(0, receiver.Techniques.Count);
                            Technique tech = receiver.Techniques[randomIndex];
                            if (tech != null && !sender.HasTechnique(tech.Id))
                            {
                                sender.AddTechnique(tech.Id);
                                DiplomacyManager.Instance.ReduceRelation(sender, receiver, 100);
                                string message = $"{sender.ColorName}与{receiver.ColorName}进行了技术交流，{sender.ColorName}获得了技术{tech.Name}！";
                                if (sender.Governor?.BelongCity != null)
                                {
                                    PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                                }
                            }
                        }
                    };
                    break;

                case "RequestTroops":
                    ev.Effect = (sender, receiver) =>
                    {
                        int minTroops = 1000;
                        int maxTroops = 5000;
                        if (ev.EffectParams != null)
                        {
                            if (ev.EffectParams.TryGetValue("MinTroops", out object minObj))
                                minTroops = System.Convert.ToInt32(minObj);
                            if (ev.EffectParams.TryGetValue("MaxTroops", out object maxObj))
                                maxTroops = System.Convert.ToInt32(maxObj);
                        }
                        int troopCount = Random.Range(minTroops, maxTroops);
                        bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.RequestTroops, receiver, sender, null, troopCount);
                        string message = $"{receiver.ColorName}请求{sender.ColorName}提供军事援助，请求兵力{troopCount}！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    };
                    break;

                case "Marriage":
                    ev.Effect = (sender, receiver) =>
                    {
                        bool success = DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.Marriage, sender, receiver);
                        if (success)
                        {
                            string message = $"{sender.ColorName}与{receiver.ColorName}达成了和亲，关系得到了加强！";
                            if (sender.Governor?.BelongCity != null)
                            {
                                PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                            }
                        }
                    };
                    break;

                case "CulturalExchange":
                    ev.Effect = (sender, receiver) =>
                    {
                        int relationValue = 80;
                        if (ev.EffectParams != null && ev.EffectParams.TryGetValue("RelationValue", out object valueObj))
                        {
                            relationValue = System.Convert.ToInt32(valueObj);
                        }
                        DiplomacyManager.Instance.AddRelation(sender, receiver, relationValue);
                        string message = $"{sender.ColorName}与{receiver.ColorName}进行了文化交流，促进了相互了解，关系增加了{relationValue}点！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                        if (GameRandom.Chance(20) && receiver.Techniques.Count > 0)
                        {
                            int randomIndex = Random.Range(0, receiver.Techniques.Count);
                            Technique tech = receiver.Techniques[randomIndex];
                            if (tech != null && !sender.HasTechnique(tech.Id))
                            {
                                sender.AddTechnique(tech.Id);
                                string techMessage = $"{sender.ColorName}与{receiver.ColorName}进行了文化交流，{sender.ColorName}获得了技术{tech.Name}！";
                                if (sender.Governor?.BelongCity != null)
                                {
                                    PlayerMessage.AddTextMessage(techMessage, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                                }
                            }
                        }
                    };
                    break;

                case "CommonCelebration":
                    ev.Effect = (sender, receiver) =>
                    {
                        int relationValue = 100;
                        int goldValue = 1000;
                        if (ev.EffectParams != null)
                        {
                            if (ev.EffectParams.TryGetValue("RelationValue", out object relObj))
                                relationValue = System.Convert.ToInt32(relObj);
                            if (ev.EffectParams.TryGetValue("GoldValue", out object goldObj))
                                goldValue = System.Convert.ToInt32(goldObj);
                        }
                        DiplomacyManager.Instance.AddRelation(sender, receiver, relationValue);
                        if (sender.Governor?.BelongCity != null)
                        {
                            sender.Governor.BelongCity.gold += goldValue;
                        }
                        if (receiver.Governor?.BelongCity != null)
                        {
                            receiver.Governor.BelongCity.gold += goldValue;
                        }
                        string message = $"{sender.ColorName}与{receiver.ColorName}共同举办了庆典，增进了友谊，关系增加了{relationValue}点，双方各获得了{goldValue}金！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    };
                    break;

                case "BorderTrade":
                    ev.Effect = (sender, receiver) =>
                    {
                        int relationValue = 60;
                        int goldValue = 500;
                        if (ev.EffectParams != null)
                        {
                            if (ev.EffectParams.TryGetValue("RelationValue", out object relObj))
                                relationValue = System.Convert.ToInt32(relObj);
                            if (ev.EffectParams.TryGetValue("GoldValue", out object goldObj))
                                goldValue = System.Convert.ToInt32(goldObj);
                        }
                        DiplomacyManager.Instance.AddRelation(sender, receiver, relationValue);
                        if (sender.Governor?.BelongCity != null)
                        {
                            sender.Governor.BelongCity.gold += goldValue;
                        }
                        if (receiver.Governor?.BelongCity != null)
                        {
                            receiver.Governor.BelongCity.gold += goldValue;
                        }
                        string message = $"{sender.ColorName}与{receiver.ColorName}在边境开展了贸易活动，促进了经济发展，关系增加了{relationValue}点，双方各获得了{goldValue}金！";
                        if (sender.Governor?.BelongCity != null)
                        {
                            PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                        }
                    };
                    break;

                case "TechnicalAssistance":
                    ev.Effect = (sender, receiver) =>
                    {
                        int relationValue = 80;
                        if (ev.EffectParams != null && ev.EffectParams.TryGetValue("RelationValue", out object valueObj))
                        {
                            relationValue = System.Convert.ToInt32(valueObj);
                        }
                        DiplomacyManager.Instance.AddRelation(sender, receiver, relationValue);
                        if (receiver.Techniques.Count > 0)
                        {
                            int randomIndex = Random.Range(0, receiver.Techniques.Count);
                            Technique tech = receiver.Techniques[randomIndex];
                            if (tech != null && !sender.HasTechnique(tech.Id))
                            {
                                sender.AddTechnique(tech.Id);
                                string message = $"{receiver.ColorName}向{sender.ColorName}提供了技术援助，{sender.ColorName}获得了技术{tech.Name}，关系增加了{relationValue}点！";
                                if (sender.Governor?.BelongCity != null)
                                {
                                    PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                                }
                            }
                        }
                        else
                        {
                            string message = $"{receiver.ColorName}向{sender.ColorName}提供了技术援助，关系增加了{relationValue}点！";
                            if (sender.Governor?.BelongCity != null)
                            {
                                PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                            }
                        }
                    };
                    break;

                case "EconomicAssistance":
                    ev.Effect = (sender, receiver) =>
                    {
                        int relationValue = 100;
                        int minGold = 1000;
                        int maxGold = 3000;
                        if (ev.EffectParams != null)
                        {
                            if (ev.EffectParams.TryGetValue("RelationValue", out object relObj))
                                relationValue = System.Convert.ToInt32(relObj);
                            if (ev.EffectParams.TryGetValue("MinGold", out object minObj))
                                minGold = System.Convert.ToInt32(minObj);
                            if (ev.EffectParams.TryGetValue("MaxGold", out object maxObj))
                                maxGold = System.Convert.ToInt32(maxObj);
                        }
                        DiplomacyManager.Instance.AddRelation(sender, receiver, relationValue);
                        if (sender.Governor?.BelongCity != null)
                        {
                            int aidAmount = Random.Range(minGold, maxGold);
                            sender.Governor.BelongCity.gold += aidAmount;
                            string message = $"{receiver.ColorName}向{sender.ColorName}提供了{aidAmount}金的经济援助，帮助{sender.ColorName}度过难关，关系增加了{relationValue}点！";
                            if (sender.Governor?.BelongCity != null)
                            {
                                PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                            }
                        }
                        else
                        {
                            string message = $"{receiver.ColorName}向{sender.ColorName}提供了经济援助，关系增加了{relationValue}点！";
                            if (sender.Governor?.BelongCity != null)
                            {
                                PlayerMessage.AddTextMessage(message, sender, sender.Governor.BelongCity.x, sender.Governor.BelongCity.y);
                            }
                        }
                    };
                    break;

                default:
                    ev.Effect = (sender, receiver) => { };
                    break;
            }
        }

        /// <summary>
        /// 在势力回合开始时检查外交事件
        /// </summary>
        /// <param name="force"></param>
        /// <param name="scenario"></param>
        private void OnForceTurnStart(Force force, Scenario scenario)
        {
            CheckEventsForForces(force, scenario);
        }

        /// <summary>
        /// 为所有势力检查外交事件
        /// </summary>
        /// <param name="scenario">游戏场景</param>
        public void CheckEventsForForces(Force force, Scenario scenario)
        {
            // 检查是否已经达到本季度事件触发上限
            if (_eventTriggerCount <= 0)
                return;

            Force forceA = force;
            if (forceA == null || !forceA.IsAlive) return;

            // 遍历所有势力对
            int forceCount = scenario.forceSet.Count;
            for (int i = 0; i < forceCount; i++)
            {
                Force forceB = scenario.forceSet[i];
                if (forceB == null || !forceB.IsAlive || forceB.IsPlayer)
                    continue;

                // 检查并触发事件
                CheckAndTriggerEvents(forceA, forceB);

                // 检查是否已经达到本季度事件触发上限
                if (_eventTriggerCount <= 0)
                    return;
            }
        }
    }

    /// <summary>
    /// 外交事件
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DiplomacyEvent
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        [JsonProperty]
        public int Id { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// 事件描述
        /// </summary>
        [JsonProperty]
        public string Description { get; set; }

        /// <summary>
        /// 最小关系要求
        /// </summary>
        [JsonProperty]
        public int MinRelation { get; set; }

        /// <summary>
        /// 最大关系要求
        /// </summary>
        [JsonProperty]
        public int MaxRelation { get; set; }

        /// <summary>
        /// 触发概率
        /// </summary>
        [JsonProperty]
        public int Probability { get; set; }

        /// <summary>
        /// 事件效果类型
        /// </summary>
        [JsonProperty]
        public string EffectType { get; set; }

        /// <summary>
        /// 事件效果参数
        /// </summary>
        [JsonProperty]
        public Dictionary<string, object> EffectParams { get; set; }

        /// <summary>
        /// 事件效果
        /// </summary>
        [JsonIgnore]
        public System.Action<Force, Force> Effect { get; set; }
    }
}