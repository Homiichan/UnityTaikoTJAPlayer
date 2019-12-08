using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScale : MonoBehaviour
{
    // Start is called before the first frame update
    bool StartScale = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(StartScale)
        {
            float NewScale = Mathf.Lerp(GetComponent<RectTransform>().sizeDelta.x, 250, Time.deltaTime * 3);
            GetComponent<RectTransform>().sizeDelta = new Vector2(NewScale, GetComponent<RectTransform>().sizeDelta.y);
            Canvas.ForceUpdateCanvases();
            GameObject Layout = GameObject.FindGameObjectWithTag("coucou");
            LayoutRebuilder.ForceRebuildLayoutImmediate(Layout.GetComponent<RectTransform>());
        }
    }

    public void OnEditorClick()
    {
        StartScale = true;
    }

}
