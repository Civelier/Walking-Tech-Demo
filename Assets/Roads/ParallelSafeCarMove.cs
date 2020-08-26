using UnityEngine;

namespace Roads
{
    [ExecuteInEditMode]
    public class ParallelSafeCarMove : CarMovement
    {
        public Quaternion Rotation;
        public Vector3 Position;

        protected override void Move(Vector3 pos, Quaternion rotation)
        {
            Position = pos;
            Rotation = rotation;
        }

        void Start()
        {
            if (InitialRoad == null) InitialRoad = GetComponent<PathRoadLayout>();
            if (Body == null) Body = GetComponent<Rigidbody>();
            if (Car == null) Car = GetComponent<CarCollider>();
            if (SpeedProvider == null) SpeedProvider = GetComponent<CarSpeedProvider>();
            Travel = new RoadTravel(InitialRoad);
            SpeedProvider.Acceleration = Acceleration;
            SpeedProvider.Anticipation = Anticipation;
            SpeedProvider.Brake = Brake;
            SpeedProvider.Decceleration = Decceleration;
            SpeedProvider.MaxSpeed = MaxSpeed;
            changeLaneLeftQueued = false;
            changeLaneRightQueued = false;
            Rotation = transform.rotation;
            Position = transform.position;
        }

        // ----------- Use to migrate from CarMovement to ParallelSafeCarMove -------
        //void OnValidate()
        //{
        //    if (TryGetComponent(out CarMovement move))
        //    {
        //        PositionOffset = move.PositionOffset;
        //        RotationOffset = move.RotationOffset;
        //        InitialRoad = move.InitialRoad;
        //        speed = move.speed;
        //        ChooseRoadBehaviour = move.ChooseRoadBehaviour;
        //        ChangeBehaviour = move.ChangeBehaviour;

        //        Acceleration = move.Acceleration;
        //        Decceleration = move.Decceleration;
        //        Brake = move.Brake;
        //        MaxSpeed = move.MaxSpeed;
        //        FrontTrigger = move.FrontTrigger;
        //        FrontTriggerTooClose = move.FrontTriggerTooClose;
        //        ChangeLaneLeft = move.ChangeLaneLeft;
        //        ChangeLaneRight = move.ChangeLaneRight;
        //        Car = move.Car;
        //        Body = move.Body;
        //        SpeedProvider = move.SpeedProvider;
        //        Anticipation = move.Anticipation;

        //        if (InitialRoad == null) InitialRoad = GetComponent<PathRoadLayout>();
        //        if (Body == null) Body = GetComponent<Rigidbody>();
        //        if (Car == null) Car = GetComponent<CarCollider>();
        //        if (SpeedProvider == null) SpeedProvider = GetComponent<CarSpeedProvider>();
        //        Travel = new RoadTravel(InitialRoad);
        //        SpeedProvider.Acceleration = Acceleration;
        //        SpeedProvider.Anticipation = Anticipation;
        //        SpeedProvider.Brake = Brake;
        //        SpeedProvider.Decceleration = Decceleration;
        //        SpeedProvider.MaxSpeed = MaxSpeed;
        //        changeLaneLeftQueued = false;
        //        changeLaneRightQueued = false;
        //    }
        //}

        public void PreCalculate()
        {
            Travel.PreCalculate();
        }

        public void PostCalculate()
        {
            if (Body != null)
            {
                Body.MovePosition(Position);
                Body.MoveRotation(Rotation);
            }
            else base.Move(Position, Rotation);
        }
    }
}
