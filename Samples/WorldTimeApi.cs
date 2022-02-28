using System;
using GameWarriors.TimeDomain.Abstraction;
using UnityEngine;
using UnityEngine.Networking;

namespace Managements.Handlers.TimeApi
{
    [Serializable]
    public struct WorldTimeApiDto
    {
        [SerializeField]
        private string abbreviation;
        [SerializeField]
        private string client_ip;
        [SerializeField]
        private string datetime;
        [SerializeField]
        private int day_of_week;
        [SerializeField]
        private int day_of_year;
        [SerializeField]
        private bool dst;
        [SerializeField]
        private string dst_from;
        [SerializeField]
        private int dst_offset;
        [SerializeField]
        private string dst_until;
        [SerializeField]
        private int raw_offset;
        [SerializeField]
        private string timezone;
        [SerializeField]
        private long unixtime;
        [SerializeField]
        private string utc_datetime;
        [SerializeField]
        private string utc_offset;
        [SerializeField]
        private int week_number;

        public string Abbreviation => abbreviation;
        public string ClientIp => client_ip;
        public string DateTime => datetime;
        public int DayOfWeek => day_of_week;
        public int DayOfYear => day_of_year;
        public bool Dst => dst;
        public string DstFrom => dst_from;
        public int DstOffset => dst_offset;
        public string DstUntil => dst_until;
        public int RawOffset => raw_offset;
        public string TimeZone => timezone;
        public long Unixtime => unixtime;
        public string UtcDatetime => utc_datetime;
        public string UtcOffset => utc_offset;
        public int WeekNumber => week_number;
    }

    public class WorldTimeApi : IDateTimeApi
    {
        private const string API_URL = "https://worldtimeapi.org/api/timezone/Etc/UTC";

        public void GetDateTime(Action<bool, DateTime> currentDateTime)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(API_URL);
            UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();
            operation.completed += asyncOperation =>
            {
                DateTime currentUtc = DateTime.UtcNow;

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        WorldTimeApiDto dto = JsonUtility.FromJson<WorldTimeApiDto>(webRequest.downloadHandler.text);
                        bool isParsed = DateTime.TryParse(dto.UtcDatetime, out currentUtc);

                        if (isParsed)
                        {
                            currentUtc = currentUtc.ToUniversalTime();
                            currentDateTime(true, currentUtc);
                        }
                        else
                        {
                            currentDateTime(false, currentUtc);
                        }
                    }
                    catch (Exception e)
                    {
                        currentDateTime(false, currentUtc);
                    }
                }
                else
                {
                    currentDateTime(false, currentUtc);
                }
            };
        }
    }
}