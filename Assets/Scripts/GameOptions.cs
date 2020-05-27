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
    public class GameOptions
    {
        [FieldAttributes.Range(0, 100)]
        public float GameVolume
        {
            get => GameSettings.Instance.GameVolume;
            set => GameSettings.Instance.GameVolume = value;
        }

        [FieldAttributes.Range(0, 100)]
        public float SFXVolume
        {
            get => GameSettings.Instance.SFXVolume;
            set => GameSettings.Instance.SFXVolume = value;
        }

        [Readonly]
        [FieldAttributes.DisplayName("Game version")]
        public Version Version = new Version(1, 0, 7);

        [Readonly]
        public Version OptionsVersion = new Version(1, 0);

        public GameOptions()
        {
            GameVolume = 100;
            SFXVolume = 100;
        }
        
        public void Action()
        {
            Debug.Log("Action");
        }

        public void BackToDefaults()
        {
            GameVolume = 100;
            SFXVolume = 100;
            Save();
        }
        
        public void Save()
        {
            PlayerPrefs.SetFloat("Game volume", GameVolume);
            PlayerPrefs.SetFloat("SFX volume", SFXVolume);
            PlayerPrefs.SetInt("Major options version", OptionsVersion.Major);
            PlayerPrefs.SetInt("Minor options version", OptionsVersion.Minor);
            PlayerPrefs.Save();
        }

        public void Load()
        {
            if (!PlayerPrefs.HasKey("Major options version"))
            {
                PlayerPrefs.DeleteAll();
                Save();
            }
            else
            {
                if (!OptionsVersion.Equals(new Version(PlayerPrefs.GetInt("Major options version"), PlayerPrefs.GetInt("Minor options version"))))
                {
                    PlayerPrefs.DeleteAll();
                    BackToDefaults();
                }
                GameVolume = PlayerPrefs.GetFloat("Game volume");
                SFXVolume = PlayerPrefs.GetFloat("SFX volume");
            }
        }
    }
}
