using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    [Flags]
    public enum RoadChange
    {
        None = 0x00,
        Left = 0x01,
        Right = 0x10,
        Both = 0x11,
    }
    public class AdvancedRoadChangeBehaviour : MonoBehaviour, IRoadChangeBehaviour
    {
        public CarMovement CarMove;
        protected System.Random _random = new System.Random();

        public BasicRoadChangeBehaviour RoadChangeBehaviour = new BasicRoadChangeBehaviour();
        public float MaxSpeedPercent => 0.8f;

        protected RoadChange _possibleRoadChanges
        {
            get
            {
                var r = RoadChange.None;
                if (CarMove?.Travel?.Road?.LeftRoad != null) r |= RoadChange.Left;
                if (CarMove?.Travel?.Road?.LeftRoad != null) r |= RoadChange.Right;
                return r;
            }
        }

        private void Start()
        {
            if (CarMove == null) CarMove = GetComponent<CarMovement>();
            CarMove.Car.TriggerStayed += OnTriggerStayed;
        }

        public virtual RoadChangePath PlanRoadChange(Road initial, float distance, Road destination)
        {
            return PlanRoadChange(new RoadTravel(initial, distance), destination);
        }

        protected void OnTriggerStayed(Collider collider)
        {
            if (CarMove._changingLanes) return;
            if (_possibleRoadChanges != RoadChange.None)
            {
                if (CarMove.SpeedProvider.SlowDownCause == CauseOfSlowDown.FrontCarSlower)
                {
                    RoadChange changePossible = _possibleRoadChanges;
                    switch (_possibleRoadChanges)
                    {
                        case RoadChange.Left:
                            foreach (var carInfo in CarMove.SpeedProvider.NearbyCars)
                            {
                                if (!carInfo.IsOnPath)
                                {
                                    if (carInfo.RelativeDistance.x < 0 && carInfo.RelativeDistance.z < 0)
                                    {
                                        changePossible = RoadChange.None;
                                        break;
                                    }
                                }
                            }
                            break;
                        case RoadChange.Right:
                            foreach (var carInfo in CarMove.SpeedProvider.NearbyCars)
                            {
                                if (!carInfo.IsOnPath)
                                {
                                    if (carInfo.RelativeDistance.x < 0 && carInfo.RelativeDistance.z > 0)
                                    {
                                        changePossible = RoadChange.None;
                                        break;
                                    }
                                }
                            }
                            break;
                        case RoadChange.Both:
                            foreach (var carInfo in CarMove.SpeedProvider.NearbyCars)
                            {
                                if (!carInfo.IsOnPath)
                                {
                                    if ((changePossible & RoadChange.Left) == RoadChange.Left && carInfo.RelativeDistance.x < 0 && carInfo.RelativeDistance.z < 0)
                                    {
                                        changePossible ^= RoadChange.Left;
                                        break;
                                    }
                                    if ((changePossible & RoadChange.Right) == RoadChange.Right && carInfo.RelativeDistance.x < 0 && carInfo.RelativeDistance.z > 0)
                                    {
                                        changePossible ^= RoadChange.Right;
                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    switch (changePossible)
                    {
                        case RoadChange.None:
                            break;
                        case RoadChange.Left:
                            CarMove.SetLaneChange(RoadChangeBehaviour.PlanRoadChange(CarMove.Travel, CarMove.Travel.Road.LeftRoad));
                            break;
                        case RoadChange.Right:
                            CarMove.SetLaneChange(RoadChangeBehaviour.PlanRoadChange(CarMove.Travel, CarMove.Travel.Road.RightRoad));
                            break;
                        case RoadChange.Both:
                            int r = _random.Next(2);
                            if (r == 0) CarMove.SetLaneChange(RoadChangeBehaviour.PlanRoadChange(CarMove.Travel, CarMove.Travel.Road.LeftRoad));
                            else CarMove.SetLaneChange(RoadChangeBehaviour.PlanRoadChange(CarMove.Travel, CarMove.Travel.Road.RightRoad));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public virtual bool IsPossible(RoadTravel travel, Road destination)
        {
            return false;
        }

        public virtual RoadChangePath PlanRoadChange(RoadTravel travel, Road destination)
        {
            return null;
        }
    }
}
