using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    [ExecuteInEditMode]
    public class RaycastCarSpeedProvider : CarSpeedProvider
    {
        public override bool changeLaneLeftQueued { get; set; }
        public override bool changeLaneRightQueued { get; set; }

        public AnimationCurve BrakeCurve;

        public float ScanDistanceFront;
        public float OptimalDistanceToOtherCars;

        private CarCollider _carCollider;
        public CarCollider carCollider => _carCollider = _carCollider ?? GetComponent<CarCollider>();

        public List<CarMovementInfo> NearbyCars = new List<CarMovementInfo>();

        float NewSpeedDistance => CarMove.Travel.Length;
        float NewTravelSpeed => CarMove.TravelPlan.FirstOrDefault()?.Road?.MaxSpeed ?? 0;
        float DeltaSpeed => CarMove.Travel.Road.MaxSpeed - NewTravelSpeed;

        float? DeccelerateDistance;
        bool _firstFrame = true;

        protected override float TargetSpeed
        {
            get
            {
                var carInfo = GetClosestCar();
                if (carInfo != null)
                {
                    //return 0;
                    carInfo.Value.RecalculateDistance();
                    return CurrentMaxSpeed * (1 - BrakeCurve.Evaluate(carInfo.Value.LastActualDistance / ScanDistanceFront));
                }
                else
                {
                    if (DeccelerateDistance != null) // Has a deccelerarion been planned
                    {
                        if (DeccelerateDistance < CarMove.Travel.Distance) // Is it time to deccelerate
                        {
                            return NewTravelSpeed; // Deccelerate
                        }
                        else
                        {
                            return CurrentMaxSpeed; // Accellerate / go at speed limit
                        }
                    }
                    else
                    {
                        return CurrentMaxSpeed; // Accellerate / go at speed limit
                    }
                }
            }
        }

        void Start()
        {
            carCollider.TriggerEntered += OnTriggerEntered;
            carCollider.TriggerExitted += OnTriggerExitted;
            CarMove.TravelChanged += OnTravelChanged;
        }

        private void OnTravelChanged(object sender, RoadTravelChangeEventArgs args)
        {
            ComputeDecceleration();
        }

        void ComputeDecceleration()
        {
            if (DeltaSpeed <= 0)
            {
                DeccelerateDistance = null;
                return;
            }
            var acc = new LinearEquation(Acceleration, speed);
            var decc = LinearEquation.FromAAndPoint(-Decceleration, new Vector2(NewSpeedDistance, NewTravelSpeed));
            DeccelerateDistance = LinearEquation.Resolve(acc, decc, CurrentMaxSpeed)?.x;
            if (DeccelerateDistance != null) DeccelerateDistance -= DeccelerateDistance / 2 * Anticipation;
        }

        private void OnTriggerEntered(Collider other)
        {

            if (_firstFrame) return;
            if (other.gameObject.TryGetComponent(out CarMovement car))
            {
                var carInfo = new CarMovementInfo(CarMove, car);
                if (!NearbyCars.Contains(carInfo)) NearbyCars.Add(carInfo);
            }
        }

        private void OnTriggerExitted(Collider other)
        {
            if (_firstFrame) return;
            if (other.gameObject.TryGetComponent(out CarMovement car))
            {
                var carInfo = new CarMovementInfo(CarMove, car);
                if (!NearbyCars.Contains(carInfo)) NearbyCars.Remove(carInfo);
            }
        }

        public override void LaneChange(bool left)
        {
        }

        public override void QueueChangeLaneLeft()
        {
        }

        public override void QueueChangeLaneRight()
        {
        }

        CarMovementInfo? GetClosestCar()
        {
            if (_firstFrame)
            {
                _firstFrame = false;
                return null;
            }
            if (NearbyCars.Count == 0)
            {
                return null;
            }
            CarMovementInfo closest = new CarMovementInfo();
            bool first = true;
            foreach (var carInfo in NearbyCars)
            {
                if (true) //carInfo.IsOnPath)
                {
                    if (first)
                    {
                        closest = carInfo;
                        first = false;
                    }
                    else
                    {
                        if (carInfo.LastActualDistance < closest.LastActualDistance) closest = carInfo;
                    }
                }
            }
            if (closest.Equals(new CarMovementInfo())) return null;
            return closest;
        }
    }
}
