using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Roads
{
    [ExecuteInEditMode]
    public class AnticipatingLaneCarSpeedProvider : CarSpeedProvider
    {
        public override bool changeLaneLeftQueued { get; set; }
        public override bool changeLaneRightQueued { get; set; }

        [Tooltip("Relative speed depending on the distance (if point at (0.5;0.5) the speed will be the same when the distance is half the scanning distance)")]
        public AnimationCurve BrakeCurve;

        public float ScanDistanceFront;
        public float OptimalDistanceToOtherCars;

        private CarCollider _carCollider;
        public CarCollider carCollider => _carCollider = _carCollider ?? GetComponent<CarCollider>();

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
                    //carInfo.Value.RecalculateDistance();
                    if (carInfo.Value.IsOnPath && carInfo.Value.LastActualDistance <= ScanDistanceFront)
                    {
                        
                        //return 0;
                        //carInfo.Value.RecalculateDistance();

                        float s = CalculateTargetSpeed(carInfo.Value);
                        if (carInfo.Value.CarMove.SpeedProvider.MaxSpeed < MaxSpeed) SlowDownCause = CauseOfSlowDown.FrontCarSlower;
                        return s;
                    }
                }
                if (DeccelerateDistance != null) // Has a deccelerarion been planned
                {
                    if (DeccelerateDistance < CarMove.Travel.Distance) // Is it time to deccelerate
                    {
                        if (changeLaneLeftQueued || changeLaneRightQueued) SlowDownCause = CauseOfSlowDown.ChangingLanes;
                        else SlowDownCause = CauseOfSlowDown.AnticipationForNextRoad;
                        return NewTravelSpeed; // Deccelerate
                    }
                    else
                    {
                        SlowDownCause = CauseOfSlowDown.None;
                        return CurrentMaxSpeed; // Accellerate / go at speed limit
                    }
                }
                else
                {
                    SlowDownCause = CauseOfSlowDown.None;
                    return CurrentMaxSpeed; // Accellerate / go at speed limit
                }
            }
        }

        void Start()
        {
            carCollider.Active = false;
            carCollider.TriggerEntered += OnTriggerEntered;
            carCollider.TriggerExitted += OnTriggerExitted;
            carCollider.TriggerStayed += OnTriggerStayed;
            CarMove.TravelChanged += OnTravelChanged;
        }

        float CalculateTargetSpeed(CarMovementInfo carInfo)
        {
            var v = carInfo.LastActualDistance;
            var relativeDistance = v / ScanDistanceFront;
            var relativeSpeed = BrakeCurve.Evaluate(relativeDistance);
            var currentMaxSpeed = DeccelerateDistance < CarMove.Travel.Distance ? NewTravelSpeed : CurrentMaxSpeed;
            if (relativeSpeed < 0.5f) 
                brake = true;
            return relativeSpeed > 0.5f ? Mathf.Lerp(carInfo.CarMove.speed, currentMaxSpeed, relativeSpeed) :
                Mathf.Lerp(0, currentMaxSpeed, relativeSpeed);
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
                if (!NearbyCars.Contains(carInfo))
                {
                    NearbyCars.Add(carInfo);
                    Debug.Log($"Added {car} to {CarMove}");
                }
            }
        }

        private void OnTriggerStayed(Collider other)
        {
            if (_firstFrame) return;
            if (other.gameObject.TryGetComponent(out CarMovement car))
            {
                var carInfo = new CarMovementInfo(CarMove, car);
                if (!NearbyCars.Contains(carInfo))
                {
                    NearbyCars.Add(carInfo);
                    Debug.LogWarning($"Added {car} to {CarMove} from OnTriggerStay");
                }
            }
        }

        private void OnTriggerExitted(Collider other)
        {
            if (_firstFrame) return;
            if (other.gameObject.TryGetComponent(out CarMovement car))
            {
                var carInfo = new CarMovementInfo(CarMove, car);
                if (NearbyCars.Contains(carInfo))
                {
                    NearbyCars.Remove(carInfo);
                    Debug.Log($"Removed {car} to {CarMove}");
                }
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

        IEnumerable<CarMovementInfo> GetNearbyCars()
        {
            List<CarMovementInfo> carInfos = new List<CarMovementInfo>();

            foreach (var user in CarMove.Travel.Road.GetUsers())
            {
                if (user != this) carInfos.Add(new CarMovementInfo(CarMove, user));
            }

            foreach (var travel in CarMove.TravelPlan)
            {
                foreach (var user in travel.Road.GetUsers())
                {
                    carInfos.Add(new CarMovementInfo(CarMove, user));
                }
            }

            foreach (var carInfo in carInfos)
            {
                if (carInfo.IsInFront && carInfo.IsValid) yield return carInfo;
            }
        }

        //CarMovementInfo? GetClosestCar()
        //{
        //    if (_firstFrame)
        //    {
        //        _firstFrame = false;
        //        return null;
        //    }
        //    CarMovementInfo closest = new CarMovementInfo();
        //    bool first = true;
        //    foreach (var carInfo in GetNearbyCars())
        //    {
        //        if (true) //carInfo.IsOnPath)
        //        {
        //            if (first)
        //            {
        //                closest = carInfo;
        //                first = false;
        //            }
        //            else
        //            {
        //                if (carInfo.LastActualDistance < closest.LastActualDistance) closest = carInfo;
        //            }
        //        }
        //    }
        //    if (closest.Equals(new CarMovementInfo())) return null;
        //    return closest;
        //}

        CarMovementInfo? GetClosestCar()
        {
            if (_firstFrame)
            {
                _firstFrame = false;
                carCollider.Active = true;
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
                carInfo.RecalculateDistance();
                if (carInfo.IsOnPath)
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
