using System;

namespace GameWarriors.TimeDomain.Abstraction
{
    public interface ITimeData
    {
        DateTime EndDate { get; }

        public void SetEndDate(DateTime endDate);
    }
}