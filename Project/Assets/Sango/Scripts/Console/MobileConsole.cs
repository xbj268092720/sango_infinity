using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace SKFramework
{
    public class MobileConsole : MonoBehaviour
    {
        // 日志显示最大条数
        private static readonly int MAX_LOG = 250;
        private static readonly int WND_ID = 0x1435;
        private static readonly float EDGE_X = 100, EDGE_Y = 50;

        struct LogData
        {
            public string str;
            public int type;
            public LogData(string s, int t)
            {
                str = s;
                type = t;
            }
        }

        private List<LogData> logList;

        public bool Visible = false;

        public string cmd = "";

        private Vector2 scrollPos;

        GUIStyle errorStyle;
        GUIStyle waringStyle;
        GUIStyle normalStyle;


        private bool beScroll = false;
        private Vector3 saveLastMousePosition;

        #region 开关,在移动设备上3点触控打开,或者在PC上鼠标左右键同时点击打开
        private float CoolDown_ = 0;
        void Update()
        {
            if (Visible && Input.GetKeyDown(KeyCode.Escape))
            {
                Visible = false;
                return;
            }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            //if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && Time.time - CoolDown_ > 2.0f)
#else
            if (Input.touchCount > 3 && Time.time - CoolDown_ > 2.0f)
            {
                Visible = !Visible;
                CoolDown_ = Time.time;
            }
#endif

        }
        #endregion // 开关

        private Rect rcWindow_;
        void OnGUI()
        {
            if (!Visible) { return; }

            EventType et = Event.current.type;
            if (et == EventType.Repaint || et == EventType.Layout)
            {
                this.rcWindow_ = new Rect(EDGE_X, EDGE_Y, Screen.width - EDGE_X * 2, Screen.height - EDGE_Y * 2);
                GUI.Window(WND_ID, rcWindow_, WindowFunc, string.Empty);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!beScroll)
                    saveLastMousePosition = Input.mousePosition;

            }

            if (Input.GetMouseButtonUp(0))
            {

            }



        }

        void WindowFunc(int id)
        {
            try
            {
                GUILayout.BeginVertical();
                try
                {
                    var queue = this.logList;
                    if (queue.Count > 0)
                    {
                        scrollPos = GUILayout.BeginScrollView(scrollPos);
                        int fs = GUI.skin.label.fontSize;
                        GUI.skin.label.fontSize = 35;
                        try
                        {
                            for (int i = 0; i < queue.Count; ++i)
                            {
                                LogData s = queue[i];
                                switch (s.type)
                                {
                                    case 0:
                                        GUILayout.Label(s.str, normalStyle);
                                        break;
                                    case 1:
                                        GUILayout.Label(s.str, waringStyle);
                                        break;
                                    case 2:
                                        GUILayout.Label(s.str, errorStyle);
                                        break;
                                }
                            }
                        }
                        finally
                        {
                            GUILayout.EndScrollView();
                        }
                        GUI.skin.label.fontSize = fs;
                    }
                }
                finally
                {
                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) SendCmd();

                    GUILayout.BeginHorizontal();
                    cmd = GUILayout.TextField(cmd);
                    if (GUILayout.Button("发送", GUILayout.Width(100))) SendCmd();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        void Awake()
        {
            this.scrollPos = new Vector2(0, 1);

            errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;

            waringStyle = new GUIStyle();
            waringStyle.normal.textColor = Color.yellow;

            normalStyle = new GUIStyle();
            normalStyle.normal.textColor = Color.green;

            logList = new List<LogData>(MAX_LOG);

            Application.logMessageReceived += LogCallback;
        }

        void LogCallback(string condition, string stackTrace, LogType type)
        {
            while (logList.Count >= MAX_LOG)
            {
                logList.RemoveAt(0);
            }
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                    logList.Add(new LogData(condition + stackTrace, 2));
                    break;
                case LogType.Warning:
                    logList.Add(new LogData(condition, 1));
                    break;
                default:
                    logList.Add(new LogData(condition, 0));
                    break;
            }
        }

        //向lua发送控制台输入命令
        void SendCmd()
        {
            if (string.IsNullOrEmpty(cmd)) return;
            //;;;;;;;;;;;;;;;;;;;;;;;

            cmd = string.Empty;
        }
    }
}
