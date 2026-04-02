using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIWorker : MonoBehaviour
    {
        public RawImage head;
        public void SetPerson(Person person)
        {
            if (person == null)
            {
                head.enabled = false;
                return;
            }
            head.texture = GameRenderHelper.LoadHeadIcon(person.headIconID, 1);
            head.enabled = true;
        }

        public void SetEnabled(bool b)
        {
            gameObject.SetActive(b);
        }
    }
}