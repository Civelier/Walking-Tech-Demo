using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class PlayerRaycastDetector : MonoBehaviour
{
    public Camera CurrentCamera;
    public Collider Collider;
    public MeshRenderer Renderer;
    public bool Hit = false;
    // Start is called before the first frame update
    void Start()
    {
        CurrentCamera = Camera.main;
        if (Collider == null) Collider = GetComponent<Collider>();
        if (Renderer == null) Renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(CurrentCamera.ScreenPointToRay(new Vector3(CurrentCamera.pixelWidth / 2.0f, CurrentCamera.pixelHeight / 2.0f)), out RaycastHit hit))
        {
            if (hit.collider == Collider)
            {
                Hit = true;
            }
            else Hit = false;
        }
        else Hit = false;

        if (Hit) Renderer.material.SetColor("_Color", Color.red);
        else Renderer.material.SetColor("_Color", Color.blue);
    }
}
