using Roads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityTests
{
    public class VehiculeTests : UnityTest
    {
        public GameObject VehiculePrefab;

#if UNITY_EDITOR
        void Start()
        {
            AddTest(TestCarMovementInfoEquals, "Test CarMovementInfo Equals");
            AddTest(TestCarMovementInfoNotEquals, "Test CarMovementInfo not Equals");
            AddTest(TestCarMovementInfoDistance, "Test CarMovementInfo distance");
            AddTest(TestCarMovementInfoBehind, "Test CarMovementInfo behind");
            AddTest(TestCarMovementInfoRelativeDistance, "Test CarMovementInfo relative distance");
            AddTest(TestLineCross, "Test line cross");
            RunTests();
        }
#endif

        IEnumerator<bool?> TestLineCross()
        {
            var a1 = new Vector2(1, 1);
            var a2 = new Vector2(6, 5);
            var b1 = new Vector2(1, 3);
            var b2 = new Vector2(8, 3);
            var success = PathUtilities.DoVectorsCross(a1, a2, b1, b2, out Vector2 result);
            success.ShouldBeTrue();
            result.ShouldBeEqual(new Vector2(3.5f, 3));
            yield return true;
        }

        IEnumerator<bool?> TestCarMovementInfoEquals()
        {
            var car1 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car1");
            var car2 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car2");
            var r1 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path1");
            var r2 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path2");

            car1.InitialRoad = r1;
            car2.InitialRoad = r1;

            r1.Tail = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 tail");
            r1.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 head");
            r1.Head.AddAsOutgoing(r2);
            r2.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R2 head");

            var c1 = new CarMovementInfo(car1, car2);
            var c2 = new CarMovementInfo(car1, car2);
            c1.ShouldBeEqual(c2);
            yield return true;
        }

        IEnumerator<bool?> TestCarMovementInfoNotEquals()
        {
            var car1 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car1");
            var car2 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car2");
            var car3 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car3");
            var r1 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path1");
            var r2 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path2");

            car1.InitialRoad = r1;
            car2.InitialRoad = r1;
            car3.InitialRoad = r2;

            r1.Tail = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 tail");
            r1.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 head");
            r1.Head.AddAsOutgoing(r2);
            r2.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R2 head");

            var c1 = new CarMovementInfo(car1, car2);
            var c2 = new CarMovementInfo(car1, car3);
            c1.ShouldNotBeEqual(c2);
            yield return true;
        }

        IEnumerator<bool?> TestCarMovementInfoDistance()
        {
            var car1 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car1");
            var car2 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car2");
            var car3 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car3");
            var r1 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path1");
            var r2 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path2");


            car1.InitialRoad = r1;
            car2.InitialRoad = r1;
            car3.InitialRoad = r2;

            r1.Tail = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 tail");
            r1.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 head");

            r1.Head.AddAsOutgoing(r2);
            r2.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R2 head");

            r1.Path.bezierPath.AddSegmentToEnd(new Vector3(1, 0, 0));
            r1.Path.bezierPath.AddSegmentToEnd(new Vector3(0, 0, 0));
            r1.Path.bezierPath.DeleteSegment(1);
            r1.Path.bezierPath.DeleteSegment(0);

            r2.Path.bezierPath.AddSegmentToEnd(new Vector3(2, 0, 0));
            r2.Path.bezierPath.AddSegmentToEnd(new Vector3(1, 0, 0));
            r2.Path.bezierPath.DeleteSegment(1);
            r2.Path.bezierPath.DeleteSegment(0);

            yield return null;

            car1.Travel = new RoadTravel(r1);
            car2.Travel = new RoadTravel(r1, 0.5f);
            car3.Travel = new RoadTravel(r2);

            var c1 = new CarMovementInfo(car1, car2);
            var c2 = new CarMovementInfo(car2, car3);

            c1.LastActualDistance.ShouldBeEqual(0.5f);
            c2.LastActualDistance.ShouldBeEqual(0.5f);
            yield return true;
        }

        IEnumerator<bool?> TestCarMovementInfoBehind()
        {
            var car1 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car1");
            var car2 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car2");
            var car3 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car3");
            var car4 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car4");
            var r1 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path1");
            var r2 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path2");
            var r3 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path3");

            car1.InitialRoad = r1;
            car2.InitialRoad = r1;
            car3.InitialRoad = r2;
            car4.InitialRoad = r3;

            r1.Tail = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 tail");
            r1.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 head");

            r1.Head.AddAsOutgoing(r2);
            r2.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R2 head");

            r1.Path.bezierPath.AddSegmentToEnd(new Vector3(1, 0, 0));
            r1.Path.bezierPath.AddSegmentToEnd(new Vector3(0, 0, 0));
            r1.Path.bezierPath.DeleteSegment(1);
            r1.Path.bezierPath.DeleteSegment(0);

            r2.Path.bezierPath.AddSegmentToEnd(new Vector3(2, 0, 0));
            r2.Path.bezierPath.AddSegmentToEnd(new Vector3(1, 0, 0));
            r2.Path.bezierPath.DeleteSegment(1);
            r2.Path.bezierPath.DeleteSegment(0);

            r3.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R3 head");
            r3.Tail = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R3 tail");

            yield return null;

            car1.Travel = new RoadTravel(r1);
            car2.Travel = new RoadTravel(r1, 0.5f);
            car3.Travel = new RoadTravel(r2);
            car4.Travel = new RoadTravel(r3);

            var c1 = new CarMovementInfo(car2, car1);
            var c2 = new CarMovementInfo(car2, car3);
            var c3 = new CarMovementInfo(car1, car4);

            c1.IsOnPath.ShouldBeFalse();
            c1.IsInFront.ShouldBeFalse();
            c2.IsOnPath.ShouldBeTrue();
            c2.IsInFront.ShouldBeTrue();
            c2.LastActualDistance.ShouldBeEqual(0.5f);
            c3.IsOnPath.ShouldBeFalse();
            c3.IsValid.ShouldBeFalse();
            yield return true;
        }

        IEnumerator<bool?> TestCarMovementInfoRelativeDistance()
        {
            var car1 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car1");
            var car2 = SafeInstantiate<CarMovement>(VehiculePrefab, "Car2");
            var r1 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path1");
            var r2 = SafeInstantiate<PathRoadLayout>(PathFactory.Instance.PathPrefab, "path2");

            car1.InitialRoad = r1;
            car2.InitialRoad = r1;

            r1.Tail = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 tail");
            r1.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R1 head");

            r1.Head.AddAsOutgoing(r2);
            r2.Head = SafeInstantiate<PathNode>(PathFactory.Instance.NodePrefab, "R2 head");

            r1.Path.bezierPath.AddSegmentToEnd(new Vector3(1, 0, 0));
            r1.Path.bezierPath.AddSegmentToEnd(new Vector3(0, 0, 0));
            r1.Path.bezierPath.DeleteSegment(1);
            r1.Path.bezierPath.DeleteSegment(0);

            r2.Path.bezierPath.AddSegmentToEnd(new Vector3(2, 0, 0));
            r2.Path.bezierPath.AddSegmentToEnd(new Vector3(1, 0, 0));
            r2.Path.bezierPath.DeleteSegment(1);
            r2.Path.bezierPath.DeleteSegment(0);


            yield return null;

            car1.Travel = new RoadTravel(r1, 0.5f);
            car2.Travel = new RoadTravel(r1, 1);

            car1.transform.position = new Vector3(0.5f, 0, 0);
            car2.transform.position = new Vector3(0.5f, 0, 0);
            car1.transform.Rotate(new Vector3(0, 15, 0));

            var c1 = new CarMovementInfo(car1, car2);

            var v1 = c1.RelativeDistance;
            var relPos = new Vector3(Mathf.Round(v1.x * 100) / 100, Mathf.Round(v1.y * 100) / 100, Mathf.Round(v1.z * 100) / 100);

            var v2 = new Vector3(0.5f + 0.5f * Mathf.Cos(-15 * Mathf.Deg2Rad), 0, 0.5f * Mathf.Sin(-15 * Mathf.Deg2Rad));
            var result = new Vector3(Mathf.Round(v2.x * 100) / 100, Mathf.Round(v2.y * 100) / 100, Mathf.Round(v2.z * 100) / 100);
            relPos.ShouldBeApproximately(result);
            yield return true;
        }
    }
}
