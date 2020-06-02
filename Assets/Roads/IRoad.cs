using PathCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public interface IRoad
    {
        PathCreator Path { get; }
        IEnumerable<RoadTravel> EndTravels { get; }
        void Exitted(GameObject user);
        void Entered(GameObject user);

    }
}
