using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    [Serializable]
    public class RoadTravel
    {
        public readonly IRoad Road;
        public float Distance;
        public float Length => Road?.Path.path.length ?? 0;
        public Vector3 CurentPoint => Road?.Path.path.GetPointAtDistance(Distance, PathCreation.EndOfPathInstruction.Stop) ?? new Vector3();
        public Quaternion CurentRotation => Road?.Path.path.GetRotationAtDistance(Distance, PathCreation.EndOfPathInstruction.Stop) ?? new Quaternion();
        public Vector3 CurentDirection => Road?.Path.path.GetDirectionAtDistance(Distance, PathCreation.EndOfPathInstruction.Stop) ?? new Vector3();
        public Vector3 CurentNormal => Road?.Path.path.GetNormalAtDistance(Distance, PathCreation.EndOfPathInstruction.Stop) ?? new Vector3();

        public RoadTravel(IRoad road, float distance = 0)
        {
            Road = road;
            Distance = distance;
        }

        /// <summary>
        /// Increments the <see cref="Distance"/> for the amount of speed.
        /// </summary>
        /// <param name="speed">The object speed</param>
        /// <returns>True when the distance is still within the length.</returns>
        public bool MoveAtSpeed(float speed)
        {
            Distance += speed * Time.deltaTime;
            return Distance >= 0 && Distance <= Road.Path.path.length;
        }

        public void SetClosestDistanceAlongPath(Vector3 position)
        {
            Distance = Road.Path.path.GetClosestDistanceAlongPath(position);
        }
    }
}
