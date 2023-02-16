using System;
using GameWarriors.TimeDomain.Data;

namespace GameWarriors.TimeDomain.Abstraction
{
    public enum ETimeUpdateState { None, Success, Failed, NoInternet }

    public interface ITime
    {
        bool IsBaseTimeValid { get; }
        DateTime? RemoteDateTime { get; }
        DateTime BaseUtcNow { get; }

        void RegisterTimeUpdateApi(IDateTimeApi item);
        void OverrideMinDate(DateTime minDate);
        void MentionTime(string id, ITimeData data);
        TimeSpan TimeOffset(string key);
        bool AddNewTime(string key, ITimeData data);
        bool UpdateTime(string key, TimeSpan offset);
        bool UpdateTime(string key, DateTime date);
        bool HasKey(string key);
        DateTime? FindDate(string key);
        void UpdateBaseTime(bool isForce, int retryCount);
    }
}