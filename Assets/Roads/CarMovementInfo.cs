using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roads
{
    public enum CarSituation
    {
        CarIsThere = 0x00001,
        CarWillChangeLaneLeft = 0x00010,
        CarWillChangeLaneRight = 0x00100,
        Trigger = 0x01000,
        TriggerTooClose = 0x10000,
        SlowDown = 0x00111,
    }
    [Serializable]
    public struct CarMovementInfo
    {
        public CarSituation Situation;
    }
}
