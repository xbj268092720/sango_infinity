using Sango.Core;
using System.Collections.Generic;

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
        private bool success;
        public void Init(Person person, DiplomacyActionType actionType, Force receiverForce, City targetCity, int resourceValue, int captiveId)
        {
            this.person = person;
            this.actionType = actionType;
            this.receiverForce = receiverForce;
            this.targetCity = targetCity;
            this.resourceValue = resourceValue;
            this.captiveId = captiveId;
            IsDone = false;
            success = false;
        }
        public override void Exit(Scenario scenario)
        {
            Window.Instance.Close("window_city_diplomacy_scene");
            GameDialog.Close();
        }

        public override void Enter(Scenario scenario)
        {
            if (person.IsPlayer || receiverForce.IsPlayer)
            {
                Window.Instance.Open("window_city_diplomacy_scene");
                switch (actionType)
                {
                    case DiplomacyActionType.Alliance:
                        // 关系值越高，成功率越高，最高90%
                        break;
                    case DiplomacyActionType.Truce:
                        // 关系值越高，成功率越高，最高80%
                        break;
                    case DiplomacyActionType.DeclareWar:
                        // 宣战总是成功
                        break;
                    case DiplomacyActionType.SendGift:
                        DelayEvent delay = RenderEvent.Instance.Create<DelayEvent>();
                        delay.Init(1, () =>
                        {
                            List<GameDialog.TalkData> talkDatas = new List<GameDialog.TalkData>
                            {
                                new GameDialog.TalkData
                                {
                                    person = receiverForce.Governor,
                                    text = $"{person.ColorName},别来无恙!",
                                },
                                 new GameDialog.TalkData
                                {
                                    person = person,
                                    text = $"我代表我家主公{person.BelongForce.Governor.ColorName}向您呈上最真挚的情谊,此次携带1000金,还请{person.ColorName}笑纳!",
                                },
                                 new GameDialog.TalkData
                                {
                                     person = receiverForce.Governor,
                                    text = $"既然{person.BelongForce.Governor.ColorName}这么有心意,那我就收下了!!!",
                                },
                            };
                            GameDialog.StartTalk(talkDatas, () => { IsDone = true; });
                        });
                        RenderEvent.Instance.Add(delay);
                        // 送礼总是成功
                        break;
                    case DiplomacyActionType.RequestTechnique:
                        // 关系值越高，成功率越高，最高85%
                        break;
                    case DiplomacyActionType.RequestTroops:
                        // 关系值越高，成功率越高，最高80%
                        break;
                    case DiplomacyActionType.Trade:
                        // 关系值越高，成功率越高，最高95%
                        break;
                    case DiplomacyActionType.Marriage:
                        // 关系值越高，成功率越高，最高95%
                        break;
                    case DiplomacyActionType.AllianceRequest:
                        // 关系值越高，成功率越高，最高85%
                        break;
                    case DiplomacyActionType.TruceRequest:
                        // 关系值越高，成功率越高，最高75%
                        break;
                    case DiplomacyActionType.Ransom:
                        // 关系值越高，成功率越高，最高90%
                        break;
                    default:
                        break;
                }

                //if (receiverForce.IsPlayer)
                //{
                //    // 计算成功率
                //    int successRate = DiplomacyManager.Instance.CalculateDiplomacySuccessRate(actionType, person.BelongForce, receiverForce, person, resourceValue);
                //    success = GameRandom.Chance(successRate);


                //}
                //else
                //{
                //    // 计算成功率
                //    int successRate = DiplomacyManager.Instance.CalculateDiplomacySuccessRate(actionType, person.BelongForce, receiverForce, person, resourceValue);
                //    success = GameRandom.Chance(successRate);
                //    switch (actionType)
                //    {
                //        case DiplomacyActionType.Alliance:
                //            // 关系值越高，成功率越高，最高90%
                //            break;
                //        case DiplomacyActionType.Truce:
                //            // 关系值越高，成功率越高，最高80%
                //            break;
                //        case DiplomacyActionType.DeclareWar:
                //            // 宣战总是成功
                //            break;
                //        case DiplomacyActionType.SendGift:
                //            DelayEvent delay = RenderEvent.Instance.Create<DelayEvent>();
                //            delay.Init(1);
                //            RenderEvent.Instance.Add(delay);
                //            List<GameDialog.TalkData> talkDatas = new List<GameDialog.TalkData>
                //            {
                //                new GameDialog.TalkData
                //                {
                //                    person = receiverForce.Governor,
                //                    text = $"{person.ColorName},别来无恙!",
                //                },
                //                 new GameDialog.TalkData
                //                {
                //                    person = person,
                //                    text = $"我代表我家主公{person.BelongForce.Governor.ColorName}向您呈上最真挚的情谊,此次携带1000金,还请{person.ColorName}笑纳!",
                //                },
                //                 new GameDialog.TalkData
                //                {
                //                     person = receiverForce.Governor,
                //                    text = $"既然{person.BelongForce.Governor.ColorName}这么有心意,那我就收下了!!!",
                //                },
                //            };

                //            GameDialog.StartTalk(talkDatas, () => { IsDone = true; });
                //            // 送礼总是成功
                //            break;
                //        case DiplomacyActionType.RequestTechnique:
                //            // 关系值越高，成功率越高，最高85%
                //            break;
                //        case DiplomacyActionType.RequestTroops:
                //            // 关系值越高，成功率越高，最高80%
                //            break;
                //        case DiplomacyActionType.Trade:
                //            // 关系值越高，成功率越高，最高95%
                //            break;
                //        case DiplomacyActionType.Marriage:
                //            // 关系值越高，成功率越高，最高95%
                //            break;
                //        case DiplomacyActionType.AllianceRequest:
                //            // 关系值越高，成功率越高，最高85%
                //            break;
                //        case DiplomacyActionType.TruceRequest:
                //            // 关系值越高，成功率越高，最高75%
                //            break;
                //        case DiplomacyActionType.Ransom:
                //            // 关系值越高，成功率越高，最高90%
                //            break;
                //        default:
                //            break;
                //    }

                //}
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
