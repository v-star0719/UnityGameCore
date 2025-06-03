using System.Net;

namespace GameCore.Unity.RemoteProfile
{
    public class ProfileBase
    {
        public virtual void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
        }
    }
}
