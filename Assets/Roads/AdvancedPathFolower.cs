using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public class AdvancedPathFolower : MonoBehaviour
    {
        private PathRoadLayout Road;
        public PathRoadLayout InitialRoad;
        public float speed = 5;
        float distanceTravelled;
        public IChooseRoadBehaviour Behaviour = new RandomPathBehaviour();

        void Start()
        {
            if (InitialRoad == null) InitialRoad = GetComponent<PathRoadLayout>();
            Road = InitialRoad;
            Road.ThisPath.pathUpdated += OnPathChanged;
        }

        void Update()
        {
            if (Road != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                if (distanceTravelled > Road.ThisPath.path.length)
                {
                    if (Road.Head.OutgoingRoads.Count == 0)
                    {
                        return;
                    }
                    var lastLength = Road.ThisPath.path.length;
                    Road = Behaviour.ChoosePath(Road.Head.OutgoingRoads);
                    
                    distanceTravelled -= lastLength;
                }
                transform.position = Road.ThisPath.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
                transform.rotation = Road.ThisPath.path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            distanceTravelled = Road.ThisPath.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}
