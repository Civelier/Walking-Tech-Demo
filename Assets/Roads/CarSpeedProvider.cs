using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public enum CauseOfSlowDown
    {
        None,
        FrontCarSlower,
        ChangingLanes,
        AnticipationForNextRoad
    }
    public abstract class CarSpeedProvider : MonoBehaviour
    {
        public float Acceleration = 8;
        public float Decceleration = 15;
        public float Brake = 40;

        public abstract bool changeLaneLeftQueued { get; set; }
        public abstract bool changeLaneRightQueued { get; set; }

        public float MaxSpeed;

        private CarMovement _carMove;
        protected CarMovement CarMove => _carMove = _carMove ?? GetComponent<CarMovement>();
        protected RoadTravel Travel => CarMove.Travel;
        protected float RoadMaxSpeed => Travel.Road.MaxSpeed;
        protected float CurrentMaxSpeed => Mathf.Min(MaxSpeed, RoadMaxSpeed);

        public List<CarMovementInfo> NearbyCars = new List<CarMovementInfo>();
        public CauseOfSlowDown SlowDownCause { get; protected set; }

        public float Anticipation = 0.8f;
        protected bool brake;
        protected float speed;
        protected abstract float TargetSpeed { get; }

        public virtual float GetSpeed()
        {
            var targetSpeed = TargetSpeed;
            if (brake)
            {
                brake = false;
                return speed = Mathf.MoveTowards(speed, targetSpeed, Time.deltaTime * Brake);
            }
            else if (speed <= targetSpeed)
                return speed = Mathf.MoveTowards(speed, targetSpeed, Time.deltaTime * Acceleration);
            else return speed = Mathf.MoveTowards(speed, targetSpeed, Time.deltaTime * Decceleration);
        }
        public abstract void QueueChangeLaneLeft();
        public abstract void QueueChangeLaneRight();
        public abstract void LaneChange(bool left);
    }
}
