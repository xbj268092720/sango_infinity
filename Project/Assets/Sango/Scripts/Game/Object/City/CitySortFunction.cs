using Sango.Core.Player;
using System.Collections.Generic;
using System.Text;

namespace Sango.Core
{
    public enum CitySortTileType : int
    {
        Name = 0,

    }

    public enum CitySortGroupType : int
    {
        //自定义,功能独有
        Custom = 0,
        //状态
        State,
        //战力
        FightPower,
        //兵装
        Item,
        //资金
        Gold,
        //兵粮
        Food,
        //灾害
        Disaster,

        Max
    }

    public class CitySortFunction : Singleton<CitySortFunction>
    {
        public delegate string CityValueStrGet(City city);
        public delegate int CityValueGet(City city);
        public delegate int CitySortFunc(City city1, City city2);

        public City CurCity;

        public class SortTitle : ObjectSortTitle
        {
            public CityValueStrGet valueStrGetCall;
            public CitySortFunc valueSortFunc;

            public override string GetValueStr(SangoObject obj)
            {
                return valueStrGetCall.Invoke((City)obj);
            } 

            public override int Sort(SangoObject a, SangoObject b)
            {
                return valueSortFunc.Invoke((City)a, (City)b);
            }

            public SortTitle Copy()
            {
                return new SortTitle
                {
                    name = name,
                    alignment = alignment,
                    width = width,
                    valueStrGetCall = valueStrGetCall,
                    valueSortFunc = valueSortFunc,
                };
            }
        }

        public void GetSortTitleGroup(CitySortGroupType citySortTileGroupType, List<ObjectSortTitle> titleList)
        {
            switch (citySortTileGroupType)
            {
                case CitySortGroupType.State:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case CitySortGroupType.FightPower:
                    {
                        titleList.Add(SortByName);
                        titleList.Add(SortByTroopsLimit);
                        break;
                    }
                case CitySortGroupType.Item:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case CitySortGroupType.Gold:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case CitySortGroupType.Food:
                    {

                        titleList.Add(SortByName);
                        break;
                    }
                case CitySortGroupType.Disaster:
                    {

                        titleList.Add(SortByName);
                        break;
                    }
            }
        }

        public string GetSortTitleGroupName(CitySortGroupType citySortTileGroupType)
        {
            switch (citySortTileGroupType)
            {
                case CitySortGroupType.State: return "状态";
                case CitySortGroupType.FightPower: return "战力";
                case CitySortGroupType.Item: return "兵装";
                case CitySortGroupType.Gold: return "资金";
                case CitySortGroupType.Food: return "兵粮";
                case CitySortGroupType.Disaster: return "灾害";
            }

            return "";
        }


        public static SortTitle SortByName = new SortTitle()
        {
            name = "城池",
            width = 100,
            valueStrGetCall = x => x.Name,
            valueSortFunc = (a, b) => a.Name.CompareTo(b.Name),
        };

        public static SortTitle SortByLeader = new SortTitle()
        {
            name = "太守",
            width = 100,
            valueStrGetCall = x => x.Leader?.Name ?? "---",
            valueSortFunc = (a, b) => SangoObject.Compare(a.Leader, b.Leader),
        };

        public static SortTitle SortByPersonCount = new SortTitle()
        {
            name = "现役",
            width = 60,
            valueStrGetCall = x => x.allPersons.Count.ToString(),
            valueSortFunc = (a, b) => a.allPersons.Count.CompareTo(b.allPersons.Count),
        };

        public static SortTitle SortByTroops = new SortTitle()
        {
            name = "士兵",
            width = 100,
            valueStrGetCall = x => x.troops.ToString(),
            valueSortFunc = (a, b) => a.troops.CompareTo(b.troops),
        };

        public static SortTitle SortByTroopsLimit = new SortTitle()
        {
            name = "士兵上限",
            width = 100,
            valueStrGetCall = x => x.TroopsLimit.ToString(),
            valueSortFunc = (a, b) => a.TroopsLimit.CompareTo(b.TroopsLimit),
        };

        public static SortTitle SortByGold = new SortTitle()
        {
            name = "资金",
            width = 100,
            valueStrGetCall = x => x.gold.ToString(),
            valueSortFunc = (a, b) => a.gold.CompareTo(b.gold),
        };

        public static SortTitle SortByGoldLimit = new SortTitle()
        {
            name = "资金上限",
            width = 100,
            valueStrGetCall = x => x.GoldLimit.ToString(),
            valueSortFunc = (a, b) => a.GoldLimit.CompareTo(b.GoldLimit),
        };

        public static SortTitle SortByFood = new SortTitle()
        {
            name = "兵粮",
            width = 100,
            valueStrGetCall = x => x.food.ToString(),
            valueSortFunc = (a, b) => a.food.CompareTo(b.food),
        };

        public static SortTitle SortByFoodLimit = new SortTitle()
        {
            name = "兵粮上限",
            width = 100,
            valueStrGetCall = x => x.FoodLimit.ToString(),
            valueSortFunc = (a, b) => a.FoodLimit.CompareTo(b.FoodLimit),
        };

        public static SortTitle SortByLevel = new SortTitle()
        {
            name = "等级",
            width = 60,
            valueStrGetCall = x => x.CityLevelType.Name,
            valueSortFunc = (a, b) => a.CityLevelType.Id.CompareTo(b.CityLevelType.Id),
        };

        public static SortTitle SortByIsFree = new SortTitle()
        {
            name = "空闲",
            width = 60,
            valueStrGetCall = x => x.freePersons.Count.ToString(),
            valueSortFunc = (a, b) => a.freePersons.Count.CompareTo(b.freePersons.Count),
        };

        public static SortTitle SortByCaptiveCount = new SortTitle()
        {
            name = "俘虏",
            width = 60,
            valueStrGetCall = x => x.captiveList.Count.ToString(),
            valueSortFunc = (a, b) => a.captiveList.Count.CompareTo(b.captiveList.Count),
        };

        public static SortTitle SortByWildCount= new SortTitle()
        {
            name = "在野",
            width = 60,
            valueStrGetCall = x => x.wildPersons.Count.ToString(),
            valueSortFunc = (a, b) => a.wildPersons.Count.CompareTo(b.wildPersons.Count),
        };

        public static SortTitle SortByBelongForce = new SortTitle()
        {
            name = "势力",
            width = 60,
            valueStrGetCall = x => x.BelongForce?.Name ?? "无",
            valueSortFunc = (a, b) => SangoObject.Compare(a.BelongForce, b.BelongForce),
        };

        public static SortTitle SortByBelongCorps = new SortTitle()
        {
            name = "军团",
            width = 100,
            valueStrGetCall = x => x.BelongCorps?.Name ?? "无",
            valueSortFunc = (a, b) => SangoObject.Compare(a.BelongCorps, b.BelongCorps),
        };

        public static SortTitle SortByBelongCity = new SortTitle()
        {
            name = "所属",
            width = 60,
            valueStrGetCall = x => x.BelongCity?.Name ?? "无",
            valueSortFunc = (a, b) => SangoObject.Compare(a.BelongCity, b.BelongCity),
        };

        public static SortTitle SortBySecurity = new SortTitle()
        {
            name = "治安",
            width = 60,
            valueStrGetCall = x => x.security.ToString(),
            valueSortFunc = (a, b) => a.security.CompareTo(b.security),
        };

        public static SortTitle SortBySecurity_SecurityLimit = new SortTitle()
        {
            name = "治安",
            width = 60,
            valueStrGetCall = x => $"{x.security}/100",
            valueSortFunc = (a, b) => a.security.CompareTo(b.security),
        };


        public static SortTitle SortByDurability = new SortTitle()
        {
            name = "耐久",
            width = 60,
            valueStrGetCall = x => x.durability.ToString(),
            valueSortFunc = (a, b) => a.durability.CompareTo(b.durability),
        };

        public static SortTitle SortByDurability_DurabilityLimit = new SortTitle()
        {
            name = "耐久",
            width = 60,
            valueStrGetCall = x => $"{x.durability}/{x.DurabilityLimit}",
            valueSortFunc = (a, b) => a.durability.CompareTo(b.durability),
        };

        public static SortTitle SortByAllPersonCountInfo = new SortTitle()
        {
            name = "现役",
            width = 60,
            valueStrGetCall = x => $"{x.freePersons.Count}/{x.allPersons.Count}",
            valueSortFunc = (a, b) => a.allPersons.Count.CompareTo(b.allPersons.Count),
        };

        public static SortTitle SortByBuildingBuildCount_TotalCount = new SortTitle()
        {
            name = "设施",
            width = 100,
            valueStrGetCall = x => $"{x.GetInteriorCellUsedCount()}/{x.InteriorCellCount}",
            valueSortFunc = (a, b) => a.InteriorCellCount.CompareTo(b.InteriorCellCount),
        };

        public static SortTitle SortByMorale_MoraleLimit = new SortTitle()
        {
            name = "气力",
            width = 60,
            valueStrGetCall = x => $"{x.morale}/{x.MaxMorale}",
            valueSortFunc = (a, b) => a.morale.CompareTo(b.morale),
        };

        public static SortTitle SortByMorale = new SortTitle()
        {
            name = "气力",
            width = 60,
            valueStrGetCall = x => x.morale.ToString(),
            valueSortFunc = (a, b) => a.morale.CompareTo(b.morale),
        };

        public static SortTitle GetSortByItemId(int id)
        {
            ItemType itemType = Scenario.Cur.GetObject<ItemType>(id);
            return new SortTitle()
            {
                name = itemType.Name,
                width = 50,
                valueStrGetCall = x => x.itemStore.GetNumber(itemType.storeKind).ToString(),
                valueSortFunc = (a, b) => a.itemStore.GetNumber(itemType.storeKind).CompareTo(b.itemStore.GetNumber(itemType.storeKind)),
            };
        }

        public static SortTitle GetSortByItemId(int id, City city)
        {
            ItemType itemType = Scenario.Cur.GetObject<ItemType>(id);
            if (itemType.kind == 3 || itemType.kind == 4)
            {
                ItemType tempType = Scenario.Cur.GetObject<ItemType>(id + 1);
                if (tempType.storeKind == itemType.storeKind && tempType.IsValid(city.BelongForce))
                {
                    itemType = tempType;
                }
            }
            return new SortTitle()
            {
                name = itemType.Name,
                width = 50,
                valueStrGetCall = x => x.itemStore.GetNumber(itemType.storeKind).ToString(),
                valueSortFunc = (a, b) => a.itemStore.GetNumber(itemType.storeKind).CompareTo(b.itemStore.GetNumber(itemType.storeKind)),
            };
        }

        public static SortTitle SortByTotalGainGold = new SortTitle()
        {
            name = "资金收入",
            width = 60,
            valueStrGetCall = x => x.totalGainGold.ToString(),
            valueSortFunc = (a, b) => a.totalGainGold.CompareTo(b.totalGainGold),
        };

        public static SortTitle SortByTotalGainFood = new SortTitle()
        {
            name = "兵粮收入",
            width = 60,
            valueStrGetCall = x => x.totalGainFood.ToString(),
            valueSortFunc = (a, b) => a.totalGainFood.CompareTo(b.totalGainFood),
        };

        public static SortTitle SortByHasBusiness = new SortTitle()
        {
            name = "市价",
            width = 60,
            valueStrGetCall = x => $"兵粮{x.hasBusiness}=资金1",
            valueSortFunc = (a, b) => a.hasBusiness.CompareTo(b.hasBusiness),
        };
    }
}
