using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;
namespace Sango.Game.Render.UI

{
    public class UIBuildingBaseHeadbar : UGUIWindow
    {
        public Text name;
        public Image durability;
        public UIAnimationText aniText;
        public BuildingBase building;

        GameObject durabilityObj;

        protected override void Awake()
        {
            durabilityObj = durability.transform.parent.gameObject;
        }

        public virtual void Init(BuildingBase building)
        {
            aniText = null;
            this.building = building;
            if (name != null)
                name.text = building.Name;
            UpdateState(building);
        }

        public virtual void UpdateState(BuildingBase building)
        {
            if (durability == null) return;
            durability.fillAmount = (float)building.durability / building.DurabilityLimit;
            if(building.IsIntorBuilding())
            {
                durabilityObj.SetActive(durability.fillAmount < 1);
            }
            else
            {
                durabilityObj.SetActive(true);
            }
        }

        public virtual void ShowInfo(int damage, int damageType = 13)
        {
            aniText = UIAnimationText.Show(aniText, building, damage, damageType);
            if(aniText != null)
            {
                aniText.onAnimationComplate = OnAnimationComplate;
            }
        }
        void OnAnimationComplate()
        {
            aniText = null;
        }
    }
}
