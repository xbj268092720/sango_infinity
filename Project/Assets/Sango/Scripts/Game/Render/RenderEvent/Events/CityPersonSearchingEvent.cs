using Sango.Game.Render.UI;
using UnityEngine;

namespace Sango.Game.Render
{
    public class CityPersonSearchingEvent : RenderEventBase
    {
        public City city;
        public Person person;
        public Person target;

        public void Init(City city, Person person)
        {
            this.city = city;
            this.person = person;
            this.target = null;
            IsDone = false;
        }
        public override void Enter(Scenario scenario)
        {
            int rs = city.DoJobSearching(person, out target);
            if (rs < 0)
            {
                if (city.BelongCorps.IsPlayer)
                {
                    UIDialog dialog3 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, "很遗憾, 什么都没有发现...", () =>
                    {
                        UIDialog.Close();
                        IsDone = true;
                    });
                    dialog3.SetPerson(person);
                }
                else
                {
                    IsDone = true;
                }
                return;
            }

            if(target == null)
            {
                IsDone = true;
                return;
            }

            if (!city.BelongCorps.IsPlayer)
            {
                person.JobRecruitPerson(target, 3);
                IsDone = true;
                return;
            }

            if (rs == 0)
            {

                string content = $"搜索结果，\n发现了名为{target.ColorName}的武将。";
                UIDialog dialog = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, content, () =>
                {
                    UIDialog.Close();

                    //展示武将
                    GameSystem.GetSystem<PersonRecruit>().Start(person, target, 0, 1, x =>
                    {
                        if (x.result == 1)
                        {
                            UIDialog dialog1 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"成功招募了{target.ColorName}", () =>
                            {
                                UIDialog.Close();
                                UIDialog dialog2 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"{target.ColorName}愿为主公献犬马之劳", () =>
                                {
                                    UIDialog.Close();
                                    IsDone = true;
                                });
                                dialog2.SetPerson(target);
                            });
                            dialog1.SetPerson(person);
                        }
                        else if (x.result == 0)
                        {
                            UIDialog dialog1 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"很遗憾，\n未能招募到{target.ColorName}", () =>
                            {
                                UIDialog.Close();
                                IsDone = true;
                            });
                            dialog1.SetPerson(person);
                        }
                        else
                            IsDone = true;
                    });
                });
                dialog.SetPerson(person);
            }
            else
            {

                UIDialog dialog = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"发现了资金{rs}", () =>
                {
                    UIDialog.Close();
                    IsDone = true;
                });
                dialog.SetPerson(person);
            }
        }

        public override void Exit(Scenario scenario)
        {

        }

        public override bool IsVisible()
        {
            return city.BelongCorps.IsPlayer;
        }

        public override bool Update(Scenario scenario, float deltaTime)
        {
            return IsDone;
        }
    }
}
