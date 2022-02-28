using System;
using GameWarriors.TimeDomain.Abstraction;
using UnityEngine;

namespace GameWarriors.TimeDomain.Data
{
    [Serializable]
    public class DefaultTimeData : ITimeData
    {
        [SerializeField]
        private long _end;

        public DateTime EndDate { get => DateTime.FromBinary(_end); set => _end = value.ToBinary(); }

        public DefaultTimeData(DateTime endDate)
        {
            _end = endDate.ToBinary();
        }

        public DefaultTimeData(DateTime startDate, DateTime endDate)
        {
            _end = endDate.ToBinary();
        }

        public void SetEndDate(DateTime endDate)
        {
            EndDate = endDate;
        }
    }
}