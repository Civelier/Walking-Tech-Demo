using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HealthBar : MonoBehaviour
{
    public Image WhiteFill;
    public Image RedFill;
    public Image WhiteOutline;
    public Image RedOutline;

    void SetSize(Image image, RectTransform.Edge edge, float factor)
    {
        image.rectTransform.SetInsetAndSizeFromParentEdge(edge, 3, factor * 344);
    }

    public float Stamina;
    public float Health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetSize(WhiteFill, RectTransform.Edge.Left, Stamina * (1 - Health));
        SetSize(RedFill, RectTransform.Edge.Right, Health);
    }
}
