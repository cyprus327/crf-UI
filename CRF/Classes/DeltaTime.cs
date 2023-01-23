using System;

namespace CRF.Classes {
    public class DeltaTime {
        public DeltaTime() {
            t1 = DateTime.Now;
            t2 = DateTime.Now;
        }

        private DateTime t1;
        private DateTime t2;

        public float Get() {
            t2 = DateTime.Now;
            float deltaTime = (t2.Ticks - t1.Ticks) / 10000000f;
            t1 = t2;
            return deltaTime;
        }
    }
}