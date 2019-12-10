using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.Extensions;

public class ScrollRectSnap : MonoBehaviour
{

    public List<RectTransform> Song;
    public RectTransform Content;
    public ContentScrollSnapHorizontal SCN;
    public GameObject CurrentSelectedSong;

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
    {

        if(Input.GetButtonDown("LeftDon"))
        {
            //SCN.PreviousItem();
            
            StartCoroutine(WaitForRetraclOnSwitch("Previous"));
        }

        if (Input.GetButtonDown("RightDon"))
        {
            //SCN.NextItem();
            StartCoroutine(WaitForRetraclOnSwitch("Next"));
        }
    }

    public void OnSongSelected(int songSelected)
    {
        if(CurrentSelectedSong)
        {
            StartCoroutine(WaitForRetract(.05f, CurrentSelectedSong, Song[songSelected].gameObject));
            Debug.Log("Current Selected" + Song[songSelected].GetComponentInChildren<TextMeshProUGUI>().text);
        }
        else
        {
            Debug.Log("Current Selected" + Song[songSelected].GetComponentInChildren<TextMeshProUGUI>().text);
            CurrentSelectedSong = Song[songSelected].gameObject;
            CurrentSelectedSong.GetComponent<UIScale>().ExpandUI();
        }
        
    }

    public IEnumerator WaitForRetract(float timeToWait, GameObject OldSong, GameObject newSong)
    {
        OldSong.GetComponent<UIScale>().RetracUI();
        yield return new WaitForSeconds(timeToWait);
        newSong.GetComponent<UIScale>().ExpandUI();
        CurrentSelectedSong = newSong;
    }


    public IEnumerator WaitForRetraclOnSwitch(string Case)
    {
        if(CurrentSelectedSong)
        {
            CurrentSelectedSong.GetComponent<UIScale>().RetracUI();
        }
        yield return new WaitForSeconds(.5f);

        switch (Case)
        {
            case "Next":
                SCN.NextItem();
                break;
            case "Previous":
                SCN.PreviousItem();
                break;
        }

    }

}
