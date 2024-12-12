using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public class CaptureScreenshotEditor : EditorWindow
    {
        private string fullScreenShotPath;
        private string cameraShotPath;
        private Camera camera;


        [MenuItem("Tools/Misc/CaptureScreenshot")]
        public static void Open()
        {
            GetWindow<CaptureScreenshotEditor>("CaptureScreenshot");
        }

        private void OnGUI()
        {
            //截屏
            GUILayout.Label("FullScreenshot");
            fullScreenShotPath = EditorGUILayout.TextField("fullScreenShotPath", fullScreenShotPath);
            if (GUILayout.Button("FullScreenshot"))
            {
                CheckFileExists(fullScreenShotPath, () => { ScreenCapture.CaptureScreenshot(fullScreenShotPath); });
            }

            //截相机
            GUILayout.Space(12);
            GUILayout.Label("FullScreenshot");
            cameraShotPath = EditorGUILayout.TextField("cameraShotPath", cameraShotPath);
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
                CheckFileExists(cameraShotPath, () =>
                {
                    var rdt = camera.activeTexture;
                    RenderTexture.active = rdt;
                    Texture2D tex = new Texture2D(rdt.width, rdt.height);
                    tex.ReadPixels(new Rect(0, 0, rdt.width, rdt.height), 0, 0);
                    RenderTexture.active = null;
                    var bytes = tex.EncodeToPNG();
                    File.WriteAllBytes(cameraShotPath, bytes);
                });
            }
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