using System.Text;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIPortHeadbar : UICityHeadbar
    {
        public override void UpdateState(BuildingBase building)
        {
            base.UpdateState(building);
            City city = (City)building;
            state.enabled = false;
            food.enabled = false;
            number.text = city.troops.ToString();
            string cityInfo = $"(人:{city.allPersons.Count} 闲:{city.freePersons.Count}) [商:{city.commerce},农:{city.agriculture},治:{city.security},建:{city.GetInteriorCellUsedCount()}/{city.InteriorCellCount}]\n<金:{city.gold}+{city.totalGainGold}>\n<粮:{city.food}+{city.totalGainFood}>";
            if (city.IsBorderCity)
                cityInfo = $"*{cityInfo}";
            info.text = cityInfo;
        }
    }
}
