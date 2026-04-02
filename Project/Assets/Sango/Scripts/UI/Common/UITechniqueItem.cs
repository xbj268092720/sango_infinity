using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    /// <summary>
    /// 剧本选择界面
    /// </summary>
    public class UITechniqueItem : MonoBehaviour
    {
        public RectTransform root;

        public Text techName;
        //public Text costGold;
        //public Text costTP;
        //public Text costCounter;
        public Image progress;
        public Image doneImg;
        public Image selectImg;
        public Image bg;
        public Text needAttributeType;
        public RectTransform itemNode;
        public Image invalidMask;

        public Technique technique;

        public System.Action<UITechniqueItem> onClick;

        public UITechniqueItem SetTechnique(Technique t)
        {
            technique = t;
            techName.text = t.Name;
            //costGold.text = t.goldCost.ToString();
            //costTP.text = t.techPointCost.ToString();
            //costCounter.text = t.counter.ToString();
            needAttributeType.text = Scenario.Cur.Variables.GetAttributeNameWithColor(t.needAttr);
            bg.color = t.tabColor;
            doneImg.color = t.tabColor;
            return this;
        }

        public void OnClick()
        {
            onClick?.Invoke(this);
        }

        public UITechniqueItem SetAction(System.Action<UITechniqueItem> action)
        {
            onClick = action;
            return this;
        }

        public UITechniqueItem SetValid(bool b)
        {
            doneImg.gameObject.SetActive(b);
            return this;
        }
        public bool IsValid() { return doneImg.enabled; }

        public UITechniqueItem SetCanResearch(bool b)
        {
            invalidMask.enabled = !b;
            return this;
        }

        public bool CanResearch() { return !invalidMask.enabled; }

        public UITechniqueItem SetProgress(float p)
        {
            return this;
        }

        public UITechniqueItem SetSelected(bool b)
        {
            selectImg.enabled = b;
            return this;
        }

    }
}
