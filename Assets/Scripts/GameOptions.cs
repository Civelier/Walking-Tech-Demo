using Assets.FieldAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    [DisplayableInFieldGrid]
    public class GameOptions
    {
        [FieldAttributes.Range(0, 100)]
        public float GameVolume = 100;

        [FieldAttributes.Range(0, 100)]
        public float SFXVolume = 100;
        
        [Readonly]
        [DisplayName("Game version")]
        public Version Version = new Version(1, 0, 6);

        public void Action()
        {
            Debug.Log("Action");
        }

    }
}
