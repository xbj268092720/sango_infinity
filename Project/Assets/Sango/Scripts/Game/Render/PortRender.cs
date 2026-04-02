using Sango.Core;

namespace Sango.Render
{
    public class PortRender : BuildingBaseRender
    {
        public PortRender(Port city) : base(city)
        {

        }

        protected override string GetHeadbarAsset()
        {
            return GameRenderHelper.CityHeadbarRes;
        }
    }
}
