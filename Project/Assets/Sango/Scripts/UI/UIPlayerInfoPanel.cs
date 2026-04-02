using Sango.Game.Player;
using Sango.Loader;
using Sango.Render;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIPlayerInfoPanel : UGUIWindow
    {
        public UIForceElementItem governorItem;
        public UIForceElementItem[] uIForceElementItems;

        public GameObject[] showTypeSelectObjs;

        public GameObject maxBtn;
        public GameObject miniBtn;
        public GameObject longBtn;
        public GameObject shortBtn;

        public RectTransform itemRoot;
        public RectTransform sliderRoot;

        public Scrollbar scrollbar;

        SangoObject governorObj;
        List<SangoObject> curDataList = new List<SangoObject>();
        int startIndex = 0;

        private void Start()
        {
            GameEvent.OnTroopCreated += OnTroopCreated;
            GameEvent.OnTroopDestroyed += OnTroopDestroyed;
            GameEvent.OnCityFall += OnCityFall;
            GameEvent.OnTroopActionOver += OnTroopActionOver;
        }

        protected override void OnDestroy()
        {
            GameEvent.OnTroopCreated -= OnTroopCreated;
            GameEvent.OnTroopDestroyed -= OnTroopDestroyed;
            GameEvent.OnCityFall -= OnCityFall;
            GameEvent.OnTroopActionOver -= OnTroopActionOver;
        }

        void OnTroopActionOver(Troop troop)
        {
            if ((curShowType == ShowType.Troop || curShowType == ShowType.Person) && troop.BelongForce.IsPlayer && troop.BelongForce == Scenario.Cur.CurRunForce)
            {
                UpdateShowType();
            }
        }


        void OnCityFall(City city, Force lastForce, Troop atk)
        {
            if (curShowType == ShowType.City && city.BelongForce.IsPlayer && city.BelongForce == Scenario.Cur.CurRunForce)
            {
                UpdateShowType();
            }
        }

        void OnTroopCreated(Troop troop, Scenario scenario)
        {
            if (curShowType == ShowType.Troop && troop.BelongForce.IsPlayer && troop.BelongForce == scenario.CurRunForce)
            {
                UpdateShowType();
            }
        }

        void OnTroopDestroyed(Troop troop, Scenario scenario)
        {
            if (curShowType == ShowType.Troop && troop.BelongForce.IsPlayer && troop.BelongForce == scenario.CurRunForce)
            {
                UpdateShowType();
            }
        }

        public enum ShowType
        {
            City = 0,
            Person,
            Troop
        }

        ShowType curShowType = ShowType.City;

        public void UpdateShowType()
        {
            ChangeShowType(curShowType, true);
        }

        public void ChangeShowType(ShowType showType, bool forceUpdate = false)
        {
            if (!forceUpdate && showType == curShowType)
                return;
            curShowType = showType;

            for (int i = 0; i < showTypeSelectObjs.Length; i++)
                showTypeSelectObjs[i].SetActive(i == (int)showType);

            Force force = Scenario.Cur.CurRunForce;
            curDataList.Clear();
            startIndex = 0;

            switch (curShowType)
            {
                case ShowType.City:
                    {
                        governorObj = force.Governor.BelongCity;
                        force.ForEachCityBase(obj =>
                        {
                            if (governorObj != obj)
                                curDataList.Add(obj);
                        });
                    }
                    break;
                case ShowType.Person:
                    {
                        governorObj = force.Governor;
                        force.ForEachPerson(obj =>
                        {
                            if (governorObj != obj)
                                curDataList.Add(obj);
                        });
                    }
                    break;
                case ShowType.Troop:
                    {
                        governorObj = force.Governor.BelongTroop;
                        force.ForEachTroop(obj =>
                        {
                            if (governorObj != obj)
                                curDataList.Add(obj);
                        });
                    }
                    break;
            }

            governorItem.SetSangoObject(governorObj);

            if (curDataList.Count < uIForceElementItems.Length)
            {
                sliderRoot.gameObject.SetActive(false);
                Vector2 size = itemRoot.sizeDelta;
                size.x = 150;
                itemRoot.sizeDelta = size;

                for (int i = 0; i < uIForceElementItems.Length; i++)
                {
                    UIForceElementItem uIForceElement = uIForceElementItems[i];
                    if (i < curDataList.Count)
                    {
                        uIForceElement.SetSangoObject(curDataList[i]);
                    }
                    else
                    {
                        uIForceElement.SetSangoObject(null);
                    }
                }
            }
            else
            {
                sliderRoot.gameObject.SetActive(true);
                Vector2 size = itemRoot.sizeDelta;
                size.x = 132;
                itemRoot.sizeDelta = size;

                scrollbar.size = (float)uIForceElementItems.Length / (float)curDataList.Count;
                scrollbar.SetValueWithoutNotify(0);
                OnScrollBarValueChange(0);
            }
        }

        public void ShowCities()
        {
            ChangeShowType(ShowType.City);
        }

        public void ShowPersons()
        {
            ChangeShowType(ShowType.Person);
        }

        public void ShowTroops()
        {
            ChangeShowType(ShowType.Troop);
        }


        public void MaxSize()
        {

        }

        public void MiniSize()
        {

        }

        public void LongSize() { }
        public void ShortSize() { }

        public void UpShow()
        {
            if (startIndex > 0)
                startIndex--;
            UpdateItemStartIndex(startIndex);
            scrollbar.SetValueWithoutNotify((float)startIndex / (curDataList.Count - uIForceElementItems.Length));
        }

        public void DownShow()
        {
            if (startIndex < curDataList.Count - uIForceElementItems.Length)
                startIndex++;
            UpdateItemStartIndex(startIndex);
            scrollbar.SetValueWithoutNotify((float)startIndex / (curDataList.Count - uIForceElementItems.Length));
        }

        public void OnScrollBarValueChange(float value)
        {
            startIndex = (int)UnityEngine.Mathf.Lerp(0, curDataList.Count - uIForceElementItems.Length, value);
            UpdateItemStartIndex(startIndex);
        }
        public void UpdateItemStartIndex(int startIndex)
        {
            for (int i = 0; i < uIForceElementItems.Length; i++)
            {
                SangoObject sango = curDataList[i + startIndex];
                UIForceElementItem uIForceElement = uIForceElementItems[i];
                uIForceElement.SetSangoObject(sango);
            }
        }
    }
}
