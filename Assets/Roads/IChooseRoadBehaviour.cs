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
        /// <summary>
        /// Chooses one of the <see cref="RoadTravel"/> and returns it
        /// </summary>
        /// <param name="roads"></param>
        /// <returns>A path determined by the behaviour</returns>
        RoadTravel ChoosePath(IEnumerable<RoadTravel> roads);
    }
}
