using System;

namespace GameWarriors.TimeDomain.Abstraction
{
    public interface IDateTimeApi
    {
        void GetDateTime(Action<bool, DateTime> currentDateTime);
    }
}