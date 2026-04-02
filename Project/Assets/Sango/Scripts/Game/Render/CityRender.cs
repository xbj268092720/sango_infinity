using Sango.Core;

namespace Sango.Render
{
    public class CityRender : BuildingBaseRender
    {
        public CityRender(City city) : base(city)
        {
            
        }

        protected override string GetHeadbarAsset()
        {
            return GameRenderHelper.CityHeadbarRes;
        }
    }
}
