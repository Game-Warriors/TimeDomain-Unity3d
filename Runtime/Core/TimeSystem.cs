using System;
using System.Collections.Generic;
using GameWarriors.TimeDomain.Abstraction;
using UnityEngine;

namespace GameWarriors.TimeDomain.Core
{
    public class TimeSystem : ITime
    {
        private readonly Dictionary<string, ITimeData> _startTimeTable;
        private List<IDateTimeApi> _apiList;

        private DateTime? _minDataTime;
        private bool _isCheckingTime;
        private int _retryCount;
        private int _apiIndex;

        public event Action<ETimeUpdateState> OnTimeUpdate;
        public bool IsUpdateingTime => _isCheckingTime;
        public int RemoteAndStaticThreshold { get; }
        public bool IsBaseTimeValid
        {
            get
            {
                if (_minDataTime.HasValue)
                {
                    TimeSpan timeSpan = _minDataTime.Value - DateTime.UtcNow;
                    return timeSpan.TotalSeconds > RemoteAndStaticThreshold;
                }
                return false;
            }
        }

        public DateTime BaseUtcNow
        {
            get
            {
                if (_minDataTime.HasValue)
                {
                    TimeSpan timeSpan = _minDataTime.Value - DateTime.UtcNow;
                    return timeSpan.TotalSeconds > RemoteAndStaticThreshold ? DateTime.UtcNow : _minDataTime.Value;
                }
                return DateTime.UtcNow;
            }
        }

        public DateTime? RemoteDateTime => _minDataTime;

        [UnityEngine.Scripting.Preserve]
        public TimeSystem(ITimeSystemConfig timeSystemConfig)
        {
            RemoteAndStaticThreshold = timeSystemConfig?.RemoteAndStaticTimeThreshold ?? -30;
            _startTimeTable = new Dictionary<string, ITimeData>();
        }

        public void RegisterTimeUpdateApi(IDateTimeApi item)
        {
            _apiList ??= new List<IDateTimeApi>();
            _apiList.Add(item);
        }

        public void OverrideMinDate(DateTime minDate)
        {
            if (minDate > _minDataTime)
                _minDataTime = minDate;
        }

        public void MentionTime(string id, ITimeData data)
        {
            if (string.IsNullOrEmpty(id))
                return;

            _startTimeTable.Add(id, data);
        }

        public TimeSpan TimeOffset(string key)
        {
            if (_startTimeTable.TryGetValue(key, out var data))
            {
                TimeSpan timeSpan = data.EndDate - BaseUtcNow;
                return timeSpan;
            }

            return TimeSpan.FromSeconds(-1);
        }

        public bool AddNewTime(string key, ITimeData timeData)
        {
            if (!_startTimeTable.ContainsKey(key))
            {
                _startTimeTable.Add(key, timeData);
                return true;
            }
            return false;
        }

        public bool HasKey(string key)
        {
            return _startTimeTable.ContainsKey(key);
        }

        public bool UpdateTime(string key, TimeSpan offset)
        {
            if (_startTimeTable.TryGetValue(key, out var data))
            {
                data.SetEndDate(BaseUtcNow + offset);
                return true;
            }

            return false;
        }

        public bool UpdateTime(string key, DateTime date)
        {
            if (_startTimeTable.TryGetValue(key, out var data))
            {
                data.SetEndDate(date);
                return true;
            }

            return false;
        }

        public DateTime? FindDate(string key)
        {
            if (_startTimeTable.TryGetValue(key, out var data))
            {
                return data.EndDate;
            }
            return null;
        }


        public void UpdateBaseTime(bool isForce, int retryCount)
        {
            if (_isCheckingTime)
            {
                if (isForce)
                {
                    _retryCount = retryCount;
                }
                return;
            }
            else
                _retryCount = retryCount;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                OnTimeUpdate?.Invoke(ETimeUpdateState.NoInternet);
                return;
            }

            _isCheckingTime = true;
            _apiList[_apiIndex].GetDateTime(UpdateBaseTimeDone);
        }

        private void UpdateBaseTimeDone(bool isSuccess, DateTime utc)
        {
            _apiIndex++;
            _apiIndex %= _apiList.Count;
            _isCheckingTime = false;
            if (isSuccess)
            {
                _minDataTime = utc;
                _retryCount = 0;
                OnTimeUpdate?.Invoke(ETimeUpdateState.Success);
            }
            else
            {
                _retryCount--;
                if (_retryCount > 0)
                {
                    UpdateBaseTime(false, _retryCount);
                }
                else
                {
                    OnTimeUpdate?.Invoke(ETimeUpdateState.Failed);
                }
            }
        }

        public void ClearRemoteDate()
        {
            _minDataTime = null;
        }
    }
}