using UnityEngine;

namespace GameCore.Unity.Misc
{
    class NetworkUtils
    {
        public static bool HaveWifi()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                return true;
            else
                return false;
        }
        public static bool HaveNetwork()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                return true;
            else
                return false;
        }
	}
}
