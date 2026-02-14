using System;
using System.IO;
using GameCore.Edit;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public class CaptureScreenshotEditor : EditorWindowBase
    {
        private EditDataStringField fullScreenShotDir;
        private EditDataStringField fullScreenShotFileName;
        private EditDataStringField cameraShotDir;
        private EditDataStringField cameraShotFileName;
        private Camera camera;


        [MenuItem("Tools/Misc/CaptureScreenshot")]
        public static void Open()
        {
            GetWindow<CaptureScreenshotEditor>("CaptureScreenshot");
        }

        protected override void InitEditDataFields()
        {
            base.InitEditDataFields();
            fullScreenShotDir = new EditDataStringField("CaptureScreenshotEditor_FullScreenShotDir", "", this);
            fullScreenShotFileName = new EditDataStringField("CaptureScreenshotEditor_FullScreenShotFileName", "", this);
            cameraShotDir = new EditDataStringField("CaptureScreenshotEditor_cameraShotDir", "", this);
            cameraShotFileName = new EditDataStringField("CaptureScreenshotEditor_cameraShotFileName", "", this);
        }

        private void OnGUI()
        {
            //截屏
            GUILayout.Label("FullScreenshot");
            using (GUIUtil.LayoutHorizontal())
            {
                var dir = GetFullScreenShotDir();
                GUILayout.Label(dir, GUILayout.ExpandWidth(false));
                fullScreenShotFileName.Value = GUILayout.TextField(GetFullScreenShotFileName(), GUILayout.Width(200));
                GUILayout.Label(".png", GUILayout.ExpandWidth(false));
                if(GUIUtil.Button("···"))
                {
                    fullScreenShotDir.Value = EditorUtility.OpenFolderPanel("", dir, "");
                }
            }
            if (GUILayout.Button("FullScreenshot"))
            {
                var savePath = Path.Combine(GetFullScreenShotDir(), GetFullScreenShotFileName()+".png");
                CheckFileExists(savePath, () =>
                {
                    ScreenCapture.CaptureScreenshot(savePath);
                    if (EditorUtility.DisplayDialog("", "截图成功，是否查看 (如果没有图返回unity让unity执行完)", "Y", "N"))
                    {
                        EditorUtility.RevealInFinder(savePath);
                    }
                });
            }

            //截相机
            GUILayout.Space(12);
            GUILayout.Label("FullScreenshot");
            using(GUIUtil.LayoutHorizontal())
            {
                var dir = GetCameraShotDir();
                GUILayout.Label(dir, GUILayout.ExpandWidth(false));
                cameraShotFileName.Value = GUILayout.TextField(GetFullScreenShotFileName(), GUILayout.Width(200));
                GUILayout.Label(".png", GUILayout.ExpandWidth(false));
                if(GUIUtil.Button("···"))
                {
                    cameraShotDir.Value = EditorUtility.OpenFolderPanel("", dir, "");
                }
            }
            camera = EditorGUILayout.ObjectField("camera", camera, typeof(Camera), true) as Camera;
            if (camera == null)
            {
                GUILayout.Label("No camera");
            }
            else
            {
                if (camera.activeTexture == null)
                {
                    GUILayout.Label("No RenderTexture on camera");
                }
            }

            if (GUILayout.Button("CaptureCamera"))
            {
                var savePath = Path.Combine(GetCameraShotDir(), GetCameraShotFileName()+".png");
                CheckFileExists(savePath, () =>
                {
                    var rdt = camera.activeTexture;
                    RenderTexture.active = rdt;
                    Texture2D tex = new Texture2D(rdt.width, rdt.height);
                    tex.ReadPixels(new Rect(0, 0, rdt.width, rdt.height), 0, 0);
                    RenderTexture.active = null;
                    var bytes = tex.EncodeToPNG();
                    File.WriteAllBytes(savePath, bytes);

                    if(EditorUtility.DisplayDialog("", "截图成功，是否查看 (如果没有图返回unity让unity执行完)", "Y", "N"))
                    {
                        EditorUtility.RevealInFinder(savePath);
                    }
                });
            }
        }

        private string GetFullScreenShotDir()
        {
            return string.IsNullOrEmpty(fullScreenShotDir.Value) ? Application.dataPath : fullScreenShotDir.Value;
        }

        private string GetFullScreenShotFileName()
        {
            return string.IsNullOrEmpty(fullScreenShotFileName.Value) ? "1" : fullScreenShotFileName.Value;
        }
        

        private string GetCameraShotDir()
        {
            return string.IsNullOrEmpty(cameraShotDir.Value) ? Application.dataPath : cameraShotDir.Value;
        }

        private string GetCameraShotFileName()
        {
            return string.IsNullOrEmpty(cameraShotFileName.Value) ? "1" : cameraShotFileName.Value;
        }

        private void CheckFileExists(string file, Action action)
        {
            if (File.Exists(file))
            {
                var rt = EditorUtility.DisplayDialog("提示", "文件已存在，是否替换", "确定", "取消");
                if (rt)
                {
                    action();
                }
            }
            else
            {
                action();
            }
        }
    }
}