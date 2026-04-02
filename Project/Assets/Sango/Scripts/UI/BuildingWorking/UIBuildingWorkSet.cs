using TKNewtonsoft.Json.Utilities.LinqBridge;
using Sango.Game.Player;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIBuildingWorkSet : UGUIWindow
    {
        public Text windiwTitle;

        public UIPersonItem[] personItems;

        public UITextField value_target;
        public UITextField value_attr;
        public UITextField value_product;
        public UITextField value_factor;

        public UITextField action_value;

        BuildingWorking currentSystem;
        Building TargetBuilding { get; set; }

        List<Person> selectedPersonList = new List<Person>();

        public override void OnShow()
        {
            currentSystem = GameSystem.GetSystem<BuildingWorking>();
            windiwTitle.text = currentSystem.customTitleName;
            TargetBuilding = currentSystem.TargetBuilding;
            BuildingType targetBuildingType = TargetBuilding.BuildingType;
            selectedPersonList.Clear();
            if (TargetBuilding.Workers != null)
                TargetBuilding.Workers.ForEach(p =>
                {
                    selectedPersonList.Add(p);
                });

            ShowContent();
        }

        void ShowContent()
        {
            BuildingType targetBuildingType = TargetBuilding.BuildingType;
            for (int i = 0; i < personItems.Length; ++i)
            {
                UIPersonItem personItem = personItems[i];
                personItem.gameObject.SetActive(i < targetBuildingType.workerLimit);
                if (i < selectedPersonList.Count)
                {
                    personItems[i].SetPerson(selectedPersonList[i]);
                }
                else
                {
                    personItems[i].SetPerson(null);
                }
            }

            value_target.text = TargetBuilding.Name;
            value_attr.text = Scenario.Cur.Variables.GetAttributeNameWithColor(targetBuildingType.effectAttrType);

        }


        public void OnSure()
        {
            currentSystem.SetBuildingWorker(TargetBuilding, selectedPersonList);
            TargetBuilding.Render?.UpdateRender();
            currentSystem.Done();
        }

        public void OnCancel()
        {
            currentSystem.Exit();
        }

        public virtual void OnSelectPerson()
        {
            GameSystem.GetSystem<PersonSelectSystem>().Start(TargetBuilding.BelongCity.freePersons,
               selectedPersonList, TargetBuilding.BuildingType.workerLimit, OnPersonChange, currentSystem.customTitleList[TargetBuilding.BuildingType.effectAttrType], currentSystem.customTitleName);

        }

        public virtual void OnPersonChange(List<Person> personList)
        {
            selectedPersonList = personList;
            ShowContent();
        }
    }
}
