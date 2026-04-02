using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Core

{
    public class GameRenderHelper
    {
        public static string BuildingTypeIconPath = "Assets/UI/AtlasTexture";
        public static string TroopStatePath = "Assets/UI/AtlasTexture";

        public static string HeadIconPath = "Assets/Face";
        public static string TroopHeadbarRes = "Assets/UI/Prefab/window_troop_bar.prefab";
        public static string CityHeadbarRes = "Assets/UI/Prefab/window_city_bar.prefab";
        public static string BuildingHeadbarRes = "Assets/UI/Prefab/window_building_bar.prefab";
        public static string AnimationTextInfoRes = "Assets/UI/Prefab/window_aniTextInfo.prefab";
        public static string FireRes = "Assets/Effect/Prefab/ef_scene_fire.prefab";
        public static string[] CityResPath = new string[]{
        "Assets/Model/Prefab/p_city_1.prefab",
        "Assets/Model/Prefab/p_city_2.prefab",
        };

        public static Texture LoadHeadIcon(int id)
        {
            return LoadHeadIcon(id, 2);
        }
        public static Texture LoadHeadIcon(int id, int type)
        {
            string headPath = $"{HeadIconPath}/{id}_{type}.png";
            Texture headSpr = ObjectLoader.LoadObject<Texture>(headPath, "Face");
            if (headSpr == null)
            {
                headPath = $"{HeadIconPath}/0_{type}.png";
                headSpr = ObjectLoader.LoadObject<Texture>(headPath);
            }
            return headSpr;
        }

        public static string GetCityModelAsset(int type)
        {
            if (type < 0 || type >= CityResPath.Length)
                type = 0;
            return CityResPath[type];
        }

        public static UnityEngine.Sprite LoadBuildingTypeIcon(string name)
        {
            string headPath = $"{BuildingTypeIconPath}/{name}.png";
            UnityEngine.Sprite headSpr = ObjectLoader.LoadObject<UnityEngine.Sprite>(headPath);
            if (headSpr == null)
            {
                headPath = $"{BuildingTypeIconPath}/4845_5_22.png";
                headSpr = ObjectLoader.LoadObject<UnityEngine.Sprite>(headPath);
            }
            return headSpr;
        }

        public static UnityEngine.Sprite LoadTroopStateIcon(string name)
        {
            return ObjectLoader.LoadObject<UnityEngine.Sprite>($"{TroopStatePath}/{name}.png");
        }
    }
}
