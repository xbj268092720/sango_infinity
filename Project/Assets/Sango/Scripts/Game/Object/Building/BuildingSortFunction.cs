using Sango.Core.Player;
using System.Collections.Generic;
using System.Text;

namespace Sango.Core
{
    public enum BuildingSortTileType : int
    {
        Name = 0,

    }

    public enum BuildingSortGroupType : int
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

    public class BuildingSortFunction : Singleton<BuildingSortFunction>
    {
        public delegate string BuildingValueStrGet(Building Building);
        public delegate int BuildingValueGet(Building Building);
        public delegate int BuildingSortFunc(Building Building1, Building Building2);

        public Building CurBuilding;

        public class SortTitle : ObjectSortTitle
        {
            public BuildingValueStrGet valueStrGetCall;
            public BuildingSortFunc valueSortFunc;

            public override string GetValueStr(SangoObject obj)
            {
                return valueStrGetCall.Invoke((Building)obj);
            }

            public override int Sort(SangoObject a, SangoObject b)
            {
                return valueSortFunc.Invoke((Building)a, (Building)b);
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

        public void GetSortTitleGroup(BuildingSortGroupType BuildingSortTileGroupType, List<ObjectSortTitle> titleList)
        {
            switch (BuildingSortTileGroupType)
            {
                case BuildingSortGroupType.State:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case BuildingSortGroupType.FightPower:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case BuildingSortGroupType.Item:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case BuildingSortGroupType.Gold:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case BuildingSortGroupType.Food:
                    {

                        titleList.Add(SortByName);
                        break;
                    }
                case BuildingSortGroupType.Disaster:
                    {

                        titleList.Add(SortByName);
                        break;
                    }
            }
        }

        public string GetSortTitleGroupName(BuildingSortGroupType BuildingSortTileGroupType)
        {
            switch (BuildingSortTileGroupType)
            {
                case BuildingSortGroupType.State: return "状态";
                case BuildingSortGroupType.FightPower: return "战力";
                case BuildingSortGroupType.Item: return "兵装";
                case BuildingSortGroupType.Gold: return "资金";
                case BuildingSortGroupType.Food: return "兵粮";
                case BuildingSortGroupType.Disaster: return "灾害";
            }

            return "";
        }


        public static SortTitle SortByName = new SortTitle()
        {
            name = "建筑",
            width = 100,
            valueStrGetCall = x => x.Name,
            valueSortFunc = (a, b) => a.Name.CompareTo(b.Name),
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

        public static SortTitle SortByBelongBuilding = new SortTitle()
        {
            name = "所属",
            width = 60,
            valueStrGetCall = x => x.BelongCity?.Name ?? "无",
            valueSortFunc = (a, b) => SangoObject.Compare(a.BelongCity, b.BelongCity),
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

        public static SortTitle GetSortByWorkSlot(int slotIndex)
        {
            return new SortTitle()
            {
                name = slotIndex == 0 ? "工作" : "",
                width = 50,
                valueStrGetCall = x => x.GetWorker(slotIndex) != null ? x.GetWorker(slotIndex).Name : "-",
                valueSortFunc = (a, b) => SangoObject.Compare(a.GetWorker(slotIndex), a.GetWorker(slotIndex)),
            };
        }
    }
}
