using Sango.Game.Player;
using Sango.Loader;
using Sango.Render;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{

    public class UIPersonMessageItem : MonoBehaviour
    {
        public GameObject personNode;
        public RawImage personHead;
        public Text personName;
        public Text content;
        public Text contentLeft;

        public void SetData(string text, Person person)
        {
            if (person == null)
            {
                personNode.SetActive(false);
                contentLeft.text = text;
            }
            else
            {
                personNode.SetActive(true);
                content.text = text;
                personHead.texture = GameRenderHelper.LoadHeadIcon(person.headIconID);
                personName.text = person.Name;
            }
        }

    }
}
