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
        //[Range(0,1)]
        //public float Anticipation = 0.8f;

        // Start is called before the first frame update
        void Start()
        {
            if (InitialRoad == null) InitialRoad = GetComponent<PathRoadLayout>();
            if (Body == null) Body = GetComponent<Rigidbody>();
            if (Car == null) Car = GetComponent<CarCollider>();
            Travel = new RoadTravel(InitialRoad);
            FrontTrigger.TriggerEntered += OnTriggerEntered;
            FrontTrigger.TriggerExitted += OnTriggerExitted;
            FrontTriggerTooClose.TriggerEntered += OnTriggerTooCloseEntered;
            FrontTriggerTooClose.TriggerExitted += OnTriggerTooCloseExitted;
            changeLaneLeftQueued = false;
            changeLaneRightQueued = false;
            //ChooseRoadBehaviour = new RandomPathBehaviour();
            //ChangeBehaviour = new BasicRoadChangeBehaviour(0.15f, 55);
            ComputeDecceleration();
        }

        float CurrentMaxSpeed => Mathf.Min(MaxSpeed, Travel.Road.MaxSpeed);
        float NewSpeedDistance => _laneChangeDistance ?? Travel.Length;
        float NewTravelSpeed => _laneChangeDistance == null ? Travel.Road.EndTravels.FirstOrDefault()?.Road.MaxSpeed ?? 0 : CurrentMaxSpeed * ChangeBehaviour.MaxSpeedPercent;
        float DeltaSpeed => Travel.Road.MaxSpeed - NewTravelSpeed;

        float? DeccelerateDistance;

        CarCollider OtherCar;
        bool TooClose = false;

        float TargetSpeed
        {
            get
            {
                if (OtherCar?.Trigger.bounds.Intersects(Car.Trigger.bounds) ?? false) OtherCar = null;
                if (OtherCar != null && (OtherCar.Info.Situation | CarSituation.SlowDown) == CarSituation.SlowDown) // Other car detected (collision trigger)
                {
                    var d = GetDistanceToOtherCar();
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
                        return OtherCar.Speed * s;
                    }
                }
                else
                {
                    if (DeccelerateDistance != null) // Has a deccelerarion been planned
                    {
                        if (DeccelerateDistance < Travel.Distance) // Is it time to deccelerate
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

        public float Anticipation = 0.8f;
        private bool brake;

        protected override bool changeLaneLeftQueued 
        { 
            get => base.changeLaneLeftQueued; 
            set
            {
                base.changeLaneLeftQueued = value;
                ChangeLaneLeft.Trigger.enabled = value;
            }
        }

        protected override bool changeLaneRightQueued
        {
            get => base.changeLaneRightQueued;
            set
            {
                base.changeLaneRightQueued = value;
                ChangeLaneRight.Trigger.enabled = value;
            }
        }

        protected override void LaneChange(bool left)
        {
            base.LaneChange(left);
            if (_laneChangeDistance != null)
            {
                if (left) ChangeLaneLeft.Trigger.enabled = true;
                else ChangeLaneRight.Trigger.enabled = true;
            }
        }

        float ExpEasing(float value)
        {
            return Mathf.Pow(2, 10 * (value - 1)) - 0.001f;
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

        protected override void Move(Vector3 pos, Quaternion rotation)
        {
            if (Body != null)
            {
                Body.MovePosition(pos);
                Body.MoveRotation(rotation);
            }
            else base.Move(pos, rotation);
        }

        void OnTriggerEntered(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                OtherCar = car;
            }
        }

        float GetDistanceToOtherCar()
        {
            var p1 = Car.Trigger.ClosestPointOnBounds(OtherCar.transform.localPosition);
            var p2 = transform.worldToLocalMatrix.MultiplyPoint(OtherCar.Trigger.ClosestPointOnBounds(transform.position));
            
            return Mathf.Sign(p2.x - p1.x) * Vector3.Distance(p1, p2);
        }

        float GetPercentSpeedToAvoidCollision()
        {
            float boxLength = FrontTrigger.Trigger.bounds.size.x;
            var posOther = OtherCar.Trigger.center;
            var posBackOther = new Vector3(posOther.x - OtherCar.Trigger.bounds.extents.x, posOther.y, posOther.z);

            var posBackTrigger = new Vector3(FrontTrigger.Trigger.center.x - FrontTrigger.Trigger.bounds.extents.x, FrontTrigger.Trigger.center.y, FrontTrigger.Trigger.center.z);
            var d = Vector3.Distance(posBackTrigger, posBackOther);
            return d / boxLength;
        }

        void OnTriggerTooCloseEntered(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                if ((car.Info.Situation | CarSituation.SlowDown) == CarSituation.SlowDown)
                    TooClose = true;
            }
        }

        void OnTriggerExitted(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                OtherCar = null;
            }
        }
        void OnTriggerTooCloseExitted(Collider collider)
        {
            if (collider.gameObject.TryGetComponent(out CarCollider car))
            {
                TooClose = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger entered car");
        }

        protected override void OnTravelChanged()
        {
            base.OnTravelChanged();
            ComputeDecceleration();
        }

        void UpdateSpeed()
        {
            if (brake)
            {
                speed = Mathf.MoveTowards(speed, TargetSpeed, Time.deltaTime * Brake);
                brake = false;
            }
            else if (speed <= TargetSpeed) 
                speed = Mathf.MoveTowards(speed, TargetSpeed, Time.deltaTime * Acceleration);
            else speed = Mathf.MoveTowards(speed, TargetSpeed, Time.deltaTime * Decceleration);
        }

        void Update()
        {
            UpdateSpeed();
            BasicUpdate();
        }
    }
}
