using Streaming.Crystal.Connection.Connection.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Connection
{

    public class CrystalConnectionStatistics : ICrystalConnectionStatistics
    {
        private long countCurrent;
        private long periodClosed;
        private double secounds;
        private DateTime datePeriodRef;

        private int PERIOD_IN_SECOUNDS = 120;

        public CrystalConnectionStatistics()
        {
            datePeriodRef = DateTime.Now;
        }

        public void ChangeMessage()
        {
            countCurrent ++;

            VerifyPeriod();

            if (IsClosePeriod())
            {
                Interlocked.Exchange(ref periodClosed, countCurrent);
                datePeriodRef = DateTime.Now;
            }
        }

        private bool IsClosePeriod()
        {
            return secounds > PERIOD_IN_SECOUNDS;
        }

        private void VerifyPeriod()
        {
            secounds = (DateTime.Now - datePeriodRef).TotalSeconds;
        }

        public long StatisticsLevel()
        {
            return periodClosed;
        }
    }
}
