using Sango.Core;
using Sango.Render;
using Sango.Tools.UndoRedo;
using UnityEngine;

namespace Sango.Tools
{
    public class CityEditCommand : IUndoableCommand
    {
        private MapEditor editor;
        private City city;
        private string propertyName;
        private object oldValue;
        private object newValue;
        private string actionName;

        public CityEditCommand(MapEditor editor, City city, string propertyName, object oldValue, object newValue, string actionName)
        {
            this.editor = editor;
            this.city = city;
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.actionName = actionName;
        }

        public string Description
        {
            get { return actionName; }
        }

        public void Execute()
        {
            SetPropertyValue(newValue);
        }

        public void Undo()
        {
            SetPropertyValue(oldValue);
        }

        public void Redo()
        {
            Execute();
        }

        public void Destroy()
        {
        }

        private void SetPropertyValue(object value)
        {
            switch (propertyName)
            {
                case "food":
                    city.food = (int)value;
                    break;
                case "gold":
                    city.gold = (int)value;
                    break;
                case "population":
                    city.population = (int)value;
                    break;
                case "trooppopulation":
                    city.troopPopulation = (int)value;
                    break;
                case "workingappointtype":
                    city.workingAppointType = (int)value;
                    break;
                case "commerce":
                    city.commerce = (int)value;
                    break;
                case "agriculture":
                    city.agriculture = (int)value;
                    break;
                case "popularsupport":
                    city.popularSupport = System.Convert.ToByte(value);
                    break;
                case "security":
                    city.security = (int)value;
                    break;
                case "energy":
                    city.energy = (int)value;
                    break;
                case "morale":
                    city.morale = (int)value;
                    break;
                case "maxmorale":
                    city.MaxMorale = (int)value;
                    break;
                case "hasbusiness":
                    city.hasBusiness = System.Convert.ToByte(value);
                    break;
                case "troops":
                    city.troops = (int)value;
                    break;
                case "woundedtroops":
                    city.woundedTroops = (int)value;
                    break;
                case "troopslimit":
                    city.troopsLimit = (int)value;
                    break;
                case "storelimit":
                    city.storeLimit = (int)value;
                    break;
                case "goldlimit":
                    city.goldLimit = (int)value;
                    break;
                case "foodlimit":
                    city.foodLimit = (int)value;
                    break;
                case "basegainingold":
                    city.baseGainGold = (int)value;
                    break;
                case "basegainfood":
                    city.baseGainFood = (int)value;
                    break;
                case "commercelimit":
                    city.commerceLimit = (int)value;
                    break;
                case "agriculturelimit":
                    city.agricultureLimit = (int)value;
                    break;
                case "durabilitylimit":
                    city.durabilityLimit = (int)value;
                    break;
                case "durability":
                    city.durability = (int)value;
                    break;
                case "x":
                    city.x = (int)value;
                    break;
                case "y":
                    city.y = (int)value;
                    break;
                case "rot":
                    city.rot = (float)value;
                    break;
                case "heightoffset":
                    city.heightOffset = (float)value;
                    break;
                case "totalgainfood":
                    city.totalGainFood = (int)value;
                    break;
                case "totalgainingold":
                    city.totalGainGold = (int)value;
                    break;
                case "extragainfoodfactor":
                    city.extraGainFoodFactor = (float)value;
                    break;
                case "extragainingoldfactor":
                    city.extraGainGoldFactor = (float)value;
                    break;
                case "extrapolulationfactor":
                    city.extraPopulationFactor = (float)value;
                    break;
                case "population_increase_factor":
                    city.population_increase_factor = (float)value;
                    break;
                case "personhole":
                    city.PersonHole = (int)value;
                    break;
            }
        }
    }
}