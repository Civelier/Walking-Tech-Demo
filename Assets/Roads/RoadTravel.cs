using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    [Serializable]
    public class RoadTravel : IEquatable<RoadTravel>
    {
        public readonly Road Road;
        public float Distance { get; set; }
        public float DistanceRemaining => Length - Distance;
        public float Length;
        public Vector3 CurrentPoint => Road?.Path.path.GetPointAtDistance(Distance, PathCreation.EndOfPathInstruction.Stop) ?? new Vector3();
        public Quaternion CurrentRotation => Road?.Path.path.GetRotationAtDistance(Distance, PathCreation.EndOfPathInstruction.Stop) ?? new Quaternion();
        public Vector3 CurrentDirection => Road?.Path.path.GetDirectionAtDistance(Distance, PathCreation.EndOfPathInstruction.Stop) ?? new Vector3();
        public Vector3 CurrentNormal => Road?.Path.path.GetNormalAtDistance(Distance, PathCreation.EndOfPathInstruction.Stop) ?? new Vector3();

        public RoadTravel(Road road, float distance = 0, float? length = null)
        {
            Road = road;
            Distance = distance;
            Length = length ?? Road?.Path.path.length ?? 0;
        }

        /// <summary>
        /// Increments the <see cref="Distance"/> for the amount of speed.
        /// </summary>
        /// <param name="speed">The object speed</param>
        /// <returns>True when the distance is still within the length.</returns>
        public bool MoveAtSpeed(float speed)
        {
            Distance += speed * Time.deltaTime;
            return DistanceRemaining > 0;
        }

        public void SetClosestDistanceAlongPath(Vector3 position)
        {
            Distance = Road.Path.path.GetClosestDistanceAlongPath(position);
        }

        public CarMovement GetClosestCarFoward(int level, float maxDistance, out float distance)
        {
            //CarMovement car = null;
            //float d = 0;
            //Parallel.ForEach(Road.FindCars(level), (c, state) =>
            //{
            //    if (c != null)
            //    {
            //        d = GetDistanceToOtherTravel(c.Travel, maxDistance);
            //        if (d > Distance)
            //        {
            //            car = c;
            //            state.Break();
            //        }
            //    }
            //});
            //distance = d;
            distance = 0;
            foreach (var car in Road.FindCars(level))
            {
                distance = GetDistanceToOtherTravel(car.Travel);
                if (distance > Distance) return car;
            }
            return null;
        }

        public CarMovement GetClosestCarFoward(int level, out float distance)
        {
            CarMovement car = null;
            float d = 0;
            Parallel.ForEach(Road.FindCars(level), (c, state) =>
            {
                d = GetDistanceToOtherTravel(car.Travel);
                if (d > Distance)
                {
                    car = c;
                    state.Break();
                }
            });
            distance = d;
            //foreach (var car in Road.FindCars(level))
            //{
            //    if (GetDistanceToOtherTravel(car.Travel) > Distance) return car;
            //}
            return car;
        }

        IEnumerator<bool> ExploreTravelsBetween(RoadTravel other, List<RoadTravel> travels, int level)
        {
            if (other.Road == Road) yield return true;
            if (level - 1 > 0)
            {
                travels.Add(this);
                foreach (var travel in Road.EndTravels)
                {
                    using (var e = travel.ExploreTravelsBetween(other, travels, level - 1))
                    {
                        while (e.MoveNext())
                        {
                            var result = e.Current;
                            if (result) yield return true;
                            else
                            {
                                travels.Remove(this);
                                yield return false;
                                travels.Add(this);
                            }
                        }
                    }
                }
                travels.Remove(this);
            }
            yield return false;
        }

        IEnumerator<bool> ExploreTravelsBetween(RoadTravel other, List<RoadTravel> travels, int level, float maxDistance, float currentDistance = 0)
        {
            if (other.Road == Road) yield return true;
            if (level - 1 > 0)
            {
                travels.Add(this);
                foreach (var travel in Road.EndTravels)
                {
                    if (travel.DistanceRemaining + currentDistance < maxDistance)
                    {
                        using (var e = travel.ExploreTravelsBetween(other, travels, level - 1, maxDistance, currentDistance + travel.DistanceRemaining))
                        {
                            while (e.MoveNext())
                            {
                                var result = e.Current;
                                if (result) yield return true;
                                else
                                {
                                    travels.Remove(this);
                                    yield return false;
                                    travels.Add(this);
                                }
                            }
                        }
                    }
                }
                travels.Remove(this);
            }
            yield return false;
        }

        public IEnumerable<RoadTravel> GetTravelsBetween(RoadTravel other)
        {
            List<RoadTravel> travels = new List<RoadTravel>();
            if (other.Road == Road)
            {
                List<IEnumerator<bool>> branches = new List<IEnumerator<bool>>();
                foreach (var travel in Road.EndTravels)
                {
                    branches.Add(travel.ExploreTravelsBetween(other, travels, 2));
                }
                bool interrupt = false;
                bool[] isComplete = new bool[branches.Count];
                while (!interrupt)
                {
                    int i = 0;
                    foreach (var branch in branches)
                    {
                        if (!isComplete[i])
                        {
                            bool end = !branch.MoveNext();
                            if (branch.Current)
                            {
                                interrupt = true;
                                break;
                            }
                            if (end)
                            {
                                branch.Dispose();
                                isComplete[i] = true;
                            }
                        }
                        i++;
                    }
                    bool complete = true;
                    foreach (var result in isComplete)
                    {
                        if (!result)
                        {
                            complete = false;
                            break;
                        }
                    }
                    if (complete) break;
                }
            }
            return travels;
        }
        public IEnumerable<RoadTravel> GetTravelsBetween(RoadTravel other, float maxDistance)
        {
            List<RoadTravel> travels = new List<RoadTravel>();
            if (other.Road == Road)
            {
                List<IEnumerator<bool>> branches = new List<IEnumerator<bool>>();
                foreach (var travel in Road.EndTravels)
                {
                    branches.Add(travel.ExploreTravelsBetween(other, travels, 2, maxDistance));
                }
                bool interrupt = false;
                bool[] isComplete = new bool[branches.Count];
                while (!interrupt)
                {
                    int i = 0;
                    foreach (var branch in branches)
                    {
                        if (!isComplete[i])
                        {
                            bool end = !branch.MoveNext();
                            if (branch.Current)
                            {
                                interrupt = true;
                                break;
                            }
                            if (end)
                            {
                                branch.Dispose();
                                isComplete[i] = true;
                            }
                        }
                        i++;
                    }
                    bool complete = true;
                    foreach (var result in isComplete)
                    {
                        if (!result)
                        {
                            complete = false;
                            break;
                        }
                    }
                    if (complete) break;
                }
            }
            return travels;
        }


        public float GetDistanceToOtherTravel(RoadTravel other)
        {
            float sum = DistanceRemaining;
            RoadTravel last = null;
            foreach (var travel in GetTravelsBetween(other))
            {
                sum += travel.DistanceRemaining;
                last = travel;
            }
            if (last != null)
            {
                sum -= last.DistanceRemaining;
                sum += other.Distance - last.Distance;
            }
            return sum;
        }

        public float GetDistanceToOtherTravel(RoadTravel other, float maxDistance)
        {
            float sum = DistanceRemaining;
            RoadTravel last = null;
            foreach (var travel in GetTravelsBetween(other))
            {
                sum += travel.DistanceRemaining;
                last = travel;
            }
            if (last != null)
            {
                sum -= last.DistanceRemaining;
                sum += other.Distance - last.Distance;
            }
            return sum;
        }

        public bool Equals(RoadTravel other)
        {
            if (other == null) return false;
            return other.Road == Road && other.Distance == Distance;
        }
    }
}
