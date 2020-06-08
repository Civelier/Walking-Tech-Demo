using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    [Serializable]
    public class BasicRoadChangeBehaviour : IRoadChangeBehaviour
    {
        [Range(0, 1)]
        [Tooltip("The smoothing of the curve")]
        public float CurveFactor;
        [Range(1, 89)]
        [Tooltip("The angle to the end point relative to the current orientation")]
        public float AttackAngle;
        public float MaxSpeedPercentEditor = 0.8f;
        public float MaxSpeedPercent => MaxSpeedPercentEditor;

        public BasicRoadChangeBehaviour(float curveFactor = 0.2f, float attackAngle = 75, float maxSpeedPercent = 0.8f)
        {
            CurveFactor = curveFactor;
            AttackAngle = attackAngle;
            MaxSpeedPercentEditor = maxSpeedPercent;
        }

        public RoadChangePath PlanRoadChange(RoadTravel travel, Road destination)
        {
            if (destination == null) return null;
            // Get the closest distance on the destination path to the curent position
            var destDistance = destination.Path.path.GetClosestDistanceAlongPath(travel.CurrentPoint);
            // Get the point of the destination path at the distance
            var destPoint = destination.Path.path.GetPointAtDistance(destDistance, PathCreation.EndOfPathInstruction.Stop);
            // Get distance between the two points
            var delta = Vector3.Distance(destPoint, travel.CurrentPoint);
            // Get the final distance on the destination path
            var destFinalDistance = delta * Mathf.Tan(Mathf.Deg2Rad * AttackAngle) + destDistance;
            // If the final distance is more than the length of the destination path, it is not possible to change path
            if (destFinalDistance > destination.Path.path.length) return null;
            // Calculate the distance of the secondary (mid) points from the end points (to make the curve smoother)
            var curveDistance = (destFinalDistance - destDistance) / 2 * CurveFactor;
            
            // Generate the road change object
            RoadChangePath path = UnityEngine.Object.Instantiate(PathFactory.Instance.RoadChangePrefab).GetComponent<RoadChangePath>();
            path.MaxSpeed = travel.Road.MaxSpeed * MaxSpeedPercentEditor;
            
            // Set the points positions
            path.GlobalTail = travel.CurrentPoint;
            path.GlobalMidTail = path.GlobalTail + travel.CurrentDirection * curveDistance;
            path.GlobalHead = destination.Path.path.GetPointAtDistance(destFinalDistance);
            path.GlobalMidHead = path.GlobalHead - destination.Path.path.GetDirectionAtDistance(destFinalDistance) * curveDistance;

            // Update the curve smooth
            path.Path.bezierPath.ControlPointMode = PathCreation.BezierPath.ControlMode.Aligned;
            path.Path.bezierPath.ControlPointMode = PathCreation.BezierPath.ControlMode.Automatic;

            // Set the travel for the end of the road change
            path.InitialTravel = travel;
            path.DestinationTravel = new RoadTravel(destination, destFinalDistance);
            return path;
        }

        public RoadChangePath PlanRoadChange(Road initial, float distance, Road destination)
        {
            return PlanRoadChange(new RoadTravel(initial, distance), destination);
        }

        public bool IsPossible(RoadTravel travel, Road destination)
        {
            if (destination == null) return false;
            // Get the closest distance on the destination path to the curent position
            var destDistance = destination.Path.path.GetClosestDistanceAlongPath(travel.CurrentPoint);
            // Get the point of the destination path at the distance
            var destPoint = destination.Path.path.GetPointAtDistance(destDistance, PathCreation.EndOfPathInstruction.Stop);
            // Get distance between the two points
            var delta = Vector3.Distance(destPoint, travel.CurrentPoint);
            // Get the final distance on the destination path
            var destFinalDistance = delta * Mathf.Tan(Mathf.Deg2Rad * AttackAngle) + destDistance;
            // If the final distance is more than the length of the destination path, it is not possible to change path
            return destFinalDistance <= destination.Path.path.length;
        }
    }
}
