using Sango.UI;

namespace Sango.Core.Player
{
    [GameSystem]
    public class PlayerEndTurn : GameSystem
    {

        bool updateTroopAI = false;
        public string customTitleName;

        public override void Init()
        {
            customTitleName = "进行";
            //GameEvent.OnRightMouseButtonContextMenuShow += OnRightMouseButtonContextMenuShow;
        }

        public override void Clear()
        {
            //GameEvent.OnRightMouseButtonContextMenuShow -= OnRightMouseButtonContextMenuShow;
        }

        //protected virtual void OnRightMouseButtonContextMenuShow(IContextMenuData menuData)
        //{
        //    menuData.Add("进行", -9999, null, OnClickMenuItem, true);
        //}

        //protected virtual void OnClickMenuItem(IContextMenuItem contextMenuItem)
        //{
        //    ContextMenu.CloseAll();
        //    GameSystemManager.Instance.Push(this);
        //}

        public override void OnEnter()
        {
            updateTroopAI = false;
            GameDialog.Open("是否需要结束玩家回合", () =>
            {
                updateTroopAI = true;

                GameDialog.Close();
            }).cancelAction = ()=>
            {
                GameDialog.Close();
                Done();
            };
        }

        public override void Update()
        {
            if (!updateTroopAI) return;
            base.Update();
            Force force = Scenario.Cur.CurRunForce;
            Scenario scenario = Scenario.Cur;
            for (int i = 0; i < scenario.troopsSet.Count; ++i)
            {
                var c = scenario.troopsSet[i];
                if (c != null && c.IsAlive && c.BelongForce == force && !c.ActionOver && c.missionType > 0)
                {
                    if (!c.DoAI(scenario))
                        return;
                    c.Render?.UpdateRender();
                }
            }
            Scenario.Cur.CurRunForce.CurRunCorps.ActionOver = true;
            Done();
        }

        public override void OnDestroy()
        {
            GameDialog.Close();
        }

        public override void HandleEvent(CommandEventType eventType, Cell cell, UnityEngine.Vector3 clickPosition, bool isOverUI)
        {
            switch (eventType)
            {
                case CommandEventType.Cancel:
                case CommandEventType.RClickUp:
                    GameSystemManager.Instance.Back(); break;
            }

        }
    }
}
