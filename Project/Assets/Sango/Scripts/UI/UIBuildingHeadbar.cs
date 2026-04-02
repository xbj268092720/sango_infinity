using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;
namespace Sango.Game.Render.UI

{
    public class UIBuildingHeadbar : UIBuildingBaseHeadbar
    {
        public UIWorker[] workers;
        public RectTransform workerNode;

        public void ShowWorker(Building building)
        {
            int workerCount = building.BuildingType.workerLimit;
            for (int i = 0; i < workers.Length; i++)
            {
                UIWorker uIWorker = workers[i];
                if (i < workerCount || (building.Workers != null && !building.isComplate && i < building.Workers.Count))
                {
                    if (building.Workers != null && i < building.Workers.Count)
                    {
                        uIWorker.SetEnabled(true);
                        uIWorker.SetPerson(building.Workers.Get(i));
                    }
                    else
                    {
                        uIWorker.SetEnabled(false);
                        uIWorker.SetPerson(null);
                    }
                }
                else
                {
                    uIWorker.SetEnabled(false);
                }
            }
        }

        public override void UpdateState(BuildingBase building)
        {
            base.UpdateState(building);
            ShowWorker(building as Building);
        }
    }
}
