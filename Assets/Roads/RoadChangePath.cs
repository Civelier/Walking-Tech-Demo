using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public class RoadChangePath : Road
    {
        public RoadTravel InitialTravel;
        public RoadTravel DestinationTravel;
        GameObject _user;

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

        public override IEnumerable<RoadTravel> EndTravels => new[]{ DestinationTravel };

        public override void Exitted(GameObject user)
        {
            InitialTravel.Road.Exitted(user);
            Destroy(gameObject);
        }

        public override void Entered(GameObject user)
        {
            InitialTravel.Road.Entered(user);
            _user = user;
        }

        public override bool ContainsUser(GameObject user)
        {
            return user == _user;
        }
    }
}
