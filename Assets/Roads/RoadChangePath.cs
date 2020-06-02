using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public class RoadChangePath : MonoBehaviour, IRoad
    {
        public RoadTravel InitialTravel;
        public RoadTravel DestinationTravel;

        public PathCreator ThisPath;
        public PathCreator Path => ThisPath;

        public Vector3 this[int i]
        {
            get
            {
                return ThisPath.bezierPath.GetPoint(AnchorToPointIndex(i));
            }
            set
            {
                if (value != this[i])
                {
                    ThisPath.bezierPath.SetPoint(AnchorToPointIndex(i), value, true);
                    var mode = ThisPath.bezierPath.ControlPointMode;
                    ThisPath.bezierPath.ControlPointMode = BezierPath.ControlMode.Free;
                    ThisPath.bezierPath.ControlPointMode = mode;
                    ThisPath.EditorData.PathTransformed();
                    ThisPath.TriggerPathUpdate();
                }
            }
        }

        IEnumerable<int> GetSegmentsNumPoints()
        {
            for (int i = 0; i < ThisPath.bezierPath.NumSegments; i++)
            {
                yield return ThisPath.bezierPath.GetPointsInSegment(i).Length;
            }
        }

        public int AnchorToPointIndex(int anchorIndex)
        {
            if (anchorIndex < 0) anchorIndex += ThisPath.bezierPath.NumAnchorPoints;
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

        public Vector3 Head
        {
            get => this[-1];
            set => this[-1] = value;
        }

        public Vector3 MidHead
        {
            get => this[-2];
            set => this[-2] = value;
        }

        //public Vector3 Middle
        //{
        //    get => this[ThisPath.bezierPath.NumAnchorPoints / 2 - 1];
        //    set => this[ThisPath.bezierPath.NumAnchorPoints / 2 - 1] = value;
        //}

        public Vector3 MidTail
        {
            get => this[1];
            set => this[1] = value;
        }

        public Vector3 Tail
        {
            get => this[0];
            set => this[0] = value;
        }

        public Vector3 GlobalHead
        {
            get => GetGlobalPositionPoint(-1);
            set => SetGlobalPositionPoint(-1, value);
        }

        public Vector3 GlobalMidHead
        {
            get => GetGlobalPositionPoint(-2);
            set => SetGlobalPositionPoint(-2, value);
        }

        //public Vector3 GlobalMiddle
        //{
        //    get => GetGlobalPositionPoint(ThisPath.bezierPath.NumAnchorPoints / 2 - 1);
        //    set => SetGlobalPositionPoint(ThisPath.bezierPath.NumAnchorPoints / 2 - 1, value);
        //}

        public Vector3 GlobalMidTail
        {
            get => GetGlobalPositionPoint(1);
            set => SetGlobalPositionPoint(1, value);
        }

        public Vector3 GlobalTail
        {
            get => GetGlobalPositionPoint(0);
            set => SetGlobalPositionPoint(0, value);
        }

        public IEnumerable<RoadTravel> EndTravels => new[]{ DestinationTravel };

        void Start()
        {
            if (ThisPath == null) ThisPath = GetComponent<PathCreator>();
        }

        public void Exitted(GameObject user)
        {
            Destroy(gameObject);
        }

        public void Entered(GameObject user)
        {
        }
    }
}
