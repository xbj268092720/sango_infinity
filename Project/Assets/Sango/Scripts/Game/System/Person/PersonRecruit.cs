using Sango.Game.Render;
using Sango.Game.Render.UI;

namespace Sango.Game
{
    [GameSystem]
    public class PersonRecruit : GameSystem
    {
        public Person target;
        public Person recruitor;
        public City fallCity;
        public Troop atker;
        public int recruitType;
        public int tryLimit;
        public int result = 0;
        public System.Action<PersonRecruit> doneAction;

        public void Start(Person recruitor, Person target, int recruitType, int limit, System.Action<PersonRecruit> doneAction)
        {
            result = 0;
            tryLimit = limit;
            this.recruitor = recruitor;
            this.fallCity = null;
            this.atker = null;
            this.target = target;
            this.recruitType = recruitType;
            this.doneAction = doneAction;
            Push();
        }

        public void Start(City fallCity, Troop atker, Person target, int recruitType, int limit, System.Action<PersonRecruit> doneAction)
        {
            result = 0;
            tryLimit = limit;
            this.recruitor = atker.BelongForce.Governor;
            this.fallCity = fallCity;
            this.atker = atker;
            this.target = target;
            this.recruitType = recruitType;
            this.doneAction = doneAction;
            Push();
        }

        public void Start(Troop atker, Person target, int recruitType, int limit, System.Action<PersonRecruit> doneAction)
        {
            result = 0;
            tryLimit = limit;
            this.recruitor = atker.BelongForce.Governor;
            this.fallCity = null;
            this.atker = atker;
            this.target = target;
            this.recruitType = recruitType;
            this.doneAction = doneAction;
            Push();
        }

        public override void OnEnter()
        {
            Window.Instance.Open("window_person_recruit_info");
        }

        public override void OnDestroy()
        {
            Window.Instance.Close("window_person_recruit_info");
        }

        public override void HandleEvent(CommandEventType eventType, Cell cell, UnityEngine.Vector3 clickPosition, bool isOverUI)
        {

        }

        string[] talk = new string[]
        {
            "杀了我吧，我是不会加入你们的！！",
            "休想让我替你卖命！！",
            "宁死不降！！"
        };

        // 招募
        public void RecruitTarget()
        {
            if (tryLimit > 0)
            {
                if (fallCity != null)
                    result = recruitor.JobRecruitPerson(target, fallCity, recruitType) == true ? 1 : 0;
                else
                    result = recruitor.JobRecruitPerson(target, recruitType) == true ? 1 : 0;
                if (result == 1)
                {
                    UIDialog dialog1 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"{target.ColorName}愿为主公献犬马之劳", () =>
                    {
                        UIDialog.Close();
                        Done();
                        doneAction?.Invoke(this);
                    });
                    dialog1.SetPerson(target);
                }
                tryLimit--;
            }

            if (tryLimit <= 0 && atker == null && fallCity == null)
            {
                Done();
                doneAction?.Invoke(this);
            }
        }

        public void RecruitTarget2()
        {
            if (tryLimit > 0)
            {
                if (fallCity != null)
                    result = recruitor.JobRecruitPerson(target, fallCity, recruitType) == true ? 1 : 0;
                else
                    result = recruitor.JobRecruitPerson(target, recruitType) == true ? 1 : 0;
                if (result == 0)
                {
                    UIDialog dialog1 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, talk[GameRandom.Range(0, talk.Length)], () =>
                    {
                        UIDialog.Close();
                    });
                    dialog1.SetPerson(target);
                }
                else if (result == 1)
                {
                    UIDialog dialog1 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"{target.ColorName}愿为主公献犬马之劳", () =>
                    {
                        UIDialog.Close();
                        Done();
                        doneAction?.Invoke(this);
                    });
                    dialog1.SetPerson(target);
                }
                tryLimit--;
            }
        }


        // 释放
        public void ReleaseTarget()
        {
            result = 2;
            Force releaseForce = atker?.BelongForce ?? fallCity?.BelongForce;
            target.Escape(EscapeType.Released);
            GameEvent.OnPersonRelease?.Invoke(target, releaseForce);
            Done();
            doneAction?.Invoke(this);
        }

        // 斩首
        public void KillTarget()
        {
            result = 3;
            Force executeForce = atker?.BelongForce ?? fallCity?.BelongForce;
            target.Dead();
            GameEvent.OnPersonExecute?.Invoke(target, executeForce);
            Done();
            doneAction?.Invoke(this);
        }

        // 收押
        public void DetainTarget()
        {
            result = 3;
            if (fallCity != null)
                target.BeCaptive(fallCity);
            else if (atker != null)
                target.BeCaptive(atker);
            Done();
            doneAction?.Invoke(this);
        }

        public void Cancel()
        {
            result = -1;
            Done();
            doneAction?.Invoke(this);
        }
    }
}
