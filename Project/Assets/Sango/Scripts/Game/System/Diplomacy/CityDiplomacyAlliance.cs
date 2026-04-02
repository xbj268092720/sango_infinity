using Sango.UI;
using System.Collections.Generic;

namespace Sango.Core.Player
{
    [GameSystem]
    public class CityDiplomacyAlliance : CityBaseSystem
    {
        public List<Person> counsellorRecommendList = new List<Person>();

        public CityDiplomacyAlliance()
        {
            customTitleName = "结盟";

            customMenuName = "外交/结盟";
            customMenuOrder = 301;
            windowName = "window_city_diplomacy_alliance";

        }

        public override bool IsValid
        {
            get
            {
                return TargetCity.freePersons.Count > 0 && TargetCity.BelongCorps.ActionPoint >= 1;
            }
        }

        public override void OnEnter()
        {
            counsellorRecommendList.Clear();
            Person[] recommandList = ForceAI.CounsellorRecommendDiplomacy(TargetCity.freePersons);
            if (recommandList != null)
            {
                for (int i = 0; i < recommandList.Length; i++)
                {
                    counsellorRecommendList.Add(recommandList[i]);
                }
            }

            personList.Clear();
            if (counsellorRecommendList.Count > 0)
                personList.Add(counsellorRecommendList[0]);

            if (customTitleList == null)
            {
                customTitleList = new List<ObjectSortTitle>()
                {
                    PersonSortFunction.SortByName,
                    PersonSortFunction.GetSortByContainsInList("军师推荐", counsellorRecommendList),
                    PersonSortFunction.SortByPolitics,
                    PersonSortFunction.SortByGlamour,
                };
            }

            Window.Instance.Open(windowName);
        }

        public override void OnDestroy()
        {
            GameDialog.Close();
            Window.Instance.Close(windowName);
        }

        public override void DoJob()
        {
            if (personList.Count <= 0)
                return;

            // 这里应该调用外交系统的结盟方法
            // 暂时留空，等待具体实现

            Done();
        }
    }
}