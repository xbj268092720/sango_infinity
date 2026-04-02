using Sango.Core.Player;
using Sango.UI;
using System.Collections.Generic;

namespace Sango.Core
{
    /// <summary>
    /// 城池治安系统逻辑
    /// </summary>
    [GameSystem]
    public class PortGateInformation : CityInformation
    {
        public PortGateInformation()
        {
            windowName = "window_information_port_gate";
        }
        protected override bool CityMenuCanShow()
        {
            return !Target.IsCity();
        }

        public override void Init()
        {
            base.Init();
            Name = "港关情报";
        }

        public override void OnScenarioInit(Scenario scenario)
        {
            default_objects.Clear();
            scenario.citySet.ForEach(city => {
                if (!city.IsCity())
                    default_objects.Add(city);
            });
        }
    }
}
