using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UISortButtonGroup : MonoBehaviour
    {
        List<UISortButton> groupList = new List<UISortButton>();
        public void Add(UISortButton button)
        {
            groupList.Remove(button);
            groupList.Add(button);
        }

        public void Select(UISortButton button)
        {
            foreach (UISortButton other in groupList)
                if (other != button)
                    other.Clear();
        }

    }
}