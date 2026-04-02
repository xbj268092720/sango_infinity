using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Game.Render.UI
{
    public class TroopInteractiveDialogData : ITroopInteractiveDialogData
    {
        public static TroopInteractiveDialogData InteractiveDialogData = new TroopInteractiveDialogData();

        public string content;
        public System.Action sureAction;

        public void Clear()
        {
            content = null;
            sureAction = null;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(content);
        }

        public void SetContent(string content)
        {
            this.content = content;
        }

        public void SetSureAction(System.Action action)
        {
            this.sureAction = action;
        }
    }
}
