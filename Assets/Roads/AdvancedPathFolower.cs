﻿using PathCreation;
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
        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
        private RoadTravel _travel;
        public RoadTravel Travel
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
        public Road InitialRoad;
        public float speed = 5;
        [Range(0, 1)]
        public float LaneChangeProbability = 0.2f;
        public IChooseRoadBehaviour ChooseRoadBehaviour = new RandomPathBehaviour();
#if UNITY_EDITOR
        public BasicRoadChangeBehaviour ChangeBehaviour = new BasicRoadChangeBehaviour();
#else
        public IRoadChangeBehaviour ChangeBehaviour = new BasicRoadChangeBehaviour();
#endif
        protected float? _laneChangeDistance = null;
        private bool _changeLaneLeftQueued = false;
        private bool _changeLaneRightQueued = false;

        public RoadTravelChangeEventHandler TravelChanged;
        private bool _laneSide;

        protected virtual void OnTravelChanged()
        {
            RandomLaneChange(LaneChangeProbability);
            RoadTravelChangeEventHandler handler = TravelChanged;
            handler?.Invoke(this, new RoadTravelChangeEventArgs(Travel));
        }

        void RandomLaneChange(float probability)
        {
            if (UnityEngine.Random.value <= probability)
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
                if (ChangeBehaviour.IsPossible(new RoadTravel(Travel.Road, r), _laneSide ? Travel.Road.LeftRoad : Travel.Road.RightRoad))
                {
                    _laneChangeDistance = r;
                }
                else _laneChangeDistance = null;
            }
        }

        void LaneChange(bool left)
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

        public void BasicUpdate()
        {
            if (Travel != null)
            {
                var r = Travel.Road;
                if (_changeLaneRightQueued)
                {
                    if (r.RightRoad != null)
                    {
                        var c = ChangeBehaviour.PlanRoadChange(Travel, r.RightRoad);
                        if (c != null)
                        {
                            Travel = new RoadTravel(c);
                            _changeLaneRightQueued = false;
                        }
                    }
                }
                if (_changeLaneLeftQueued)
                {
                    if (r.LeftRoad != null)
                    {
                        var c = ChangeBehaviour.PlanRoadChange(Travel, r.LeftRoad);
                        if (c != null)
                        {
                            Travel = new RoadTravel(c);
                            _changeLaneLeftQueued = false;
                        }
                    }
                }
                LaneChange(_laneSide);

                if (!Travel.MoveAtSpeed(speed))
                {
                    if (Travel.Road.EndTravels.Count() == 0)
                    {
                        return;
                    }
                    var oldDistance = Travel.Distance - Travel.Length;
                    Travel = ChooseRoadBehaviour.ChoosePath(Travel.Road.EndTravels);
                    Travel.Distance += oldDistance;
                }
                transform.position = Travel.CurentPoint + PositionOffset;
                transform.rotation = Quaternion.Euler(Travel.CurentRotation.eulerAngles + RotationOffset);
            }
        }

        void Update()
        {
            BasicUpdate();
        }

        public void QueueChangeLaneRight()
        {
            _changeLaneRightQueued = true;
            _changeLaneLeftQueued = false;
        }

        public void QueueChangeLaneLeft()
        {
            _changeLaneLeftQueued = true;
            _changeLaneRightQueued = false;
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            Travel.SetClosestDistanceAlongPath(transform.position);
        }
    }
}