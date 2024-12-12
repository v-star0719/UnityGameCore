using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using GameCore.Edit;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace GameCore.Unity
{
    public class GameConsoleWindow : EditorWindow
    {
        public static GameConsoleWindow window;
        public static string configFileName = "/GameKitGameConsoleWindow.xml";

        private string playerName;
        private UnityWebRequest www;
        private string log = "";
        private UnityWebRequestAsyncOperation operation;
        private List<GameConsoleFuncBase> functionList = new ();
        private float refreshTimer = 0;

        private Vector2 scrollPos;

        [MenuItem("Tools/GameConsole", false)]
        public static void ShowWindow()
        {
            GetWindow<GameConsoleWindow>();
        }

        private void Init()
        {
            if(functionList.Count > 0 && functionList[0].name != null)
            {
                return;
            }

            functionList.Clear();
            if(!Load())
            {
                functionList.Add(new GameConsoleGame());
            }

            foreach(var funcBase in functionList)
            {
                funcBase.Init(this);
            }
        }

        private bool Load()
        {
            if(File.Exists(GetConfigFilePath()))
            {
                FileStream fs = File.Open(GetConfigFilePath(), FileMode.Open);
                using(StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    XmlSerializer xz = new XmlSerializer(functionList.GetType());
                    functionList = (List<GameConsoleFuncBase>)xz.Deserialize(sr);
                }

                return true;
            }

            return false;
        }

        private void Save()
        {
            using(StringWriter sw = new StringWriter())
            {
                XmlSerializer xz = new XmlSerializer(functionList.GetType());
                xz.Serialize(sw, functionList);
                File.WriteAllText(GetConfigFilePath(), sw.ToString());
            }
        }

        private string GetConfigFilePath()
        {
            //Debug.Log(Application.persistentDataPath + configFileName);
            return Application.persistentDataPath + configFileName;
        }

        private void Update()
        {
            //todo 优化放到别的地方
            Init();

            refreshTimer += Time.deltaTime;
            if (refreshTimer > 0.3f)
            {
                refreshTimer = 0f;
                Repaint();
            }

            if (Application.isPlaying)
            {
                //var seceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                //if (seceneName == "Game" || seceneName == "TowerDefense" || seceneName == "City")
                {
                }
            }

            if (operation != null)
            {
                if (operation.isDone)
                {
                    if (www.result == UnityWebRequest.Result.ProtocolError)
                    {
                        log = www.error;
                    }
                    else
                    {
                        log = www.downloadHandler.text;
                    }

                    operation = null;
                    www = null;
                }
            }

            foreach(var funcBase in functionList)
            {
                funcBase.Update();
            }
        }

        private void OnGUI()
        {
            using (GUIUtil.Scroll(ref scrollPos))
            {
                using (GUIUtil.LayoutHorizontal("box"))
                {
                    GUILayout.Label("用户名：", GUILayout.ExpandWidth(false));
                    var name = GetUserName();
                    var serverId = GetServerId();
                    var alliShort = GetAllianceShortName();
                    var server = serverId > 0 ? serverId.ToString() : "未知";
                    GUILayout.Label($"服务器：{server}, 角色：{name}, 联盟：{alliShort}", GUILayout.ExpandWidth(false));
                }

                foreach (var func in functionList)
                {
                    var foldout = EditorPrefs.GetBool(func.GetType().Name, false);
                    var foldt = EditorGUIUtil.Foldout(foldout, func.name, true);
                    if (foldt != foldout)
                    {
                        EditorPrefs.SetBool(func.GetType().Name, foldt);
                    }

                    if (foldout)
                    {
                        using (GUIUtil.LayoutVertical("box"))
                        {
                            func.OnGUI();
                        }
                    }
                }

                GUILayout.Label(log);
            }
        }

        public void Send(string method, object v1 = null, object v2 = null, object v3 = null, object v4 = null,
            object v5 = null, object v6 = null)
        {
            if(v6 == null)
            {
                v6 = GetUserName();
            }

            string info = "";
            WWWForm form = new WWWForm();
            if(v1 != null) { form.AddField("v1", v1.ToString()); info += v1.ToString() + "; "; }
            if(v2 != null) { form.AddField("v2", v2.ToString()); info += v2.ToString() + "; "; }
            if(v3 != null) { form.AddField("v3", v3.ToString()); info += v3.ToString() + "; "; }
            if(v4 != null) { form.AddField("v4", v4.ToString()); info += v4.ToString() + "; "; }
            if(v5 != null) { form.AddField("v5", v5.ToString()); info += v5.ToString() + "; "; }
            if(v6 != null) { form.AddField("v6", v6.ToString()); info += v6.ToString() + "; "; }
            Debug.Log("Request Form：" + info);

            string url = "www.baidu.com";

            if(!string.IsNullOrEmpty(url))
            {
                www = UnityWebRequest.Post(url + method, form);
                operation = www.SendWebRequest();
                Save();
            }
        }

        public string GetUserName()
        {
            return "";
        }

        public string GetAllianceShortName()
        {
            return "";
        }

        public void CallServerAPI(string funcName, params object[] p)
        {
        }

        private int GetServerId()
        {
            return 0;
        }
    }
}
