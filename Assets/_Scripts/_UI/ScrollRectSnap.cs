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
    public int CurrentSelectedSongIndex;

    // Start is called before the first frame update
    void Start()
    {
        for(int  i = 0; i <= Content.childCount - 1 ; i++ )
        {
            Song.Add(Content.GetChild(i).GetComponent<RectTransform>());
            //Content.GetChild(i).Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Song No " + i;
        }
        SCN.PreviousItem();

    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetButtonDown("LeftKa"))
        {
            //SCN.PreviousItem();
            if(CurrentSelectedSongIndex > 0)
            {
                StartCoroutine(WaitForRetraclOnSwitch("Previous"));
            }
        }

        if (Input.GetButtonDown("RightKa"))
        {
            if(CurrentSelectedSongIndex < Song.Count - 1)
            {
                StartCoroutine(WaitForRetraclOnSwitch("Next"));
            }
            
        }

        if(Input.GetButtonDown("RightDon"))
        {
            Debug.Log("select");
            CurrentSelectedSong.GetComponent<songUI>().OnSongSelected();
        }

        if (Input.GetButtonDown("LeftDon"))
        {
            Debug.Log("cancel");
        }
    }

    public void OnSongSelected(int songSelected)
    {
        
        if(CurrentSelectedSong)
        {
            StartCoroutine(WaitForRetract(.03f, CurrentSelectedSong, Song[songSelected].gameObject, songSelected));
            Debug.Log("Current Selected poppie" + Song[songSelected].GetComponentInChildren<TextMeshProUGUI>().text + "index is = " + songSelected.ToString());
        }
        else
        {
            Debug.Log("Current Selected" + Song[songSelected].GetComponentInChildren<TextMeshProUGUI>().text);
            CurrentSelectedSong = Song[songSelected].gameObject;
            CurrentSelectedSong.GetComponent<songUI>().ExpandUI();
            CurrentSelectedSongIndex = songSelected;
        }
        
    }

    public IEnumerator WaitForRetract(float timeToWait, GameObject OldSong, GameObject newSong, int songIndex)
    {
        OldSong.GetComponent<songUI>().RetracUI();
        yield return new WaitForSeconds(timeToWait);
        newSong.GetComponent<songUI>().ExpandUI();
        CurrentSelectedSong = newSong;
        CurrentSelectedSongIndex = songIndex;
    }


    public IEnumerator WaitForRetraclOnSwitch(string Case)
    {
        if(CurrentSelectedSong)
        {
            CurrentSelectedSong.GetComponent<songUI>().RetracUI();
        }
        yield return new WaitForSeconds(.3f);

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
