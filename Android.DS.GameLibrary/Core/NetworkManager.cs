using System;
using System.Threading;

#if IPHONE
using MonoTouch.SystemConfiguration;
#elif WINDOWS_PHONE
using System.Net.NetworkInformation;
using Microsoft.Phone.Net.NetworkInformation;
using System.Net;
using GameLibrary.Utils;
using System.Windows;
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
#elif WINDOWS_PHONE
            DeviceNetworkInformation.NetworkAvailabilityChanged += (sender, e) =>
            {
                CheckInternetConnection(false);
            };
#elif ANDROID
            // TODO
#endif
		}

        public void CheckInternetConnection(bool asynch)
        {
#if WINDOWS_PHONE
            if (!asynch)
            {
                if (DeviceNetworkInformation.IsNetworkAvailable)
                {
                    TestInternetConnectivity();
                }
                else
                {
                    UpdateNetworkAvailability(false);
                }
            }
            else
            {
                ThreadPool.QueueUserWorkItem((o) =>
                        {
                            CheckInternetConnection(false);
                        });
            }
#endif
        }

        private void TestInternetConnectivity()
        {
            // TODO: Add custom timeout!!
#if WINDOWS_PHONE
            HttpWebRequest request = HttpWebRequest.CreateHttp("http://www.google.com");
            request.Method = "HEAD";

            request.BeginGetResponse(
                (result) =>
                {
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            UpdateNetworkAvailability(response.StatusCode == HttpStatusCode.OK);
                        });
                    }
                    catch
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            UpdateNetworkAvailability(false);
                        });
                    }
                },
                null);
#endif
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
