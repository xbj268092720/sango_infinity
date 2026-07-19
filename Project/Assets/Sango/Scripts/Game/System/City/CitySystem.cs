using Sango.UI;
using UnityEngine;
using ContextMenu = Sango.UI.ContextMenu;

namespace Sango.Core.Player
{
    /// <summary>
    /// 城市菜单系统
    /// </summary>
    [GameSystem]
    public class CitySystem : GameSystem
    {
        public override void Init()
        {
            GameEvent.OnClick += OnClick;
            //GameSystem.GetSystem<CityBuildBuilding>().Init();

            //GameSystem.GetSystem<CityTrade>().Init();
            //Singleton<CityTechniqueResearch>.Instance.Init();

            //军事
            //GameSystem.GetSystem<CityExpedition>().Init();    // 出征
            //GameSystem.GetSystem<CityTransport>().Init();     // 运输

            //人事
            //GameSystem.GetSystem<CityCallPerson>().Init();
            //Singleton<CityTransformPerson>.Instance.Init();
            //GameSystem.GetSystem<CityRecruit>().Init();
            //Singleton<CityReward>.Instance.Init();
        }

        public override void Clear()
        {
            GameEvent.OnClick -= OnClick;

            //GameSystem.GetSystem<CityBuildBuilding>().Clear();

            //GameSystem.GetSystem<CityTrade>().Clear();
            //Singleton<CityTechniqueResearch>.Instance.Init();

            //军事
            //GameSystem.GetSystem<CityExpedition>().Clear();    // 出征
            //GameSystem.GetSystem<CityTransport>().Clear();     // 运输

            //人事
            //GameSystem.GetSystem<CityCallPerson>().Clear();
            //Singleton<CityTransformPerson>.Instance.Clear();
            //GameSystem.GetSystem<CityRecruit>().Clear();
            //Singleton<CityReward>.Instance.Clear();
        }

        void OnClick(Cell clickCell, Vector3 clickPosition, bool isOverUI)
        {
            if (clickCell.building == null) return;

            BuildingBase building = clickCell.building;
            if (!building.IsCityBase()) return;
            City city = (City)building;
            ContextMenuData.MenuData.Clear();
            GameEvent.OnCityContextMenuShow?.Invoke(ContextMenuData.MenuData, city);
            if (!ContextMenuData.MenuData.IsEmpty())
            {
                TargetCity = city;
                ContextMenu.Show(ContextMenuData.MenuData, clickPosition);
                Enter();
            }
        }

        public City TargetCity { get; set; }

        public override void OnEnter()
        {
            base.OnEnter();
            Window.Instance.Open("window_city_info_panel", TargetCity);
            TargetCity.Render?.SetFlash(true);
        }

        public override void OnExit()
        {
            ContextMenu.SetVisible(false);
        }
        public override void OnBack(ICommandEvent commandEvent)
        {
            ContextMenu.SetVisible(true);
        }

        /// <summary>
        /// 离开当前命令的时候触发
        /// </summary>
        public override void OnDestroy()
        {
            TargetCity.Render?.SetFlash(false);
            ContextMenu.CloseAll();
            Window.Instance.Close("window_city_info_panel");
        }

        public override void HandleEvent(CommandEventType eventType, Cell cell, UnityEngine.Vector3 clickPosition, bool isOverUI)
        {
            switch (eventType)
            {
                case CommandEventType.Cancel:
                case CommandEventType.RClickDown:
                    {
                        if (ContextMenu.Close())
                            Exit();

                        break;
                    }

                case CommandEventType.ClickDown:
                    {
                        if (isOverUI) return;
                        Done();
                        break;
                    }
            }
        }
    }
}
