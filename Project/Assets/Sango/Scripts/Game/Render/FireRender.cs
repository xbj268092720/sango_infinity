using Sango.Core;
using UnityEngine;

namespace Sango.Render
{
    public class FireRender : ObjectRender
    {
        Fire Fire { get; set; }
        GameObject FireModel { get; set; }

        public FireRender(Fire fire)
        {
            Owener = fire;
            Fire = fire;

            MapObject = MapObject.Create("火");
            MapObject.objType = Fire.Id;
            MapObject.modelId = Fire.Id;
            MapObject.modelAsset = GameRenderHelper.FireRes;
            MapObject.transform.position = Fire.cell.Position;
            MapObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            MapObject.transform.localScale = Vector3.one;
            MapObject.bounds = new Sango.Tools.Rect(0, 0, 32, 32);
            MapRender.Instance.AddDynamic(MapObject);
        }

    }
}
