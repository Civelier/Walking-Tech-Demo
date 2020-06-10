using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roads
{
    public interface IRoadChangeBehaviour
    {
        RoadChangePath PlanRoadChange(RoadTravel travel, Road destination);
        float MaxSpeedPercent { get; }
        bool IsPossible(RoadTravel travel, Road destination);
        RoadChangePath PlanRoadChange(Road initial, float distance, Road destination);
    }
}
