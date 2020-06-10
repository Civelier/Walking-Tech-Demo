using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.Linq;

namespace Roads
{
    [ExecuteInEditMode]
    public class PathRoadLayout : Road
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

        public override IEnumerable<RoadTravel> EndTravels => GetTravels();

        private bool _update = true;

        IEnumerable<RoadTravel> GetTravels()
        {
            foreach (var road in Head.OutgoingRoads)
            {
                yield return new RoadTravel(road);
            }
        }

        public override IEnumerable<CarMovement> FindCars(int level)
        {
            foreach (var car in Users)
            {
                if (car.TryGetComponent(out CarMovement m)) yield return m;
            }
            if (level - 1 > 0)
            {
                foreach (var car in base.FindCars(level))
                {
                    yield return car;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Path.pathUpdated += OnUpdate;
            GenerateNodes();
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
            Path.pathUpdated += OnUpdate;
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
            Path.pathUpdated -= OnUpdate;
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

        public override void Exitted(GameObject user)
        {
            Users.Remove(user);
        }

        public override void Entered(GameObject user)
        {
            Users.Add(user);
        }

        public override bool ContainsUser(GameObject user)
        {
            return Users.Contains(user);
        }

        public override IEnumerable<CarMovement> GetUsers()
        {
            foreach (var user in Users)
            {
                if (user.TryGetComponent(out CarMovement car))
                {
                    yield return car;
                }
            }
        }
    }
}
