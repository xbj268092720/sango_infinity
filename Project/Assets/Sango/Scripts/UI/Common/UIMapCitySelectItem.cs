using Sango.Loader;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIMapCitySelectItem : MonoBehaviour
    {
        public GameObject selectedObj;
        public GameObject normalObj;
        public GameObject inavtiveObj;
        public GameObject[] overObj;
        public Image[] colorImage;
        public City city;
        public Action<City, bool> onSelectAction;

        public ShortCity shortCity;
        public Action<UIMapCitySelectItem, ShortCity, bool> onSelectShortAction;

        public void OnSelect()
        {
            selectedObj.SetActive(!selectedObj.activeSelf);
            if (city != null)
            {
                onSelectAction?.Invoke(city, selectedObj.activeSelf);
            }
            if (shortCity != null)
            {
                onSelectShortAction?.Invoke(this, shortCity, selectedObj.activeSelf);
            }
        }

        public UIMapCitySelectItem SetInavtive(bool b)
        {
            inavtiveObj.SetActive(b);
            return this;
        }

        public UIMapCitySelectItem SetSelected(bool b)
        {
            selectedObj.SetActive(b);
            return this;
        }

        public UIMapCitySelectItem SetColor(Color c)
        {
            foreach (var item in colorImage)
                if (item != null)
                    item.color = c;
            return this;
        }
        public UIMapCitySelectItem SetOver(bool b)
        {
            foreach (var item in overObj)
                if (item != null)
                    item.SetActive(b);
            return this;
        }
    }
}