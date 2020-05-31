using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public class RandomPathBehaviour : IChooseRoadBehaviour
    {
        System.Random _random = new System.Random();
        public PathRoadLayout ChoosePath(IEnumerable<PathRoadLayout> roads)
        {
            int i = _random.Next(roads.Count());
            Debug.Log($"Random = {i}");
            return roads.ElementAtOrDefault(i);
        }
    }
}
