using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roads
{
    [RequireComponent(typeof(CarCollider))]
    public class CarMovement : AdvancedPathFolower
    {
        public float Acceleration = 8;
        public float Decceleration = 15;
        public float Brake = 40;
        public float MaxSpeed = 40;
        public CarCollider FrontTrigger;
        public CarCollider FrontTriggerTooClose;
        public CarCollider ChangeLaneLeft;
        public CarCollider ChangeLaneRight;
        public CarCollider Car;
        public Rigidbody Body;
        public AnticipatingLaneCarSpeedProvider SpeedProvider;
        [Range(0, 1)]
        public float Anticipation = 0.8f;

        // Start is called before the first frame update
        void Start()
        {
            if (InitialRoad == null) InitialRoad = GetComponent<PathRoadLayout>();
            if (Body == null) Body = GetComponent<Rigidbody>();
            if (Car == null) Car = GetComponent<CarCollider>();
            if (SpeedProvider == null) SpeedProvider = GetComponent<AnticipatingLaneCarSpeedProvider>();
            Travel = new RoadTravel(InitialRoad);
            SpeedProvider.Acceleration = Acceleration;
            SpeedProvider.Anticipation = Anticipation;
            SpeedProvider.Brake = Brake;
            SpeedProvider.Decceleration = Decceleration;
            SpeedProvider.MaxSpeed = MaxSpeed;
            //FrontTrigger.TriggerEntered += OnTriggerEntered;
            //FrontTrigger.TriggerExitted += OnTriggerExitted;
            //FrontTrigger.TriggerStayed += OnTriggerStayed;
            //FrontTriggerTooClose.TriggerEntered += OnTriggerTooCloseEntered;
            //FrontTriggerTooClose.TriggerExitted += OnTriggerTooCloseExitted;
            //ChangeLaneLeft.TriggerEntered += OnTriggerLeftEntered;
            //ChangeLaneLeft.TriggerExitted += OnTriggerLeftExitted;
            //ChangeLaneRight.TriggerEntered += OnTriggerRightEntered;
            //ChangeLaneRight.TriggerExitted += OnTriggerRightExitted;
            changeLaneLeftQueued = false;
            changeLaneRightQueued = false;
            //ChooseRoadBehaviour = new RandomPathBehaviour();
            //ChangeBehaviour = new BasicRoadChangeBehaviour(0.15f, 55);
            //ComputeDecceleration();
        }

        //private void OnTriggerStayed(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (car.NeedToSlowDown && !OtherCars.Contains(car)) 
        //            OtherCars.Add(car);
        //    }
        //}

        //private void OnTriggerRightExitted(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (car.Situation != CarSituation.Trigger) RightCars.Remove(car);
        //    }
        //}

        //private void OnTriggerRightEntered(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (car.Situation != CarSituation.Trigger) RightCars.Add(car);
        //    }
        //}

        //private void OnTriggerLeftExitted(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (car.Situation != CarSituation.Trigger) LeftCars.Remove(car);
        //    }
        //}

        //private void OnTriggerLeftEntered(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (car.Situation != CarSituation.Trigger) LeftCars.Add(car);
        //    }
        //}

        //float CurrentMaxSpeed => Mathf.Min(MaxSpeed, Travel.Road.MaxSpeed);
        //float NewSpeedDistance => _laneChangeDistance ?? Travel.Length;
        //float NewTravelSpeed => _laneChangeDistance == null ? Travel.Road.EndTravels.FirstOrDefault()?.Road.MaxSpeed ?? 0 : CurrentMaxSpeed * ChangeBehaviour.MaxSpeedPercent;
        //float DeltaSpeed => Travel.Road.MaxSpeed - NewTravelSpeed;

        //float? DeccelerateDistance;

        //public List<CarCollider> OtherCars = new List<CarCollider>();
        //public List<CarCollider> LeftCars = new List<CarCollider>();
        //public List<CarCollider> RightCars = new List<CarCollider>();
        //bool TooClose = false;

        //float TargetSpeed
        //{
        //    get
        //    {
        //        var otherCar = GetClosestCarInFront();
        //        if (otherCar != null && NeedToSlowDown()) // Other car detected (collision trigger)
        //        {
        //            var d = FrontTrigger.GetBackToBackPercent(otherCar);
        //            if (TooClose) // Other car is too close
        //            {
        //                if (d < 0) // If the distance is negative
        //                {
        //                    return MaxSpeed; // Speed up to get away from the other car
        //                }
        //                else
        //                {
        //                    brake = true;
        //                    return 0; // Slow down to avoid a collision
        //                }
        //            }
        //            else
        //            {
        //                float s = ExpEasing(1 - GetPercentSpeedToAvoidCollision());
        //                brake = true;
        //                //return 0;
        //                return CurrentMaxSpeed * s;
        //            }
        //        }
        //        else
        //        {
        //            if (DeccelerateDistance != null) // Has a deccelerarion been planned
        //            {
        //                if (DeccelerateDistance < Travel.Distance) // Is it time to deccelerate
        //                {
        //                    return NewTravelSpeed; // Deccelerate
        //                }
        //                else
        //                {
        //                    return CurrentMaxSpeed; // Accellerate / go at speed limit
        //                }
        //            }
        //            else
        //            {
        //                return CurrentMaxSpeed; // Accellerate / go at speed limit
        //            }
        //        }
        //    }
        //}

        //public float Anticipation = 0.8f;
        //private bool brake;

        protected override bool changeLaneLeftQueued 
        { 
            get => SpeedProvider.changeLaneLeftQueued; 
            set
            {
                SpeedProvider.changeLaneLeftQueued = value;
            }
        }

        protected override bool changeLaneRightQueued
        {
            get => SpeedProvider.changeLaneRightQueued;
            set
            {
                SpeedProvider.changeLaneRightQueued = value;
            }
        }

        protected override void OnTravelChanged()
        {
            if (!_laneChangeDistance.HasValue && _changingLanes)
            {
                _changingLanes = false;
                changeLaneLeftQueued = false;
                changeLaneRightQueued = false;
            }
            //PlanRoadChange();
            TravelPlan.Enqueue(ChooseRoadBehaviour.ChoosePath(Travel.Road.EndTravels));
            RoadTravelChangeEventHandler handler = TravelChanged;
            handler?.Invoke(this, new RoadTravelChangeEventArgs(Travel));
        }

        public override void PlanRoadChange()
        {
            RandomLaneChange(LaneChangeProbability);
            if (_laneChangeDistance == null)
            {
                TravelPlan.Enqueue(ChooseRoadBehaviour.ChoosePath(Travel.Road.EndTravels));
            }
        }

        /// <summary>
        /// Set the planned lane change as the next path and queue the new lane in the plan
        /// </summary>
        /// <param name="path"></param>
        public void SetLaneChange(RoadChangePath path)
        {
            if (path == null) return;
            _changingLanes = true;
            TravelPlan.Dequeue();
            Travel.Length = Travel.Distance;
            TravelPlan.Enqueue(new RoadTravel(path));
            TravelPlan.Enqueue(path.DestinationTravel);
            _laneChangeDistance = Travel.Distance;
        }

        protected override void LaneChange(bool left)
        {
            base.LaneChange(left);
            SpeedProvider.LaneChange(left);
            //if (_laneChangeDistance.HasValue)
            //{
            //    if (left) ChangeLaneLeft.Active = true;
            //    else ChangeLaneRight.Active = true;
            //}
        }

        //float ExpEasing(float value)
        //{
        //    return Mathf.Pow(2, 10 * (value - 1)) - 0.001f;
        //}

        //CarCollider GetClosest(IEnumerable<CarCollider> colliders)
        //{
        //    float distance = float.MaxValue;
        //    CarCollider closest = null;
        //    foreach (var car in colliders)
        //    {
        //        if (car != null)
        //        {
        //            var d = FrontTrigger.GetBackToBackDistance(car);
        //            if (d < distance)
        //            {
        //                distance = d;
        //                closest = car;
        //            }
        //        }
        //    }
        //    return closest;
        //}

        //CarCollider GetClosestCarInFront()
        //{
        //    var colliders = new List<CarCollider>();
        //    foreach (var car in OtherCars)
        //    {
        //        if (car.Situation == CarSituation.CarWillChangeLaneLeft && Travel.Road.RightRoad != null && Travel.Road.RightRoad.ContainsUser(car.Car.gameObject))
        //            colliders.Add(car);
        //        if (car.Situation == CarSituation.CarWillChangeLaneRight && Travel.Road.LeftRoad != null && Travel.Road.LeftRoad.ContainsUser(car.Car.gameObject))
        //            colliders.Add(car);
        //        if (car.NeedToSlowDown && FrontTriggerTooClose.GetBackToBackDistance(car) > 0 && RoadContainsUser(car.Car.gameObject))
        //            colliders.Add(car);
        //    }
        //    return GetClosest(colliders);
        //}

        //bool RoadContainsUser(GameObject user, int level = 2)
        //{
        //    return Travel.Road.FindUser(user, level);
        //}

        //bool NeedToSlowDown()
        //{
        //    foreach (var car in OtherCars)
        //    {
        //        if (car.NeedToSlowDown)
        //        {
        //            if (FrontTrigger.GetBackToBackDistance(car) > 0) return true;
        //        }
        //    }
        //    return false;
        //}

        //void ComputeDecceleration()
        //{
        //    if (DeltaSpeed <= 0)
        //    {
        //        DeccelerateDistance = null;
        //        return;
        //    }
        //    var acc = new LinearEquation(Acceleration, speed);
        //    var decc = LinearEquation.FromAAndPoint(-Decceleration, new Vector2(NewSpeedDistance, NewTravelSpeed));
        //    DeccelerateDistance = LinearEquation.Resolve(acc, decc, CurrentMaxSpeed)?.x;
        //    if (DeccelerateDistance != null) DeccelerateDistance -= DeccelerateDistance / 2 * Anticipation;
        //}

        protected override void Move(Vector3 pos, Quaternion rotation)
        {
            if (Body != null)
            {
                Body.MovePosition(pos);
                Body.MoveRotation(rotation);
            }
            else base.Move(pos, rotation);
        }

        //void OnTriggerEntered(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (car.NeedToSlowDown) OtherCars.Add(car);
        //    }
        //}

        //float GetPercentSpeedToAvoidCollision()
        //{
        //    float boxLength = FrontTrigger.Trigger.bounds.size.x;
        //    var otherCar = GetClosestCarInFront();
        //    var posOther = otherCar.Trigger.center;
        //    var posBackOther = new Vector3(posOther.x - otherCar.Trigger.bounds.extents.x, posOther.y, posOther.z);

        //    var posBackTrigger = new Vector3(FrontTrigger.Trigger.center.x - FrontTrigger.Trigger.bounds.extents.x, FrontTrigger.Trigger.center.y, FrontTrigger.Trigger.center.z);
        //    var d = Vector3.Distance(posBackTrigger, posBackOther);
        //    return d / boxLength;
        //}

        //void OnTriggerTooCloseEntered(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (car == GetClosestCarInFront() && car.NeedToSlowDown)
        //            TooClose = true;
        //    }
        //}

        //void OnTriggerExitted(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (OtherCars.Contains(car)) OtherCars.Remove(car);
        //    }
        //}
        //void OnTriggerTooCloseExitted(Collider collider)
        //{
        //    if (collider.gameObject.TryGetComponent(out CarCollider car))
        //    {
        //        if (car == GetClosestCarInFront() && car.NeedToSlowDown)
        //            TooClose = false;
        //    }
        //}

        //protected override void OnTravelChanged()
        //{
        //    base.OnTravelChanged();
        //    ComputeDecceleration();
        //}

        /// <summary>
        /// Calculates the speed using the speed provider
        /// </summary>
        public void UpdateSpeed()
        {
            speed = SpeedProvider.GetSpeed();
        }

        void Update()
        {
            UpdateSpeed();
            BasicUpdate();
        }
    }
}
