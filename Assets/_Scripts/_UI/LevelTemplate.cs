using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTemplate : MonoBehaviour
{
    
    public TaikoSongContainer SongInside;

    public Text text = null;

    public Dropdown Drop = null;
    List<string> m_DropOptions = new List<string> {};
    public TaikoGameInstance TGI = null;
    public Taiko_Difficulty CurrentSelectedDif = Taiko_Difficulty.Dif_Oni;
    public int SongIndex;
    GameObject Center;
    SongLoader SL;
    bool TimeStart = false;
    int x;
    int y;
    float distance;

    // Start is called before the first frame update
    void Start()
    {
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        SL = GameObject.FindObjectOfType<SongLoader>();
        Drop.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(Drop);
        });
        Center = GameObject.FindGameObjectWithTag("UICenter");
        
    }

    // Update is called once per frame
    void Update()
    {
        x = Screen.width / 2;
        y = Screen.width / 2;
        distance = Vector3.Distance(Center.transform.position, transform.position);

        if (distance <= 30)
        {
            //Debug.Log(SongInside.TitleName);
            if(!TimeStart)
            {
                StartCoroutine(test());
                TimeStart = true;
            }
        }
        else
        {
            StopCoroutine(test());
            TimeStart = false;
        }
    }

    public void AssignStruc(TaikoSongContainer strucToSet, int CurrentSongIndex)
    {
        SongInside = strucToSet;

        text.text = SongInside.TitleName;
        foreach(TaikoSongStruc currentstruc in SongInside.AllSongDifficulty)
        {
            m_DropOptions.Add(currentstruc.Difficulty.ToString());
        }
        Drop.AddOptions(m_DropOptions);
        SongIndex = CurrentSongIndex;
    }

    public void OnClickedButton()
    {
        TGI.FindSongAndDifficulty(SongIndex, CurrentSelectedDif);
        SceneManager.LoadScene(1);
    }


    void DropdownValueChanged(Dropdown change)
    {
        Debug.Log(Drop.options[change.value].text);
        CurrentSelectedDif = ParseDifficultyByString(Drop.options[change.value].text);
        Debug.Log("SelectedDif = " + CurrentSelectedDif);
        
    }

    Taiko_Difficulty ParseDifficultyByString(string DifText)
    {
        switch(DifText)
        {
            case "Dif_Easy":
                return Taiko_Difficulty.Dif_Easy;

            case "Dif_Normal":
                return Taiko_Difficulty.Dif_Normal;
            case "Dif_Hard":
                return Taiko_Difficulty.Dif_Hard;
            case "Dif_Oni":
                return Taiko_Difficulty.Dif_Oni;
            case "Dif_Edit":
                return Taiko_Difficulty.Dif_Edit;
        }
        return Taiko_Difficulty.Dif_Oni;
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(3);
        if (distance <= 30)
            SL.OnSongSwitch(SongInside);
    }


}