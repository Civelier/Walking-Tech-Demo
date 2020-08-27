using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Roads
{

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AdvancedPathFolower))]
    public class AdvancedPathFolowerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var t = (AdvancedPathFolower)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Queue move left lane"))
            {
                t.QueueChangeLaneLeft();
            }
            if (GUILayout.Button("Queue move right lane"))
            {
                t.QueueChangeLaneRight();
            }
        }
    }
#endif

    public class AdvancedPathFolower : MonoBehaviour
    {
        /// <summary>
        /// Position offset of the object from the path
        /// </summary>
        public Vector3 PositionOffset;
        /// <summary>
        /// Rotation offset of the object from the path
        /// </summary>
        public Vector3 RotationOffset;
        private RoadTravel _travel;
        /// <summary>
        /// The current <see cref="RoadTravel"/>
        /// </summary>
        public virtual RoadTravel Travel
        {
            get => _travel;
            set
            {
                if (value.Equals(_travel)) return;
                if (_travel != null)
                {
                    _travel.Road.Path.pathUpdated -= OnPathChanged;
                    _travel.Road.Exitted(gameObject);
                }
                _travel = value;
                if (_travel != null)
                {
                    _travel.Road.Path.pathUpdated += OnPathChanged;
                    _travel.Road.Entered(gameObject);
                }
                OnTravelChanged();
            }
        }
        /// <summary>
        /// The initial <see cref="Road"/> of the path folower
        /// </summary>
        public Road InitialRoad;
        /// <summary>
        /// Current speed
        /// </summary>
        public float speed = 5;
        /// <summary>
        /// Probability to change lanes if possible
        /// </summary>
        [Range(0, 1)]
        public float LaneChangeProbability = 0.2f;
        /// <summary>
        /// The road choosing behaviour
        /// </summary>
        public IChooseRoadBehaviour ChooseRoadBehaviour = new RandomPathBehaviour();
        /// <summary>
        /// The lane changing behaviour
        /// </summary>
        public AdvancedRoadChangeBehaviour ChangeBehaviour;

        /// <summary>
        /// If a lane change is planned
        /// </summary>
        internal bool _changingLanes = false;
        /// <summary>
        /// The distance at which the lane change is going to be if one is planned
        /// </summary>
        protected float? _laneChangeDistance = null;
        /// <summary>
        /// If changing to the left lane is planned
        /// </summary>
        private bool _changeLaneLeftQueued = false;
        /// <summary>
        /// If changing to the left lane is planned
        /// </summary>
        protected virtual bool changeLaneLeftQueued
        {
            get => _changeLaneLeftQueued;
            set
            {
                if (value)
                {
                    _changeLaneRightQueued = false;
                }
                _changeLaneLeftQueued = value;
            }
        }
        /// <summary>
        /// If changing to the left lane is planned
        /// </summary>
        private bool _changeLaneRightQueued = false;
        /// <summary>
        /// If changing to the left lane is planned
        /// </summary>
        protected virtual bool changeLaneRightQueued 
        {
            get => _changeLaneRightQueued;
            set
            { 
                if (value)
                {
                    _changeLaneLeftQueued = false;
                }
                _changeLaneRightQueued = value;
            }
        }

        /// <summary>
        /// The travel plan of the path follower
        /// </summary>
        public Queue<RoadTravel> TravelPlan = new Queue<RoadTravel>();
        /// <summary>
        /// The event for travels being changed
        /// </summary>
        public RoadTravelChangeEventHandler TravelChanged;
        /// <summary>
        /// The side of the lane that has been selected
        /// </summary>
        private bool _laneSide;

        protected virtual void OnTravelChanged()
        {
            if (!_laneChangeDistance.HasValue && _changingLanes)
            {
                _changingLanes = false;
                changeLaneLeftQueued = false;
                changeLaneRightQueued = false;
            }
            PlanRoadChange();
            RoadTravelChangeEventHandler handler = TravelChanged;
            handler?.Invoke(this, new RoadTravelChangeEventArgs(Travel));
        }

        /// <summary>
        /// Used to auto plan the next road
        /// </summary>
        public virtual void PlanRoadChange()
        {
            RandomLaneChange(LaneChangeProbability);
            if (_laneChangeDistance == null)
            {
                TravelPlan.Enqueue(ChooseRoadBehaviour.ChoosePath(Travel.Road.EndTravels));
            }
        }

        /// <summary>
        /// Randomly change lane
        /// </summary>
        /// <param name="probability">Probability of changing lanes</param>
        protected virtual void RandomLaneChange(float probability)
        {
            if (UnityEngine.Random.value < probability)
            {
                var r = UnityEngine.Random.Range(Travel.Distance + 0.25f * Travel.Distance, 0.75f * Travel.Length);
                if (Travel.Road.LeftRoad != null && Travel.Road.RightRoad != null)
                {
                    int v = UnityEngine.Random.Range(0, 1);
                    _laneSide = v == 1;
                }
                else
                {
                    if (Travel.Road.LeftRoad != null) _laneSide = true;
                    if (Travel.Road.RightRoad != null) _laneSide = false;
                }
                //var nextTravel = new RoadTravel(Travel.Road, r);
                var plan = ChangeBehaviour.PlanRoadChange(Travel.Road, r, _laneSide ? Travel.Road.LeftRoad : Travel.Road.RightRoad);
                if (plan != null)
                {
                    Travel.Length = r;
                    TravelPlan.Enqueue(new RoadTravel(plan));
                    TravelPlan.Enqueue(plan.DestinationTravel);
                    _laneChangeDistance = r;
                }
                else _laneChangeDistance = null;
            }
        }

        /// <summary>
        /// Changes the travle to the next planned travel if any
        /// </summary>
        void NextTravel()
        {
            if (TravelPlan.Count > 0) Travel = TravelPlan.Dequeue();
        }

        /// <summary>
        /// Moves the folower
        /// </summary>
        /// <param name="pos">New position</param>
        /// <param name="rotation">New rotation</param>
        protected virtual void Move(Vector3 pos, Quaternion rotation)
        {
            transform.position = pos;
            transform.rotation = rotation;
        }

        /// <summary>
        /// Plan a lane change
        /// </summary>
        /// <param name="left">Direction</param>
        protected virtual void LaneChange(bool left)
        {
            if (_laneChangeDistance != null)
            {
                if (_travel.Distance >= _laneChangeDistance)
                {
                    if (left) QueueChangeLaneLeft();
                    else QueueChangeLaneRight();
                    _laneChangeDistance = null;
                }
            }
        }

        void Start()
        {
            if (InitialRoad == null) InitialRoad = GetComponent<PathRoadLayout>();
            Travel = new RoadTravel(InitialRoad);
        }

        /// <summary>
        /// Updates the folowers position and rotation
        /// </summary>
        public virtual void BasicUpdate()
        {
            if (Travel != null)
            {
                var r = Travel.Road;
                //if (changeLaneRightQueued && !_changingLanes)
                //{
                //    if (r.RightRoad != null)
                //    {
                //        //var c = ChangeBehaviour.PlanRoadChange(Travel, r.RightRoad);
                //        if (c != null)
                //        {
                //            //Travel = new RoadTravel(c);
                //            _changingLanes = true;
                //            //changeLaneRightQueued = false;
                //        }
                //    }
                //}
                //if (changeLaneLeftQueued && !_changingLanes)
                //{
                //    if (r.LeftRoad != null)
                //    {
                //        var c = ChangeBehaviour.PlanRoadChange(Travel, r.LeftRoad);
                //        if (c != null)
                //        {
                //            //Travel = new RoadTravel(c);
                //            _changingLanes = true;
                //            //changeLaneLeftQueued = false;
                //        }
                //    }
                //}
                LaneChange(_laneSide);

                if (!Travel.MoveAtSpeed(speed))
                {
                    if (Travel.Road.EndTravels.Count() == 0)
                    {
                        return;
                    }
                    _laneChangeDistance = null;
                    var oldDistance = Travel.Distance - Travel.Length;
                    NextTravel();
                    Travel.Distance += oldDistance;
                }
                Move(Travel.CurrentPoint + PositionOffset, Quaternion.Euler(Travel.CurrentRotation.eulerAngles + RotationOffset));
            }
        }

        void Update()
        {
            BasicUpdate();
        }

        public void QueueChangeLaneRight()
        {
            changeLaneRightQueued = true;
        }

        public void QueueChangeLaneLeft()
        {
            changeLaneLeftQueued = true;
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            Travel.SetClosestDistanceAlongPath(transform.position);
        }
    }
}
