using System;
using System.Collections.Generic;
using Sango.Core;
using Sango.UI;

namespace Sango.Core.Player
{
    /// <summary>
    /// 玩家军团系统
    /// </summary>
    [GameSystem]
    public class CorpsSystem : CityBaseSystem
    {
        ContextMenuData menuData = new ContextMenuData();
        public bool[] has = new bool[8];
        public int targetNumber = 0;
        public List<Corps> corps_list = new List<Corps>();
        public List<Corps> target_corps_list = new List<Corps>();
        public CorpsSystem()
        {
            customTitleName = "军团";
            customTitleList = new List<ObjectSortTitle>()
            {
                CorpsSortFunction.SortByName,
                CorpsSortFunction.SortByNumber,
                CorpsSortFunction.SortByLeader,
            };
            customMenuName = "君主/军团";
            customMenuOrder = 900;
            windowName = "window_corps_menu";


        }

        public override bool IsValid
        {
            get
            {
                return TargetCity.BelongForce.CityCount > 1;
            }
        }

        public override void OnEnter()
        {
            corps_list.Clear();
            targetNumber = 0;
            for (int i = 0; i < has.Length; i++)
                has[i] = false;
            menuData.Clear();
            Scenario.Cur.corpsSet.ForEach(x =>
            {
                if (x.BelongForce == TargetCity.BelongForce)
                {
                    has[x.number-1] = true;
                    if (x.number > 1)
                    {
                        corps_list.Add(x);
                    }
                }
            });

            for (int i = 0; i < has.Length; i++)
            {
                if (!has[i])
                {
                    targetNumber = i+1;
                    break;
                }
            }

            // 新建军团菜单
            menuData.Add("新建军团", 10, this, OnClickMenuItem_CreateCorps, targetNumber > 1);

            // 重编军团菜单
            menuData.Add("重编军团", 11, this, OnClickMenuItem_RearrangeCorps, corps_list.Count > 0);

            // 解散军团菜单
            menuData.Add("解散军团", 12, this, OnClickMenuItem_DisbandCorps, corps_list.Count > 0);

            ContextMenu.Show(menuData, UnityEngine.Input.mousePosition);
        }

        public override void OnDestroy()
        {
            ContextMenu.Show(ContextMenuData.MenuData, ContextMenuData.MenuData.startPosition);
        }

        /// <summary>
        /// 新建军团菜单点击事件
        /// </summary>
        private void OnClickMenuItem_CreateCorps(IContextMenuItem contextMenuItem)
        {
            Window.Instance.Open("window_corps_setting", this);
        }

        /// <summary>
        /// 重编军团菜单点击事件
        /// </summary>
        private void OnClickMenuItem_RearrangeCorps(IContextMenuItem contextMenuItem)
        {
            //GameSystem.GetSystem<CorpsSelectSystem>().Start(corps_list,
            //target_corps_list, 1, OnCityChange, currentSystem.citySortTitleList, "目的城池选择");

        }

        /// <summary>
        /// 解散军团菜单点击事件
        /// </summary>
        private void OnClickMenuItem_DisbandCorps(IContextMenuItem contextMenuItem)
        {
            GameSystem.GetSystem<CorpsSelectSystem>().Start(corps_list,
            target_corps_list, 1, (x) =>
            {
                Corps t = x[0];
                GameDialog.Open($"要将{t.ColorName}解散吗?", () =>
                {
                    GameDialog.Close();
                    GameDialog.Open(GameDialog.DialogStyle.ClickPersonSay, $"如今{t.ColorName}已经没有必要了。", () =>
                    {
                        DeleteCorps(t.number);
                        GameDialog.Close();
                        ContextMenu.CloseAll();
                        Done();
                    }).SetPerson(TargetCity.BelongForce.Governor);
                }).cancelAction = () =>
                {
                    GameDialog.Close();
                };
            },
            CorpsSortFunction.DefaultSortList, "解散军团");
        }

        public void CreateCorps(int number, Person commander, List<City> cities)
        {
            TargetCity.BelongForce.CreateCorps(number, commander, cities);
        }

        public void DeleteCorps(int number)
        {
            TargetCity.BelongForce.DeleteCorps(number);
        }

        public void DeleteCorps(Corps corps)
        {
            TargetCity.BelongForce.DeleteCorps(corps);
        }
    }
}
