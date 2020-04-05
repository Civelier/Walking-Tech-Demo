using Assets.FieldAttributes;
using Assets.FieldGridElements;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public FieldGrid Grid;
    public GameOptions Options = new GameOptions();
    public FieldDisplayer<GameOptions> Displayer;

    // Start is called before the first frame update
    void Start()
    {
        Displayer = new FieldDisplayer<GameOptions>(Grid, Options);
        Displayer.Display();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
