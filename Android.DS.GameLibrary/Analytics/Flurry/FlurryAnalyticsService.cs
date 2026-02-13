#if WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlurryWP7SDK.Models;
using GameLibrary.Social.Gaming;
using GameLibrary.Core;
using Microsoft.Phone.Net.NetworkInformation;
using System.Threading;
using Microsoft.Phone.Info;
using Microsoft.Xna.Framework.GamerServices;

namespace GameLibrary.Analytics.Flurry
{
    public sealed class FlurryAnalyticsService : IAnalyticsService
    {
        private string ApiKey { get; set; }
        private string AppVersion { get; set; }
        private bool IsTrialMode { get; set; }

        private FlurryAnalyticsService() { }

        public void Initialize(string apiKey, string appVersion)
        {
            IsTrialMode = Guide.IsTrialMode;
            ApiKey = apiKey;

#if DEBUG
            AppVersion = appVersion + " [D]";
#elif BETA
            AppVersion = appVersion + " [B]";
#else
            AppVersion = appVersion;
#endif

            FlurryWP7SDK.Api.SetSecureTransportEnabled();
            FlurryWP7SDK.Api.SetVersion(AppVersion);

            FlurryWP7SDK.Api.StartSession(ApiKey);
        }

        public void Uninitialize()
        {
            FlurryWP7SDK.Api.EndSession();
        }

        public void LogError(Exception exception)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                FlurryWP7SDK.Api.LogError(exception.Message, exception);

                // Also log as an event to keep track of more unhandled exception details
                LogEvent(Events.Error, CreateExceptionParams(exception));
            });
        }

        private AnalyticsParameter[] CreateExceptionParams(Exception exception)
        {
            List<AnalyticsParameter> parameters = new List<AnalyticsParameter>();
            
            string uniqueDeviceID = GetUniqueDeviceID();
            if (uniqueDeviceID != null)
            {
                parameters.Add(new AnalyticsParameter(EventParameters.UniqueDeviceID.ToString(), uniqueDeviceID));
            }

            parameters.Add(new AnalyticsParameter(EventParameters.DeviceName, DeviceStatus.DeviceName));
            parameters.Add(new AnalyticsParameter(EventParameters.OSVersion, Environment.OSVersion.Version.ToString()));
            parameters.Add(new AnalyticsParameter(EventParameters.PurchaseStatus, IsTrialMode ? "Trial" : "Paid"));
            parameters.Add(new AnalyticsParameter(EventParameters.ErrorType, exception.GetType().FullName));
            parameters.Add(new AnalyticsParameter(EventParameters.ErrorReason, exception.Message));

            if (exception.StackTrace != null)
            {
                string[] stackTraceLines = exception.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                parameters.Add(new AnalyticsParameter("StackTrace Line 1", stackTraceLines[0]));
            }

            return parameters.ToArray();
        }

        public void LogEvent(Enum eventToLog, params AnalyticsParameter[] parameters)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                List<Parameter> parametersList = new List<Parameter>();

                // The following parameters are always logged with every single event log
                string uniqueDeviceID = GetUniqueDeviceID();
                if (uniqueDeviceID != null)
                {
                    parametersList.Add(new Parameter(EventParameters.UniqueDeviceID.ToString(), uniqueDeviceID));
                }

                parametersList.Add(new Parameter(EventParameters.OSVersion.ToString(), Environment.OSVersion.Version.ToString()));
                parametersList.Add(new Parameter(EventParameters.PurchaseStatus.ToString(), IsTrialMode ? "Trial" : "Paid"));

                if (ScoreloopService.Instance.ScoreloopClient != null && ScoreloopService.Instance.ScoreloopClient.Session != null)
                {
                    if (ScoreloopService.Instance.ScoreloopClient.Session.User.Login != null)
                    {
                        parametersList.Add(new Parameter(EventParameters.ScoreloopUsername.ToString(), ScoreloopService.Instance.ScoreloopClient.Session.User.Login));
                    }
                }

                for (int i = 0; i < parameters.Length; i++)
                {
                    parametersList.Add(new Parameter(parameters[i].Name.ToString(), parameters[i].Value != null ? parameters[i].Value.Substring(0, Math.Min(MaxEventParameterLength, parameters[i].Value.Length)) : ""));
                }

                FlurryWP7SDK.Api.LogEvent(eventToLog.ToString(), parametersList);
            });
        }

        private string GetUniqueDeviceID()
        {
            object deviceUniqueId;
            if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out deviceUniqueId))
            {
                byte[] deviceIDbyte = (byte[])deviceUniqueId;
                return Convert.ToBase64String(deviceIDbyte);
            }

            return null;
        }

        private static FlurryAnalyticsService instance;
        public static FlurryAnalyticsService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FlurryAnalyticsService();
                }

                return instance;
            }
        }

        private const int MaxEventParameterLength = 255;
        private const int MaxEventNumber = 10;
    }
}
#endif