using System;
using System.Threading;

#if IPHONE
using MonoTouch.SystemConfiguration;
#endif

namespace GameLibrary.Core
{
    public class NetworkManager
    {
        private NetworkManager() { }

        public void Initialize()
        {
#if IPHONE
			NetworkReachability = new NetworkReachability("0.0.0.0");
			NetworkReachability.SetCallback((flags) =>
				{
					UpdateNetworkAvailability((flags & NetworkReachabilityFlags.Reachable) == NetworkReachabilityFlags.Reachable);
				});
#elif ANDROID
            // TODO
#endif
		}

        public void CheckInternetConnection(bool asynch)
        {
            // TODO: Implement for non-WINDOWS_PHONE platforms
        }

        private void TestInternetConnectivity()
        {
            // TODO: Implement for non-WINDOWS_PHONE platforms
        }

        private void UpdateNetworkAvailability(bool isNetworkAvailable)
        {
            if ((!WasOnNetworkOnlineEventEverFired && isNetworkAvailable) || !IsNetworkAvailable && isNetworkAvailable)
            {
                WasOnNetworkOnlineEventEverFired = true;

                IsNetworkAvailable = true;
                if (OnNetworkOnline != null)
                {
                    OnNetworkOnline(this, EventArgs.Empty);
                }
            }
            else if ((!WasOnNetworkOfflineEventEverFired && !isNetworkAvailable) || (IsNetworkAvailable && !isNetworkAvailable))
            {
                WasOnNetworkOfflineEventEverFired = true;

                IsNetworkAvailable = false;
                if (OnNetworkOffline != null)
                {
                    OnNetworkOffline(this, EventArgs.Empty);
                }
            }
        }

        public static NetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkManager();
                }
                return instance;
            }
        }

        private static NetworkManager instance;

#if IPHONE
		private NetworkReachability NetworkReachability { get; set; }
#endif

        private volatile bool isNetworkAvailable;
        public bool IsNetworkAvailable
        {
            get
            {
                return this.isNetworkAvailable;
            }

            private set
            {
                this.isNetworkAvailable = value;
            }
        }

        public bool WasOnNetworkOnlineEventEverFired { get; private set; }
        public bool WasOnNetworkOfflineEventEverFired { get; private set; }

        public event EventHandler OnNetworkOnline;
        public event EventHandler OnNetworkOffline;
    }
}
