using TKNewtonsoft.Json;
using Sango.Game.Action;
using Sango.Game.Render;
using Sango.Game.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Sango.Game.Player;

namespace Sango.Game
{
    [JsonObject(MemberSerialization.OptIn)]
    public class City : BuildingBase
    {

        public virtual bool AIFinished { get; set; }
        public virtual bool AIPrepared { get; set; }
        public override SangoObjectType ObjectType { get { return SangoObjectType.City; } }
        public override string ColorName => $"<color=#93C86D>{Name}</color>";

        /// <summary>
        /// 粮食
        /// </summary>
        [JsonProperty] public int food;

        /// <summary>
        /// 金钱
        /// </summary>
        [JsonProperty] public int gold;

        /// <summary>
        /// 人口
        /// </summary>
        [JsonProperty] public int population;

        /// <summary>
        /// 兵役人口
        /// </summary>
        [JsonProperty] public int troopPopulation;

        /// <summary>
        /// 工作委任类型
        /// </summary>
        [JsonProperty] public int workingAppointType;

        /// <summary>
        /// 库存
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(ItemStoreConverter))]
        public ItemStore itemStore = new ItemStore();

        /// <summary>
        /// 商业值
        /// </summary>
        [JsonProperty] public int commerce;

        /// <summary>
        /// 农业值
        /// </summary>
        [JsonProperty] public int agriculture;

        /// <summary>
        /// 民心
        /// </summary>
        [JsonProperty] public byte popularSupport;

        /// <summary>
        /// 治安
        /// </summary>
        [JsonProperty] public int security;

        /// <summary>
        /// 战意
        /// </summary>
        [JsonProperty] public int energy;

        /// <summary>
        /// 士气
        /// </summary>
        [JsonProperty] public int morale;

        public int MaxMorale { get; set; }

        /// <summary>
        /// 是否有商人, 数字为兑换比例 0为没有商人
        /// </summary>
        [JsonProperty] public byte hasBusiness;

        /// <summary>
        /// 当前兵力
        /// </summary>
        [JsonProperty] public int troops;

        /// <summary>
        /// 当前伤兵
        /// </summary>
        [JsonProperty] public int woundedTroops;

        /// <summary>
        /// 可容纳兵力
        /// </summary>
        [JsonProperty] public int troopsLimit;
        public int TroopsLimit => troopsLimit + CityLevelType.troopsLimitAdd + troopsLimitAdd;
        int troopsLimitAdd;
        /// <summary>
        /// 仓库大小
        /// </summary>
        [JsonProperty] public int storeLimit;
        public int StoreLimit => storeLimit + CityLevelType.storeLimitAdd + storeLimitAdd;
        int storeLimitAdd;

        /// <summary>
        /// 金库大小
        /// </summary>
        [JsonProperty] public int goldLimit;
        public int GoldLimit => goldLimit + CityLevelType.goldLimitAdd + goldLimitAdd;
        int goldLimitAdd;

        /// <summary>
        /// 粮仓大小
        /// </summary>
        [JsonProperty] public int foodLimit;
        public int FoodLimit => foodLimit + CityLevelType.foodLimitAdd + foodLimitAdd;
        int foodLimitAdd;

        /// <summary>
        /// 城内建筑槽位
        /// </summary>
        //[JsonProperty] public int insideSlot;
        //public int InsideSlot => insideSlot + CityLevelType.insideSlotAdd;

        /// <summary>
        /// 城外建筑槽位
        /// </summary>
        //[JsonProperty] public int outsideSlot;
        //public int OutsideSlot => outsideSlot + CityLevelType.outsideSlotAdd;

        /// <summary>
        /// 村庄槽位
        /// </summary>
        //[JsonProperty] public int villageSlot;
        //public int VillageSlot => villageSlot + CityLevelType.villageSlotAdd;

        /// <summary>
        /// 基础金钱收入 基础收入 = 基础收入 * 当前商业值 / 最大商业值
        /// </summary>
        [JsonProperty] public int baseGainGold;
        public virtual int BaseGainGold => baseGainGold + CityLevelType.baseGainGoldAdd;

        /// <summary>
        /// 基础粮食收入 基础收入 = 基础粮食收入 * 当前农业值 / 最大农业值
        /// </summary>
        [JsonProperty] public int baseGainFood;
        public virtual int BaseGainFood => baseGainFood + CityLevelType.baseGainFoodAdd;

        /// <summary>
        /// 最大商业值
        /// </summary>
        [JsonProperty] public int commerceLimit;
        public int CommerceLimit => commerceLimit + CityLevelType.commerceLimitAdd;

        /// <summary>
        /// 最大农业值
        /// </summary>
        [JsonProperty] public int agricultureLimit;

        public int AgricultureLimit => agricultureLimit + CityLevelType.agricultureLimitAdd;
        /// <summary>
        /// 最大耐久
        /// </summary>
        [JsonProperty] public int durabilityLimit;
        int durabilityLimitAdd;

        public override int DurabilityLimit => durabilityLimit + CityLevelType.durabilityLimitAdd + durabilityLimitAdd;

        /// <summary>
        /// 是否,满兵
        /// </summary>
        public bool TroopsIsFull => troops + woundedTroops >= TroopsLimit;

        /// <summary>
        /// 太守
        /// </summary>
        //[JsonConverter(typeof(Id2ObjConverter<Person>))]
        //[JsonProperty]
        public Person Leader;

        /// <summary>
        /// 所属州
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<State>))]
        [JsonProperty]
        public State State;

        /// <summary>
        /// 相邻城市
        /// </summary>
        [JsonConverter(typeof(SangoObjectListIDConverter<City>))]
        [JsonProperty]
        public SangoObjectList<City> NeighborList = new SangoObjectList<City>();

        /// <summary>
        /// 城市等级数据
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<CityLevelType>))]
        [JsonProperty]
        public CityLevelType CityLevelType;

        /// <summary>
        /// 俘虏
        /// </summary>
        [JsonConverter(typeof(SangoObjectListIDConverter<Person>))]
        [JsonProperty]
        public SangoObjectList<Person> captiveList = new SangoObjectList<Person>();


        [JsonProperty]
        public Dictionary<int, int> jobCounter = new Dictionary<int, int>();

        //public List<Building> villageList = new List<Building>();
        public List<Port> portList = new List<Port>();
        public List<Gate> gateList = new List<Gate>();

        public int totalGainFood = 0;
        public int totalGainGold = 0;

        /// <summary>
        /// 额外的影响倍率(事件等)
        /// </summary>
        public float extraGainFoodFactor = 0;
        public float extraGainGoldFactor = 0;
        public float extraPopulationFactor = 1;


        public float population_increase_factor = 0;
        internal int borderLine;
        public bool IsBorderCity => borderLine == 0;
        public int BorderLine => borderLine;

        public List<Person> freePersons = new List<Person>();
        public List<Person> wildPersons = new List<Person>();
        public List<Person> invisiblePersons = new List<Person>();

        public int FreePersonCount => freePersons.Count;
        public int PersonHole { get; set; }

        //public int eventId;
        //public int specialtyId;
        //public int model_wall;
        //public int model_city;
        internal int virtualFightPower;
        internal bool isUpdatedFightPower;
        internal bool boderLineChecked = false;

        /// <summary>
        /// 所有武将
        /// </summary>
        public SangoObjectList<Person> allPersons = new SangoObjectList<Person>();

        /// <summary>
        /// 所有部队
        /// </summary>
        public List<Troop> allAttackTroops = new List<Troop>();
        public List<Troop> allTroops = new List<Troop>();

        /// <summary>
        /// 城池直辖范围cell
        /// </summary>
        public List<Cell> areaCellList = new List<Cell>();

        // 初始化地图的时候就添加完全了
        public void AddAreaCell(Cell cell)
        {
            areaCellList.Add(cell);
            if (BelongCity != null)
            {
                BelongCity.AddAreaCell(cell);
            }
        }

        /// <summary>
        /// 所有内城设施
        /// </summary>
        //public int[] innerSlot;

        public List<TroopType> activedTroopType = new List<TroopType>();
        int fightPower = 0;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<City, List<Cell>> raodToNeighborCache = new Dictionary<City, List<Cell>>();

        /// <summary>
        /// 所有设施
        /// </summary>
        public SangoObjectList<Building> allBuildings = new SangoObjectList<Building>();
        public Dictionary<int, int> buildingCountMap = new Dictionary<int, int>();


        //public SangoObjectList<Building> allIntriorBuildings = new SangoObjectList<Building>();
        //public Dictionary<int, int> buildingCountMap = new Dictionary<int, int>();

        /// <summary>
        /// AI指令集
        /// </summary> 
        public List<System.Func<City, Scenario, bool>> AICommandList = new List<System.Func<City, Scenario, bool>>();

        public List<Cell> defenceCellList = new List<Cell>();
        public List<Cell> interiorCellList = new List<Cell>();

        public int InteriorCellCount => interiorCellList.Count;

        public int AttackTroopsCount
        {
            get
            {
                //int troopsCount = 0;
                //SangoObjectSet<Troop> troopSet = Scenario.Cur.troopsSet;
                //for (int i = 0; i < troopSet.Count; i++)
                //{
                //    Troop troop = troopSet[i];
                //    if (troop != null && troop.IsAlive && troop.BelongCity == this)
                //        troopsCount++;
                //}
                //return troopsCount;
                return allAttackTroops.Count;
            }
        }

        public int AddJobCounter(int jobId)
        {
            if (jobCounter.TryGetValue(jobId, out var job))
            {
                job++;
                jobCounter[jobId] = job;
                return job;
            }
            else
            {
                jobCounter.Add(jobId, 1);
                return 1;
            }
        }

        public int GetJobCounter(int jobId)
        {
            if (jobCounter.TryGetValue(jobId, out var job))
            {
                return job;
            }
            else
            {
                return 0;
            }
        }

        public int AddGold(int v)
        {
            if (v == 0) return gold;
            gold += v;
            if (gold > GoldLimit)
                gold = GoldLimit;
            else if (gold < 0)
                gold = 0;
            return gold;
        }

        public int AddFood(int v)
        {
            if (v == 0) return food;
            food += v;
            if (food > FoodLimit)
                food = FoodLimit;
            else if (food < 0)
                food = 0;
            return food;
        }
        public int AddTroops(int v)
        {
            if (v == 0) return troops;
            troops += v;
            if (troops > TroopsLimit)
                troops = TroopsLimit;
            else if (troops < 0)
                troops = 0;
            return troops;
        }

        public int AddItem(int itemTypeId, int v)
        {
            return itemStore.Add(itemTypeId, v);
        }

        public int AddSecurity(int v)
        {
            security += v;
            if (security > 100)
                security = 100;
            else if (security < 0)
                security = 0;
            return security;
        }
        public int AddMorale(int v)
        {
            morale += v;
            if (morale > MaxMorale)
                morale = MaxMorale;
            else if (morale < 0)
                morale = 0;
            return morale;
        }

        public List<Cell> GetRoadToNeighbor(City city)
        {
            List<Cell> list;
            if (!raodToNeighborCache.TryGetValue(city, out list))
            {
                list = new List<Cell>();
                Scenario.Cur.Map.GetDirectPath(CenterCell, city.CenterCell, list);
                raodToNeighborCache.Add(city, list);
                city.raodToNeighborCache.Add(this, list);
            }
            return list;
        }

        public int FightPower => fightPower;
        public Person Add(Person person) { allPersons.Add(person); return person; }
        public Person Remove(Person person)
        {
            allPersons.Remove(person); freePersons.Remove(person);
            return person;
        }


        public void UpdateActiveTroopTypes()
        {
            activedTroopType.Clear();
            Scenario.Cur.CommonData.TroopTypes.ForEach(x =>
            {
                //if (x.activeCondition == null || (x.activeCondition != null && x.activeCondition.Check(null)))
                //    activedTroopType.Add(x);
            });
        }

        public override void OnScenarioPrepare(Scenario scenario)
        {
            //x *= 2;
            //y *= 2;

            base.OnScenarioPrepare(scenario);
            isComplate = true;

            //innerSlot = new int[InsideSlot];
            if (durability <= 0)
                durability = DurabilityLimit;

            // 地格占用
            OccupyCellList = new List<Cell>();
            scenario.Map.GetSpiral(x, y, BuildingType.radius, OccupyCellList);
            foreach (Cell cell in OccupyCellList)
                cell.building = this;
            CenterCell = OccupyCellList[0];

            // 效果范围
            effectCells = new System.Collections.Generic.List<Cell>();
            scenario.Map.GetDirectSpiral(CenterCell, BuildingType.radius + 1, BuildingType.radius + BuildingType.atkRange, effectCells);

            for (int i = 0; i < areaCellList.Count; i++)
            {
                Cell cell = areaCellList[i];
                if (cell.HasGridState(Sango.Render.MapGrid.GridState.Defence))
                    defenceCellList.Add(cell);

                if (cell.IsInterior)
                {
                    if (BelongCity != null)
                        BelongCity.interiorCellList.Add(cell);
                    else
                        interiorCellList.Add(cell);
                }
            }

            foreach (Person person in captiveList)
            {
                if (person.BelongForce != null)
                    person.BelongForce.CaptiveList.Add(person);
            }
        }

        public override void OnPrepareRender()
        {
            Render = new CityRender(this);
        }

        public override void Init(Scenario scenario)
        {
            base.Init(scenario);

            // 空闲人员判断
            freePersons.Clear();
            allPersons.ForEach(person =>
            {
                if (!person.ActionOver && person.IsFree)
                    freePersons.Add(person);
            });

            if (BelongForce != null)
            {
                BelongForce.CityBaseCount++;
                if (IsCity())
                    BelongForce.CityCount++;
            }

            if (IsPort())
                BelongCity.portList.Add((Port)this);
            else if (IsGate())
                BelongCity.gateList.Add((Gate)this);
            if (BuildingType.Id == 1)
            {
                UpdateActiveTroopTypes();
                UpdateFightPower();
            }

            for (int i = 0; i < interiorCellList.Count; i++)
            {
                Cell c = interiorCellList[i];
                c.CreateInteriorModel();
            }

            //计算最大士气
            CalculateMaxMorale();
            CalculateHarvest();
        }

        public void LeaveToWild()
        {
            Leader = null;
            BelongCorps = null;
            BelongForce = null;
            Render?.UpdateRender();
        }

        public void UpdateFightPower()
        {
            fightPower = 1000 + troops;
            fightPower += UnityEngine.Mathf.Min(fightPower, allPersons.Count * 5000);
            isUpdatedFightPower = true;
            virtualFightPower = FightPower;
            //ForeachNeighborCities(x =>
            //{
            //    if (x.IsSameForce(this))
            //    {
            //        if (!x.isUpdatedFightPower)
            //        {
            //            x.UpdateFightPower();
            //            x.isUpdatedFightPower = true;
            //        }
            //        virtualFightPower += x.FightPower / 3;
            //    }
            //});
        }

        public void CalculateHarvest()
        {
            GameEvent.OnCityCalculateHarvest?.Invoke(this);
        }

        public void ForeachNeighborCities(Action<City> action)
        {
            for (int i = 0; i < NeighborList.Count; i++)
            {
                City c = NeighborList[i];
                if (c == null) continue;
                action(c);
            }
        }


        static bool _IsBorderCity(City city)
        {
            if (city.NeighborList == null)
                return false;
            for (int i = 0; i < city.NeighborList.Count; i++)
            {
                City c = city.NeighborList[i];
                if (c == null) continue;
                if (!city.IsSameForce(c)) return true;
            }
            return false;
        }

        static int _CheckBorder(City city, int len)
        {
            if (!_IsBorderCity(city))
            {
                city.boderLineChecked = true;
                for (int i = 0; i < city.NeighborList.Count; i++)
                {
                    City c = city.NeighborList[i];
                    if (c == null) continue;
                    if (!c.boderLineChecked && city.IsSameForce(c)) return _CheckBorder(c, len + 1);
                }
            }
            else
                return len;

            return 0;
        }

        /// <summary>
        /// 季度粮食收入
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public override bool OnSeasonStart(Scenario scenario)
        {
            //if (BelongCorps == null)
            //    return true;

            //            int harvest = GameRandom.Random(totalGainFood, 0.05f);
            //            Render?.ShowInfo(harvest, (int)InfoType.Food);
            //            food += harvest;
            //#if SANGO_DEBUG
            //            Sango.Log.Print($"城市：{Name}, 收获粮食：{harvest}, 现有粮食: {food}");
            //#endif

            GameEvent.OnCitySeasonStart?.Invoke(this, scenario);


            return base.OnSeasonStart(scenario);
        }


        /// <summary>
        /// 月度金钱收入
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public override bool OnMonthStart(Scenario scenario)
        {
            int pop = 0;
            int troopPop = 0;
            if (scenario.Variables.populationEnable)
            {
                pop = GameRandom.Random((int)(population * population_increase_factor), 0.1f);
                population += pop;
                troopPop = (int)(pop * 0.3f);
                troopPopulation += troopPop;
            }

            if (BelongCorps == null)
                return true;

            if (Render != null)
                Render.UpdateRender();

            GameEvent.OnCityMonthStart?.Invoke(this, scenario);

            int cost = -GoldCost(scenario);
            if (cost < 0)
            {
                AddGold(cost);
                Render?.ShowInfo(cost, (int)InfoType.Gold);

#if SANGO_DEBUG
                Sango.Log.Print($"城市：{Name},  支出：{cost}, 现有资金: {gold}");
#endif
            }

            return base.OnMonthStart(scenario);
        }
        public override bool OnDayStart(Scenario scenario)
        {
            return base.OnDayStart(scenario);
        }

        public virtual void OnBuildingComplete(Building building, SangoObjectList<Person> builder)
        {
            this.CalculateHarvest();
        }

        public virtual void OnBuildingUpgradeComplete(Building building, SangoObjectList<Person> builder)
        {
            this.CalculateHarvest();
        }

        public void OnBuildingCreate(Building building)
        {
            allBuildings.Add(building);
            int buildingKind = building.BuildingType.kind;
            int count;
            if (buildingCountMap.TryGetValue(buildingKind, out count))
                buildingCountMap[buildingKind] = count + 1;
            else
                buildingCountMap.Add(buildingKind, 1);
        }

        public void OnBuildingDestroy(Building building)
        {
            allBuildings.Remove(building);
            this.CalculateHarvest();
            int buildingKind = building.BuildingType.kind;
            int count;
            if (buildingCountMap.TryGetValue(buildingKind, out count))
                buildingCountMap[buildingKind] = count - 1;
        }

        /// <summary>
        /// 获取一个未行动的建筑
        /// </summary>
        /// <param name="buildingKindType"></param>
        /// <returns></returns>
        public Building GetCommandBuilding(BuildingKindType buildingKindType)
        {
            Building rs = null;
            int maxLv = 0;
            for (int i = 0; i < allBuildings.Count; i++)
            {
                Building building = allBuildings[i];
                if (!building.ActionOver && building.BuildingType.kind == (int)buildingKindType)
                {
                    if (building.BuildingType.level > maxLv)
                    {
                        rs = building;
                        maxLv = building.BuildingType.level;
                    }
                }
            }
            return rs;
        }

        public override bool OnForceTurnStart(Scenario scenario)
        {
            jobCounter.Clear();
            AIPrepared = false;
            AIFinished = false;
            ActionOver = false;
            boderLineChecked = false;
            CalculateHarvest();
            UpdateFightPower();
            JobHealingTroop();

            // 空闲人员判断
            freePersons.Clear();
            allPersons.ForEach(person =>
            {
                if (!person.ActionOver && person.IsFree)
                    freePersons.Add(person);
            });

            // 粮食消耗
            CostFood(scenario);

            // 耐久自修复
            if (durability < DurabilityLimit)
            {
                durability += Leader?.BaseBuildAbility * 2 + 50 ?? 50;
                if (durability > DurabilityLimit)
                    durability = DurabilityLimit;
            }

            if (Render != null)
                Render.UpdateRender();

            // 俘虏羁押天数累积
            for (int i = captiveList.Count - 1; i >= 0; i--)
            {
                Person person = captiveList[i];
                person.missionCounter++;
            }

            GameEvent.OnCityTurnStart?.Invoke(this, scenario);

            return base.OnForceTurnStart(scenario);
        }
        public override bool OnForceTurnEnd(Scenario scenario)
        {
            CurActiveTroop = null;
            isUpdatedFightPower = false;

            // 计算俘虏越狱
            for (int i = captiveList.Count - 1; i >= 0; i--)
            {
                Person person = captiveList[i];
                if (GameRandom.Chance(GameFormula.Instance.PersonEscapeProbablility_InCity(person, this, scenario), 10000))
                {
                    person.Escape();
#if SANGO_DEBUG
                    Sango.Log.Print($"{person.Name}逃跑!");
#endif
                    GameEvent.OnPersonEscape?.Invoke(person, this);
                }
            }
            GameEvent.OnCityTurnEnd?.Invoke(this, scenario);

            if (Leader == null || Leader.BelongCity != this)
                UpdateNewLeader();

            return base.OnForceTurnEnd(scenario);
        }

        public int FoodCost(Scenario scenario)
        {
            int foodCost = 0;
            foodCost += (int)System.Math.Ceiling(scenario.Variables.baseFoodCostInCity * (troops + woundedTroops));
            return foodCost;
        }

        public void CostFood(Scenario scenario)
        {
            if (food > 0)
            {
                int foodCost = 0;
                foodCost += (int)System.Math.Ceiling(scenario.Variables.baseFoodCostInCity * (troops + woundedTroops));
                int needFood = foodCost - food;
                if (needFood > 0)
                {
                    float runawayTroops = ((float)needFood / (float)foodCost) * scenario.Variables.runawayWhenCityFoodNotEnough;
                    troops = (int)System.Math.Ceiling(troops * (1.0f - runawayTroops));
                    if (woundedTroops > 100)
                    {
                        woundedTroops = (int)System.Math.Ceiling(woundedTroops * (1.0f - runawayTroops));
                    }
                    else
                    {
                        woundedTroops = 0;
                    }
                    food = 0;
                }
                else
                    food -= foodCost;

            }
            else
            {
                food = 0;
                float runawayTroops = scenario.Variables.runawayWhenCityFoodNotEnough;
                troops = (int)System.Math.Ceiling(troops * (1.0f - runawayTroops));
                if (woundedTroops > 100)
                {
                    woundedTroops = (int)System.Math.Ceiling(woundedTroops * (1.0f - runawayTroops));
                }
                else
                {
                    woundedTroops = 0;
                }
            }
        }
        public int GoldCost(Scenario scenario)
        {
            int goldCost = 0;
            allPersons.ForEach(person =>
            {
                if (person.Official != null)
                {
                    goldCost += person.Official.cost;
                }
            });

            // 计算俘虏的消耗
            for (int i = 0; i < captiveList.Count; i++)
            {
                goldCost += 100;
            }

            return goldCost;
        }

        //public void CreateTroop(Troop troop)
        //{
        //    AddTroop(troop);

        //}

        //public void AddTroop(Troop troop)
        //{
        //    // 先加入剧本才能分配ID
        //    Add(troop);
        //    troop.Leader.BelongTroop = troop;
        //    for (int i = 0; i < troop.MemberList.Count; i++)
        //        troop.MemberList[i].BelongTroop = troop;

        //    troop.BelongCity = this;
        //    troop.cell = CenterCell;
        //    troop.cell.troop = troop;
        //    troop.x = troop.cell.x;
        //    troop.y = troop.cell.y;
        //}

        //public Person Add(Person person)
        //{
        //    allPersons.Add(person);
        //    if (BelongCorps == null)
        //    {
        //        Sango.Log.Error($"why {Name}->BelongCorps is null");
        //    }
        //    BelongCorps.Add(person);
        //    return person;
        //}
        //public Troop Add(Troop troops)
        //{
        //    allTroops.Add(BelongCorps.Add(troops));
        //    return troops;
        //}
        //public Building Add(Building building)
        //{
        //    allBuildings.Add(BelongCorps.Add(building));
        //    return building;
        //}
        //public Troop Add(Troop troop)
        //{
        //    this.TroopList.Add(troop);
        //    return troop;
        //}
        //public Person Remove(Person person)
        //{
        //    allPersons.Remove(person);
        //    BelongCorps.Remove(person);
        //    return person;
        //}
        //public Troop Remove(Troop troop)
        //{
        //    this.TroopList.Remove(troop);
        //    return troop;
        //}
        //public Troop Remove(Troop troops)
        //{
        //    allTroops.Remove(BelongCorps.Remove(troops));
        //    return troops;
        //}
        //public Building Remove(Building building)
        //{
        //    allBuildings.Remove(BelongCorps.Remove(building));
        //    return building;
        //}

        public override bool ChangeDurability(int num, SangoObject atk, bool showDamage = true)
        {
            bool rs = base.ChangeDurability(num, atk, showDamage);
            if (rs)
            {
                durability = 1500;
            }

            if (Render != null)
                Render.UpdateRender();

            return rs;
        }

        public City GetNearnestForceCity()
        {
            City checkCity = BelongCity;
            if (checkCity != null)
            {
                if (checkCity.BelongForce == BelongForce)
                    return checkCity;

                for (int i = 0; i < checkCity.NeighborList.Count; i++)
                {
                    City city = checkCity.NeighborList[i];
                    if (city.BelongForce == BelongForce)
                        return city;
                }
            }
            else
            {
                for (int i = 0; i < NeighborList.Count; i++)
                {
                    City city = NeighborList[i];
                    if (city.IsSameForce(this))
                        return city;
                }
            }

            City nearnest = null;
            int distance = 100000;
            if (BelongForce != null)
            {
                BelongForce.ForEachCity(city =>
                {
                    if (city != this)
                    {
                        int dis = Scenario.Cur.Map.Distance(city.CenterCell, this.CenterCell);
                        if (dis < distance)
                        {
                            distance = dis;
                            nearnest = city;
                        }
                    }
                });
            }
            return nearnest;
        }
        public Corps ChangeCorps(Corps other)
        {
            Corps last = null;
            if (BelongCorps != other)
            {
                last = BelongCorps;
                BelongCorps = other;
                if (BelongForce != other.BelongForce)
                {
                    BelongForce = other.BelongForce;
                }
                Render?.UpdateRender();
            }
            return last;
        }

        public bool ChangeTroops(int num, SangoObject atk, bool showDamage = true)
        {
            // 白城直接占领
            if (this.BelongForce == null)
                return false;

            if (showDamage)
                Render?.ShowInfo(num, (int)InfoType.Troop);

            troops = troops + num;
            if (troops < 0)
                troops = 0;

            if (Render != null)
            {
                Render.UpdateRender();
            }
            return troops > 0;
        }

        public override void OnFall(SangoObject atker)
        {
            Scenario scenario = Scenario.Cur;
            ScenarioVariables scenarioVariables = Scenario.Cur.Variables;
            Troop atk = atker as Troop;
            if (atk == null) return;

            Force lastBelongForce = BelongForce;
            freePersons.Clear();
            // 清理火
            for (int i = 0; i < OccupyCellList.Count; ++i)
            {
                Cell cell = OccupyCellList[i];
                if (cell.fire != null)
                {
                    cell.fire.Clear();
                    cell.fire = null;
                }
            }

            // 城倒,俘虏逃
            for (int i = 0; i < this.captiveList.Count; i++)
            {
                Person person = this.captiveList[i];
                person.Escape();
            }
            this.captiveList.Clear();

            // 白城
            if (BelongCorps == null)
            {
                ChangeCorps(atk.BelongCorps);
                Leader = atk.Leader;
                atk.EnterCity(this);
                Render?.UpdateRender();
                CalculateHarvest();
                GameEvent.OnCityFall?.Invoke(this, lastBelongForce, atk);
                return;
            }


            BelongForce.CityBaseCount--;
            if (IsCity())
            {
                BelongForce.CityCount--;
            }

            // 确认一个撤退城市
            City escapeCity = null;
            if (BelongForce.CityCount != 0 || !IsCity())
            {
                if (this == BelongForce.Governor.BelongCity)
                    escapeCity = GetNearnestForceCity();
                else
                    escapeCity = BelongForce.Governor.BelongCity;
            }

            if (IsPort() || IsGate())
            {
                if (escapeCity == null)
                {
                    Debug.LogError("为啥 escapeCity == null");
                }
            }

            // 基础抓捕率
            int cacaptureChangce = escapeCity != null ? scenarioVariables.captureChangceWhenCityFall : scenarioVariables.captureChangceWhenLastCityFall;

            // 处理俘虏
            List<Person> captiveList = new List<Person>();

            // 必须优先处理队伍
            if (escapeCity == null)
            {
                // 灭亡后,队伍要清除
                scenario.troopsSet.ForEach((troop) =>
                {
                    if (troop.IsAlive && troop.BelongForce == this.BelongForce)
                        troop.Clear();
                });

                // 清理港口,关卡
                for (int i = 0; i < scenario.citySet.Count; ++i)
                {
                    var c = scenario.citySet[i];
                    if (c != null && c.IsAlive && c.BelongForce == BelongForce)
                    {
                        if (c.IsGate() || c.IsPort())
                        {
                            c.allPersons.ForEach(p =>
                             {
                                 p.ClearMission();
                                 p.LeaveToWild();
                             });
                            c.allPersons.Clear();
                            c.LeaveToWild();
                        }
                    }
                }
            }

            // 处理缓存队伍信息
            allAttackTroops.Clear();
            allTroops.Clear();

            for (int i = allPersons.Count - 1; i >= 0; --i)
            {
                Person person = allPersons[i];

                // 没有执行任务的才能被捕获,暂时不能抓捕主公
                if (person.IsFree && person != person.BelongForce.Governor && GameRandom.Chance(cacaptureChangce))
                {
                    captiveList.Add(person);
                }
                else
                {
                    if (escapeCity != null)
                    {
                        person.ChangeCity(escapeCity);
                        if (person.BelongTroop == null)
                            person.SetMission(MissionType.PersonReturn, person.BelongCity, 1);
                    }
                    else
                    {
                        person.ClearMission();
                        person.LeaveToWild();
                    }
                }


            }

            //处理建筑
            for (int i = allBuildings.Count - 1; i >= 0; i--)
            {
                Building building = allBuildings[i];
                if (building.isComplate && GameRandom.Chance(30))
                {
                    building.ChangeCorps(atk.BelongCorps);
                }
                else
                {
                    building.OnFall(atk);
                }
            }

            if (escapeCity == null)
            {
                Scenario.Cur.Remove(BelongCorps);
#if SANGO_DEBUG
                Sango.Log.Print($"{BelongForce.Name} 灭亡!!!");
#endif
                BelongForce.IsAlive = false;
                Scenario.Cur.Remove(BelongForce);

                // 势力灭亡事件
                GameEvent.OnForceFall?.Invoke(BelongForce, this, atk);

                if (Scenario.Cur.forceSet.DataCount == 1)
                {
                    Sango.Log.Print($"{Scenario.Cur.GetDateStr()} --> {atk.BelongForce.Name} 统一!!!!!!!!!!!!!!");
                }
            }

            ChangeCorps(atk.BelongCorps);

            atk.BelongForce.CityBaseCount++;
            if (IsCity())
            {
                atk.BelongForce.CityCount++;
            }

            // 处理库存和钱粮,兵力
            food = food * (GameRandom.RandomWeightIndex(scenarioVariables.cityFallCanKeepFoodFactor) * 10 + 10) / 100;
            gold = gold * (GameRandom.RandomWeightIndex(scenarioVariables.cityFallCanKeepGoldFactor) * 10 + 10) / 100;
            troops = troops * (GameRandom.RandomWeightIndex(scenarioVariables.cityFallCanKeepTroopsFactor) * 10 + 10) / 100;
            itemStore.Split((100 - (GameRandom.RandomWeightIndex(scenarioVariables.cityFallCanKeepItemFactor) * 10 + 10)) / 100);
            agriculture = agriculture * (GameRandom.RandomWeightIndex(scenarioVariables.cityFallCanKeepAgriculture) * 10 + 10) / 100;
            commerce = agriculture * (GameRandom.RandomWeightIndex(scenarioVariables.cityFallCanKeepCommerce) * 10 + 10) / 100;

            Leader = atk.Leader;
            atk.EnterCity(this);

            Render?.UpdateRender();

            CalculateHarvest();

            GameEvent.OnCityFall?.Invoke(this, lastBelongForce, atk);

            if (atk.BelongCorps.IsPlayer)
            {
                Render.WindowEvent windowEvent = new Render.WindowEvent()
                {
                    windowName = "window_city_complete",
                    args = new object[] { Name }
                };
                RenderEvent.Instance.Add(windowEvent);
            }

            CityRecruitPersonWhenCityFallEvent te = new CityRecruitPersonWhenCityFallEvent()
            {
                captiveList = captiveList,
                atk = atk,
                targetCity = this,
                recruitType = escapeCity == null ? 2 : 0
            };
            RenderEvent.Instance.Add(te);

        }

        /// <summary>
        /// 获取城市之间的距离
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int Distance(City other)
        {
            if (this == other)
                return 0;

            if (BelongCity != null)
            {
                // 隶属范围内,需要1回合
                if (BelongCity == other) return 1;
                return BelongCity.Distance(other);
            }

            if (other.BelongCity != null)
            {
                // 隶属范围内,需要1回合
                if (other.BelongCity == this) return 1;
                other = other.BelongCity;
            }

            return Scenario.Cur.GetCityDistance(this, other);
        }

        public void OnPersonReturnCity(Person person)
        {
#if SANGO_DEBUG
            Sango.Log.Print($"[{person.BelongForce.Name}]{person.Name}回到[{BelongForce.Name}]<{Name}>");
#endif
        }
        public void OnPersonTransformEnd(Person person)
        {

        }

        public bool CheckJobCost(CityJobType cityJobType)
        {
            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)cityJobType;
            int goldNeed = JobType.GetJobCost(jobId);

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, null, overrideData);
            goldNeed = overrideData.Value;
            return gold >= goldNeed;
        }

        public int GetJobCost(CityJobType cityJobType)
        {
            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)cityJobType;
            int goldNeed = JobType.GetJobCost(jobId);

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, null, overrideData);
            goldNeed = overrideData.Value;
            return goldNeed;
        }


        public Building BuildBuilding(Cell buildCenter, Troop builder, BuildingType buildingType)
        {
            Building building = new Building();
            building.BelongForce = BelongForce;
            building.BelongCorps = BelongCorps;
            building.BelongCity = this;
            building.BuildingType = buildingType;
            building.x = buildCenter.x;
            building.y = buildCenter.y;
            building.rot = GameRandom.Range(0, 10) * 90 * Mathf.Deg2Rad;

            building.durability = 0;

            Scenario scenario = Scenario.Cur;

            // TODO: 获取高度
            // ------
            scenario.Add(building);
            building.Init(scenario);
            building.Workers = null;
            building.isComplate = false;
            building.durability = 1;
            building.ChangeDurability(GameUtility.Method_TroopBuildAbility(builder), null);

#if SANGO_DEBUG
            Sango.Log.Print($"[{BelongForce.Name}]在<{Name}>由{builder.Name}开始修建: {building.Name}");
#endif
            building.Render.UpdateRender();
            return building;
        }

        public Cell GetEmptyInteriorCell()
        {
            for (int i = 0; i < interiorCellList.Count; ++i)
            {
                Cell c = interiorCellList[i];
                if (c.building == null)
                    return c;
            }
            return null;
        }

        public void InitJobFeature(Person[] people)
        {
            GameUtility.InitJobFeature(people, this);
        }

        public void InitJobFeature(Person person)
        {
            GameUtility.InitJobFeature(person, this);
        }

        public void ClearJobFeature()
        {
            GameUtility.ClearJobFeature();
        }

        /// <summary>
        /// 建造内城建筑
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="builders"></param>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        public Building JobBuildBuilding(Cell buildCenter, Person[] builders, BuildingType buildingType, int buildCount)
        {
            Building building = new Building();
            building.BelongForce = BelongForce;
            building.BelongCorps = BelongCorps;
            building.BelongCity = this;
            building.BuildingType = buildingType;
            building.x = buildCenter.x;
            building.y = buildCenter.y;
            building.durability = 0;
            building.rot = GameRandom.Range(0, 10) * 90 * Mathf.Deg2Rad;

            Scenario scenario = Scenario.Cur;
#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            // TODO: 获取高度
            // ------
            scenario.Add(building);
            building.Init(scenario);
            SangoObjectList<Person> sangoObjectList = new SangoObjectList<Person>();
            foreach (Person person in builders)
            {
                if (person == null) continue;
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(" ");
#endif
                person.SetMission(MissionType.PersonBuild, building, 0);
                person.ActionOver = true;
                freePersons.Remove(person);
                sangoObjectList.Add(person);
            }

            building.Builder = sangoObjectList;
            building.isComplate = false;
            building.durability = 1;
            building.LeftCounter = buildCount;
            gold -= buildingType.cost;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP((int)CityJobType.Build));

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]在<{Name}>由{stringBuilder}开始修建: {building.Name} 需耗时:{buildCount} 回合");
#endif
            building.Render.UpdateRender();
            return building;
        }

        /// <summary>
        /// 升级内城建筑
        /// </summary>
        /// <param name="building"></param>
        /// <param name="builders"></param>
        /// <param name="upgradeBuildingType"></param>
        /// <returns></returns>
        public Building JobUpgradeBuilding(Building building, Person[] builders, BuildingType upgradeBuildingType, int buildCount)
        {
            building.isUpgrading = true;
            building.durability = 1;

            Scenario scenario = Scenario.Cur;
#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            // TODO: 获取高度
            // ------
            SangoObjectList<Person> sangoObjectList = new SangoObjectList<Person>();
            foreach (Person person in builders)
            {
                if (person == null) continue;
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(" ");
#endif
                person.SetMission(MissionType.PersonBuild, building, 0);
                person.ActionOver = true;
                freePersons.Remove(person);
                sangoObjectList.Add(person);
            }

            building.Builder = sangoObjectList;
            building.LeftCounter = buildCount;
            gold -= upgradeBuildingType.cost;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP((int)CityJobType.UpgradeBuilding));

            building.Render?.UpdateRender();

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]在<{Name}>由{stringBuilder}开始升级建筑: {building.Name} 需耗时: {buildCount}回合");
#endif
            return building;
        }

        /// <summary>
        /// 生产船
        /// </summary>
        /// <param name="personList"></param>
        /// <param name="itemType"></param>
        /// <param name="buildingTotalLevel"></param>
        /// <returns></returns>
        public int[] JobCreateBoat(Person[] personList, ItemType itemType, Building building, bool isTest = false)
        {
            if (personList == null || personList.Length == 0 || itemType == null) return null;

            if (itemType.kind != (int)ItemKindType.Boat) return null;

            if (itemStore.TotalNumber >= StoreLimit) return null;

            Scenario scenario = Scenario.Cur;

            InitJobFeature(personList);
            int empty = StoreLimit - itemStore.TotalNumber;

            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.CreateBoat;

            int goldNeed = JobType.GetJobCost(jobId) + itemType.cost;

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, personList, overrideData);
            goldNeed = overrideData.Value;

            if (gold < goldNeed)
            {
                ClearJobFeature();
                return null;
            }

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            int maxValue = 0;
            Person maxPerson = null;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person.BaseCreativeAbility > maxValue)
                {
                    maxPerson = person;
                    maxValue = person.BaseCreativeAbility;
                }
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
            }

            int subValue = 0;

            // 最高属性武将获得100%加成,其余两个获取50%加成
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person != maxPerson)
                {
                    subValue += maxPerson.BaseCreativeAbility;
                }
            }

            int turnCount = GameUtility.Method_CreateBoatCounter(maxValue, subValue, building.BuildingType.level);
            overrideData = GameUtility.IntOverrideData.Set(turnCount);
            GameEvent.OnCityJobCounterResult?.Invoke(this, jobId, personList, overrideData);
            turnCount = overrideData.Value;

            // 最高属性武将获得100%加成,其余两个获取50%加成
            maxValue *= 5;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person != maxPerson)
                {
                    maxValue += maxPerson.BaseCreativeAbility * 10;
                }
            }


            if (isTest)
            {
                int totalValue = GameUtility.Method_CreateItems(maxValue, building.BuildingType.level) / itemType.p1 / 3;
                overrideData = GameUtility.IntOverrideData.Set(totalValue);
                GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
                totalValue = overrideData.Value;
                ClearJobFeature();
                return new int[] { turnCount, totalValue };
            }

            gold -= goldNeed;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));
            building.ActionOver = true;
            SangoObjectList<Person> sangoObjectList = new SangoObjectList<Person>();
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                freePersons.Remove(person);
                sangoObjectList.Add(person);
                person.ActionOver = true;
                person.SetMission(MissionType.PersonCreateBoat, itemType, turnCount, building.Id, maxValue);
            }
            building.Workers = sangoObjectList;
            building.LeftCounter = turnCount;
            building.isWorking = true;
#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了舰船生产!开始生产{itemType.Name}, 所需回合:{turnCount}, 建筑:{building.Name}");
#endif
            ClearJobFeature();
            return null;
        }

        /// <summary>
        /// 生产兵装
        /// </summary>
        /// <param name="personList"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public bool DoJobCreateBoat(ItemType itemType, int buildingId, int totalValue)
        {
            List<Person> people = new List<Person>();
            allPersons.ForEach(person =>
            {
                if (person.missionType == (int)MissionType.PersonCreateBoat)
                {
                    people.Add(person);
                }
            });
            if (people.Count == 0) return false;

            InitJobFeature(people.ToArray());
            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.CreateBoat;
            int meritGain = JobType.GetJobMeritGain(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);

            Building building = scenario.GetObject<Building>(buildingId);

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            int maxValue = 0;
            Person maxPerson = null;
            for (int i = 0; i < people.Count; i++)
            {
                Person person = people[i];
                if (person == null) continue;
                person.merit += meritGain;
                person.GainExp(meritGain);
                person.ClearMission();
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
            }

            totalValue = GameUtility.Method_CreateItems(totalValue, building.BuildingType.level) / itemType.p1 / 3;

            Person[] personList = people.ToArray();

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(totalValue);
            overrideData.Value = totalValue;
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;


            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;

            int empty = StoreLimit - itemStore.TotalNumber;
            totalValue = Math.Min(empty, totalValue);
            int exsistNumber = itemStore.Add(itemType.storeKind, totalValue);

            BelongForce.GainTechniquePoint(techniquePointGain);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了船只生产!共生产了{totalValue}{itemType.Name}, 当前数量:{exsistNumber}, 建筑:{building.Name}");
#endif

            Render?.ShowInfo(totalValue, itemType.Id + 1);
            ClearJobFeature();
            return true;
        }



        /// <summary>
        /// 生产器械
        /// </summary>
        /// <param name="personList"></param>
        /// <param name="itemType"></param>
        /// <param name="buildingTotalLevel"></param>
        /// <returns></returns>
        public int[] JobCreateMachine(Person[] personList, ItemType itemType, Building building, bool isTest = false)
        {
            if (personList == null || personList.Length == 0 || itemType == null) return null;

            if (itemType.kind != (int)ItemKindType.Machine) return null;

            if (itemStore.TotalNumber >= StoreLimit) return null;

            Scenario scenario = Scenario.Cur;
            int empty = StoreLimit - itemStore.TotalNumber;

            InitJobFeature(personList);

            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.CreateMachine;

            int goldNeed = JobType.GetJobCost(jobId) + itemType.cost;

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, personList, overrideData);
            goldNeed = overrideData.Value;

            if (gold < goldNeed)
            {
                ClearJobFeature();

                return null;
            }

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
            int lastTroops = troops;
#endif
            int maxValue = 0;
            Person maxPerson = null;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person.BaseCreativeAbility > maxValue)
                {
                    maxPerson = person;
                    maxValue = person.BaseCreativeAbility;
                }

#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
            }

            int subValue = 0;

            // 最高属性武将获得100%加成,其余两个获取50%加成
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person != maxPerson)
                {
                    subValue += maxPerson.BaseCreativeAbility;
                }
            }


            int turnCount = GameUtility.Method_CreateMachineCounter(maxValue, subValue, building.BuildingType.level);
            overrideData = GameUtility.IntOverrideData.Set(turnCount);
            GameEvent.OnCityJobCounterResult?.Invoke(this, jobId, personList, overrideData);
            turnCount = overrideData.Value;

            // 最高属性武将获得100%加成,其余两个获取50%加成
            maxValue *= 5;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person != maxPerson)
                {
                    maxValue += maxPerson.BaseCreativeAbility * 10;
                }
            }

            if (isTest)
            {
                int totalValue = GameUtility.Method_CreateItems(maxValue, building.BuildingType.level) / itemType.p1 / 3;
                overrideData = GameUtility.IntOverrideData.Set(totalValue);
                overrideData.Value = totalValue;
                GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
                totalValue = overrideData.Value;
                ClearJobFeature();
                return new int[] { turnCount, totalValue };
            }

            gold -= goldNeed;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));

            building.ActionOver = true;
            SangoObjectList<Person> sangoObjectList = new SangoObjectList<Person>();

            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                freePersons.Remove(person);
                sangoObjectList.Add(person);
                person.ActionOver = true;
                person.SetMission(MissionType.PersonCreateMachine, itemType, turnCount, building.Id, maxValue);
            }
            building.Workers = sangoObjectList;
            building.LeftCounter = turnCount;
            building.isWorking = true;

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了器械生产!开始生产{itemType.Name}, 所需回合:{turnCount}, 建筑:{building.Name}");
#endif
            Render?.UpdateRender();
            ClearJobFeature();
            return null;
        }

        public bool DoJobCreateMachine(ItemType itemType, int buildingId, int totalValue)
        {
            List<Person> people = new List<Person>();
            allPersons.ForEach(person =>
            {
                if (person.missionType == (int)MissionType.PersonCreateMachine)
                {
                    people.Add(person);
                }
            });
            if (people.Count == 0) return false;

            InitJobFeature(people.ToArray());
            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.CreateMachine;
            int meritGain = JobType.GetJobMeritGain(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);
            Building building = scenario.GetObject<Building>(buildingId);
#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            int maxValue = 0;
            Person maxPerson = null;
            for (int i = 0; i < people.Count; i++)
            {
                Person person = people[i];
                if (person == null) continue;
                person.merit += meritGain;
                person.GainExp(meritGain);
                person.ClearMission();
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
            }

            totalValue = GameUtility.Method_CreateItems(totalValue, building.BuildingType.level) / itemType.p1 / 3;

            Person[] personList = people.ToArray();

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(totalValue);
            overrideData.Value = totalValue;
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;


            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;

            int empty = StoreLimit - itemStore.TotalNumber;
            totalValue = Math.Min(empty, totalValue);
            int exsistNumber = itemStore.Add(itemType.storeKind, totalValue);

            BelongForce.GainTechniquePoint(techniquePointGain);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了器械生产!共生产了{totalValue}{itemType.Name}, 当前数量:{exsistNumber}, 建筑:{building.Name}");
#endif

            Render?.ShowInfo(totalValue, itemType.Id + 1);
            ClearJobFeature();
            return true;
        }

        /// <summary>
        /// 农业
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        public int JobFarming(Person[] personList, bool isTest = false)
        {
            if (personList == null || personList.Length == 0) return 0;
            if (agriculture >= AgricultureLimit) return 0;
            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.Farming;

            InitJobFeature(personList);

            int goldNeed = JobType.GetJobCost(jobId);

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, personList, overrideData);
            goldNeed = overrideData.Value;

            if (gold < goldNeed)
            {
                ClearJobFeature();
                return 0;
            }

            int totalValue = 0;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;

                totalValue += person.BaseAgricultureAbility;
            }

            totalValue = GameUtility.Method_FarmingAbility(totalValue);

            overrideData.Value = totalValue;
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;

            if (isTest)
            {
                ClearJobFeature();
                return totalValue;
            }

            int meritGain = JobType.GetJobMeritGain(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;

                person.merit += meritGain;
                person.GainExp(meritGain);
                freePersons.Remove(person);
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
                person.ActionOver = true;
            }

            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;

            BelongForce.GainTechniquePoint(techniquePointGain);
            gold -= goldNeed;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));
            agriculture += totalValue;
            if (agriculture > AgricultureLimit)
                agriculture = AgricultureLimit;

            Render?.ShowInfo(totalValue, (int)InfoType.Food);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了开垦!农业值达到了:{agriculture}");
#endif
            ClearJobFeature();
            return totalValue;
        }

        /// <summary>
        /// 开发
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        public int JobDevelop(Person[] personList, bool isTest = false)
        {
            if (personList == null || personList.Length == 0) return 0;
            if (commerce >= CommerceLimit) return 0;
            Scenario scenario = Scenario.Cur;

            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.Develop;

            InitJobFeature(personList);
            int goldNeed = JobType.GetJobCost(jobId);

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, personList, overrideData);
            goldNeed = overrideData.Value;

            if (gold < goldNeed)
            {
                ClearJobFeature();
                return 0;
            }

            int totalValue = 0;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;

                totalValue += person.BaseCommerceAbility;
            }

            totalValue = GameUtility.Method_DevelopAbility(totalValue);

            overrideData.Value = totalValue;
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;

            if (isTest)
            {
                ClearJobFeature();
                return totalValue;
            }

            int meritGain = JobType.GetJobMeritGain(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;

                person.merit += meritGain;
                person.GainExp(meritGain);

                freePersons.Remove(person);
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
                person.ActionOver = true;
            }

            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;

            BelongForce.GainTechniquePoint(techniquePointGain);
            gold -= goldNeed;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));
            commerce += totalValue;
            if (commerce > CommerceLimit)
                commerce = CommerceLimit;

            Render?.ShowInfo(totalValue, (int)InfoType.Gold);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了开发!商业值达到了:{commerce}");
#endif
            ClearJobFeature();
            return totalValue;
        }

        /// <summary>
        /// 治安巡视
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        public int JobInspection(Person[] personList, bool isTest = false)
        {
            if (personList == null || personList.Length == 0) return 0;
            if (security >= 100) return 0;

            Scenario scenario = Scenario.Cur;

            InitJobFeature(personList);
            //int barracksLv = GetIntriorBuildingComplateMaxLevel((int)BuildingKindType.PatrolBureau);
            //if (barracksLv == 0) return 0;

            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.Inspection;

            int goldNeed = JobType.GetJobCost(jobId);

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, personList, overrideData);
            goldNeed = overrideData.Value;


            if (gold < goldNeed)
            {
                ClearJobFeature();
                return 0;
            }

            int totalValue = 0;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;

                totalValue += person.BaseSecurityAbility;
            }

            // 最终数值
            totalValue = GameUtility.Method_SecurityAbility(totalValue, 3);

            // 
            overrideData.Value = totalValue;
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;

            if (isTest)
            {
                ClearJobFeature();
                return totalValue;
            }

            int meritGain = JobType.GetJobMeritGain(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;

                person.merit += meritGain;
                person.GainExp(meritGain);

                freePersons.Remove(person);
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
                person.ActionOver = true;
            }

            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;

            BelongForce.GainTechniquePoint(techniquePointGain);
            gold -= goldNeed;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));
            security += totalValue;
            if (security > 100)
                security = 100;
            AddJobCounter(jobId);
            Render?.ShowInfo(totalValue, (int)InfoType.Security);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了巡视!治安提升到了:{security}");
#endif
            ClearJobFeature();
            return totalValue;
        }

        /// <summary>
        /// 训练
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        public int JobTrainTroops(Person[] personList, bool isTest = false)
        {
            if (personList == null || personList.Length == 0) return 0;
            if (morale >= MaxMorale) return 0;
            Scenario scenario = Scenario.Cur;

            InitJobFeature(personList);

            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.TrainTroops;

            int goldNeed = JobType.GetJobCost(jobId);

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, personList, overrideData);
            goldNeed = overrideData.Value;

            if (gold < goldNeed)
            {
                ClearJobFeature();
                return 0;
            }

            int totalValue = 0;
            int subValue = 0;
            int maxValue = 0;
            Person maxPerson = null;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person.BaseTrainTroopAbility > maxValue)
                {
                    maxPerson = person;
                    maxValue = person.BaseTrainTroopAbility;
                }
            }

            // 最高属性武将获得100%加成,其余两个获取50%加成
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person != maxPerson)
                {
                    subValue += maxPerson.BaseTrainTroopAbility;
                }
                else
                {
                    totalValue += maxPerson.BaseTrainTroopAbility;
                }
            }

            // 最终数值
            totalValue = GameUtility.Method_TrainTroops(totalValue, subValue);

            overrideData.Value = totalValue;
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;

            if (isTest)
            {
                ClearJobFeature();
                return totalValue;
            }

            int meritGain = JobType.GetJobMeritGain(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                person.merit += meritGain;
                person.GainExp(meritGain);

                freePersons.Remove(person);
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
                person.ActionOver = true;
            }

            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;
            BelongForce.GainTechniquePoint(techniquePointGain);

            gold -= goldNeed;
            morale += totalValue;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));
            if (morale > MaxMorale)
                morale = MaxMorale;

            AddJobCounter(jobId);
            Render?.ShowInfo(totalValue, (int)InfoType.Morale);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了训练!士气提升到了:{morale}");
#endif
            ClearJobFeature();
            return totalValue;
        }


        /// <summary>
        /// 登庸武将
        /// </summary>
        /// <param name="person"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool JobRecruitPerson(Person person, Person dest)
        {
            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.RecruitPerson;
            int apCost = JobType.GetJobCostAP(jobId);

            freePersons.Remove(person);
            if (dest.BelongCity == person.BelongCity)
            {
                CityRecruitPersonEvent te = new CityRecruitPersonEvent()
                {
                    person = person,
                    target = dest,
                };
                RenderEvent.Instance.Add(te);
                BelongCorps.ReduceActionPoint(apCost);
                return true;
            }
            else
            {
                person.SetMission(MissionType.PersonRecruitPerson, dest, Math.Max(1, person.BelongCity.Distance(dest.BelongCity)));
                BelongCorps.ReduceActionPoint(apCost);
                return false;
            }
        }

        /// <summary>
        /// 褒奖武将
        /// </summary>
        /// <param name="person"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool JobRewardPersons(Person[] persons)
        {
            if (persons == null || persons.Length == 0)
                return true;

            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.Reward;
            int apCost = JobType.GetJobCostAP(jobId);
            int goldCost = JobType.GetJobCost(jobId);
#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            int totalApCost = 0;
            int totalGoldCost = 0;
            for (int i = 0; i < persons.Length; ++i)
            {
                Person person = persons[i];
                if (person == null) continue;

                totalApCost += apCost;
                totalGoldCost += goldCost;
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
                person.loyalty += 10;
            }
            gold -= totalGoldCost;
            BelongCorps.ReduceActionPoint(totalApCost);
            AddJobCounter(jobId);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]在<{Name}>使用资金对{stringBuilder}进行了褒赏!!");
#endif
            return true;
        }


        public bool JobRewardPerson(Person person)
        {
            if (person == null)
                return true;

            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.Reward;
            int apCost = JobType.GetJobCostAP(jobId);
            int goldCost = JobType.GetJobCost(jobId);
#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif

#if SANGO_DEBUG
            stringBuilder.Append(person.Name);
            stringBuilder.Append(",");
            int lastLoyalty = person.loyalty;
#endif
            person.loyalty += 10;
            gold -= goldCost;
            BelongCorps.ReduceActionPoint(apCost);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]在<{Name}>使用资金对{stringBuilder}进行了褒赏!! 忠诚从{lastLoyalty}提升到->{person.loyalty}");
#endif
            return true;
        }


        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        public bool JobSearching(Person[] personList)
        {
            if (personList == null || personList.Length == 0) return false;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                CityPersonSearchingEvent te = new CityPersonSearchingEvent()
                {
                    city = this,
                    person = person,
                };
                RenderEvent.Instance.Add(te);
            }
            return true;
        }

        public int DoJobSearching(Person person, out Person target)
        {
            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.Searching;
            int meritGain = JobType.GetJobMeritGain(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);
            int apCost = JobType.GetJobCostAP(jobId);
            target = null;
            freePersons.Remove(person);
            InitJobFeature(person);

            BelongCorps.ReduceActionPoint(apCost);

            // 发现人才
            int probality = 20 + person.Politics * 3 / 5;
            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(probality);
            if (invisiblePersons.Count > 0)
            {
                GameEvent.OnCityJobSearchingWild?.Invoke(this, jobId, person, overrideData);
                probality = overrideData.Value;
                if (GameRandom.Chance(probality))
                {
                    target = invisiblePersons[GameRandom.Range(0, invisiblePersons.Count)];
                    target.state = (int)PersonStateType.Unemployed;

                    if (IsPlayer)
                    {
                        PlayerMessage.AddTextMessage($"{person.ColorName}在{ColorName}发现人才{target.ColorName}。",
                            BelongForce, x, y);
                    }

#if SANGO_DEBUG
                    Sango.Log.Print($"@内政@[{BelongForce.Name}]<{Name}>的{person.Name}发现了人才->{target.Name}");
#endif
                    invisiblePersons.Remove(target);
                    wildPersons.Add(target);
                    person.merit += meritGain;
                    person.GainExp(meritGain);
                    person.ActionOver = true;
                    ClearJobFeature();
                    return 0;
                }
            }

            //TODO: 搜索道具
            //if (!person.ActionOver && GameRandom.Changce((int)(3 * ability_improve)))
            //{
            //    person.ActionOver = true;
            //    continue;
            //}

            // 搜索钱财
            probality = person.Politics * 3 / 5;
            if (!person.ActionOver && GameRandom.Chance(probality))
            {
                int findGold = GameRandom.Range(person.Politics * 2, person.Politics * 3);
                if (IsPlayer)
                {
                    PlayerMessage.AddTextMessage($"{person.ColorName}在{ColorName}发现资金{findGold}。",
                        BelongForce, x, y);
                }
                AddGold(findGold);
                Render?.ShowInfo(findGold, (int)InfoType.Gold);

                person.merit += meritGain;
                person.GainExp(meritGain);
                person.ActionOver = true;
                return findGold;
            }

            //TODO: 触发事件

            // 什么也没找到
            if (!person.ActionOver)
            {
                if (IsPlayer)
                {
                    PlayerMessage.AddTextMessage($"{person.ColorName}在{ColorName}什么也没发现。",
                        BelongForce, x, y);
                }

                person.merit += meritGain;
                person.GainExp(meritGain);
                person.ActionOver = true;
            }


            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, new Person[] { person }, overrideData);
            techniquePointGain = overrideData.Value;

            BelongForce.GainTechniquePoint(techniquePointGain);
            ClearJobFeature();
            return -1;
        }


        /// <summary>
        /// 治疗伤兵
        /// </summary>
        /// <returns></returns>
        public bool JobHealingTroop()
        {
            // 城池满了不再招募

            if (woundedTroops <= 0) return false;
            int recruitNum = agriculture + commerce;
            int rs = Math.Min(woundedTroops, recruitNum);
            troops += rs;
            woundedTroops -= rs;
#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]<{Name}>进行了士兵治愈!共治愈到{rs}人, 当前士兵提升到了:{troops}");
#endif
            return true;
        }

        /// <summary>
        /// 招募
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        public int JobRecruitTroop(Person[] personList, bool isTest = false)
        {
            Building barracks = GetFreeBuilding((int)BuildingKindType.Barracks);
            if (barracks == null) return 0;
            return JobRecruitTroop(personList, barracks, isTest);
        }

        /// <summary>
        /// 招募
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        public int JobRecruitTroop(Person[] personList, Building barracks, bool isTest = false)
        {
            if (personList == null || personList.Length == 0) return 0;
            // 城池满了不再招募
            if (TroopsIsFull) return 0;

            Scenario scenario = Scenario.Cur;
            InitJobFeature(personList);
            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.RecruitTroops;

            int goldNeed = JobType.GetJobCost(jobId);

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, personList, overrideData);
            goldNeed = overrideData.Value;

            if (gold < goldNeed)
            {
                ClearJobFeature();
                return 0;
            }

            int meritGain = JobType.GetJobMeritGain(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);

            int totalValue = 0;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                totalValue += person.BaseRecruitmentAbility;

            }

            totalValue = GameUtility.Method_RecruitTroops(totalValue, barracks.BuildingType.level);

            overrideData.Value = totalValue;
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;

            // 治安对征兵的影响
            totalValue = (int)(totalValue * (1f - Math.Max(0, (100 - security)) * variables.securityInfluenceRecruitTroops));

            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;

            if (Scenario.Cur.Variables.populationEnable)
            {
                totalValue = Math.Min(totalValue, troopPopulation);
            }

            if (totalValue + troops > TroopsLimit)
                totalValue = TroopsLimit - troops;

            if (isTest)
            {

                ClearJobFeature();
                return totalValue;
            }

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
            int lastTroops = troops;
#endif

            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                person.merit += meritGain;
                person.GainExp(meritGain);
                freePersons.Remove(person);
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
                person.ActionOver = true;
            }

            if (Scenario.Cur.Variables.populationEnable)
            {
                troopPopulation -= totalValue;
                population -= totalValue;
            }

            //士气减少
            morale = (troops * morale + totalValue * 30) / (troops + totalValue);
            troops += totalValue;
            gold -= goldNeed;

            //治安减少
            security -= Math.Min(6, 4 * totalValue / 1000);
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));

            BelongForce.GainTechniquePoint(techniquePointGain);

            barracks.ActionOver = true;

            Render?.UpdateRender();
            Render?.ShowInfo(totalValue, (int)InfoType.Troop);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了招募!共招募到{troops - lastTroops}人, 当前士兵人数提升到了:{troops}");
#endif
            ClearJobFeature();
            return totalValue;
        }

        /// <summary>
        /// 生产兵装
        /// </summary>
        /// <param name="personList"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public int JobCreateItems(Person[] personList, ItemType itemType, Building building, bool isTest = false)
        {
            if (personList == null || personList.Length == 0) return 0;
            if (itemStore.TotalNumber >= StoreLimit) return 0;

            Scenario scenario = Scenario.Cur;

            if (isTest && itemType == null)
            {
                building = GetFreeBuilding((int)BuildingKindType.BlacksmithShop);
                itemType = scenario.GetObject<ItemType>((int)ItemKindType.Weapon);
            }

            if (itemType == null) return 0;
            if (building == null) return 0;

            InitJobFeature(personList);

            int empty = StoreLimit - itemStore.TotalNumber;

            ScenarioVariables variables = scenario.Variables;
            int jobId = itemType.Id == 5 ? (int)CityJobType.CreateHorse : (int)CityJobType.CreateItems;

            int goldNeed = JobType.GetJobCost(jobId) + itemType.cost;

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(goldNeed);
            GameEvent.OnCityCheckJobCost?.Invoke(this, jobId, personList, overrideData);
            goldNeed = overrideData.Value;

            if (gold < goldNeed)
            {
                ClearJobFeature();
                return 0;
            }

            int totalValue = 0;
            int maxValue = 0;
            Person maxPerson = null;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person.BaseCreativeAbility > maxValue)
                {
                    maxPerson = person;
                    maxValue = person.BaseCreativeAbility;
                }
            }

            // 最高属性武将获得100%加成,其余两个获取50%加成
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                if (person != maxPerson)
                {
                    totalValue += maxPerson.BaseCreativeAbility * 5;
                }
                else
                {
                    totalValue += maxPerson.BaseCreativeAbility * 10;
                }
            }

            totalValue = GameUtility.Method_CreateItems(totalValue, building.BuildingType.level);

            overrideData.Value = totalValue;
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;
            totalValue = Math.Min(empty, totalValue);

            if (isTest)
            {
                ClearJobFeature();
                return totalValue;
            }

            int meritGain = JobType.GetJobLimit(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);

#if SANGO_DEBUG
            StringBuilder stringBuilder = new StringBuilder();
#endif
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;

                person.merit += meritGain;
                person.GainExp(meritGain);

                freePersons.Remove(person);
#if SANGO_DEBUG
                stringBuilder.Append(person.Name);
                stringBuilder.Append(",");
#endif
                person.ActionOver = true;
            }

            int exsistNumber = itemStore.Add(itemType.Id, totalValue);

            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;
            building.ActionOver = true;
            gold -= goldNeed;
            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));

            BelongForce.GainTechniquePoint(techniquePointGain);

            if (itemType.Id == 2)
                Render?.ShowInfo(totalValue, 1);
            else if (itemType.Id == 5)
                Render?.ShowInfo(totalValue, itemType.Id + 1);
            else
                Render?.ShowInfo(totalValue, itemType.Id);

#if SANGO_DEBUG
            Sango.Log.Print($"@内政@[{BelongForce.Name}]{stringBuilder}对<{Name}>进行了生产兵装!共生产了{totalValue}{itemType.Name}, 当前数量:{exsistNumber}, 建筑:{building.Name}");
#endif
            ClearJobFeature();
            return totalValue;
        }

        /// <summary>
        /// 交易粮食
        /// </summary>
        /// <param name="personList"></param>
        /// <returns></returns>
        public bool JobTradeFood(Person[] personList, int goldNum)
        {
            if (personList == null || personList.Length == 0 || goldNum == 0) return false;
            if (hasBusiness == 0) return false;

            Scenario scenario = Scenario.Cur;

            ScenarioVariables variables = scenario.Variables;
            int jobId = (int)CityJobType.TradeFood;

            int meritGain = JobType.GetJobLimit(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);

            Person person = personList[0];
            if (person == null) return false;

            InitJobFeature(personList);

            person.merit += meritGain;
            person.GainExp(meritGain);
            freePersons.Remove(person);
            person.ActionOver = true;

            int totalValue = GameUtility.Method_Trade(person.Politics);

            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(totalValue);
            GameEvent.OnCityJobResult?.Invoke(this, jobId, personList, overrideData);
            totalValue = overrideData.Value;


            overrideData.Value = techniquePointGain;
            GameEvent.OnCityJobGainTechniquePoint?.Invoke(this, jobId, personList, overrideData);
            techniquePointGain = overrideData.Value;

            // TODO : 城市粮价
            int p = hasBusiness;

            if (goldNum > 0)
            {
                totalValue = totalValue * goldNum * p / 100;
                if (totalValue + food > foodLimit)
                    totalValue = foodLimit - food;

                AddGold(-goldNum);
                AddFood(totalValue);

                Render?.ShowInfo(-goldNum, (int)InfoType.Gold);
                Render?.ShowInfo(totalValue, (int)InfoType.Food);
#if SANGO_DEBUG
                Sango.Log.Print($"@内政@[{BelongForce.Name}]{person.Name}在<{Name}>花费{goldNum}交易到了{totalValue}粮食, 现有粮食:{food}");
#endif
            }
            else
            {
                totalValue = totalValue * (-goldNum / p) / 100;
                if (totalValue + gold > goldLimit)
                    totalValue = goldLimit - gold;
                AddGold(totalValue);
                AddFood(goldNum);

                Render?.ShowInfo(goldNum, (int)InfoType.Gold);
                Render?.ShowInfo(totalValue, (int)InfoType.Food);

#if SANGO_DEBUG
                Sango.Log.Print($"@内政@[{BelongForce.Name}]{person.Name}在<{Name}>花费{-goldNum}交易到了{totalValue}资金, 现有资金:{gold}");
#endif
            }

            BelongCorps.ReduceActionPoint(JobType.GetJobCostAP(jobId));

            AddJobCounter(jobId);

            BelongForce.GainTechniquePoint(techniquePointGain);

            ClearJobFeature();
            return true;
        }

        /// <summary>
        /// 获取城池的攻击力
        /// </summary>
        /// <returns></returns>
        public override int GetAttack()
        {
            ScenarioVariables Variables = Scenario.Cur.Variables;
            // 根据太守数值来计算基础伤害
            int atk = Math.Max(BuildingType.atk, (Leader?.Strength ?? 50 * 5000 + Leader?.Command ?? 50 * 5000) / 10000);

            return atk;
        }

        public override int GetAttackBack()
        {
            ScenarioVariables Variables = Scenario.Cur.Variables;
            // 根据太守数值来计算基础伤害
            int atk = Math.Max(BuildingType.atkBack, (Leader?.Strength ?? 50 * 5000 + Leader?.Command ?? 50 * 5000) / 10000);

            return atk;
        }


        /// <summary>
        /// 获取城池的防御力
        /// </summary>
        /// <returns></returns>
        public override int GetDefence()
        {
            ScenarioVariables Variables = Scenario.Cur.Variables;

            // 根据太守数值来计算基础防御
            int def = Math.Max(70, (Leader?.Intelligence ?? 70 * 3000 + Leader?.Command ?? 70 * 7000) / 10000);

            return def;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public override int GetSkillMethodAvaliabledTroops()
        {
            return Math.Max(5000, durability * troops / DurabilityLimit);
        }

        public struct EnemyInfo
        {
            public Troop troop;
            public int distance;
        }

        protected const int SAVE_ROUND = 40;
        protected List<EnemyInfo> enemies = new List<EnemyInfo>();
        protected bool[] enemiesRound = new bool[SAVE_ROUND];

        public Troop GetNearestEnemy(Cell checkCell)
        {
            Troop target = null;
            int dis = 999999;
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyInfo enemyInfo = enemies[i];
                int distance = Scenario.Cur.Map.Distance(enemyInfo.troop.cell, checkCell);
                if (distance < dis)
                {
                    target = enemyInfo.troop;
                }
            }
            return target;
        }

        public bool IsEnemiesRound(int round)
        {
            if (round < enemiesRound.Length)
                return enemiesRound[round];
            return false;
        }
        public bool IsEnemiesRound()
        {
            for (int i = 0; i < enemiesRound.Length; i++)
            {
                if (enemiesRound[i]) return true;
            }
            return false;
        }


        public bool CheckEnemiesIfAlive(out EnemyInfo enemyInfo)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyInfo check = enemies[i];
                if (check.troop.IsAlive)
                {
                    enemyInfo = check;
                    return true;
                }
            }
            enemyInfo = default;
            return false;
        }

        public bool CheckEnemiesIfAlive()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyInfo check = enemies[i];
                if (check.troop.IsAlive)
                {
                    return true;
                }
            }
            return false;
        }

        /// 这几个属性提供给AI使用
        internal Troop CurActiveTroop = null;
        internal MissionType TroopMissionType = MissionType.None;
        internal int TroopMissionTargetId;

        public Troop EnsureTroop(Troop troop, Scenario scenario)
        {
            // 先加入剧本才能分配ID
            troop.cell = this.CenterCell;
            this.CenterCell.troop = troop;
            scenario.Add(troop);
            troop.Init(scenario);
            return troop;
        }

        public override bool DoAI(Scenario scenario)
        {
            if (AIFinished)
                return true;

            if (!AIPrepared)
            {
                AIPrepare(scenario);
                GameEvent.OnCityAIStart?.Invoke(this, scenario);
                AIPrepared = true;
            }

            if (CurActiveTroop != null)
            {
                if (!CurActiveTroop.DoAI(scenario))
                    return false;
                CurActiveTroop = null;
            }

            while (AICommandList.Count > 0)
            {
                System.Func<City, Scenario, bool> CurrentCommand = AICommandList[0];
                if (!CurrentCommand.Invoke(this, scenario))
                    return false;

                AICommandList.RemoveAt(0);
            }

            GameEvent.OnCityAIEnd?.Invoke(this, scenario);
            AIFinished = true;
            ActionOver = true;
            return true;
        }

        public virtual void PrepareEnemiesInfo(Scenario scenario)
        {
            // 准备敌人信息
            enemies.Clear();
            for (int i = 0; i < enemiesRound.Length; i++)
                enemiesRound[i] = false;

            for (int i = 0; i < areaCellList.Count; i++)
            {
                Cell cell = areaCellList[i];
                if (cell.troop != null && cell.troop.IsEnemy(this))
                {
                    int round = scenario.Map.Distance(CenterCell, cell);
                    if (round < SAVE_ROUND)
                    {
                        enemies.Add(new EnemyInfo { troop = cell.troop, distance = round });
                        for (int j = round; j < enemiesRound.Length; j++)
                            enemiesRound[j] = true;
                    }
                }
            }

            if (enemies.Count > 1)
            {
                enemies.Sort((a, b) =>
                {
                    return a.distance.CompareTo(b.distance);
                });
            }
        }

        public virtual void AIPrepare(Scenario scenario)
        {
            // 准备敌人信息
            PrepareEnemiesInfo(scenario);

            UpdateActiveTroopTypes();
            UpdateFightPower();

            if (IsBorderCity)
            {

                AICommandList.Add(CityAI.AIAttack);
                AICommandList.Add(CityAI.AITradeFood);
                //AICommandList.Add(CityAI.AISecurity);

                //if (troops < 20000)
                //{
                //    AICommandList.Add(CityAI.AIRecruitTroop);
                //    AICommandList.Add(CityAI.AIIntrior);
                //}
                //else
                //{
                //    if (scenario.Info.day == 10)
                //    {
                //        AICommandList.Add(CityAI.AIRecruitTroop);
                //        AICommandList.Add(CityAI.AICreateItems);
                //        AICommandList.Add(CityAI.AIIntrior);
                //    }
                //    else if (scenario.Info.day == 20)
                //    {
                //        AICommandList.Add(CityAI.AIIntrior);
                //        AICommandList.Add(CityAI.AIRecruitTroop);
                //        AICommandList.Add(CityAI.AICreateItems);
                //    }
                //    else
                //    {
                //        AICommandList.Add(CityAI.AICreateItems);
                //        AICommandList.Add(CityAI.AIRecruitTroop);
                //        AICommandList.Add(CityAI.AIIntrior);
                //    }
                //}
                AICommandList.Add(CityAI.AIIntrior);
            }
            else
            {
                //AICommandList.Add(CityAI.AISecurity);
                AICommandList.Add(CityAI.AITradeFood);
                // 物资输送
                AICommandList.Add(CityAI.AITransfrom);
                //if (troops < itemStore.TotalNumber)
                //    AICommandList.Add(CityAI.AIRecruitTroop);
                //else
                //    AICommandList.Add(CityAI.AICreateItems);
                AICommandList.Add(CityAI.AIIntrior);
            }

            GameEvent.OnCityAIPrepare?.Invoke(this, scenario);
        }

        /// <summary>
        /// 检查是否太守需要重新设置
        /// </summary>
        /// <param name="person"></param>
        public void CheckIfLoseLeader(Person person)
        {
            if (Leader != person) return;

            Person dest = null;
            Official higher = null;
            int commandHigher = 0;
            for (int i = 0; i < allPersons.Count; i++)
            {
                Person checker = allPersons[i];
                if (checker != null && checker != Leader && checker.IsAlive)
                {
                    if (dest == null)
                    {
                        dest = checker;
                        higher = dest.Official;
                        commandHigher = dest.Command;
                    }
                    else
                    {
                        if (checker.Official.level > higher.level)
                        {
                            dest = checker;
                            higher = dest.Official;
                            commandHigher = dest.Command;
                        }
                        else if (checker.Official.level == higher.level)
                        {
                            if (checker.Command > commandHigher)
                            {
                                dest = checker;
                                higher = dest.Official;
                                commandHigher = dest.Command;
                            }
                        }
                    }
                }
            }
            Leader = dest;
        }

        /// <summary>
        /// 更新太守
        /// </summary>
        /// <param name="person"></param>
        public void UpdateNewLeader()
        {
            Person dest = null;
            Official higher = null;
            int commandHigher = 0;
            for (int i = 0; i < allPersons.Count; i++)
            {
                Person checker = allPersons[i];
                if (checker != null && checker.IsAlive)
                {
                    if (checker.IsGovernor)
                    {
                        dest = checker;
                        break;
                    }

                    if (dest == null)
                    {
                        dest = checker;
                        higher = dest.Official;
                        commandHigher = dest.Command;
                    }
                    else
                    {
                        if (checker.Official.level > higher.level)
                        {
                            dest = checker;
                            higher = dest.Official;
                            commandHigher = dest.Command;
                        }
                        else if (checker.Official.level == higher.level)
                        {
                            if (checker.Command > commandHigher)
                            {
                                dest = checker;
                                higher = dest.Official;
                                commandHigher = dest.Command;
                            }
                        }
                    }
                }
            }
            Leader = dest;
            Leader?.SetStateLeader();
        }

        /// <summary>
        /// 获取已建造完成的建筑类型的数量
        /// </summary>
        /// <param name="buildingKindId"></param>
        /// <returns></returns>
        public int GetBuildingComplateNumber(int buildingKindId)
        {
            int complateNum = 0;
            for (int i = 0; i < allBuildings.Count; i++)
            {
                Building building = allBuildings[i];
                if (building.BuildingType.kind == buildingKindId && building.isComplate)
                {
                    complateNum++;
                }
            }
            return complateNum;
        }


        /// <summary>
        /// 获取已建造完成的建筑类型的数量
        /// </summary>
        /// <param name="buildingKindId"></param>
        /// <returns></returns>
        public Building GetFreeBuilding(int buildingKindId)
        {
            for (int i = 0; i < allBuildings.Count; i++)
            {
                Building building = allBuildings[i];
                if (building.BuildingType.kind == buildingKindId && building.isComplate && !building.ActionOver && !building.isWorking)
                {
                    return building;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取已建造的建筑类型的数量(包括未完成的)
        /// </summary>
        /// <param name="buildingKindId"></param>
        /// <returns></returns>
        public int GetBuildingNumber(int buildingKindId)
        {
            int count;
            if (buildingCountMap.TryGetValue(buildingKindId, out count))
                return count;
            return 0;
        }

        public bool IsInteriorBuildFull()
        {
            for (int i = 0; i < interiorCellList.Count; i++)
            {
                if (interiorCellList[i].building == null)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 获取内政地使用数量
        /// </summary>
        /// <returns></returns>
        public int GetInteriorCellUsedCount()
        {
            int complateNum = 0;
            for (int i = 0; i < interiorCellList.Count; i++)
            {
                if (interiorCellList[i].building != null)
                    complateNum++;
            }
            return complateNum;
        }

        ///// <summary>
        ///// 获取已建造完成的建筑类型的最大等级(不叠加)
        ///// </summary>
        ///// <param name="buildingTypeId"></param>
        ///// <returns></returns>
        //public int GetIntriorBuildingComplateMaxLevel(int buildingKindId)
        //{
        //    int complateNum = 0;
        //    for (int i = 0; i < allBuildings.Count; i++)
        //    {
        //        Building building = allBuildings[i];
        //        if (building.BuildingType.kind == buildingKindId && building.isComplte)
        //        {
        //            complateNum = Math.Max(complateNum, building.BuildingType.level);
        //        }
        //    }
        //    return complateNum;
        //}

        public int EnemyCount
        {
            get
            {
                return enemies.Count;
            }
        }

        public void CalculateMaxMorale()
        {
            int max = 100;
            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(max);
            GameEvent.OnCityCalculateMaxMorale?.Invoke(this, overrideData);
            MaxMorale = overrideData.Value;
        }

        public void CalculateLimit()
        {
            Tools.OverrideData<int> overrideData = GameUtility.IntOverrideData.Set(0);
            GameEvent.OnCityCalculateMaxGold?.Invoke(this, overrideData);
            goldLimitAdd = overrideData.Value;

            overrideData.Value = 0;
            GameEvent.OnCityCalculateMaxFood?.Invoke(this, overrideData);
            foodLimitAdd = overrideData.Value;

            overrideData.Value = 0;
            GameEvent.OnCityCalculateMaxItemStoreSize?.Invoke(this, overrideData);
            storeLimitAdd = overrideData.Value;

            overrideData.Value = 0;
            GameEvent.OnCityCalculateMaxTroops?.Invoke(this, overrideData);
            troopsLimitAdd = overrideData.Value;

            overrideData.Value = 0;
            GameEvent.OnCityCalculateMaxDurability?.Invoke(this, overrideData);
            durabilityLimitAdd = overrideData.Value;

        }
    }
}
