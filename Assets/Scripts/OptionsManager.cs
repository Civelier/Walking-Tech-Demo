using Assets.FieldAttributes;
using Assets.FieldGridElements;
using Assets.GameMenu;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using System.IO;

public class OptionsManager : MonoBehaviour
{
    public FieldGrid Grid;
    public FieldDisplayer<GameOptions> Displayer;
    public SubMenu Menu;

    // Start is called before the first frame update
    void Start()
    {
        Displayer = new FieldDisplayer<GameOptions>(Grid, GameSettings.Instance.Options);
        Displayer.Display();
        Menu.LostFocus.AddListener(GameSettings.Instance.Options.Save);
    }
}
