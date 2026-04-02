using Sango.Core.Player;
using System.Collections.Generic;
using System.Text;

namespace Sango.Core
{
   
    public enum ForceSortGroupType : int
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

    public class ForceSortFunction : Singleton<ForceSortFunction>
    {
        public delegate string ForceValueStrGet(Force force);
        public delegate int ForceValueGet(Force force);
        public delegate int ForceSortFunc(Force force1, Force force2);

        public Force CurForce;

        public class SortTitle : ObjectSortTitle
        {
            public ForceValueStrGet valueStrGetCall;
            public ForceSortFunc valueSortFunc;

            public override string GetValueStr(SangoObject obj)
            {
                return valueStrGetCall.Invoke((Force)obj);
            } 

            public override int Sort(SangoObject a, SangoObject b)
            {
                return valueSortFunc.Invoke((Force)a, (Force)b);
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

        public void GetSortTitleGroup(ForceSortGroupType forceSortTileGroupType, List<ObjectSortTitle> titleList)
        {
            switch (forceSortTileGroupType)
            {
                case ForceSortGroupType.State:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case ForceSortGroupType.FightPower:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case ForceSortGroupType.Item:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case ForceSortGroupType.Gold:
                    {
                        titleList.Add(SortByName);
                        break;
                    }
                case ForceSortGroupType.Food:
                    {

                        titleList.Add(SortByName);
                        break;
                    }
                case ForceSortGroupType.Disaster:
                    {

                        titleList.Add(SortByName);
                        break;
                    }
            }
        }

        public string GetSortTitleGroupName(ForceSortGroupType forceSortTileGroupType)
        {
            switch (forceSortTileGroupType)
            {
                case ForceSortGroupType.State: return "状态";
                case ForceSortGroupType.FightPower: return "战力";
                case ForceSortGroupType.Item: return "兵装";
                case ForceSortGroupType.Gold: return "资金";
                case ForceSortGroupType.Food: return "兵粮";
                case ForceSortGroupType.Disaster: return "灾害";
            }

            return "";
        }

        public static SortTitle SortByName = new SortTitle()
        {
            name = "势力",
            width = 100,
            valueStrGetCall = x => x.Name,
            valueSortFunc = (a, b) => a.Name.CompareTo(b.Name),
        };

        public static SortTitle SortByLeader = new SortTitle()
        {
            name = "主公",
            width = 100,
            valueStrGetCall = x => x.Governor?.Name ?? "---",
            valueSortFunc = (a, b) => SangoObject.Compare(a.Governor, b.Governor),
        };

        public static SortTitle GetSortByDistanceDay(City where)
        {
            return new SortTitle()
            {
                name = "期间",
                width = 50,
                valueStrGetCall = x => $"{x.Governor.DistanceDays(where)}0日",
                valueSortFunc = (a, b) => a.Governor.DistanceDays(where).CompareTo(b.Governor.DistanceDays(where)),
            };
        }
    }
}
