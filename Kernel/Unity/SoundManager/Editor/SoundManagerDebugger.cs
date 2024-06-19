using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sound.Editor
{
    public class SoundManagerDebugger : EditorWindow
    {
        //test one
        private SoundLayer layer;
        private string soundName;
        private bool isLoop;
        private float duration;
        private float fadeIn;
        private float fadeOut;
        private float volume = 1;

        private float refreshMargin;
        private bool isPaused;

        private string[] foldoutKeys = new[]
        {
            "SoundManagerDebugger_player0",
            "SoundManagerDebugger_player1",
            "SoundManagerDebugger_player2",
            "SoundManagerDebugger_player3",
        };

        [MenuItem("Tools/Debugger/SoundManagerDebugger")]
        public static void OpenSoundManagerDebugger()
        {
            GetWindow<SoundManagerDebugger>();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                refreshMargin += Time.deltaTime;
                if (refreshMargin > 0.5f)
                {
                    refreshMargin = 0f;
                    Repaint();
                }
            }
        }

        private void OnGUI()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Play one ", GUILayout.ExpandWidth(false));
                using(new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("SoundLayer: ", GUILayout.ExpandWidth(false));
                    layer = (SoundLayer)EditorGUILayout.EnumPopup(layer, GUILayout.Width(100));
                    GUILayout.Label("isLoop: ", GUILayout.ExpandWidth(false));
                    isLoop = EditorGUILayout.Toggle(isLoop, GUILayout.Width(30));
                    GUILayout.Label("volume: ", GUILayout.ExpandWidth(false));
                    volume = EditorGUILayout.FloatField(volume, GUILayout.Width(40));
                    GUILayout.Label("duration: ", GUILayout.ExpandWidth(false));
                    duration = EditorGUILayout.FloatField(duration, GUILayout.Width(40));
                    GUILayout.Label("fadeIn: ", GUILayout.ExpandWidth(false));
                    fadeIn = EditorGUILayout.FloatField(fadeIn, GUILayout.Width(40));
                    GUILayout.Label("fadeOut: ", GUILayout.ExpandWidth(false));
                    fadeOut = EditorGUILayout.FloatField(fadeOut, GUILayout.Width(40));
                }
                using(new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("SoundName: ", GUILayout.ExpandWidth(false));
                    soundName = EditorGUILayout.TextField(soundName);
                    if(GUILayout.Button("Play", GUILayout.ExpandWidth(false)))
                    {
                        SoundManager.Instance.PlaySound(layer, soundName, isLoop, volume, duration, fadeIn, fadeOut);
                    }
                }
            }

            if(SoundManager.Instance == null)
            {
                return;
            }

            using(new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("test global", GUILayout.ExpandWidth(false));
                using(new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("EnableBg: ", GUILayout.ExpandWidth(false));
                    SoundManager.Instance.BgmEnable = EditorGUILayout.Toggle(SoundManager.Instance.BgmEnable, GUILayout.ExpandWidth(false));
                    GUILayout.Label("EnableSound: ", GUILayout.ExpandWidth(false));
                    SoundManager.Instance.SoundEnable = EditorGUILayout.Toggle(SoundManager.Instance.SoundEnable, GUILayout.ExpandWidth(false));
                    GUILayout.Label("PauseSound: ", GUILayout.ExpandWidth(false));
                    if(GUILayout.Button(isPaused ? "继续" : "暂停"))
                    {
                        if(isPaused)
                        {
                            SoundManager.Instance.Resume();
                        }
                        else
                        {
                            SoundManager.Instance.Pause();
                        }

                        isPaused = !isPaused;
                    }
                }
            }

            var players = SoundManager.Instance.Players;
            for (var i = 0; i < players.Length; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                var player = players[i];
                bool foldout = EditorPrefs.GetBool(foldoutKeys[i], true);
                var t = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, ((SoundLayer)i).ToString());
                if (foldout != t)
                {
                    EditorPrefs.SetBool(foldoutKeys[i], t);
                }
                if (t)
                {
                    GUILayout.Label($"AliveCount:{player.AliveCount}, Volume:{player.Volume}, VolumeScale:{player.VolumeScale}");
                    GUILayout.Label("【LoadingSounds】");
                    GUISoundList(player.LoadingSounds);
                    GUILayout.Label("【FadeInSounds】");
                    GUISoundList(player.FadeInSounds);
                    GUILayout.Label("【PlayingSounds】");
                    GUISoundList(player.PlayingSounds);
                    GUILayout.Label("【FadeOutSounds】");
                    GUISoundList(player.FadeOutSounds);
                    GUILayout.Label($"【DeadSoundDatas {player.DeadSoundDatas.Count}】");
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.EndVertical();;
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);
            bool fd = EditorPrefs.GetBool("SoundManagerDebugger_AliveCounter", true);
            var tb = EditorGUILayout.BeginFoldoutHeaderGroup(fd, "AliveCounter");
            if (fd != tb)
            {
                EditorPrefs.SetBool("SoundManagerDebugger_AliveCounter", tb);
            }
            if (tb)
            {
                foreach (SoundPlayer player in SoundManager.Instance.Players)
                {
                    GUILayout.Label($"【{player.Layer.ToString()}】");
                    foreach (var kv in player.AliveCounter)
                    {
                        GUILayout.Label($"    {kv.Key}: {kv.Value}");
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.EndVertical();;
        }

        private void GUISoundList(List<SoundItem> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                using (new GUILayout.HorizontalScope())
                {
                    var curTime = item.CurTime;
                    GUILayout.Space(30);
                    GUILayout.Label($"[{i}]  0 -- {item.FadeinEndTime:f3} -- {item.FadeoutStartTime:f3} -- {item.Duration:f3} | {curTime:f2} ({100*curTime/item.Duration:f1}%)");
                    EditorGUILayout.ObjectField(item.AudioSource.clip, typeof(AudioClip), false, GUILayout.ExpandWidth(false));
                    if (GUILayout.Button("stop", GUILayout.ExpandWidth(false)))
                    {
                        SoundManager.Instance.StopSound(item.GetInfo());
                    }
                }
            }
        }
    }
}
