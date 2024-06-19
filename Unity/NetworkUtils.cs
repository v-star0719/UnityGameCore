using UnityEngine;

namespace Assets.Scripts.Kernel.Unity
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
