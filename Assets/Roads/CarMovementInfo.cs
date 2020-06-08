using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    
    [Serializable]
    public struct CarMovementInfo : IEquatable<CarMovementInfo>
    {
        public static float RecalculateTreshold = 2;

        public CarMovement Parent;
        public readonly CarMovement CarMove;

        public bool IsOnPath { get; private set; }
        
        private float _lastCalculatedQuickDistance;
        public float QuickDistance => Vector3.Distance(Parent.transform.position, CarMove.transform.position);

        float _lastActualDistance;
        public float LastActualDistance 
        { 
            get
            {
                var qd = QuickDistance;
                if (qd < _lastCalculatedQuickDistance - RecalculateTreshold && _lastCalculatedQuickDistance + RecalculateTreshold < qd)
                {
                    RecalculateDistance();
                }
                return _lastActualDistance;
            }
        }

        public float DeltaSpeed => CarMove.speed - Parent.speed;

        public CarMovementInfo(CarMovement parent, CarMovement move)
        {
            Parent = parent;
            CarMove = move;
            _lastActualDistance = 0;
            _lastCalculatedQuickDistance = 0;
            IsOnPath = false;
            RecalculateDistance();
        }

        public void RecalculateDistance()
        {
            _lastCalculatedQuickDistance = QuickDistance;
            float distance = Parent.Travel.DistanceRemaining;
            foreach (var travel in Parent.TravelPlan)
            {
                if (CarMove.Travel.Road == travel.Road)
                {
                    distance += CarMove.Travel.Distance - travel.Distance;
                    _lastActualDistance = distance;
                    IsOnPath = true;
                    return;
                }
                else distance += travel.DistanceRemaining;
            }
            IsOnPath = false;
            _lastActualDistance = _lastCalculatedQuickDistance;
        }

        public bool Equals(CarMovementInfo other)
        {
            return Parent == other.Parent && CarMove == other.CarMove;
        }
    }
}
