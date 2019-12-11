using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class DebugTest : MonoBehaviour
{

    public RectTransform Child;
    public HorizontalLayoutGroup HG;
    // Start is called before the first frame update
    void Start()
    {
        HG = GameObject.FindObjectOfType<HorizontalLayoutGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScaleUp()
    {
        Child.GetComponent<RectTransform>().sizeDelta = new Vector2(600, GetComponent<RectTransform>().sizeDelta.y);
        HG.SetLayoutHorizontal();
        HG.CalculateLayoutInputHorizontal();
    }

    public void ScaleDown()
    {
        Child.GetComponent<RectTransform>().sizeDelta = new Vector2(180, GetComponent<RectTransform>().sizeDelta.y);
        HG.SetLayoutHorizontal();
        HG.CalculateLayoutInputHorizontal();
    }
}
