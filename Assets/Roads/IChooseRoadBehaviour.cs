using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roads
{
    public interface IChooseRoadBehaviour
    {
        RoadTravel ChoosePath(IEnumerable<RoadTravel> roads);
    }
}
