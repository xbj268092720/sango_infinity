using Sango.Core;

namespace Sango.Render
{
    public class GateRender : BuildingBaseRender
    {
        public GateRender(Gate city) : base(city)
        {

        }

        protected override string GetHeadbarAsset()
        {
            return GameRenderHelper.CityHeadbarRes;
        }
    }
}
