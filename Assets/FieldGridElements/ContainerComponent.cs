using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.FieldGridElements
{
    public abstract class ContainerComponent : MonoBehaviour
    {
        public abstract string Name { get; set; }
        public abstract bool Visible { get; set; }
        public abstract void Refresh();
    }
}
