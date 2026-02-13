using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace GameLibrary.Analytics
{
    public interface IAnalyticsService
    {
        void LogError(Exception exception);
        void LogEvent(Enum eventToLog, params AnalyticsParameter[] parameters);
    }

    public enum Events
    {
        Error,
        AppStarted,
        AppTerminated,
        Purchase
    }

    public enum EventParameters
    {
        UniqueDeviceID,
        PurchaseStatus,
        ScoreloopUsername,
        ScoreloopEmail,
        DeviceName,
        OSVersion,
        ErrorType,
        ErrorReason,
    }

    public struct AnalyticsParameter
    {
        public string Name;
        public string Value;

        public AnalyticsParameter(Enum name, string value)
        {
            Name = name.ToString();
            Value = value;
        }

        public AnalyticsParameter(string eventParameter, string value)
        {
            Name = eventParameter;
            Value = value;
        }
    }
}