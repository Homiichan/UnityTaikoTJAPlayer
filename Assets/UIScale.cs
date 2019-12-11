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

    public HorizontalLayoutGroup HG;

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

        HG = GameObject.FindObjectOfType<HorizontalLayoutGroup>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ExpandUI()
    {
        
        Songbackground.sprite = SelectedSprite;
        GetComponent<RectTransform>().sizeDelta = new Vector2(600, GetComponent<RectTransform>().sizeDelta.y);
        HG.SetLayoutHorizontal();
        HG.CalculateLayoutInputHorizontal();
    }

    public void RetracUI()
    {
        
        Songbackground.sprite = NotSelectedSprite;
        GetComponent<RectTransform>().sizeDelta = new Vector2(180, GetComponent<RectTransform>().sizeDelta.y);
        HG.SetLayoutHorizontal();
        HG.CalculateLayoutInputHorizontal();
    }

    public IEnumerator MoveToPosition(float NewWidth, float timeToMove)
    {
        yield return new WaitForSeconds(timeToMove);
        CS.UpdateLayout();
    }

    public void OnEditorClick()
    {
        StartScale = true;
    }

    public IEnumerator WaitBegin()
    {
        yield return new WaitForSeconds(1);
        HG.SetLayoutHorizontal();
        HG.CalculateLayoutInputHorizontal();
    }

}
