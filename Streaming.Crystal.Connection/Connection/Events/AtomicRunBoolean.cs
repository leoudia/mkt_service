using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Connection.Events
{
    public class AtomicRunBoolean
    {
        private bool _run;
        private object semaphore = new object();

        public AtomicRunBoolean(bool isRun = true)
        {
            _run = isRun;
        }

        public bool Run
        {
            get
            {
                return _run;
            }
        }

        public bool Value
        {
            get
            {
                lock (semaphore)
                {
                    return _run;
                }
            }

            set
            {
                lock (semaphore)
                {
                    _run = value;
                }
            }

        }
    }
}
