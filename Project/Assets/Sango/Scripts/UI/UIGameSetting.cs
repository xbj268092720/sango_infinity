using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Sango.Core; namespace Sango.UI
{
    /// <summary>
    /// 剧本选择界面
    /// </summary>
    public class UIGameSetting : UIScenarioVariables
    {
        public override void OnOpen()
        {
            for (int i = 0; i < itemList.Count; i++)
                RemoveItem(itemList[i]);
            itemList.Clear();
            ShowVariables();
        }

        public void ShowVariables()
        {
            GameEvent.OnGameSetting?.Invoke(this);
        }

        public override void RefreshSetting()
        {
            for (int i = 0; i < itemList.Count; i++)
                RemoveItem(itemList[i]);
            itemList.Clear();
            ShowVariables();
        }
        public override void OnStartGame()
        {
            Close();
            GameSetting.Instance.Apply();
        }

        public override void OnCancel()
        {
            Close();
        }
    }
}
