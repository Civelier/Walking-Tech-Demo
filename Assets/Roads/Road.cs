using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public abstract class Road : MonoBehaviour
    {
        private PathCreator _path;
        public PathCreator Path
        {
            get
            {
                if (_path == null) _path = GetComponent<PathCreator>();
                return _path;
            }
        }

        public float MaxSpeed = 20;
        public Road LeftRoad;
        public Road RightRoad;
        public abstract IEnumerable<RoadTravel> EndTravels { get; }
        public abstract void Exitted(GameObject user);
        public abstract void Entered(GameObject user);

        public Vector3 this[int i]
        {
            get
            {
                if (Path == null)
                {

                }
                return Path.bezierPath.GetPoint(AnchorToPointIndex(i));
            }
            set
            {
                if (value != this[i])
                {
                    Path.bezierPath.SetPoint(AnchorToPointIndex(i), value, true);
                    var mode = Path.bezierPath.ControlPointMode;
                    Path.bezierPath.ControlPointMode = BezierPath.ControlMode.Free;
                    Path.bezierPath.ControlPointMode = mode;
                    Path.EditorData.PathTransformed();
                    Path.TriggerPathUpdate();
                }
            }
        }

        protected IEnumerable<int> GetSegmentsNumPoints()
        {
            for (int i = 0; i < Path.bezierPath.NumSegments; i++)
            {
                yield return Path.bezierPath.GetPointsInSegment(i).Length;
            }
        }

        public int AnchorToPointIndex(int anchorIndex)
        {
            if (anchorIndex < 0) anchorIndex += Path.bezierPath.NumAnchorPoints;
            int index = 0;
            var arr = GetSegmentsNumPoints().ToArray();
            for (int i = 0; i < anchorIndex; i++)
            {
                index += arr[i] - 1;
            }
            return index;
        }

        public Vector3 GetGlobalPositionPoint(int i)
        {
            return this[i] + transform.position;
        }

        public void SetGlobalPositionPoint(int i, Vector3 value)
        {
            this[i] = value - transform.position;
        }
    }
}
