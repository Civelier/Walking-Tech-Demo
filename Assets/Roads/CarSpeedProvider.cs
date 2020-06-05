using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public abstract class CarSpeedProvider : MonoBehaviour
    {
        public abstract bool changeLaneLeftQueued { get; set; }
        public abstract bool changeLaneRightQueued { get; set; }
        public abstract float GetSpeed();
        public abstract void QueueChangeLaneLeft();
        public abstract void QueueChangeLaneRight();
        public abstract void LaneChange(bool left);
    }
}
