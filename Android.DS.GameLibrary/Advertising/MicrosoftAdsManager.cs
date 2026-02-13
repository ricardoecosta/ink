#if ADS_ENABLED
using System;
using Microsoft.Advertising.Mobile.Xna;
using Microsoft.Xna.Framework;
using GameLibrary.Core;

namespace GameLibrary.Advertising
{
    public class MicrosoftAdsManager
    {
        private MicrosoftAdsManager() { }

        public void Initialize(Director director, Point adPosition, int adWidth, int adHeight, string applicationId, string adUnitId, bool adsEnabled, bool testModeEnabled)
        {
            this.adPosition = adPosition;
            this.adWidth = adWidth;
            this.adHeight = adHeight;
            this.applicationId = applicationId;
            this.adUnitId = adUnitId;
            this.adsEnabled = adsEnabled;
            this.testModeEnabled = testModeEnabled;

            /*if (this.adsEnabled)
            {
                this.msAdManager = new AdManager(director.Game, this.applicationId);
                this.msAdManager.TestMode = this.testModeEnabled;
                this.msAd = this.msAdManager.CreateAd(this.adUnitId, new Rectangle(this.adPosition.X, this.adPosition.Y, this.adWidth, this.adHeight), RotationMode.Automatic);
                director.Game.Components.Add(msAdManager);
                this.msAd.BorderColor = Color.Black;

                this.msAdManager.Visible = this.msAdManager.Enabled = false;
            }*/
        }

        public void ShowAds()
        {
            /*if (this.adsEnabled)
            {
                this.msAdManager.Visible = this.msAdManager.Enabled = true;
            }*/
        }

        public void HideAds()
        {
            /*if (this.adsEnabled)
            {
                this.msAdManager.Visible = this.msAdManager.Enabled = false;
            }*/
        }

        public static MicrosoftAdsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MicrosoftAdsManager();
                }
                return instance;
            }
        }

        /*private AdManager msAdManager;
        private Ad msAd;*/
        private Point adPosition;
        private int adWidth, adHeight;
        private string applicationId;
        private string adUnitId;
        private bool adsEnabled, testModeEnabled;

        private static MicrosoftAdsManager instance;
    }
}
#endif