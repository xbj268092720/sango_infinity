using Sango.Game.Render;
using Sango.Game.Render.UI;
using Sango.Render;
using System.Collections.Generic;
using UnityEngine;
using ContextMenu = Sango.Game.Render.UI.ContextMenu;

namespace Sango.Game.Player
{
    [GameSystem]
    public class TroopActionBuild : TroopActionBase
    {
        public List<Cell> MovePath { get; set; }
        public List<Cell> buildRangeCell = new List<Cell>();
        public List<BuildingType> canBuildBuildingType = new List<BuildingType>();

        public Cell targetBuildCell;
        public BuildingType targetBuildingType;

        protected bool isShow = false;
        protected bool isMoving = false;
        public TroopActionBuild()
        {
            customMenuName = "建造";
            customMenuOrder = 1;
        }

        void InitCanBuildingTypes()
        {
            canBuildBuildingType.Clear();
            Scenario.Cur.CommonData.BuildingTypes.ForEach(x =>
            {
                if (x.IsOutside && x.canBuild && x.kind > 3 && x.IsValid(TargetTroop.BelongForce))
                {
                    canBuildBuildingType.Add(x);
                }
            });
        }

        public override bool IsValid
        {
            get
            {
                if (TargetTroop.gold <= 0) return false;

                InitCanBuildingTypes();

                bool hasEnoughGold = false;
                for (int j = 0; j < canBuildBuildingType.Count; j++)
                {
                    if (canBuildBuildingType[j].cost <= TargetTroop.gold)
                    {
                        hasEnoughGold = true;
                        break;
                    }
                }

                if (!hasEnoughGold) return false;

                // 周围一格是可以建造的地块
                Cell stayCell = ActionCell;
                buildRangeCell.Clear();
                for (int i = 0; i < stayCell.Neighbors.Length; i++)
                {
                    Cell n = stayCell.Neighbors[i];
                    if (n != null)
                    {
                        for (int j = 0; j < canBuildBuildingType.Count; j++)
                        {
                            if (canBuildBuildingType[j].CanBuildToHere(n))
                            {
                                buildRangeCell.Add(n);
                                break;
                            }
                        }
                    }
                }

                return buildRangeCell.Count > 0;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            isShow = false;
            isMoving = false;
            ContextMenu.SetVisible(false);
            MovePath = GameSystem.GetSystem<TroopSystem>().movePath;
            Window.Instance.Open("window_troop_build");
        }

        public void OnSlectedBuildingType(BuildingType buildingType)
        {
            targetBuildingType = buildingType;
            Window.Instance.Close("window_troop_build");
            buildRangeCell.Clear();
            for (int i = 0; i < ActionCell.Neighbors.Length; i++)
            {
                Cell n = ActionCell.Neighbors[i];
                if (n != null && n.building == null && n.moveAble)
                {
                    if (targetBuildingType.CanBuildToHere(n))
                    {
                        buildRangeCell.Add(n);
                    }
                }
            }
            ShowBuildRange();
        }

        protected void ShowBuildRange()
        {
            MapRender mapRender = MapRender.Instance;
            mapRender.SetDarkMask(true);
            if (buildRangeCell.Count == 0) return;
            for (int i = 0, count = buildRangeCell.Count; i < count; ++i)
            {
                Cell c = buildRangeCell[i];
                if (!MovePath.Contains(c))
                    mapRender.SetGridMaskColor(c.x, c.y, Color.red);
                mapRender.SetDarkMaskColor(c.x, c.y, Color.black);
            }
            mapRender.EndSetGridMask();
            mapRender.EndSetDarkMask();
        }

        protected void ClearShowBuildRange()
        {
            MapRender mapRender = MapRender.Instance;
            mapRender.SetDarkMask(false);
            if (buildRangeCell.Count == 0) return;
            for (int i = 0, count = buildRangeCell.Count; i < count; ++i)
            {
                Cell c = buildRangeCell[i];
                if (!MovePath.Contains(c))
                    mapRender.SetGridMaskColor(c.x, c.y, Color.black);
                mapRender.SetDarkMaskColor(c.x, c.y, Color.clear);

            }
            mapRender.EndSetGridMask();
            mapRender.EndSetDarkMask();
        }

        public override void OnDestroy()
        {
            Window.Instance.Close("window_troop_build");
            ClearShowBuildRange();
            buildRangeCell.Clear();
        }

        protected void OnMoveDone()
        {
            isMoving = false;
        }

        public override void Update()
        {
            if (isShow)
            {
                if (!isMoving)
                {
                    if (TargetTroop.BuildBuilding(targetBuildCell, targetBuildingType))
                    {
                        TargetTroop.ActionOver = true;
                        // 委任维修
                        if(!targetBuildCell.building.isComplate)
                        {
                            TargetTroop.SetMission(MissionType.TroopFixBuilding, targetBuildCell.building.Id);
                        }
                        TargetTroop.Render?.UpdateRender();

                        Done();
                    }
                }
            }
        }

        public override void HandleEvent(CommandEventType eventType, Cell cell, UnityEngine.Vector3 clickPosition, bool isOverUI)
        {
            if (isShow) return;

            switch (eventType)
            {
                case CommandEventType.Cancel:
                case CommandEventType.RClickUp:
                    {
                        GameSystemManager.Instance.Back();
                        break;
                    }

                case CommandEventType.Click:
                    {
                        if (isOverUI) return;

                        if (buildRangeCell.Contains(cell))
                        {
                            Cell stayCell = MovePath[MovePath.Count - 1];
                            targetBuildCell = cell;

                            GameSystem.GetSystem<TroopActionMenu>().troopRender.Clear();
                            ContextMenu.CloseAll();
                            Cell start = TargetTroop.cell;

                            if (start == stayCell)
                            {
                                isShow = true;
                                isMoving = false;
                                return;
                            }

                            for (int i = 1; i < MovePath.Count; i++)
                            {
                                bool isLast = i == MovePath.Count - 1;
                                Cell dest = MovePath[i];
                                TroopMoveEvent @event = RenderEvent.Instance.Create<TroopMoveEvent>();
                                @event.Init(TargetTroop, start, dest, isLast, isLast ? OnMoveDone : null);
                                RenderEvent.Instance.Add(@event);
                                start = dest;
                            }
                            isShow = true;
                            isMoving = true;
                        }
                        break;
                    }
            }
        }
    }
}
