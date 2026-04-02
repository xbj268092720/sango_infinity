using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UITechniqueArea : MonoBehaviour
    {
        public RectTransform midLine;
        public RectTransform upLine;
        public RectTransform downLine;

        CreatePool<RectTransform> upLinePool;
        CreatePool<RectTransform> midLinePool;
        CreatePool<RectTransform> downLinePool;
        CreatePool<UITechniqueItem> itemPool;
        CreatePool<UITechniqueTitileItem> titlePool;

        public RectTransform lineRoot;
        public RectTransform itemRoot;

        public UITechniqueItem techniqueItem;
        public UITechniqueTitileItem techniqueTitileItem;

        public Vector2Int itemBounds = new Vector2Int(78, 28);
        public Vector2Int itemSpacing = new Vector2Int(33, 6);
        
        private void Awake()
        {
            upLinePool = new CreatePool<RectTransform>(upLine);
            midLinePool = new CreatePool<RectTransform>(midLine);
            downLinePool = new CreatePool<RectTransform>(downLine);
            itemPool = new CreatePool<UITechniqueItem>(techniqueItem);
            titlePool = new CreatePool<UITechniqueTitileItem>(techniqueTitileItem);
        }

        public void CreateTitles(Force force, Transform root)
        {
            titlePool.Reset();

            // 先创建等级标题
            for (int i = 0; i < force.techniqueMaxLevel; i++)
            {
                UITechniqueTitileItem titileItem = titlePool.Create(root);
                titileItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * (itemBounds.x + itemSpacing.x), 0);
                titileItem.level.text = (i + 1).ToString();
                titileItem.arrow.enabled = i < force.techniqueMaxLevel - 1;
            }
        }

        public void ShowForceTechnique(Force force, List<UITechniqueItem> instance_list)
        {
            Vector2 areaBounds = new Vector2(0, 0);
            upLinePool.Reset();
            midLinePool.Reset();
            downLinePool.Reset();
            itemPool.Reset();

            // 创建科技
            for (int i = 0; i < force.techniqueTree.Count; i++)
            {
                force.techniqueTree[i].ForEach(forceTechnique =>
                {
                    UITechniqueItem techniqueItem = itemPool.Create();
                    instance_list?.Add(techniqueItem);
                    techniqueItem.SetTechnique(forceTechnique.technique);

                    techniqueItem.techName.text = forceTechnique.technique.Name;
                    Vector2 anchoredPosition = new Vector2(forceTechnique.technique.level * (itemBounds.x + itemSpacing.x),
                        forceTechnique.GetY() * -(itemBounds.y + itemSpacing.y));
                    techniqueItem.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
                    areaBounds.x = Mathf.Max(anchoredPosition.x, areaBounds.x);
                    areaBounds.y = Mathf.Min(anchoredPosition.y, areaBounds.y);
                    // 创建连线
                    if (forceTechnique.parent != null)
                    {
                        int y = forceTechnique.GetY();
                        int parentY = forceTechnique.parent.GetY();
                        if (y == parentY)
                        {
                            RectTransform midLine = midLinePool.Create();
                            midLine.anchoredPosition = anchoredPosition + new Vector2(0, -itemBounds.y / 2);
                            midLine.sizeDelta = new Vector2((forceTechnique.technique.level - forceTechnique.parent.technique.level) * itemBounds.x, 16);
                        }
                        else if (y < parentY)
                        {
                            RectTransform midLine = downLinePool.Create();
                            midLine.anchoredPosition = anchoredPosition + new Vector2(itemBounds.x / 2, -8);
                            midLine.sizeDelta = new Vector2(
                                (forceTechnique.technique.level - forceTechnique.parent.technique.level) * (itemBounds.x + itemSpacing.x) + 8,
                                (parentY - y) * (itemBounds.y + itemSpacing.y) + 8);
                        }
                        else
                        {
                            RectTransform midLine = upLinePool.Create();
                            midLine.anchoredPosition = anchoredPosition + new Vector2(itemBounds.x / 2, -itemBounds.y / 2 - 8);
                            midLine.sizeDelta = new Vector2(
                                (forceTechnique.technique.level - forceTechnique.parent.technique.level) * (itemBounds.x + itemSpacing.x) + 8,
                                (y - parentY) * (itemBounds.y + itemSpacing.y) + 8);
                        }
                    }
                });
            }

            areaBounds.x += itemBounds.x;
            areaBounds.y = -areaBounds.y + itemBounds.y;
            GetComponent<RectTransform>().sizeDelta = areaBounds;
        }
    }
}
