using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.Linq;

namespace Roads
{
    [ExecuteInEditMode]
    public class PathRoadLayout : MonoBehaviour, IRoad
    {
        public List<GameObject> Users = new List<GameObject>();

#if UNITY_EDITOR
        public PathNode _head;
#else
        private PathNode _head;
#endif
        public PathNode Head
        {
            get => _head;
            set
            {
                if (_head == value) return;
                if (_head != null)
                {
                    //_head.NodeMoved -= OnHeadMoved;
                    _head.RemoveIncomming(this);
                    //if (_head.IncommingRoads.Count == 0 && _head.OutgoingRoads.Count == 0) Destroy(_head.gameObject);
                }
                _head = value;
                if (_head != null)
                {
                    //_head.NodeMoved += OnHeadMoved;
                    _head.AddAsIncomming(this);
                }
            }
        }

#if UNITY_EDITOR
        public PathNode _tail;
#else
        private PathNode _tail;
#endif
        public PathNode Tail
        {
            get => _tail;
            set
            {
                if (_tail == value) return;
                if (_tail != null)
                {
                    //_tail.NodeMoved -= OnTailMoved;
                    _tail.RemoveOutgoing(this);
                    //if (_tail.IncommingRoads.Count == 0 && _tail.OutgoingRoads.Count == 0) Destroy(_tail);
                }
                _tail = value;
                if (_tail != null)
                {
                    //_tail.NodeMoved += OnTailMoved;
                    _tail.AddAsOutgoing(this);
                }
            }
        }

        public PathCreator Path => ThisPath;

        public IEnumerable<RoadTravel> EndTravels => GetTravels();

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
                    ThisPath.bezierPath.MovePoint(AnchorToPointIndex(i), value, true);
                    var mode = ThisPath.bezierPath.ControlPointMode;
                    ThisPath.bezierPath.ControlPointMode = BezierPath.ControlMode.Free;
                    ThisPath.bezierPath.ControlPointMode = mode;
                    ThisPath.EditorData.PathTransformed();
                    ThisPath.TriggerPathUpdate();
                }
            }
        }
        public PathCreator ThisPath;
        public PathRoadLayout LeftRoad;
        public PathRoadLayout RightRoad;
        private bool _update = true;

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

        IEnumerable<RoadTravel> GetTravels()
        {
            foreach (var road in Head.OutgoingRoads)
            {
                yield return new RoadTravel(road);
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            if (ThisPath == null) ThisPath = GetComponent<PathCreator>();
            ThisPath.pathUpdated += OnUpdate;
            GenerateNodes();
        }

        public Vector3 GetGlobalPositionPoint(int i)
        {
            return this[i] + transform.position;
        }

        public void SetGlobalPositionPoint(int i, Vector3 value)
        {
            this[i] = value - transform.position;
        }

        public void GenerateNodes()
        {
            if (_head == null)
            {
                Head = Instantiate(PathFactory.Instance.NodePrefab, this[-1], new Quaternion()).GetComponent<PathNode>();
            }
            if (_tail == null)
            {
                Tail = Instantiate(PathFactory.Instance.NodePrefab, this[0], new Quaternion()).GetComponent<PathNode>();
            }
        }

        public void ResetHandle()
        {
            if (ThisPath == null) ThisPath = GetComponent<PathCreator>();
            ThisPath.pathUpdated += OnUpdate;
            if (_head != null)
            {
                _head.NodeMoved += OnHeadMoved;
                _head.AddAsIncomming(this);
            }
            if (_tail != null)
            {
                _tail.NodeMoved += OnTailMoved;
                _tail.AddAsOutgoing(this);
            }
        }

        private void OnValidate()
        {
            ResetHandle();
        }

        private void OnDestroy()
        {
            ThisPath.pathUpdated -= OnUpdate;
            Head = null;
            Tail = null;
        }

        void OnHeadMoved(Vector3 pos)
        {
            if (_update) SetGlobalPositionPoint(-1, pos);
        }

        void OnTailMoved(Vector3 pos)
        {
            if (_update) SetGlobalPositionPoint(0, pos);
        }

        public void DisableNotifications()
        {
            _update = false;
        }

        public void EnableNotifications()
        {
            _update = true;
        }

        public void OnUpdate()
        {
            //GenerateNodes();
            if (!_update) return;
            DisableNotifications();
            if (_head.Position != GetGlobalPositionPoint(-1))
            {
                _head.Position = GetGlobalPositionPoint(-1);// - ThisPath.transform.position;
                //ThisPath.EditorData.PathTransformed();
            }
            if (_tail.Position != GetGlobalPositionPoint(0))
            {
                _tail.Position = GetGlobalPositionPoint(0);// - ThisPath.transform.position;
                //ThisPath.EditorData.PathTransformed();
            }
            EnableNotifications();
        }

        public void Exitted(GameObject user)
        {
            Users.Remove(user);
        }

        public void Entered(GameObject user)
        {
            Users.Add(user);
        }
    }
}
