using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;

namespace MemoryProfilerEx
{
    public class MemoryProfilerExWindow : EditorWindow
    {
        [MenuItem("Tools/MemoryProfilerExWindow", false, 0)]
        public static void Open()
        {
            GetWindow<MemoryProfilerExWindow>(false);
        }


        private void OnGUI()
        {
            if(GUILayout.Button("TakeSample", GUILayout.Width(150)))
            {
                TakeSample();
                MemorySnapshot.OnSnapshotReceived += OnSnapshotReceived;
            }
        }

        private void OnSnapshotReceived(PackedMemorySnapshot obj)
        {
            //var type = Type.GetType("ProfilerWindow");
            //var wind = EditorWindow.GetWindow(type);
            //var field = type.GetField("m_ProfilerModules", BindingFlags.NonPublic);
            //var models = field.GetValue(wind) as object[];
            //var memoryModel = models[3];

            //Debug.Log(obj.ToString());
            //MemorySnapshot.OnSnapshotReceived -= OnSnapshotReceived;
        }

        private void TakeSample()
        {
            //MemoryProfiler.TakeSnapshot("mem.1", (s, b) =>
            //{
            //    Debug.Log(s + " ==> " + b);
            //});
        }

        

    }

}
