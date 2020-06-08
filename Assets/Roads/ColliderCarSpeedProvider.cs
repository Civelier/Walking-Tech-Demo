using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public class ColliderCarSpeedProvider : CarSpeedProvider
    {
        //public float MaxSpeed = 40;
        public CarCollider FrontTrigger;
        public CarCollider FrontTriggerTooClose;
        public CarCollider ChangeLaneLeft;
        public CarCollider ChangeLaneRight;
        public CarCollider Car;
        public Rigidbody Body;

        private void OnTriggerStayed(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (car.NeedToSlowDown && !OtherCars.Contains(car))
                    OtherCars.Add(car);
            }
        }

        private void OnTriggerRightExitted(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (car.Situation != CarSituation.Trigger) RightCars.Remove(car);
            }
        }

        private void OnTriggerRightEntered(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (car.Situation != CarSituation.Trigger) RightCars.Add(car);
            }
        }

        private void OnTriggerLeftExitted(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (car.Situation != CarSituation.Trigger) LeftCars.Remove(car);
            }
        }

        private void OnTriggerLeftEntered(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (car.Situation != CarSituation.Trigger) LeftCars.Add(car);
            }
        }

        
        float NewSpeedDistance => _laneChangeDistance ?? CarMove.Travel.Length;
        float NewTravelSpeed => _laneChangeDistance == null ? CarMove.Travel.Road.EndTravels.FirstOrDefault()?.Road.MaxSpeed ?? 0 : CurrentMaxSpeed * CarMove.ChangeBehaviour.MaxSpeedPercent;
        float DeltaSpeed => CarMove.Travel.Road.MaxSpeed - NewTravelSpeed;

        float? DeccelerateDistance;

        public List<CarCollider> OtherCars = new List<CarCollider>();
        public List<CarCollider> LeftCars = new List<CarCollider>();
        public List<CarCollider> RightCars = new List<CarCollider>();
        bool TooClose = false;

        protected override float TargetSpeed
        {
            get
            {
                var otherCar = GetClosestCarInFront();
                if (otherCar != null && NeedToSlowDown()) // Other car detected (collision trigger)
                {
                    var d = FrontTrigger.GetBackToBackPercent(otherCar);
                    if (TooClose) // Other car is too close
                    {
                        if (d < 0) // If the distance is negative
                        {
                            return MaxSpeed; // Speed up to get away from the other car
                        }
                        else
                        {
                            brake = true;
                            return 0; // Slow down to avoid a collision
                        }
                    }
                    else
                    {
                        float s = ExpEasing(1 - GetPercentSpeedToAvoidCollision());
                        brake = true;
                        //return 0;
                        return CurrentMaxSpeed * s;
                    }
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
            if (Body == null) Body = GetComponent<Rigidbody>();
            if (Car == null) Car = GetComponent<CarCollider>();
            CarMove.TravelChanged += OnTravelChanged;
            FrontTrigger.TriggerEntered += OnTriggerEntered;
            FrontTrigger.TriggerExitted += OnTriggerExitted;
            FrontTrigger.TriggerStayed += OnTriggerStayed;
            FrontTriggerTooClose.TriggerEntered += OnTriggerTooCloseEntered;
            FrontTriggerTooClose.TriggerExitted += OnTriggerTooCloseExitted;
            ChangeLaneLeft.TriggerEntered += OnTriggerLeftEntered;
            ChangeLaneLeft.TriggerExitted += OnTriggerLeftExitted;
            ChangeLaneRight.TriggerEntered += OnTriggerRightEntered;
            ChangeLaneRight.TriggerExitted += OnTriggerRightExitted;
            changeLaneLeftQueued = false;
            changeLaneRightQueued = false;
        }

        bool _changingLanes = false;
        protected float? _laneChangeDistance = null;
        private bool _changeLaneLeftQueued = false;
        public override bool changeLaneLeftQueued
        {
            get => _changeLaneLeftQueued;
            set
            {
                if (value)
                {
                    _changeLaneRightQueued = false;
                }
                _changeLaneLeftQueued = value;
                ChangeLaneLeft.Active = value;
                LeftCars.Clear();
            }
        }
        private bool _changeLaneRightQueued = false;
        public override bool changeLaneRightQueued
        {
            get => _changeLaneRightQueued;
            set
            {
                if (value)
                {
                    _changeLaneLeftQueued = false;
                }
                _changeLaneRightQueued = value;
                ChangeLaneRight.Active = value;
                RightCars.Clear();
            }
        }

        float ExpEasing(float value)
        {
            return Mathf.Pow(2, 10 * (value - 1)) - 0.001f;
        }

        CarCollider GetClosest(IEnumerable<CarCollider> colliders)
        {
            float distance = float.MaxValue;
            CarCollider closest = null;
            foreach (var car in colliders)
            {
                if (car != null)
                {
                    var d = FrontTrigger.GetBackToBackDistance(car);
                    if (d < distance)
                    {
                        distance = d;
                        closest = car;
                    }
                }
            }
            return closest;
        }

        CarCollider GetClosestCarInFront()
        {
            var colliders = new List<CarCollider>();
            foreach (var car in OtherCars)
            {
                if (car.Situation == CarSituation.CarWillChangeLaneLeft && CarMove.Travel.Road.RightRoad != null && CarMove.Travel.Road.RightRoad.ContainsUser(car.Car.gameObject))
                    colliders.Add(car);
                if (car.Situation == CarSituation.CarWillChangeLaneRight && CarMove.Travel.Road.LeftRoad != null && CarMove.Travel.Road.LeftRoad.ContainsUser(car.Car.gameObject))
                    colliders.Add(car);
                if (car.NeedToSlowDown && FrontTriggerTooClose.GetBackToBackDistance(car) > 0 && RoadContainsUser(car.Car.gameObject))
                    colliders.Add(car);
            }
            return GetClosest(colliders);
        }

        bool RoadContainsUser(GameObject user, int level = 2)
        {
            return CarMove.Travel.Road.FindUser(user, level);
        }

        bool NeedToSlowDown()
        {
            foreach (var car in OtherCars)
            {
                if (car.NeedToSlowDown)
                {
                    if (FrontTrigger.GetBackToBackDistance(car) > 0) return true;
                }
            }
            return false;
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

        void OnTriggerEntered(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (car.NeedToSlowDown) OtherCars.Add(car);
            }
        }

        float GetPercentSpeedToAvoidCollision()
        {
            float boxLength = FrontTrigger.Trigger.bounds.size.x;
            var otherCar = GetClosestCarInFront();
            var posOther = otherCar.Trigger.center;
            var posBackOther = new Vector3(posOther.x - otherCar.Trigger.bounds.extents.x, posOther.y, posOther.z);

            var posBackTrigger = new Vector3(FrontTrigger.Trigger.center.x - FrontTrigger.Trigger.bounds.extents.x, FrontTrigger.Trigger.center.y, FrontTrigger.Trigger.center.z);
            var d = Vector3.Distance(posBackTrigger, posBackOther);
            return d / boxLength;
        }

        void OnTriggerTooCloseEntered(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (car == GetClosestCarInFront() && car.NeedToSlowDown)
                    TooClose = true;
            }
        }

        void OnTriggerExitted(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (OtherCars.Contains(car)) OtherCars.Remove(car);
            }
        }
        void OnTriggerTooCloseExitted(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if (car == GetClosestCarInFront() && car.NeedToSlowDown)
                    TooClose = false;
            }
        }

        protected void OnTravelChanged(object sender, RoadTravelChangeEventArgs args)
        {
            ComputeDecceleration();
        }

        public override float GetSpeed()
        {
            if (brake)
            {
                brake = false;
                return speed = Mathf.MoveTowards(speed, TargetSpeed, Time.deltaTime * Brake);
            }
            else if (speed <= TargetSpeed)
                return speed = Mathf.MoveTowards(speed, TargetSpeed, Time.deltaTime * Acceleration);
            else return speed = Mathf.MoveTowards(speed, TargetSpeed, Time.deltaTime * Decceleration);
        }

        public override void QueueChangeLaneLeft()
        {
            changeLaneLeftQueued = true;
        }

        public override void QueueChangeLaneRight()
        {
            changeLaneRightQueued = true;
        }

        public override void LaneChange(bool left)
        {
            if (_laneChangeDistance.HasValue)
            {
                if (left) ChangeLaneLeft.Active = true;
                else ChangeLaneRight.Active = true;
            }
        }
    }
}
