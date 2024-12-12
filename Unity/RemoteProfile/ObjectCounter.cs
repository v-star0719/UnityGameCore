using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;


namespace GameCore.Unity.RemoteProfile
{
    public class ObjectCounter : ProfileBase
    {
        private List<Dictionary<System.Type, int>> snapshotList = new List<Dictionary<System.Type, int>>();

        public override void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var snapshot = GenerateSnapshot();
            var json = "{ " + string.Join(",", snapshot.Select(e => "\"" + e.Key.Name + "\":" + e.Value).ToArray()) + " }";
            var bodyBytes = Encoding.UTF8.GetBytes(json);
            response.ContentType = "text/json";
            response.ContentLength64 = bodyBytes.LongLength;
            response.OutputStream.Write(bodyBytes, 0, bodyBytes.Length);
        }

        private Dictionary<System.Type, int> GenerateSnapshot()
        {
            var snapshot = new Dictionary<System.Type, int>();
            foreach (var obj in Resources.FindObjectsOfTypeAll<Object>())
            {
                var type = obj.GetType();
                int count;
                snapshot.TryGetValue(type, out count);
                snapshot[type] = count + 1;
            }

            return snapshot;
        }
    }
}
