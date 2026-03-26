using Sango.Game.Action;
using Sango.Game.Render.UI;
using System.Collections.Generic;

namespace Sango.Game.Render
{
    public class CityRecruitPersonEvent : RenderEventBase
    {
        public override bool IsStack => true;
        public Person person;
        public Person target;
        static List<ActionBase> sJobActions = new List<ActionBase>();
        void InitJobFeature(Person person)
        {
            sJobActions.Clear();
            if (person != null && person.FeatureList != null)
            {
                for (int j = 0; j < person.FeatureList.Count; j++)
                {
                    Feature feature = person.FeatureList[j];
                    if (feature.kind == (int)FeatureKindType.CityProduce)
                        person.FeatureList[j].InitActions(sJobActions, person.BelongCity);
                }
            }
        }

        void ClearJobFeature()
        {
            for (int i = 0; i < sJobActions.Count; i++)
                sJobActions[i].Clear();
            sJobActions.Clear();
        }

        public override void Enter(Scenario scenario)
        {
            InitJobFeature(person);
            if (!person.BelongCorps.IsPlayer)
            {
                person.JobRecruitPerson(target, 0);
                IsDone = true;
                ClearJobFeature();
                return;
            }

            if (person.JobRecruitPerson(target, 0))
            {
                UIDialog dialog1 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"成功招募了{target.ColorName}", () =>
                {
                    // TODO:展示武将
                    // 暂时直接招募
                    UIDialog.Close();
                    UIDialog dialog2 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"{target.ColorName}愿为主公献犬马之劳", () =>
                    {
                        // TODO:展示武将
                        // 暂时直接招募
                        UIDialog.Close();
                        IsDone = true;
                    });
                    dialog2.SetPerson(target);
                });
                dialog1.SetPerson(person);
            }
            else
            {
                UIDialog dialog1 = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"很遗憾，\n未能招募到{target.ColorName}", () =>
                {
                    // TODO:展示武将
                    // 暂时直接招募
                    ClearJobFeature();
                    UIDialog.Close();
                    IsDone = true;
                });
                dialog1.SetPerson(person);
            }
            ClearJobFeature();

        }

        public override void Exit(Scenario scenario)
        {

        }

        public override bool IsVisible()
        {
            return person.BelongCorps.IsPlayer;
        }

        public override bool Update(Scenario scenario, float deltaTime)
        {
            return IsDone;
        }
    }
}
