using System.Collections.Generic;

namespace Sango.Core.Player
{
    [GameSystem]
    public class CityDiplomacySendGift : CityBaseSystem
    {
        public List<Force> targetForces = new List<Force>();
        public List<ObjectSortTitle> customForceTitleList;

        public CityDiplomacySendGift()
        {
            customTitleName = "送礼";

            customMenuName = "外交/送礼";
            customMenuOrder = 304;
            windowName = "window_city_diplomacy_send_gift";
            customTitleList = new List<ObjectSortTitle>()
            {
                PersonSortFunction.SortByName,
                PersonSortFunction.SortByPolitics,
            };

            customForceTitleList = new List<ObjectSortTitle>()
            {
                ForceSortFunction.SortByName,
                ForceSortFunction.SortByLeader,
            };
        }

        public override bool IsValid
        {
            get
            {
                return TargetCity.freePersons.Count > 0 && TargetCity.BelongCorps.ActionPoint >= JobType.GetJobCostAP((int)CityJobType.SendGift) && TargetCity.gold >= 1000;
            }
        }

        public override void OnEnter()
        {
            targetForces.Clear();
            personList.Clear();
            Window.Instance.Open(windowName);
        }

        public override void OnDestroy()
        {
            GameDialog.Close();
            Window.Instance.Close(windowName);
        }

        public override void DoJob()
        {
            if (personList.Count <= 0 || targetForces.Count <= 0)
                return;

            DiplomacyManager.Instance.PerformDiplomacyAction(DiplomacyActionType.SendGift, TargetCity.BelongForce, targetForces[0], personList[0], JobType.GetJobCost((int)CityJobType.SendGift));
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