using Sango.Game.Player;
using Sango.Loader;
using Sango.Render;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIGame : UGUIWindow
    {
        public UIPlayerInfoPanel uIPlayerInfoPanel;

        public Text forceText;
        public Text dateText;
        public Text fpsText;

        public Text cellInfoLabel;
        public Image seasonImg;
        public Text seasonLabel;
        public Text actionNumberLabel;
        public Text techPointLabel;

        public Text frameBtnText;
        public Text speedBtnText;

        public GameObject miniMapObj;
        public GameObject miniMapBtnObj;
        public GameObject messageObj;

        public RectTransform gameSettingRect;

        public bool gridShow = true;
        public bool troopListShow = false;
        public GameObject troopListObj;
        public Text troopListShowText;

        public LoopScrollRect loopScrollRect;
        public GameObject troopListItemObj;
        public Transform troopListContent;
        public int totalCount = -1;
        Stack<Transform> pool = new Stack<Transform>();
        //List<Troop> troops_list = new List<Troop>();
        List<SangoObject> item_list = new List<SangoObject>();
        public Type itemType;
        bool needUpdateItem = true;

        public GameObject pauseObj;
        public GameObject resumeObj;

        public Button endTurnButton;

        public GameObject[] fpaObj;

        int destSaveTurn = -1;
        bool needSave = false;
        private float deltaTime = 0.0f;
        bool cityInfoShow = true;

        float gameSpeed = 1;

        public override void OnShow()
        {
            base.OnShow();
            Window.Instance.Close("window_loading");
            GameController.Instance.onCellOverEnter += OnCellOverEnter;
            GameController.Instance.onCellOverExit += OnCellOverExit;

            Window.Instance.Open("window_object_pop_info");
        }

        public override void OnHide()
        {
            GameController.Instance.onCellOverEnter -= OnCellOverEnter;
            GameController.Instance.onCellOverExit -= OnCellOverExit;
            base.OnHide();
        }

        void OnCellOverEnter(Cell cell)
        {
            if (cell == null)
            {
                cellInfoLabel.text = "";
                return;
            }
            if (cell.moveAble)
            {
                string cityName = cell.BelongCity != null ? cell.BelongCity.Name : "--";
                cellInfoLabel.text = $"地形: {cell.TerrainType.Name}({cityName})  坐标: ({cell.x}, {cell.y})     ";
            }
            else
            {
                cellInfoLabel.text = $"地形: <color=#ff0000>不可进入</color>  坐标: ({cell.x}, {cell.y})     ";
            }
        }

        void OnCellOverExit(Cell cell)
        {

        }


       

        void Start()
        {
            //GameEvent.OnTroopCreated += OnTroopChange;
            //GameEvent.OnTroopDestroyed += OnTroopChange;
            GameEvent.OnForceTurnStart += OnForceStart;
            GameEvent.OnDayUpdate += OnDayUpdate;
            GameEvent.OnCityFall += OnCityFall;
            GameEvent.OnSeasonUpdate += OnSeasonUpdate;
            GameEvent.OnForceGainTechniquePoint += OnForceGainTechniquePoint;
            GameEvent.OnCorpsActionPointChange += OnCorpsActionPointChange;

            GameSystem.GetSystem<PlayerMessage>().onVisibleChange += OnMessagePlaneVisible;


            //loopScrollRect.prefabSource = this;
            //loopScrollRect.dataSource = this;

            itemType = typeof(Troop);
            needUpdateItem = true;

            InvokeRepeating("UpdateFPS", 1f, 1f);
            //loopScrollRect.totalCount = totalCount;
            //loopScrollRect.RefillCells();
            OnForceStart(Scenario.Cur.CurRunForce, Scenario.Cur);
            OnDayUpdate(Scenario.Cur);
            OnSeasonUpdate(Scenario.Cur);

            for (int i = 0; i < Scenario.Cur.corpsSet.Count; ++i)
            {
                var c = Scenario.Cur.corpsSet[i];
                if (c != null && c.IsAlive && c.BelongForce == Scenario.Cur.CurRunForce)
                {
                    OnCorpsActionPointChange(c);
                    break;
                }
            }
        }

        protected override void OnDestroy()
        {
            //GameEvent.OnTroopCreated -= OnTroopChange;
            //GameEvent.OnTroopDestroyed -= OnTroopChange;
            GameEvent.OnForceTurnStart -= OnForceStart;
            GameEvent.OnDayUpdate -= OnDayUpdate;
            GameEvent.OnCityFall -= OnCityFall;
            GameEvent.OnSeasonUpdate -= OnSeasonUpdate;
            GameEvent.OnForceGainTechniquePoint -= OnForceGainTechniquePoint;
            GameEvent.OnCorpsActionPointChange -= OnCorpsActionPointChange;


        }

        public void OnCityFall(City city, Force lastForce, Troop atker)
        {
            if (itemType == typeof(City))
            {
                loopScrollRect.RefreshCells();
            }

        }

        public void OnTroopChange(Troop troop, Scenario scenario)
        {
            if (itemType == typeof(Troop))
            {
                needUpdateItem = true;
            }

        }

        public void OnForceStart(Force force, Scenario scenario)
        {
            if(force == null)
            {
                forceText.text = "";
                techPointLabel.text = "";
                endTurnButton.interactable = false;
                uIPlayerInfoPanel.gameObject.SetActive(false);
                return;
            }
            forceText.text = force.Name;
            techPointLabel.text = force.TechniquePoint.ToString();

            endTurnButton.interactable = force.IsPlayer;
            uIPlayerInfoPanel.gameObject.SetActive(force.IsPlayer);

            if (force.IsPlayer)
            {
                uIPlayerInfoPanel.UpdateShowType();
                GameSystem.GetSystem<PlayerTurnStartGreeting>().Push();
            }
        }

        public void OnCorpsActionPointChange(Corps corps)
        {
            actionNumberLabel.text = corps.ActionPoint.ToString();
        }

        public void OnForceGainTechniquePoint(Force force, int value)
        {
            if (!force.IsPlayer) return;
            techPointLabel.text = force.TechniquePoint.ToString();
        }

        string[] seasonIconPath = new string[] {
            "Assets/UI/AtlasTexture/4846-6/4846-6_10.png",      //秋
            "Assets/UI/AtlasTexture/4846-6/4846-6_8.png",       //春
            "Assets/UI/AtlasTexture/4846-6/4846-6_9.png",       //夏
            "Assets/UI/AtlasTexture/4846-6/4846-6_11.png"       //冬
        };
        public void OnDayUpdate(Scenario scenario)
        {
            dateText.text = scenario.GetDateStr();
        }
        public void OnSeasonUpdate(Scenario scenario)
        {
            seasonImg.sprite = ObjectLoader.LoadObject<UnityEngine.Sprite>(seasonIconPath[(int)scenario.CurSeason]);
            seasonLabel.text = GameDefine.seasonName[(int)scenario.CurSeason];
        }

        public void OnBtnPause()
        {
            pauseObj.SetActive(false);
            resumeObj.SetActive(true);
            Sango.Game.Scenario.Pause();
        }

        public void OnBtnResume()
        {
            pauseObj.SetActive(true);
            resumeObj.SetActive(false);
            Sango.Game.Scenario.Resume();
        }


        public void OnBtnNextForce()
        {
            pauseObj.SetActive(false);
            resumeObj.SetActive(true);
            Sango.Game.Scenario.NextForce();
        }

        public void OnBtnNextTurn()
        {
            pauseObj.SetActive(false);
            resumeObj.SetActive(true);
            Sango.Game.Scenario.NextTurn();
        }

        public void OnBtnDebugAI()
        {

        }

        public void OnBtnGirdShow()
        {
            gridShow = !gridShow;
            MapRender.Instance.ShowGrid(gridShow);
        }

        public void OnTroopListShow()
        {
            troopListShow = !troopListShow;
            troopListObj.SetActive(troopListShow);
            troopListShowText.text = troopListShow ? "隐藏" : "显示";
        }

        public void OnTroopListSelected(int index)
        {
            if (index < 0 || index >= item_list.Count)
                return;

            SangoObject obj = item_list[index];
            if (obj is Troop)
            {
                Troop troop = (Troop)obj;
                Vector3 position = troop.cell.Position;
                MapRender.Instance.MoveCameraTo(position);
            }
            else if (obj is City)
            {
                City troop = (City)obj;
                Vector3 position = troop.CenterCell.Position;
                MapRender.Instance.MoveCameraTo(position);
            }
        }

        public void OnTroopListShow(UITroopListItem item)
        {
            if (item.index < 0 || item.index >= item_list.Count)
            {
                item.name.text = "无效";
                return;
            }
            SangoObject obj = item_list[item.index];
            if (obj is Troop)
            {
                Troop troop = (Troop)obj;

                if (troop.BelongForce == null)
                {
                    int dd = 33;
                    dd++;
                }
                if (troop.TroopType.isFight)
                    item.name.text = $"[{troop.BelongForce.Name}]<{troop.TroopType.Name}>{troop.Name}队,{troop.Member1?.Name}{troop.Member2?.Name}";
                else
                    item.name.text = $"**[{troop.BelongForce.Name}]<{troop.TroopType.Name}>{troop.Name}运输队,{troop.Member1?.Name}{troop.Member2?.Name}";

                item.name.color = troop.BelongForce.Flag.color;
            }
            else if (obj is City)
            {
                City city = (City)obj;
                if (city.BelongForce != null)
                {
                    item.name.text = $"[{city.BelongForce.Name}]{city.Name}";
                    item.name.color = city.BelongForce.Flag.color;

                }
                else
                {
                    item.name.text = $"{city.Name}";
                    item.name.color = Color.white;
                }

            }
        }

        public void OnTroopTab(Toggle b)
        {
            if (b.isOn)
            {
                itemType = typeof(Troop);
                needUpdateItem = true;
            }
        }

        public void OnCityTab(Toggle b)
        {
            if (b.isOn)
            {
                itemType = typeof(City);
                needUpdateItem = true;
            }
        }

        void Save()
        {
            int count = Scenario.all_scenario_list.Count;
            string savePath = Path.ContentRootPath + $"/Scenario/scenario_save_{count}.json";
            GameEvent.OnGameSave?.Invoke(Scenario.Cur, count);
            Scenario.Cur.Save(savePath);
        }

        public void OnSave()
        {
            Save();
            //if (Sango.Game.Scenario.Cur.PauseTrunCount == Sango.Game.Scenario.Cur.Info.turnCount)
            //{
            //    Save();
            //}
            //else
            //{
            //    needSave = true;
            //    OnBtnNextTurn();
            //}
        }

        public void OnLoad()
        {

        }

        public void OnEndPlayerTurn()
        {
            if (GameSystemManager.Instance.CurrentCommand != null)
                return;
            ContextMenu.CloseAll();
            GameSystem.GetSystem<PlayerEndTurn>().Push();
        }

        public void OnSwitchCityInfoShow()
        {
            if (GameSystemManager.Instance.CurrentCommand != null)
                return;
            UICityHeadbar.showIndo = !UICityHeadbar.showIndo;
            GameEvent.OnCityHeadbarShowInfoChange?.Invoke();
        }

        public void OnSwitchMiniMapShow()
        {
            if (GameSystemManager.Instance.CurrentCommand != null)
                return;
            miniMapObj.SetActive(!miniMapObj.activeSelf);
            miniMapBtnObj.SetActive(!miniMapBtnObj.activeSelf);
        }

        public void OnSwitchMessageShow()
        {
            if (GameSystemManager.Instance.CurrentCommand != null)
                return;
            messageObj.SetActive(false);
            GameSystem.GetSystem<PlayerMessage>().onVisibleChange?.Invoke(true); ;
        }

        public void OnGameSetting()
        {
            if (GameSystemManager.Instance.CurrentCommand != null)
                return;
            Window.Instance.Close("window_city_info_panel");
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Game.Instance.UICamera, gameSettingRect.position);
            GameSystem.GetSystem<GameSettingSystem>().Start(screenPos + new Vector2(0, -gameSettingRect.sizeDelta.y - 5));
        }

        public void OnSpeedChange()
        {
            gameSpeed = gameSpeed * 2;
            if (gameSpeed > 8)
                gameSpeed = 1;

            Time.timeScale = gameSpeed;
            speedBtnText.text = $"游戏速度:{(int)gameSpeed}倍";
        }

        public void OnLowFPS()
        {
#if UNITY_STANDALONE_WIN
            if (Application.targetFrameRate == 60)
                Application.targetFrameRate = 120;
            else
                Application.targetFrameRate = 60;

#else
            if (Application.targetFrameRate == 30)
                Application.targetFrameRate = 60;
            else
                Application.targetFrameRate = 30;
#endif
            frameBtnText.text = $"切换帧率:{Application.targetFrameRate}";
        }

        void UpdateFPS()
        {
            float FPS = 1f / deltaTime;
            fpsText.text = $"FPS:{Math.Floor(FPS)}";
        }

        void OnMessagePlaneVisible(bool b)
        {
            messageObj.SetActive(!b);
        }

        public void Update()
        {
            if (needSave)
            {
                if (Sango.Game.Scenario.Cur.PauseTrunCount == Sango.Game.Scenario.Cur.Info.turnCount)
                {
                    Save();
                    needSave = false;
                }
            }

            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            //if (!troopListShow) return;

            //if (needUpdateItem)
            //{
            //    needUpdateItem = false;
            //    if (itemType == typeof(Troop))
            //    {

            //        item_list.Clear();
            //        Scenario.Cur.troopsSet.ForEach(t =>
            //        {
            //            if (t.IsAlive)
            //            {
            //                item_list.Add(t);
            //            }
            //        });

            //        loopScrollRect.totalCount = item_list.Count;
            //        loopScrollRect.RefillCells(loopScrollRect.GetFirstItem(out _));
            //    }
            //    else if (itemType == typeof(City))
            //    {
            //        item_list.Clear();
            //        Scenario.Cur.citySet.ForEach(t =>
            //        {
            //            if (t.IsAlive)
            //            {
            //                item_list.Add(t);
            //            }
            //        });

            //        loopScrollRect.totalCount = item_list.Count;
            //        loopScrollRect.RefillCells(loopScrollRect.GetFirstItem(out _));
            //    }
            //}
        }
    }
}
