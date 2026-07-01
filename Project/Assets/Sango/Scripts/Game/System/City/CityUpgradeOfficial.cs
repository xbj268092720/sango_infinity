using System.Collections.Generic;

namespace Sango.Core.Player
{
    [GameSystem]
    public class CityUpgradeOfficial : CityBaseSystem
    {
        public List<Person> targetList = new List<Person>();

        public CityUpgradeOfficial()
        {
            customTitleName = "官职";
            customTitleList = new List<ObjectSortTitle>()
            {
                PersonSortFunction.SortByName,
                PersonSortFunction.SortByMerit,
                PersonSortFunction.SortByLoyalty,
                PersonSortFunction.SortByBelongCity,
                PersonSortFunction.SortByBelongCorps,
                PersonSortFunction.SortByLevel,
                PersonSortFunction.SortByTroopsLimit,
                PersonSortFunction.SortByCommand,
                PersonSortFunction.SortByStrength,
                PersonSortFunction.SortByIntelligence,
                PersonSortFunction.SortByPolitics,
                PersonSortFunction.SortByGlamour,
                PersonSortFunction.SortBySpearLv,
                PersonSortFunction.SortByHalberdLv,
                PersonSortFunction.SortByCrossbowLv,
                PersonSortFunction.SortByRideLv,
                PersonSortFunction.SortByWaterLv,
                PersonSortFunction.SortByMachineLv,
                PersonSortFunction.SortByFeatureList,
            };
            customMenuName = "人事/官职";
            customMenuOrder = 242;
            windowName = "window_official_set";
        }

        public override bool IsValid
        {
            get
            {
                targetList.Clear();
                if (TargetCity.gold > 500)
                {
                    TargetCity.BelongForce.ForEachPerson(x =>
                  {
                      if (x != TargetCity.BelongForce.Governor /*&& x.CanUpgradeOfficial*/)
                      {
                          targetList.Add(x);
                      }
                  });
                }
                return targetList.Count > 0;
            }
        }

        public override void OnEnter()
        {
            targetList.Sort((a, b) => -PersonSortFunction.SortByOfficial.personSortFunc.Invoke(a, b));
            base.OnEnter();
        }

        public override void RecommandPersonList()
        {
            personList.Clear();
            if (targetList.Count > 0)
            {
                personList.Add(targetList[0]);
            }
        }

        public override void OnDestroy()
        {
            GameEvent.DialogClose?.Invoke();
            base.OnDestroy();
        }

        public override void DoJob()
        {
            if (personList.Count <= 0)
                return;

            //TargetCity.JobRewardPersons(personList.ToArray());
            Done();
            GameMedia.Instance.PlayDoAcitonSfx();
        }

        public void AutoUpgrade()
        {

        }
    }
}