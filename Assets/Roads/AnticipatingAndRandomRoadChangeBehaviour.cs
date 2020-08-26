using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roads
{
    public class AnticipatingAndRandomRoadChangeBehaviour : AdvancedRoadChangeBehaviour
    {
        void Start()
        {
            if (CarMove == null) CarMove = GetComponent<CarMovement>();
            CarMove.Car.TriggerStayed += OnTriggerStayed;
        }

        public override bool IsPossible(RoadTravel travel, Road destination)
        {
            return RoadChangeBehaviour.IsPossible(travel, destination);
        }

        public override RoadChangePath PlanRoadChange(Road initial, float distance, Road destination)
        {
            return RoadChangeBehaviour.PlanRoadChange(initial, distance, destination);
        }

        public override RoadChangePath PlanRoadChange(RoadTravel travel, Road destination)
        {
            return RoadChangeBehaviour.PlanRoadChange(travel, destination);
        }
    }
}
