using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScale : MonoBehaviour
{
    // Start is called before the first frame update
    bool StartScale = false;
    GameObject Layout;
    void Start()
    {
        Layout = GameObject.FindGameObjectWithTag("coucou");
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ExpandUI()
    {
        StartCoroutine(MoveToPosition(458, .5f));
    }

    public void RetracUI()
    {
        StartCoroutine(MoveToPosition(170, .5f));
    }

    public IEnumerator MoveToPosition(float NewWidth, float timeToMove)
    {
        var currentWidth = GetComponent<RectTransform>().sizeDelta.x;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            float NewScale = Mathf.Lerp(GetComponent<RectTransform>().sizeDelta.x, NewWidth, t);
            GetComponent<RectTransform>().sizeDelta = new Vector2(NewScale, GetComponent<RectTransform>().sizeDelta.y);
            LayoutRebuilder.ForceRebuildLayoutImmediate(Layout.GetComponent<RectTransform>());
            yield return null;
        }
    }

    public void OnEditorClick()
    {
        StartScale = true;
    }

}
