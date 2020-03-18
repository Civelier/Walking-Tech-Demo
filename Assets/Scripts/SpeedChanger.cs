using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class SpeedChanger
    {
        public readonly float TransitionSeconds;
        private float _remainingSeconds;

        public float Gain => 1.0f - (_remainingSeconds / TransitionSeconds);

        public SpeedChanger(float transitionSeconds)
        {
            TransitionSeconds = transitionSeconds;
            Reset();
        }

        public void Reset()
        {
            _remainingSeconds = TransitionSeconds;
        }

        public float? GetGain()
        {
            _remainingSeconds -= Time.deltaTime;
            if (Gain >= 0) return Gain;
            return null;
        }
    }
}
