using Sango.Core;

namespace Sango.Render
{
    public class DiplomacyEvent : RenderEventBase
    {
        public Person person;
        public DiplomacyActionType actionType;
        public Force receiverForce;
        public City targetCity;
        public int resourceValue;
        public int captiveId;
        public void Init(Person person, DiplomacyActionType actionType, Force receiverForce, City targetCity, int resourceValue, int captiveId)
        {
            this.person = person;
            this.actionType = actionType;
            this.receiverForce = receiverForce;
            this.targetCity = targetCity;
            this.resourceValue = resourceValue;
            this.captiveId = captiveId;
            IsDone = false;
        }

        public override void Enter(Scenario scenario)
        {
            if (person.IsPlayer || receiverForce.IsPlayer)
            {

            }
            else
            {
                DiplomacyManager.Instance.DoPersonDiplomacyAction(person, actionType, receiverForce, resourceValue, captiveId);
                // 完成任务，返回原城市
                person.SetMission(MissionType.PersonReturn, person.BelongCity);
                IsDone = true;
            }
        }
    }
}
