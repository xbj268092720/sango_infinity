using ContextMenu = Sango.UI.ContextMenu;

namespace Sango.Core.Player
{
    /// <summary>
    /// 建筑拆除
    /// </summary>
    [GameSystem]
    public class BuildingActionDestroy : BuildingActionBase
    {
        public BuildingActionDestroy()
        {
            customMenuName = "拆除";
            customMenuOrder = 10000;
        }
        public override bool IsValid
        {
            get
            {
                return true;
            }
        }
       
        public override void OnEnter()
        {
            ContextMenu.CloseAll();
            base.OnEnter();
            TargetBuilding.OnFall(null);
            Done();
        }
    }
}
