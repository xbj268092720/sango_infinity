using System.Collections.Generic;
using System.IO;
using TKNewtonsoft.Json;
using TKNewtonsoft.Json.Linq;
using TKNewtonsoft.Json.Serialization;
using Sango.Game.Action;
using UnityEngine;
using System.Linq;

namespace Sango.Game
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Force : SangoObject
    {
        public override SangoObjectType ObjectType { get { return SangoObjectType.Force; } }
        public virtual bool AIFinished { get; set; }
        public virtual bool AIPrepared { get; set; }
        public virtual bool IsPlayer { get; set; }

        public override string Name => Governor?.Name;
        public virtual string ColorName => Governor?.ColorName;

        /// <summary>
        /// 主公
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Person>))]
        [JsonProperty]
        public Person Governor;

        /// <summary>
        /// 军师
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Person>))]
        [JsonProperty]
        public Person Counsellor;

        /// <summary>
        /// 旗帜
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Flag>))]
        [JsonProperty]
        public Flag Flag { get; set; }

        /// <summary>
        /// 联盟信息
        /// </summary>
        [JsonConverter(typeof(SangoObjectListIDConverter<Alliance>))]
        [JsonProperty]
        public SangoObjectList<Alliance> AllianceList = new SangoObjectList<Alliance>();

        /// <summary>
        /// 势力独有的初始化科技信息,以此为入口,且可以为势力设置独特的科技树
        /// </summary>
        [JsonConverter(typeof(SangoObjectListIDConverter<Technique>))]
        [JsonProperty]
        public SangoObjectList<Technique> InitTechniques = new SangoObjectList<Technique>();

        /// <summary>
        /// 已完成的科技信息
        /// </summary>
        [JsonConverter(typeof(SangoObjectListIDConverter<Technique>))]
        [JsonProperty]
        public SangoObjectList<Technique> Techniques = new SangoObjectList<Technique>();


        /// <summary>
        /// 本国被俘虏
        /// </summary>
        public List<Person> CaptiveList = new List<Person>();

        /// <summary>
        /// 技巧点数
        /// </summary>
        [JsonProperty] public int TechniquePoint { get; private set; }

        /// <summary>
        /// 霸业点数
        /// </summary>
        [JsonProperty] public int HegemonyPoint { get; set; }

        /// <summary>
        /// 势力方针
        /// </summary>
        [JsonProperty] public int PolicyType { get; set; }

        /// <summary>
        /// 国库
        /// </summary>
        public ItemStore Stroe = new ItemStore();

        /// <summary>
        /// 当前研究的技术
        /// </summary>
        [JsonProperty]
        public int ResearchTechnique { get; set; }

        /// <summary>
        /// 当前研究的技术剩余回合
        /// </summary>
        [JsonProperty]
        public int ResearchLeftCounter { get; set; }

        /// <summary>
        /// AI指令集
        /// </summary>
        public List<System.Func<Force, Scenario, bool>> AICommandList = new List<System.Func<Force, Scenario, bool>>();

        /// <summary>
        /// 相邻势力
        /// </summary>
        public List<Force> NeighborForceList = new List<Force>();

        /// <summary>
        /// 国力值
        /// </summary>
        public int FightPower;

        /// <summary>
        /// 执行建筑行为的建筑列表(建筑攻击等)
        /// </summary>
        Queue<BuildingBase> buildingBaseList = new Queue<BuildingBase>();

        /// <summary>
        /// 当前可以研发的科技列表
        /// </summary>
        public List<Technique> canResearchTechniqueList = new List<Technique>();

        /// <summary>
        /// 国家科技树
        /// </summary>
        public List<ForceTechnique> techniqueTree = new List<ForceTechnique>();
        public int techniqueMaxLevel = 0;
        public int techniqueMaxRow = 0;
        public List<ItemType> createdItemTypes = new List<ItemType>();

        public int PersonCount { get; set; }
        public int CityCount { get; set; }
        public int CityBaseCount { get; set; }
        public Color Color => Flag.color;

        public Corps CurRunCorps { get; set; }

        public List<ActionBase> actionList;

        public override void Init(Scenario scenario)
        {
            actionList = new List<ActionBase>();
            Techniques.ForEach(x =>
            {
                x.InitActions(actionList, this);
            });

            prepareTechniqueList(scenario);
            UpdateValidCreatedItemTypes();
        }

        void prepareTechniqueList(Scenario scenario)
        {
            techniqueMaxLevel = 0;

            // 在这里初始化科技树
            techniqueTree.Clear();
            InitTechniques.ForEach(x =>
            {
                ForceTechnique forceTechnique = new ForceTechnique()
                {
                    technique = x,
                    y = 0,
                };
                techniqueMaxLevel = Mathf.Max(techniqueMaxLevel, forceTechnique.FillChildren(scenario.CommonData.Techniques));
                techniqueTree.Add(forceTechnique);
            });
            techniqueTree.Sort((a, b) => a.technique.Id.CompareTo(b.technique.Id));
            techniqueMaxLevel++;

            for (int i = 0; i < techniqueTree.Count; i++)
            {
                int upY = 0, downY = 0;
                techniqueTree[i].UpdateY(ref upY, ref downY);

            }

            int startY = 0;
            for (int i = 0; i < techniqueTree.Count; i++)
            {
                ForceTechnique forceTechnique = techniqueTree[i];
                int minY = 0, maxY = 0;
                forceTechnique.GetMinMaxY(ref minY, ref maxY);
                int total = maxY - minY;
                forceTechnique.y = startY - minY;
                startY += total + 1;
            }
            techniqueMaxRow = startY + 1;

            // 初始化可研究的科技
            canResearchTechniqueList.Clear();
            scenario.CommonData.Techniques.ForEach(x =>
            {
                if (x.Id != ResearchTechnique)
                {
                    if (x.CanResearch(this))
                    {
                        canResearchTechniqueList.Add(x);
                    }
                }
            });
        }

        public bool IsAlliance(Force other)
        {
            for (int i = 0; i < AllianceList.Count; ++i)
            {
                Alliance alliance = AllianceList[i];
                if (alliance.Contains(other))
                    return true;
            }
            return false;
        }

        public Alliance CheckAlliance(Force other)
        {
            for (int i = 0; i < AllianceList.Count; ++i)
            {
                Alliance alliance = AllianceList[i];
                if (alliance.Contains(other))
                    return alliance;
            }
            return null;
        }

        //public Corps Add(Corps corps)
        //{
        //    allCorps.Add(Scenario.Cur.Add(corps));
        //    return corps;
        //}
        //public City Add(City city)
        //{
        //    allCities.Add(Scenario.Cur.Add(city));
        //    return city;
        //}
        //public Person Add(Person person)
        //{
        //    allPersons.Add(Scenario.Cur.Add(person));
        //    return person;
        //}
        //public Troop Add(Troop troops)
        //{
        //    allTroops.Add(Scenario.Cur.Add(troops));
        //    return troops;
        //}
        //public Building Add(Building building)
        //{
        //    allBuildings.Add(Scenario.Cur.Add(building));
        //    return building;
        //}
        //public Corps Remove(Corps corps)
        //{
        //    allCorps.Remove(Scenario.Cur.Remove(corps));
        //    return corps;
        //}
        //public City Remove(City city)
        //{
        //    allCities.Remove(Scenario.Cur.Remove(city));
        //    return city;
        //}
        //public Person Remove(Person person)
        //{
        //    allPersons.Remove(Scenario.Cur.Remove(person));
        //    return person;
        //}
        //public Troop Remove(Troop troops)
        //{
        //    allTroops.Remove(Scenario.Cur.Remove(troops));
        //    return troops;
        //}
        //public Building Remove(Building building)
        //{
        //    allBuildings.Remove(Scenario.Cur.Remove(building));
        //    return building;
        //}

        public override bool Run(Scenario scenario)
        {
            if (ActionOver)
                return true;

            if (!DoBuildingBehaviour(scenario))
                return false;

            // 非玩家才执行AI
            if (!IsPlayer && !DoAI(scenario))
                return false;

            for (int i = 0; i < scenario.corpsSet.Count; ++i)
            {
                Corps corps = scenario.corpsSet[i];
                if (corps != null && corps.IsAlive && corps.BelongForce == this && !corps.ActionOver)
                {
                    CurRunCorps = corps;
                    if (!corps.Run(scenario))
                        return false;
                }
            }

            ActionOver = true;
            return true;
        }

        public override bool DoAI(Scenario scenario)
        {
            if (AIFinished)
                return true;

            if (!AIPrepared)
            {
                AIPrepare(scenario);
                GameEvent.OnForceAIStart?.Invoke(this, scenario);
                AIPrepared = true;
            }

            while (AICommandList.Count > 0)
            {
                System.Func<Force, Scenario, bool> CurrentCommand = AICommandList[0];
                if (!CurrentCommand.Invoke(this, scenario))
                    return false;

                AICommandList.RemoveAt(0);
            }

            GameEvent.OnForceAIEnd?.Invoke(this, scenario);
            AIFinished = true;
            return true;
        }


        public bool DoBuildingBehaviour(Scenario scenario)
        {
            if (buildingBaseList.Count <= 0)
                return true;

            while (buildingBaseList.Count > 0)
            {
                BuildingBase currentBuilding = buildingBaseList.Peek();
                if (!currentBuilding.DoBuildingBehaviour(scenario))
                    return false;

                buildingBaseList.Dequeue();
            }

            return true;
        }

        /// <summary>
        /// AI准备
        /// </summary>
        private void AIPrepare(Scenario scenario)
        {
            // 添加外交AI
            AICommandList.Add(ForceAI.AIDiplomacy);
            AICommandList.Add(ForceAI.AICaptives);
            AICommandList.Add(ForceAI.AITechniques);

            GameEvent.OnForceAIPrepare?.Invoke(this, scenario);
        }

        public override bool OnForceTurnStart(Scenario scenario)
        {
            buildingBaseList.Clear();
            AIFinished = false;
            AIPrepared = false;
            FightPower = 0;
            PersonCount = 0;
            CityCount = 0;
#if SANGO_DEBUG
            Sango.Log.Print($"==={Name} 回合===");
#endif
            prepareTechniqueList(scenario);

            for (int i = 0; i < scenario.buildingSet.Count; ++i)
            {
                var c = scenario.buildingSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    c.OnForceTurnStart(scenario);
                    buildingBaseList.Enqueue(c);
                }
            }

            for (int i = 0; i < scenario.personSet.Count; ++i)
            {
                var c = scenario.personSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    PersonCount++;
                    c.OnForceTurnStart(scenario);
                }
            }

            for (int i = 0; i < scenario.corpsSet.Count; ++i)
            {
                var c = scenario.corpsSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    c.OnForceTurnStart(scenario);
                }
            }

            bool hasNoCheckBorder = false;
            NeighborForceList.Clear();
            for (int i = 0; i < scenario.citySet.Count; ++i)
            {
                var c = scenario.citySet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {

                    c.OnForceTurnStart(scenario);
                    FightPower += c.FightPower;
                    buildingBaseList.Enqueue(c);
                    CityBaseCount++;

                    if (c.IsCity())
                    {
                        CityCount++;

                        c.borderLine = -1;
                        // 计算相邻势力
                        foreach (City neighbor in c.NeighborList)
                        {
                            if (!neighbor.IsSameForce(c))
                            {
                                c.borderLine = 0;
                                if (neighbor.BelongForce != null)
                                {
                                    if (!NeighborForceList.Contains(neighbor.BelongForce))
                                    {
                                        NeighborForceList.Add(neighbor.BelongForce);
                                    }
                                }
                            }
                        }
                        if (c.borderLine == -1)
                            hasNoCheckBorder = true;
                    }
                }
            }

            while (hasNoCheckBorder)
            {
                for (int i = 0; i < scenario.citySet.Count; ++i)
                {
                    var c = scenario.citySet[i];
                    if (c != null && c.IsAlive && c.BelongForce == this && c.borderLine < 0)
                    {
                        int minBorder = 99;
                        // 计算相邻势力
                        foreach (City neighbor in c.NeighborList)
                        {
                            if (neighbor.borderLine >= 0)
                                minBorder = Mathf.Min(minBorder, neighbor.borderLine);
                        }
                        if (minBorder >= 0)
                        {
                            c.borderLine = minBorder + 1;
                        }
                        hasNoCheckBorder = c.borderLine == -1;
                    }
                }
            }


            for (int i = 0; i < scenario.troopsSet.Count; ++i)
            {
                var c = scenario.troopsSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    c.OnForceTurnStart(scenario);
                }
            }
            UpdateValidCreatedItemTypes();

            return base.OnForceTurnStart(scenario);
        }

        public override bool OnForceTurnEnd(Scenario scenario)
        {
            for (int i = 0; i < scenario.personSet.Count; ++i)
            {
                var c = scenario.personSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    c.OnForceTurnEnd(scenario);
                }
            }

            for (int i = 0; i < scenario.corpsSet.Count; ++i)
            {
                var c = scenario.corpsSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    c.OnForceTurnEnd(scenario);
                }
            }

            for (int i = 0; i < scenario.citySet.Count; ++i)
            {
                var c = scenario.citySet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    c.OnForceTurnEnd(scenario);
                }
            }

            for (int i = 0; i < scenario.buildingSet.Count; ++i)
            {
                var c = scenario.buildingSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    c.OnForceTurnEnd(scenario);
                }
            }

            for (int i = 0; i < scenario.troopsSet.Count; ++i)
            {
                var c = scenario.troopsSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    c.OnForceTurnEnd(scenario);
                }
            }

            return base.OnForceTurnEnd(scenario);
        }

        public override bool OnMonthStart(Scenario scenario)
        {
            return base.OnMonthStart(scenario);
        }
        public override bool OnSeasonStart(Scenario scenario)
        {
            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(100);
            GameEvent.OnForcePersonLoyaltyChangeProbability?.Invoke(this, overrideData);
            if (GameRandom.Chance(overrideData.Value))
            {
                ForcePersonLoyaltyChange();
            }

            return base.OnSeasonStart(scenario);
        }


        static int[] loyaltyWeight = new int[5] { 1, 2, 3, 2, 1 };

        /// <summary>
        /// 势力武将换季掉忠计算
        /// </summary>
        public void ForcePersonLoyaltyChange()
        {
            // TODO: 武将换季掉忠
            int v = GameRandom.RandomWeightIndex(loyaltyWeight, 9);
            ForEachPerson(person =>
            {
                person.loyalty -= v;
#if SANGO_DEBUG
                Sango.Log.Print($"势力：{Name}, 武将：{person.Name}, 忠诚度下降: {v}, 现有忠诚度:{person.loyalty}");
#endif
            });
        }

        public void ForEachCityBase(System.Action<City> action)
        {
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.citySet.Count; ++i)
            {
                var c = scenario.citySet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    action(c);
                }
            }
        }

        public void ForEachCity(System.Action<City> action)
        {
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.citySet.Count; ++i)
            {
                var c = scenario.citySet[i];
                if (c != null && c.IsAlive && c.BelongForce == this && c.IsCity())
                {
                    action(c);
                }
            }
        }

        public void ForEachGate(System.Action<City> action)
        {
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.citySet.Count; ++i)
            {
                var c = scenario.citySet[i];
                if (c != null && c.IsAlive && c.BelongForce == this && c.IsGate())
                {
                    action(c);
                }
            }
        }

        public void ForEachPort(System.Action<City> action)
        {
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.citySet.Count; ++i)
            {
                var c = scenario.citySet[i];
                if (c != null && c.IsAlive && c.BelongForce == this && c.IsPort())
                {
                    action(c);
                }
            }
        }

        public void ForEachPerson(System.Action<Person> action)
        {
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.personSet.Count; ++i)
            {
                var c = scenario.personSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    action(c);
                }
            }
        }

        public void ForEachCorps(System.Action<Corps> action)
        {
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.corpsSet.Count; ++i)
            {
                var c = scenario.corpsSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    action(c);
                }
            }
        }

        public void ForEachBuilding(System.Action<Building> action)
        {
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.buildingSet.Count; ++i)
            {
                var c = scenario.buildingSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    action(c);
                }
            }
        }

        public void ForEachTroop(System.Action<Troop> action)
        {
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.troopsSet.Count; ++i)
            {
                var c = scenario.troopsSet[i];
                if (c != null && c.IsAlive && c.BelongForce == this)
                {
                    action(c);
                }
            }
        }


        public void GainTechniquePoint(int value)
        {
            TechniquePoint += value;
            GameEvent.OnForceGainTechniquePoint?.Invoke(this, value);
        }

        public void GainHegemonyPoint(int value)
        {
            HegemonyPoint += value;
            GameEvent.OnForceGainHegemonyPoint?.Invoke(this, value);
        }

        public bool HasTechnique(int techId)
        {
            return Techniques.Contains(techId);
        }

        public Technique AddTechnique(int techId)
        {
            Technique technique = Scenario.Cur.GetObject<Technique>(techId);
            if (technique == null) return null;
            Techniques.Add(technique);
            technique.InitActions(actionList, this);
            return technique;
        }

        public void ChangeCounsellor(Person dest)
        {
            Person old = Counsellor;
            Counsellor = dest;
            GameEvent.OnForceChangeCounsellor?.Invoke(this, old);
        }

        public void UpdateValidCreatedItemTypes()
        {
            createdItemTypes.Clear();
            Scenario scenario = Scenario.Cur;
            Dictionary<int, ItemType> itemMap = new Dictionary<int, ItemType>();
            scenario.CommonData.ItemTypes.ForEach(it =>
            {
                if (it.cost > 0 && it.IsValid(this))
                {
                    ItemType itemType;
                    if (itemMap.TryGetValue(it.storeKind, out itemType))
                    {
                        if (it.Id > itemType.Id)
                        {
                            itemMap[it.storeKind] = it;
                        }
                    }
                    else
                    {
                        itemMap[it.storeKind] = it;
                    }
                }
            });

            createdItemTypes.AddRange(itemMap.Values.ToArray());
            createdItemTypes.Sort(SangoObject.Compare);
        }
    }
}
