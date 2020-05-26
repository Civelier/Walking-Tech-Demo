using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class GameSettings
    {
        public GameOptions Options;

        private float _gameVolume;
        public float GameVolume
        {
            get => _gameVolume;
            set
            {
                _gameVolume = value;
                GameVolumeChanged.Invoke();
            }
        }

        private float _sfxVolume;

        public float SFXVolume
        {
            get => _sfxVolume;
            set 
            { 
                _sfxVolume = value;
                SFXVolumeChanged.Invoke();
            }
        }

        public readonly UnityEvent GameVolumeChanged = new UnityEvent();
        public readonly UnityEvent SFXVolumeChanged = new UnityEvent();

        private static GameSettings _instance = new GameSettings();
        public static GameSettings Instance => _instance;
    }
}
