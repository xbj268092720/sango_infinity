using Sango.Game.Action;
using Sango.Game.Render.UI;
using System.Collections.Generic;

namespace Sango.Game.Render
{
    public class CityRecruitPersonWhenCityFallEvent : RenderEventBase
    {
        public List<Person> captiveList;
        public City targetCity;
        public Troop atk;
        public int recruitType;
        public override void Enter(Scenario scenario)
        {
            if (captiveList.Count == 0)
            {
                IsDone = true;
                return;
            }

            if (!atk.BelongCorps.IsPlayer)
            {
                for (int i = captiveList.Count - 1; i >= 0; i--)
                {
                    Person person = captiveList[i];
                    if (atk.BelongForce.Governor.JobRecruitPerson(person, targetCity, recruitType))
                    {
#if SANGO_DEBUG
                        Sango.Log.Print($"{person.Name} 加入了 {atk.BelongForce} 势力!!!");
#endif

                        person.ChangeCorps(atk.BelongCorps);
                        captiveList.RemoveAt(i);
                    }
                    else
                    {
                        targetCity.allPersons.Remove(person);
                        // TODO: 释放,斩杀
                        person.BeCaptive(targetCity);
                    }
                }
                IsDone = true;
                return;
            }

            Next();
        }

        void Next()
        {
            if (captiveList.Count == 0)
            {
                IsDone = true;
                return;
            }
            else
            {
                //展示武将
                GameSystem.GetSystem<PersonRecruit>().Start(targetCity, atk, captiveList[0], recruitType, 3, x =>
                {
                    captiveList.RemoveAt(0);
                    Next();
                });
            }
        }

        public override void Exit(Scenario scenario)
        {

        }

        public override bool IsVisible()
        {
            return atk.BelongCorps.IsPlayer;
        }

        public override bool Update(Scenario scenario, float deltaTime)
        {
            return IsDone;
        }
    }
}
