using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.Extensions;

public class ScrollRectSnap : MonoBehaviour
{

    public RectTransform Panel;
    public List<RectTransform> Song;
    public RectTransform Center;
    public RectTransform Content;
    public ContentScrollSnapHorizontal SCN;

    public float[] distance;
    private bool isDragging = false;
    private int songDistance;
    private int minSongName;
    // Start is called before the first frame update
    void Start()
    {
        for(int  i = 0; i <= Content.childCount - 1 ; i++ )
        {
            Song.Add(Content.GetChild(i).GetComponent<RectTransform>());
            Content.GetChild(i).Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Song No " + i;
        }

    }

    // Update is called once per frame
    void Update()
    {/*
        for (int i = 0; i < Song.Count - 1; i++)
        {
            distance[i] = Mathf.Abs(Center.transform.position.x - Song[i].transform.position.x);
        }

        float minDistance = Mathf.Min(distance);

        for (int a = 0; a < Song.Count - 1; a++)
        {
            if (minDistance == distance[a])
            {
                minSongName = a;
            }
        }

        if(!isDragging)
        {
            LerpToSong(minSongName * -songDistance);
        }
        */

        if(Input.GetButtonDown("LeftDon"))
        {
            SCN.PreviousItem();
        }

        if (Input.GetButtonDown("RightDon"))
        {
            SCN.NextItem();
        }
    }

    void LerpToSong(int Position)
    {
        float newX = Mathf.Lerp(Panel.anchoredPosition.x, Position, Time.deltaTime * 10);
        Vector2 NewPosition = new Vector2(newX, Panel.anchoredPosition.y);
        
        Panel.anchoredPosition = NewPosition;
    }

    public void onStartDragging()
    {
        isDragging = true;
    }

    public void onStopDragging()
    {
        isDragging = false;
    }
}
