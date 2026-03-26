using TKNewtonsoft.Json;
using Sango.Game.Render;
using System;
using System.Collections.Generic;

namespace Sango.Game
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Person : SangoObject
    {
        public override SangoObjectType ObjectType { get { return SangoObjectType.Person; } }

        public string ColorName => $"<color=#7CCADB>{Name}</color>";

        /// <summary>
        /// 所属势力
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<Force>))]
        public Force BelongForce { get; set; }

        public bool IsPlayer => BelongForce?.IsPlayer ?? false;


        /// <summary>
        /// 所属军团
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<Force>))]
        public Corps BelongCorps { get; set; }

        /// <summary>
        /// 所属城池
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<City>))]
        public City BelongCity { get; set; }

        /// <summary>
        /// 所在城池
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<City>))]
        public City CurrentCity { get; set; }

        /// <summary>
        /// 所属部队
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<Troop>))]
        public Troop BelongTroop { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public int familyNameID;
        [JsonProperty] public string familyName;

        /// <summary>
        /// 名
        /// </summary>
        public int giveNameID;
        [JsonProperty] public string giveName;

        /// <summary>
        /// 字
        /// </summary>
        public int nickNameID;
        [JsonProperty] public string nickName;

        /// <summary>
        /// 身平
        /// </summary>
        [JsonProperty] public int description;

        /// <summary>
        /// 头像id
        /// </summary>
        [JsonProperty] public int headIconID;

        /// <summary>
        /// 立绘id
        /// </summary>
        [JsonProperty] public int imageID;

        /// <summary>
        /// 性别
        /// </summary>
        [JsonProperty] public int sex;

        /// <summary>
        /// 登场年份
        /// </summary>
        [JsonProperty] public int yearAvailable;

        /// <summary>
        /// 是否被发现
        /// </summary>
        public bool beFinded => !Invisible;

        /// <summary>
        /// 出生年
        /// </summary>
        [JsonProperty] public int yearBorn;

        /// <summary>
        /// 死亡年
        /// </summary>
        [JsonProperty] public int yearDead;

        /// <summary>
        /// 相性
        /// </summary>
        [JsonProperty] public int compatibility;

        /// <summary>
        /// 身分
        /// </summary>
        [JsonProperty] public int state;

        /// <summary>
        /// 性格
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<Personality>))]
        public Personality personality;

        /// <summary>
        /// 义理
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<Argumentation>))]
        public Argumentation argumentation;

        /// <summary>
        /// 官职
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Official>))]
        [JsonProperty]
        public Official Official { get; set; }

        /// <summary>
        /// 忠诚
        /// </summary>
        [JsonProperty] public int loyalty;

        /// <summary>
        /// 功绩
        /// </summary>
        [JsonProperty] public int merit;

        /// <summary>
        /// 体力
        /// </summary>
        [JsonProperty] public int stamina;

        /// <summary>
        /// 经验
        /// </summary>
        [JsonProperty] public int Exp { get; private set; }

        /// <summary>
        /// 等级
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<PersonLevel>))]
        public PersonLevel Level { get; set; }

        /// <summary>
        /// 统御
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAttributeValueConverter))]
        public PersonAttributeValue command = new PersonAttributeValue();

        /// <summary>
        /// 武力
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAttributeValueConverter))]
        public PersonAttributeValue strength = new PersonAttributeValue();

        /// <summary>
        /// 智力
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAttributeValueConverter))]
        public PersonAttributeValue intelligence = new PersonAttributeValue();

        /// <summary>
        /// 政治
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAttributeValueConverter))]
        public PersonAttributeValue politics = new PersonAttributeValue();

        /// <summary>
        /// 魅力
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAttributeValueConverter))]
        public PersonAttributeValue glamour = new PersonAttributeValue();

        /// <summary>
        /// 血缘
        /// </summary>
        [JsonProperty] public int consanguinity;

        /// <summary>
        /// 伤病
        /// </summary>
        [JsonProperty] public int injury;

        /// <summary>
        /// 父亲
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Person>))]
        [JsonProperty]
        public Person Father { get; set; }

        /// <summary>
        /// 母亲
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Person>))]
        [JsonProperty]
        public Person Mother { get; set; }

        /// <summary>
        /// 配偶
        /// </summary>
        [JsonConverter(typeof(SangoObjectListIDConverter<Person>))]
        [JsonProperty]
        public SangoObjectList<Person> SpouseList { get; private set; }

        /// <summary>
        /// 兄弟
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Person>))]
        [JsonProperty]
        public Person Brother { get; set; }

        /// <summary>
        /// 兄弟
        /// </summary>
        public List<Person> BrotherList;

        /// <summary>
        /// 喜欢武将
        /// </summary>
        [JsonConverter(typeof(SangoObjectListIDConverter<Person>))]
        [JsonProperty]
        public SangoObjectList<Person> LikePersonList { get; private set; }

        /// <summary>
        /// 厌恶武将
        /// </summary>
        [JsonConverter(typeof(SangoObjectListIDConverter<Person>))]
        [JsonProperty]
        public SangoObjectList<Person> HatePersonList { get; private set; }

        /// <summary>
        /// 儿子们, 由father属性添加至父亲的属性里
        /// </summary>
        public SangoObjectList<Person> sonList = new SangoObjectList<Person>();

        /// <summary>
        /// 矛
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAbilityValueConverter))]
        public PersonAbilityValue spearLv = new PersonAbilityValue();

        /// <summary>
        /// 戟
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAbilityValueConverter))]
        public PersonAbilityValue halberdLv = new PersonAbilityValue();

        /// <summary>
        /// 弓弩
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAbilityValueConverter))]
        public PersonAbilityValue crossbowLv = new PersonAbilityValue();

        /// <summary>
        /// 骑
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAbilityValueConverter))]
        public PersonAbilityValue rideLv = new PersonAbilityValue();

        /// <summary>
        /// 水军
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAbilityValueConverter))]
        public PersonAbilityValue waterLv = new PersonAbilityValue();

        /// <summary>
        /// 器械
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(PersonAbilityValueConverter))]
        public PersonAbilityValue machineLv = new PersonAbilityValue();

        /// <summary>
        /// 行动标记
        /// </summary>
        [JsonProperty] public BitCheck32 actionFlag = new BitCheck32();

        /// <summary>
        /// 武将特性
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(SangoObjectListIDConverter<Feature>))]
        public SangoObjectList<Feature> FeatureList { get; private set; }

        /// <summary>
        /// 库存
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(ItemStoreConverter))]
        public ItemStore itemStore = new ItemStore();

        /// <summary>
        /// 装备的武器
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Equipment>))]
        [JsonProperty]
        public Equipment EquippedWeapon { get; set; }

        /// <summary>
        /// 装备的马
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Equipment>))]
        [JsonProperty]
        public Equipment EquippedHorse { get; set; }

        /// <summary>
        /// 装备的铠甲
        /// </summary>
        [JsonConverter(typeof(Id2ObjConverter<Equipment>))]
        [JsonProperty]
        public Equipment EquippedArmor { get; set; }

        [JsonProperty]
        public int bannedForceId;

        [JsonProperty]
        [JsonConverter(typeof(Id2ObjConverter<Building>))]
        public Building workingBuilding;

        public bool HasItem(int itemTypeId)
        {
            return itemStore.GetNumber(itemTypeId) > 0;
        }

        public bool IsLeader => state == (int)PersonStateType.Leader || state == (int)PersonStateType.Governor;
        public bool IsGovernor => state == (int)PersonStateType.Governor;

        public void SetStateNormal() { state = (int)PersonStateType.Normal; }
        public void SetStateLeader()
        {
            if (IsGovernor) return;
            state = (int)PersonStateType.Leader;
        }

        /// <summary>
        /// 枪兵适应
        /// </summary>
        public int SpearLv => spearLv.value;

        /// <summary>
        /// 盾兵适应
        /// </summary>
        public int HalberdLv => halberdLv.value;

        /// <summary>
        /// 弓兵适应
        /// </summary>
        public int CrossbowLv => crossbowLv.value;

        /// <summary>
        /// 骑兵适应
        /// </summary>
        public int RideLv => rideLv.value;

        /// <summary>
        /// 水军适应
        /// </summary>
        public int WaterLv => waterLv.value;

        /// <summary>
        /// 兵器适应
        /// </summary>
        public int MachineLv => machineLv.value;

        /// <summary>
        /// 统率
        /// </summary>
        public int Command => command.value + GetEquipmentBonus(x => x.commandBonus);

        /// <summary>
        /// 武力
        /// </summary>
        public int Strength => strength.value + GetEquipmentBonus(x => x.strengthBonus);

        /// <summary>
        /// 智力
        /// </summary>
        public int Intelligence => intelligence.value + GetEquipmentBonus(x => x.intelligenceBonus);

        /// <summary>
        /// 政治
        /// </summary>
        public int Politics => politics.value + GetEquipmentBonus(x => x.politicsBonus);

        /// <summary>
        /// 魅力
        /// </summary>
        public int Glamour => glamour.value + GetEquipmentBonus(x => x.glamourBonus);

        /// <summary>
        /// 是否可登场
        /// </summary>
        public virtual bool IsValid => state > 0 && state != (int)PersonStateType.Invalid && state != (int)PersonStateType.Dead;

        /// <summary>
        /// 兵力上限其他更改值(道具等加持)
        /// </summary>
        public int troopsLimitExtra = 0;

        /// <summary>
        /// 带兵上限,根据官职和国家科技决定
        /// </summary>
        public int TroopsLimit
        {
            //TODO: 增加国家科技加持
            get { return Official.troopsLimit + Level.troops + troopsLimitExtra; }
        }

        /// <summary>
        /// 军事能力
        /// </summary>
        public int MilitaryAbility
        {
            get { return Command * 2 + Strength * 3; }
        }

        /// <summary>
        /// 商业能力
        /// </summary>
        public int BaseCommerceAbility => Intelligence;

        /// <summary>
        /// 巡视能力
        /// </summary>
        public int BaseSecurityAbility => Command;

        /// <summary>
        /// 训练能力
        /// </summary>
        public int BaseTrainTroopAbility => Strength;

        /// <summary>
        /// 农业能力
        /// </summary>
        public int BaseAgricultureAbility => Politics;

        /// <summary>
        /// 建设能力
        /// </summary>
        public int BaseBuildAbility => Politics;

        /// <summary>
        /// 生产能力
        /// </summary>
        public int BaseCreativeAbility => Intelligence;

        /// <summary>
        /// 搜寻能力
        /// </summary>
        public int BaseSearchingAbility
        {
            get
            {
                return (Politics + Glamour) / 2;
            }
        }

        /// <summary>
        /// 招募能力
        /// </summary>
        public int BaseRecruitmentAbility => Glamour;


        public void OnPersonAgeUpdate(Scenario scenario)
        {
            if (scenario.Variables.AgeEnabled)
            {
                Age = scenario.Info.year - yearBorn;
            }
            else
            {
                Age = 25;
            }

            if (scenario.Variables.EnableAgeAbilityFactor)
            {
                command.Update(); strength.Update(); intelligence.Update(); politics.Update(); glamour.Update();
                //spearLv.Update(); halberdLv.Update(); crossbowLv.Update(); horseLv.Update(); waterLv.Update(); machineLv.Update();
            }
        }

        public ushort skill;

        [JsonProperty] public int missionType;
        [JsonProperty] public int missionTarget;
        [JsonProperty] public int missionCounter;
        [JsonProperty] public int missionParams1;
        [JsonProperty] public int missionParams2;
        [JsonProperty] public int missionParams3;

        /// <summary>
        /// 在当前城市的停留回合数
        /// </summary>
        [JsonProperty] public int stayTurnCount;

        public bool rewardOver;

        public int Age { get; private set; }

        /// <summary>
        /// 是否空闲
        /// </summary>
        public bool IsFree { get { return BelongTroop == null && missionType == (int)MissionType.None; } }

        /// <summary>
        /// 是否在野
        /// </summary>
        public bool IsWild { get { return state == (int)PersonStateType.Unemployed; } }

        /// <summary>
        /// 是否为俘虏
        /// </summary>
        public bool IsPrisoner { get { return state == (int)PersonStateType.Prisoner; } }

        /// <summary>
        /// 是否未发现
        /// </summary>
        public bool Invisible { get { return state == (int)PersonStateType.Invisible; } }

        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead { get { return state == (int)PersonStateType.Dead; } }

        public bool IsAlliance(BuildingBase other)
        {
            return IsAlliance(BelongForce, other.BelongForce);
        }

        public bool IsEnemy(BuildingBase other)
        {
            return IsEnemy(BelongForce, other.BelongForce);
        }

        public bool IsSameForce(BuildingBase other)
        {
            return IsSameForce(BelongForce, other.BelongForce);
        }

        public bool IsAlliance(Troop other)
        {
            return IsAlliance(BelongForce, other.BelongForce);
        }

        public bool IsEnemy(Troop other)
        {
            return IsEnemy(BelongForce, other.BelongForce);
        }

        public bool IsSameForce(Troop other)
        {
            return IsSameForce(BelongForce, other.BelongForce);
        }

        public bool IsSameForce(Person other)
        {
            return IsSameForce(BelongForce, other.BelongForce);
        }


        public override void OnScenarioPrepare(Scenario scenario)
        {
            if (Brother != null)
            {
                if (Brother.BrotherList == null)
                    Brother.BrotherList = new List<Person>();

                Brother.BrotherList.Add(this);
            }

            if (IsPrisoner)
            {
                if (BelongTroop != null)
                {
                    BelongTroop.captiveList.Add(this);
                }
                else if (BelongCity != null)
                {
                    BelongCity.captiveList.Add(this);
                }
            }
            else
            {
                if (IsValid && BelongCity != null)
                {

                    if (Invisible)
                    {
                        BelongCity.invisiblePersons.Add(this);
                    }
                    else if (IsWild)
                    {
                        BelongCity.wildPersons.Add(this);
                    }
                    else
                    {
                        BelongCity.Add(this);
                        if (state == (int)PersonStateType.Leader)
                        {
                            BelongCity.Leader = this;
                        }
                        else if (state == (int)PersonStateType.Governor)
                        {
                            BelongCity.Leader = this;
                        }

                        if (BelongForce != BelongCity.BelongForce || BelongCorps != BelongCity.BelongCorps)
                        {
                            Sango.Log.Error($"[{Id}]{Name}归属force:{BelongForce.Name} corps:{BelongCorps.Name}, 但在city[{BelongCity.Id}] force:{BelongCity.BelongForce.Name} corps:{BelongCity.BelongCorps.Name}");
                        }
                    }
                }

            }


            if (Father != null)
                Father.sonList.Add(this);

            //else if (this.missionType == (int)MissionType.PersonBuild)
            //{
            //    Building building = scenario.buildingSet.Get(missionTarget);
            //    building.Builder = this;
            //}

            OnPersonAgeUpdate(scenario);
        }

        public override void Init(Scenario scenario)
        {
            base.Init(scenario);

            if (Brother != null)
            {
                if (Brother == this)
                {
                    BrotherList.Sort(SangoObject.Compare);

                }
                else
                {
                    BrotherList = Brother.BrotherList;
                }
            }
        }

        public override bool OnYearStart(Scenario scenario)
        {
            OnPersonAgeUpdate(scenario);
            return base.OnYearStart(scenario);
        }

        public void UpdateMission(Scenario scenario)
        {
            if (missionType == 0) return;

            switch (missionType)
            {
                case (int)MissionType.PersonReturn:
                    {
                        City dest = scenario.citySet.Get(missionTarget);
                        if (!this.IsSameForce(dest))
                        {
                            SetMission(MissionType.PersonReturn, BelongForce.Governor.BelongCity, 1);
                            return;
                        }
                        else
                        {
                            missionCounter--;
                            if (missionCounter <= 0)
                            {
                                ClearMission();
                                dest.OnPersonReturnCity(this);
                            }
                        }
                    }
                    break;
                case (int)MissionType.PersonTransform:
                    {
                        missionCounter--;
                        if (missionCounter <= 0)
                        {
                            ClearMission();
                            BelongCity.OnPersonTransformEnd(this);
                        }
                    }
                    break;
                case (int)MissionType.PersonRecruitPerson:
                    {
                        Person dest = scenario.personSet.Get(missionTarget);
                        if (BelongCorps != null && this.IsSameForce(dest))
                        {
                            SetMission(MissionType.PersonReturn, BelongCity, 1);
                        }
                        else
                        {
                            missionCounter--;
                            if (missionCounter <= 0)
                            {
                                CityRecruitPersonEvent te = new CityRecruitPersonEvent()
                                {
                                    person = this,
                                    target = dest,
                                };
                                RenderEvent.Instance.Add(te);
                                SetMission(MissionType.PersonReturn, BelongCity, 1);
                            }
                        }
                    }
                    break;
                case (int)MissionType.PersonCreateBoat:
                    {
                        missionCounter--;
                        if (missionCounter <= 0)
                        {
                            int buildingId = missionParams1;
                            int totalValue = missionParams2;
                            ItemType itemType = scenario.GetObject<ItemType>(missionTarget);
                            BelongCity.DoJobCreateBoat(itemType, buildingId, totalValue);
                        }
                    }
                    break;
                case (int)MissionType.PersonCreateMachine:
                    {
                        missionCounter--;
                        if (missionCounter <= 0)
                        {
                            int buildingId = missionParams1;
                            int totalValue = missionParams2;
                            ItemType itemType = scenario.GetObject<ItemType>(missionTarget);
                            BelongCity.DoJobCreateMachine(itemType, buildingId, totalValue);
                        }
                    }
                    break;
                case (int)MissionType.PersonResearch:
                    {
                        missionCounter--;
                        if (missionCounter <= 0)
                        {
                            ClearMission();
                        }
                    }
                    break;
                case (int)MissionType.PersonDiplomacy:
                    {
                        // 向目标城市移动
                        missionCounter--;
                        if (missionCounter <= 0)
                        {
                            // 到达目标城市
                            //ChangeCity(targetCity);
                            // 重置计数器，准备执行外交行动
                            //missionCounter = 1;
                            // 检查是否到达目标城市
                            City targetCity = scenario.citySet.Get(missionTarget);

                            // 执行外交行动
                            Force receiverForce = scenario.forceSet.Get(missionParams1);
                            if (receiverForce == null)
                            {
                                // 完成任务，返回原城市
                                SetMission(MissionType.PersonReturn, BelongForce.Governor.BelongCity, 1);
                                return;
                            }

                            DiplomacyActionType actionType = (DiplomacyActionType)missionParams2;

                            // 计算成功率
                            int successRate = DiplomacyManager.Instance.CalculateDiplomacySuccessRate(actionType, BelongForce, receiverForce, this, missionParams3);
                            bool success = false;

                            // 根据成功率判断是否执行成功
                            if (GameRandom.Chance(successRate))
                            {
                                switch (actionType)
                                {
                                    case DiplomacyActionType.Alliance:
                                        success = DiplomacyManager.Instance.PerformAlliance(BelongForce, receiverForce);
                                        break;
                                    case DiplomacyActionType.Truce:
                                        success = DiplomacyManager.Instance.PerformTruce(BelongForce, receiverForce);
                                        break;
                                    case DiplomacyActionType.DeclareWar:
                                        success = DiplomacyManager.Instance.PerformDeclareWar(BelongForce, receiverForce);
                                        break;
                                    case DiplomacyActionType.SendGift:
                                        // 使用missionParams3存储礼物价值
                                        success = DiplomacyManager.Instance.PerformSendGift(BelongForce, receiverForce, missionParams3);
                                        break;
                                    case DiplomacyActionType.RequestTechnique:
                                        // 使用missionParams3存储技术ID
                                        success = DiplomacyManager.Instance.PerformRequestTechnique(BelongForce, receiverForce, missionParams3);
                                        break;
                                    case DiplomacyActionType.RequestTroops:
                                        // 使用missionParams3存储兵力数量
                                        success = DiplomacyManager.Instance.PerformRequestTroops(BelongForce, receiverForce, missionParams3);
                                        break;
                                    case DiplomacyActionType.Trade:
                                        success = DiplomacyManager.Instance.PerformTrade(BelongForce, receiverForce);
                                        break;
                                    case DiplomacyActionType.Marriage:
                                        success = DiplomacyManager.Instance.PerformMarriage(BelongForce, receiverForce);
                                        break;
                                    case DiplomacyActionType.AllianceRequest:
                                        success = DiplomacyManager.Instance.PerformAllianceRequest(BelongForce, receiverForce);
                                        break;
                                    case DiplomacyActionType.TruceRequest:
                                        success = DiplomacyManager.Instance.PerformTruceRequest(BelongForce, receiverForce);
                                        break;
                                    case DiplomacyActionType.Ransom:
                                        // 使用missionParams3存储赎金
                                        success = DiplomacyManager.Instance.PerformRansom(BelongForce, receiverForce, missionParams3);
                                        break;
                                }
                            }

                            // 输出调试信息
                            if (success)
                            {
#if SANGO_DEBUG
                                Sango.Log.Print($"@外交@{BelongForce.Name} 对 {receiverForce.Name} 的{DiplomacyManager.Instance.GetActionName(actionType)}行动成功了！成功率: {successRate}%");
#endif
                            }
                            else
                            {
#if SANGO_DEBUG
                                Sango.Log.Print($"@外交@{BelongForce.Name} 对 {receiverForce.Name} 的{DiplomacyManager.Instance.GetActionName(actionType)}行动失败了！成功率: {successRate}%");
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

                                DiplomacyManager.Instance.ReduceRelation(BelongForce, receiverForce, relationDecrease);

#if SANGO_DEBUG
                                Sango.Log.Print($"@外交@{BelongForce.Name} 与 {receiverForce.Name} 的关系减少了 {relationDecrease}！");
#endif
                            }
                            // 完成任务，返回原城市
                            SetMission(MissionType.PersonReturn, BelongForce.Governor.BelongCity, 1);
                        }
                    }
                    break;
            }
        }
        public void SetMission(MissionType missionType, SangoObject missionTarget, int missionCounter, int p1, int p2, int p3)
        {
            this.missionType = (int)missionType;
            this.missionTarget = missionTarget.Id;
            this.missionCounter = missionCounter;
            this.missionParams1 = p1;
            this.missionParams2 = p2;
            this.missionParams3 = p3;
        }

        public void SetMission(MissionType missionType, SangoObject missionTarget, int missionCounter, int p1, int p2)
        {
            this.missionType = (int)missionType;
            this.missionTarget = missionTarget.Id;
            this.missionCounter = missionCounter;
            this.missionParams1 = p1;
            this.missionParams2 = p2;
        }

        public void SetMission(MissionType missionType, SangoObject missionTarget, int missionCounter, int p1)
        {
            this.missionType = (int)missionType;
            this.missionTarget = missionTarget.Id;
            this.missionCounter = missionCounter;
            this.missionParams1 = p1;
        }

        public void SetMission(MissionType missionType, SangoObject missionTarget, int missionCounter)
        {
            this.missionType = (int)missionType;
            this.missionTarget = missionTarget.Id;
            this.missionCounter = missionCounter;
        }

        public void ClearMission()
        {
            this.missionType = 0;
            this.missionTarget = 0;
            this.missionCounter = 0;
        }

        public override bool OnTurnStart(Scenario scenario)
        {
            // 在野武将移动逻辑
            if (IsWild && BelongCity != null)
            {
                stayTurnCount++;
                if (stayTurnCount > 5 && GameRandom.Chance(10)) // 10%概率
                {
                    // 随机选择一个邻接城市
                    SangoObjectList<City> neighborCities = BelongCity.NeighborList;
                    if (neighborCities.Count > 0)
                    {
                        int randomIndex = GameRandom.Random(0, neighborCities.Count - 1);
                        City targetCity = neighborCities[randomIndex];
                        if (targetCity != null)
                        {
                            // 移动到新城市
                            ChangeCity(targetCity);
                            // 重置停留时间
                            stayTurnCount = 0;
#if SANGO_DEBUG
                            Sango.Log.Print($"@人才@在野武将{Name}从{BelongCity.Name}移动到{targetCity.Name}");
#endif
                        }
                    }
                }
            }
            return base.OnTurnStart(scenario);
        }
        public override bool OnForceTurnStart(Scenario scenario)
        {
            //TODO:在野角色随机移动
            UpdateMission(scenario);
            if (BelongForce != null && IsAlive)
            {
                BelongForce.GainHegemonyPoint(1);
            }

            ActionOver = !IsFree;
            return base.OnForceTurnStart(scenario);
        }

        public override bool OnForceTurnEnd(Scenario scenario)
        {
            //TODO:在野角色随机移动
            //UpdateMission(scenario);
            return base.OnForceTurnEnd(scenario);
        }


        public void TransformToCity(City dest)
        {
            dest.allPersons.Add(this);
            SetMission(MissionType.PersonTransform, BelongCity, Math.Max(1, BelongCity.Distance(dest)));
            BelongCity?.Remove(this);
            BelongCity = dest;
            ActionOver = true;
#if SANGO_DEBUG
            Sango.Log.Print($"*{BelongForce?.Name}的{Name}从{BelongCity.Name}向{dest.Name}转移*");
#endif
        }

        public Corps ChangeCorps(Corps corps)
        {
            Corps last = null;
            if (BelongCorps != corps)
            {
                last = BelongCorps;
                BelongCorps = corps;
                if (BelongForce != corps.BelongForce)
                {
                    BelongForce = corps.BelongForce;
                }
            }
            return last;
        }

        public City ChangeCity(City city)
        {
            City last = null;
            if (BelongCity != city)
            {
                last = BelongCity;
#if SANGO_DEBUG
                Sango.Log.Print($"*{BelongForce?.Name}的{Name}从{BelongCity.Name}向{city.Name}转移* 移动完成!!");
#endif
                if (!IsWild)
                {
                    if (BelongCity.allPersons.Contains(this))
                        BelongCity.allPersons.Remove(this);

                    city.allPersons.Add(this);
                    BelongCity = city;
                    if (BelongCorps != city.BelongCorps)
                    {
                        BelongCorps = city.BelongCorps;
                        if (BelongForce != city.BelongForce)
                        {
                            BelongForce = city.BelongForce;
                        }
                    }

                    if (!IsGovernor)
                    {
                        SetStateNormal();
                    }
                }
                else
                {
                    BelongCity.wildPersons.Remove(this);
                    city.wildPersons.Add(this);
                    BelongCity = city;
                }

                BelongTroop?.OnPersonChangeCity(this, last, city);
            }
            return last;
        }

        public bool JobRecruitPerson(Person person, City targetCity, int type)
        {
            int probability = GameFormula.Instance.RecruitPersonProbability(this, person, type);
#if SANGO_DEBUG
            Sango.Log.Print($"[{BelongForce.Name}]<{Name}>招募 -> {person.Name} 成功率:{probability}");
#endif
            //TODO: 招募成功概率计算
            bool success = GameRandom.Chance(probability);
            if (success)
            {
#if SANGO_DEBUG
                Sango.Log.Print($"[{BelongForce.Name}]<{Name}>招募成功, {person.Name}加入了势力{BelongForce.Name}");
#endif
                person.loyalty = 80;
                // 如果在部队里,如果是主将则带部队加入,如果为副将则退出部队
                Troop personTroop = person.BelongTroop;
                if (personTroop != null)
                {
                    if (person == personTroop.Leader)
                    {
                        personTroop.JoinToForce(targetCity);
                        personTroop.ActionOver = true;
                    }
                    else
                    {
                        personTroop.RemovePerson(person);
                        if (!person.JoinToForce(targetCity))
                        {
                            person.SetMission(MissionType.PersonReturn, targetCity, 1);
                        }
                    }
                    personTroop.Render?.UpdateRender();
                }
                else
                {
                    // 有归属
                    if (!person.JoinToForce(targetCity))
                    {
                        person.SetMission(MissionType.PersonReturn, targetCity, 1);
                    }
                }
                person.ActionOver = true;
            }
            ScenarioVariables variables = Scenario.Cur.Variables;
            int jobId = (int)CityJobType.RecruitPerson;
            int meritGain = JobType.GetJobLimit(jobId);
            int techniquePointGain = JobType.GetJobTPGain(jobId);
            merit += meritGain;
            BelongForce?.GainTechniquePoint(techniquePointGain);
            ActionOver = true;
            return success;
        }

        public bool JobRecruitPerson(Person person, int type)
        {
            return JobRecruitPerson(person, BelongCity, type);
        }

        /// <summary>
        /// 加入某个势力,需要指定一个城市
        /// </summary>
        /// <param name="city"></param>
        public bool JoinToForce(City city)
        {
            // 先从原有势力移除
            if (BelongCorps != null)
            {
                BelongCity.allPersons.Remove(this);
            }
            else
            {
                if (BelongCity != null)
                    BelongCity.wildPersons.Remove(this);
            }

            bool isSameCity = BelongCity == city;
            BelongCity = city; ;
            BelongCorps = city.BelongCorps;
            BelongForce = city.BelongForce;
            Official = Scenario.Cur.CommonData.Officials.Get(0);
            state = (int)PersonStateType.Normal;
            BelongCity.allPersons.Add(this);

            return isSameCity;
        }

        /// <summary>
        /// 下野
        /// </summary>
        public void LeaveToWild()
        {
            workingBuilding = null;
            loyalty = 0;
            BelongCity.allPersons.Remove(this);
            Official = Scenario.Cur.CommonData.Officials.Get(0);
            state = (int)PersonStateType.Unemployed;
            // 关卡和港口的武将下野到对应的城池里
            if (BelongCity.BelongCity != null)
            {
                BelongCity.BelongCity.wildPersons.Add(this);
#if SANGO_DEBUG
                Sango.Log.Print($"@人才@[{BelongForce.Name}]的<{Name}>下野至{BelongCity.BelongCity.Name}");
#endif
            }
            else
            {
                BelongCity.wildPersons.Add(this);
#if SANGO_DEBUG
                Sango.Log.Print($"@人才@[{BelongForce.Name}]的<{Name}>下野至{BelongCity.Name}");
#endif
            }
            BelongCorps = null;
            BelongForce = null;
            BelongTroop = null;
        }

        public Person BeCaptive(City city)
        {
            BelongCity.allPersons.Remove(this);
            state = (int)PersonStateType.Prisoner;
            city.captiveList.Add(this);
            this.BelongForce.CaptiveList.Add(this);
            return this;
        }

        public Person BeCaptive(Troop troop)
        {
            BelongCity.allPersons.Remove(this);
            state = (int)PersonStateType.Prisoner;
            troop.captiveList.Add(this);
            this.BelongForce.CaptiveList.Add(this);
            return this;
        }

        public Person Escape()
        {
            // 有归属的武将
            if (BelongForce != null && BelongForce.IsAlive)
            {
                City where = BelongCity;
                if (BelongTroop != null)
                {
                    where = BelongTroop.cell.BelongCity;
                }
                else
                {
                    BelongCity.wildPersons.Add(this);
                }
                ChangeCity(BelongForce.Governor.BelongCity);
                ChangeCorps(BelongForce.Governor.BelongCorps);
                state = (int)PersonStateType.Normal;
                SetMission(MissionType.PersonReturn, BelongCity, DistanceDays(where));
            }
            else
            {
                state = (int)PersonStateType.Unemployed;
                if (BelongCity.allPersons.Contains(this))
                    BelongCity.allPersons.Remove(this);
                // 下野
                if (BelongTroop != null)
                {
                    BelongTroop.cell.BelongCity.wildPersons.Add(this);
                }
                else
                {
                    BelongCity.wildPersons.Add(this);
                }
            }
            return this;
        }


        /// <summary>
        /// 获取经验
        /// </summary>
        /// <param name="add"></param>
        public void GainExp(int add)
        {
            Exp += add;
            while (Level.exp > 0)
            {
                if (Exp > Level.exp)
                {
                    Exp = Level.exp - Exp;
                    Level = Level.Next;
#if SANGO_DEBUG
                    Sango.Log.Print($"@个人@{Name}升级到{Level.Id}级");
#endif
                    GameEvent.OnPersonLevelUp?.Invoke(this);
                }
                else
                    break;
            }
        }

        public bool HasFeatrue(int id)
        {
            if (FeatureList == null || FeatureList.Count == 0) return false;
            return FeatureList.Contains(id);
        }

        public bool HasFeatrue(int[] ids)
        {
            if (FeatureList == null || FeatureList.Count == 0) return false;
            if (ids == null) return false;
            for (int i = 0; i < ids.Length; i++)
            {
                if (FeatureList.Contains(ids[i])) return true;
            }
            return false;
        }

        public int Distance(Person other)
        {
            if (other == null) return 0;
            Cell cell = BelongTroop != null ? BelongTroop.cell : BelongCity.CenterCell;
            Cell otherCell = other.BelongTroop != null ? other.BelongTroop.cell : other.BelongCity.CenterCell;
            return cell.Distance(otherCell);
        }

        public int DistanceDays(Person other)
        {
            if (other == null) return 0;
            City otherCity = other.BelongTroop != null ? other.BelongTroop.cell.BelongCity : other.BelongCity;
            City thisCity = BelongTroop != null ? BelongTroop.cell.BelongCity : BelongCity;
            return otherCity.Distance(thisCity);
        }

        public int DistanceDays(City otherCity)
        {
            if (otherCity == null) return 0;
            City thisCity = BelongTroop != null ? BelongTroop.cell.BelongCity : BelongCity;
            return otherCity.Distance(thisCity);
        }

        public int CompatibilityDistance(Person other)
        {
            if (other == null) return 0;
            return Math.Abs(compatibility - (other.compatibility));
        }

        public bool IsLike(Person other)
        {
            if (other == null || LikePersonList == null) return false;
            return LikePersonList.Contains(other);
        }

        public bool IsHate(Person other)
        {
            if (other == null || HatePersonList == null) return false;
            return HatePersonList.Contains(other);
        }

        public bool IsBrother(Person other)
        {
            if (other == null || BrotherList == null) return false;
            return BrotherList.Contains(other);
        }

        public bool IsParentchild(Person other)
        {
            if (other == null) return false;
            if (other.Father == this) return true;
            if (other.Mother == this) return true;
            if (Father == other) return true;
            if (Mother == other) return true;
            return false;
        }

        public void Dead()
        {
            state = (int)PersonStateType.Dead;
            if (BelongCity != null)
            {
                BelongCity.allPersons.Remove(this);
                BelongCity.wildPersons.Remove(this);
            }

            if (IsPrisoner)
            {
                if (BelongTroop != null)
                {
                    BelongTroop.captiveList.Remove(this);
                }
                else
                    BelongCity.captiveList.Remove(this);
            }
            else if (BelongTroop != null)
            {
                BelongTroop.RemovePerson(this);
            }
        }

        public int GetAttribute(int attrType)
        {
            switch (attrType)
            {
                case 0:// (int)AttributeType.Command:
                    return Command;
                case 1:// (int)AttributeType.Strength:
                    return Strength;
                case 2:// (int)AttributeType.Intelligence:
                    return Intelligence;
                case 3:// (int)AttributeType.Politics:
                    return Politics;
                case 4:// (int)AttributeType.Glamour:
                    return Glamour;
            }
            return 0;
        }

        /// <summary>
        /// 获取装备的属性加成
        /// </summary>
        /// <param name="getBonus">获取单个装备加成的委托</param>
        /// <returns>总加成值</returns>
        private int GetEquipmentBonus(System.Func<Equipment, int> getBonus)
        {
            int bonus = 0;

            if (EquippedWeapon != null)
            {
                bonus += getBonus(EquippedWeapon);
            }

            if (EquippedHorse != null)
            {
                bonus += getBonus(EquippedHorse);
            }

            if (EquippedArmor != null)
            {
                bonus += getBonus(EquippedArmor);
            }

            return bonus;
        }

        /// <summary>
        /// 装备武器
        /// </summary>
        /// <param name="weapon">武器</param>
        public void EquipWeapon(Equipment weapon)
        {
            if (weapon != null && weapon.kind == (int)ItemKindType.Equipment_Weapon)
            {
                EquippedWeapon = weapon;
            }
        }

        /// <summary>
        /// 装备马
        /// </summary>
        /// <param name="horse">马</param>
        public void EquipHorse(Equipment horse)
        {
            if (horse != null && horse.kind == (int)ItemKindType.Equipment_Horse)
            {
                EquippedHorse = horse;
            }
        }

        /// <summary>
        /// 装备铠甲
        /// </summary>
        /// <param name="armor">铠甲</param>
        public void EquipArmor(Equipment armor)
        {
            if (armor != null && armor.kind == (int)ItemKindType.Equipment_Armor)
            {
                EquippedArmor = armor;
            }
        }

        /// <summary>
        /// 卸下武器
        /// </summary>
        public void UnequipWeapon()
        {
            EquippedWeapon = null;
        }

        /// <summary>
        /// 卸下马
        /// </summary>
        public void UnequipHorse()
        {
            EquippedHorse = null;
        }

        /// <summary>
        /// 卸下铠甲
        /// </summary>
        public void UnequipArmor()
        {
            EquippedArmor = null;
        }
    }
}
