using Sango.UI;
using System.Collections.Generic;

namespace Sango.Core.Player
{
    [GameSystem]
    public class CityDiplomacyAlliance : CityBaseSystem
    {
        public List<Force> targetForces = new List<Force>();
        public List<ObjectSortTitle> customForceTitleList;
        public int gold;
        public CityDiplomacyAlliance()
        {
            customTitleName = "结盟";

            customMenuName = "外交/结盟";
            customMenuOrder = 301;
            windowName = "window_city_diplomacy_alliance";
            customTitleList = new List<ObjectSortTitle>()
            {
                PersonSortFunction.SortByName,
                PersonSortFunction.SortByPolitics,
            };


        }

        public override bool IsValid
        {
            get
            {
                return TargetCity.freePersons.Count > 0 && TargetCity.BelongCorps.ActionPoint >= JobType.GetJobCostAP((int)CityJobType.Alliance) && TargetCity.gold >= 0;
            }
        }

        public override void OnEnter()
        {
            gold = 0;
            targetForces.Clear();
            personList.Clear();
            customForceTitleList = new List<ObjectSortTitle>()
            {
                ForceSortFunction.SortByName,
                ForceSortFunction.SortByLeader,
                ForceSortFunction.GetSortByDistanceDay(TargetCity)
            };

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

            DiplomacyManager.Instance.PlayerInitiateDiplomacyAction(DiplomacyActionType.Alliance, personList[0], targetForces[0], gold);
            GameDialog.IDialog dialog1 = GameDialog.Open(GameDialog.DialogStyle.ClickPersonSay, $"交给我吧, 保证完成任务!!", () =>
            {
                // 暂时直接招募
                GameDialog.Close();
                Done();

            });
            dialog1.SetPerson(personList[0]);
        }
    }
}