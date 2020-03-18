using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
[RequireComponent(typeof(MeshRenderer))]
public class SidewalkBuilder : MonoBehaviour
{
    public Vector2Int Tiles = new Vector2Int(1, 1);
    public Vector3 TileScale = new Vector3(1.6f, 0.3f, 1.75f);
    public MeshRenderer Renderer;

    Vector3 GetScale()
    {
        var output = TileScale;
        output.x *= Tiles.x;
        output.z *= Tiles.y;
        return output;
    }

    void UpdateScale()
    {
        transform.localScale = GetScale();
        Renderer.material.mainTextureScale = Tiles;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Renderer == null) Renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        UpdateScale();
#endif
    }
}
