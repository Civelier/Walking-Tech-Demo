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
        public static bool AlwaysRecalculate = true;
        public static float RecalculateTreshold = 0.5f;

        public CarMovement Parent;
        public readonly CarMovement CarMove;

        public bool IsOnPath { get; private set; }

        public bool IsInFront => LastActualDistance > 0;

        public bool IsValid { get; private set; }
        
        private float _lastCalculatedQuickDistance;
        public float QuickDistance => Vector3.Distance(Parent.transform.position, CarMove.transform.position);

        float _lastActualDistance;
        public float LastActualDistance 
        { 
            get
            {
                if (AlwaysRecalculate) RecalculateDistance();
                else
                {
                    var qd = QuickDistance;
                    if (qd < _lastCalculatedQuickDistance - RecalculateTreshold && _lastCalculatedQuickDistance + RecalculateTreshold < qd)
                    {
                        RecalculateDistance();
                    }
                }
                return _lastActualDistance;
            }
        }

        public Vector3 RelativeDistance
        {
            get
            {
                var worldToParent = Matrix4x4.TRS(Parent.transform.position, Parent.transform.rotation, new Vector3(1, 1, 1));
                return worldToParent.MultiplyPoint3x4(CarMove.transform.position);
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
            IsValid = true;
            RecalculateDistance();
        }

        public void RecalculateDistance()
        {
            if (Parent.Travel?.Road == null) return;
            if (!AlwaysRecalculate) _lastCalculatedQuickDistance = QuickDistance;
            
            float distance = 0;
            
            if (CarMove.Travel.Road == Parent.Travel.Road)
            {
                if (Parent.Travel.Distance < CarMove.Travel.Distance)
                {
                    distance += CarMove.Travel.Distance - Parent.Travel.Distance;
                    _lastActualDistance = distance;
                    IsOnPath = true;
                }
                //else IsValid = false;
                return;
            }
            else distance += Parent.Travel.DistanceRemaining;
            foreach (var travel in Parent.TravelPlan)
            {
                if (travel != null)
                {
                    if (CarMove.Travel.Road == travel.Road)
                    {
                        if (travel.Distance + distance > CarMove.Travel.Distance)
                        {
                            distance += CarMove.Travel.Distance - travel.Distance;
                            _lastActualDistance = distance;
                            IsOnPath = true;
                        }
                        //else IsValid = false;
                        return;
                    }
                    else distance += travel.DistanceRemaining;
                }
            }
            IsOnPath = false;
            _lastActualDistance = _lastCalculatedQuickDistance;
            IsValid = false;
        }

        public bool Equals(CarMovementInfo other)
        {
            return Parent == other.Parent && CarMove == other.CarMove;
        }
    }
}
