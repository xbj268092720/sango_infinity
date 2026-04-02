using Sango.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    /// <summary>
    /// 游戏开始界面
    /// </summary>
    public class UIDialog : UGUIWindow
    {
        public enum DialogStyle
        {
            Normal,
            ChoosePersonSay,
            Window,
            ClickPersonSay,
        }

        public Text content;
        public System.Action cancelAction;
        public System.Action sureAction;
        public RectTransform panelRect;
        public RectTransform btnRect;
        public RawImage headImg;
        public Text nameText;
        public List<TalkData> talkData;
        public System.Action talkEndAction;

        static UIDialog CurInstance;

        public void OnSure()
        {
            sureAction?.Invoke();
        }

        public void OnCancel()
        {
            cancelAction?.Invoke();
        }

        public struct TalkData
        {
            public string text;
            public Person person;
            public string sound;
            public string bgm;
        }

        public static UIDialog StartTalk(List<TalkData> talk_content, System.Action endAction)
        {
            string windowName = "window_dialog2";
            UIDialog uIDialog = Open(windowName, "", null, Input.mousePosition);
            uIDialog._StartTalk(talk_content, endAction);
            return uIDialog;
        }

        public static UIDialog Open(string content, System.Action sureAction)
        {
            return Open(content, sureAction, Input.mousePosition);
        }

        public static UIDialog Open(DialogStyle dialogStyle, string content, System.Action sureAction)
        {
            string windowName = "window_dialog";
            switch (dialogStyle)
            {
                case DialogStyle.ChoosePersonSay:
                    windowName = "window_dialog2"; break;
                case DialogStyle.Window:
                    windowName = "window_dialog3"; break;
                case DialogStyle.ClickPersonSay:
                    windowName = "window_dialog4"; break;
            }
            return Open(windowName, content, sureAction, Input.mousePosition);
        }

        public static UIDialog Open(string content, System.Action sureAction, Vector3 startPoint)
        {
            return Open("window_dialog", content, sureAction, startPoint);
        }

        public static UIDialog Open(string windowName, string content, System.Action sureAction, Vector3 startPoint)
        {
            Window.WindowInterface windowInterface = Window.Instance.Open(windowName);
            if (windowInterface.ugui_instance == null)
                return null;

            UIDialog uIDialog = windowInterface.ugui_instance.GetComponent<UIDialog>();
            if (uIDialog == null) return null;

            uIDialog.content.text = content;
            uIDialog.sureAction = sureAction;
            uIDialog.cancelAction = Close;
            CurInstance = uIDialog;

            if (uIDialog.btnRect != null)
            {
                Vector2 anchorPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(uIDialog.GetComponent<RectTransform>(),
                    startPoint, Game.Instance.UICamera, out anchorPos);

                uIDialog.btnRect.anchoredPosition = anchorPos + new Vector2(-74, 0);
            }

            return uIDialog;
        }

        void _StartTalk(List<TalkData> talkData, System.Action talkEndAction)
        {
            this.talkData = talkData;
            this.talkEndAction = sureAction;
            NextTalk();
        }

        void NextTalk()
        {
            TalkData data = talkData[0];
            talkData.RemoveAt(0);
            if (talkData.Count == 0)
                sureAction = talkEndAction;
            else
                sureAction = NextTalk;
            SetPerson(data.person);
            content.text = data.text;
            if (string.IsNullOrEmpty(data.sound))
                AudioManager.Instance.PlaySfx(data.sound);
            if (string.IsNullOrEmpty(data.bgm))
                AudioManager.Instance.PlayBgm(data.bgm);
        }


        public void SetPerson(Person person)
        {
            if (headImg == null || nameText == null) return;
            if (person == null)
            {
                headImg.enabled = false;
                nameText.text = "";
                return;
            }

            headImg.enabled = true;
            headImg.texture = GameRenderHelper.LoadHeadIcon(person.headIconID, 1);
            nameText.text = person.Name;
        }

        public static void Close()
        {
            CurInstance?.Hide();
            CurInstance = null;
        }

        public static void Close(UIDialog uIDialog)
        {
            if (uIDialog == CurInstance)
                uIDialog.Hide();
            CurInstance = null;
        }
    }
}
