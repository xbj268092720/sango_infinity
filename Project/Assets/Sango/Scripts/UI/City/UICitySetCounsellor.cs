using Sango.Game.Player;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UICitySetCounsellor : UGUIWindow
    {
        public Text windiwTitle;

        public UIPersonItem nowPersonItems;
        public UIStatusItem nowStatusItem;

        public UIPersonItem targetPersonItems;
        public UIStatusItem targetStatusItem;

        CitySetCounsellor currentSystem;
        public Button sureButton;
        Force TargetForce;

        public override void OnShow()
        {
            currentSystem = GameSystem.GetSystem<CitySetCounsellor>();
            TargetForce = currentSystem.TargetForce;
            windiwTitle.text = currentSystem.customTitleName;
            UpdateNowCounsellor();
            UpdateContent();
        }

        void UpdateNowCounsellor()
        {
            nowPersonItems.SetPerson(currentSystem.counsellor);
            nowStatusItem.SetPerson(currentSystem.counsellor);
        }

        public void UpdateContent()
        {
            Person target = currentSystem.personList.Count > 0 ? currentSystem.personList[0] : null;
            sureButton.interactable = target != null;
            targetPersonItems.SetPerson(target);
            targetStatusItem.SetPerson(target);
        }

        public void OnSure()
        {
            currentSystem.DoJob();
        }

        public void OnCancel()
        {
            currentSystem.Exit();
        }

        public void OnClearCounsellor()
        {
            currentSystem.ClearCounsellor();
            UpdateNowCounsellor();
            sureButton.interactable = true;
        }

        public void OnSelectPerson()
        {
            GameSystem.GetSystem<PersonSelectSystem>().Start(currentSystem.targetList,
               currentSystem.personList, 1, OnPersonChange, currentSystem.customTitleList, currentSystem.customTitleName);
        }

        public virtual void OnPersonChange(List<Person> personList)
        {
            currentSystem.personList = personList;
            UpdateContent();
        }

    }
}
