using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roads
{
    public delegate void RoadTravelChangeEventHandler(object sender, RoadTravelChangeEventArgs args);
    public class RoadTravelChangeEventArgs
    {
        public readonly RoadTravel Travel;

        public RoadTravelChangeEventArgs(RoadTravel travel)
        {
            Travel = travel;
        }
    }
}
