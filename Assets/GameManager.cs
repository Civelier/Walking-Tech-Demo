using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameSettings.Instance.Options = new GameOptions();
        GameSettings.Instance.Options.Load();
    }
}
