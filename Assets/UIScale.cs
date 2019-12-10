using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIScale : MonoBehaviour
{
    // Start is called before the first frame update
    bool StartScale = false;
    GameObject Layout;

    public ContentScrollSnapHorizontal CS;

    public Sprite SelectedSprite;
    public Sprite NotSelectedSprite;

    public Image Songbackground;
    void Start()
    {
        Layout = GameObject.FindGameObjectWithTag("coucou");

        for (int i = 0; i <= (gameObject.transform.childCount - 1); i++)
        {
            if(gameObject.transform.GetChild(i).tag == "SongBackground")
            {
                Songbackground = gameObject.transform.GetChild(i).GetComponent<Image>();
            }
        }

        CS = GameObject.FindObjectOfType<ContentScrollSnapHorizontal>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ExpandUI()
    {
        
        Songbackground.sprite = SelectedSprite;
        GetComponent<RectTransform>().sizeDelta = new Vector2(600, GetComponent<RectTransform>().sizeDelta.y);
        //Layout.GetComponent<HorizontalLayoutGroup>().spacing = 120;
        //StartCoroutine(MoveToPosition(458, 3f));
        //CS.UpdateLayout();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(Layout.GetComponent<RectTransform>());
    }

    public void RetracUI()
    {
        
        Songbackground.sprite = NotSelectedSprite;
        GetComponent<RectTransform>().sizeDelta = new Vector2(180, GetComponent<RectTransform>().sizeDelta.y);
        //Layout.GetComponent<HorizontalLayoutGroup>().spacing = 30;
        //StartCoroutine(MoveToPosition(170, 3));
        //CS.UpdateLayout();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(Layout.GetComponent<RectTransform>());
    }

    public IEnumerator MoveToPosition(float NewWidth, float timeToMove)
    {
        /*
        var currentWidth = GetComponent<RectTransform>().sizeDelta.x;
        var t = 0f;
        while (t < 1)
        {
            
            t += Time.deltaTime / timeToMove;
            float NewScale = Mathf.Lerp(GetComponent<RectTransform>().sizeDelta.x, NewWidth, t);
            GetComponent<RectTransform>().sizeDelta = new Vector2(NewScale, GetComponent<RectTransform>().sizeDelta.y);
            LayoutRebuilder.ForceRebuildLayoutImmediate(Layout.GetComponent<RectTransform>());
            
            CS.UpdateLayout();
            yield return null;
        }
    */

        yield return new WaitForSeconds(timeToMove);
        CS.UpdateLayout();
    }

    public void OnEditorClick()
    {
        StartScale = true;
    }

}
