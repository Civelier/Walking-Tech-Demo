using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roads
{
    public class CarMovement : AdvancedPathFolower
    {
        public float Acceleration = 8;
        public float Decceleration = 15;
        public float MaxSpeed = 40;
        //[Range(0,1)]
        //public float Anticipation = 0.8f;

        // Start is called before the first frame update
        void Start()
        {
            if (InitialRoad == null) InitialRoad = GetComponent<PathRoadLayout>();
            Travel = new RoadTravel(InitialRoad);
            //ChooseRoadBehaviour = new RandomPathBehaviour();
            //ChangeBehaviour = new BasicRoadChangeBehaviour(0.15f, 55);
            ComputeDecceleration();
        }

        float CurrentMaxSpeed => Mathf.Min(MaxSpeed, Travel.Road.MaxSpeed);
        float NewSpeedDistance => _laneChangeDistance ?? Travel.Length;
        float NewTravelSpeed => _laneChangeDistance == null ? Travel.Road.EndTravels.FirstOrDefault()?.Road.MaxSpeed ?? 0 : CurrentMaxSpeed * ChangeBehaviour.MaxSpeedPercent;
        float DeltaSpeed => Travel.Road.MaxSpeed - NewTravelSpeed;

        float? DeccelerateDistance;

        float TargetSpeed => DeccelerateDistance != null ? (DeccelerateDistance > Travel.Distance ? CurrentMaxSpeed : NewTravelSpeed) : CurrentMaxSpeed;

        public float Anticipation = 0.8f;

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

        protected override void OnTravelChanged()
        {
            base.OnTravelChanged();
            ComputeDecceleration();
        }

        void UpdateSpeed()
        {
            if (speed <= TargetSpeed) speed = Mathf.MoveTowards(speed, TargetSpeed, Time.deltaTime * Acceleration);
            else speed = Mathf.MoveTowards(speed, TargetSpeed, Time.deltaTime * Decceleration);
        }

        void Update()
        {
            UpdateSpeed();
            BasicUpdate();
        }
    }
}
