using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTemplate : MonoBehaviour
{
    
    public TaikoSongContainer SongInside;

    public TextMeshProUGUI text = null;

    List<string> m_DropOptions = new List<string> {};
    public TaikoGameInstance TGI = null;
    public Taiko_Difficulty CurrentSelectedDif = Taiko_Difficulty.Dif_Oni;
    public int SongIndex;
    GameObject Center;
    SongLoader SL;
    bool TimeStart = false;
    public RectTransform child;
    float LerpValue = 0;
    public bool IsOpened = false;
    public bool DEbug = false;
    public float timeToMove = 1;
    public Sprite LargerBar;
    public Sprite TinyBar;

    // Start is called before the first frame update
    void Start()
    {
        TGI = GameObject.FindObjectOfType<TaikoGameInstance>();
        SL = GameObject.FindObjectOfType<SongLoader>();
        transform.GetComponentInChildren<Image>().SetNativeSize();
        child.sizeDelta = new Vector2(transform.GetComponentInChildren<RectTransform>().sizeDelta.x, 680);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(DEbug)
                switchUI();
        }
        
    }

    void switchUI()
    {
        Debug.Log("wouaaaaaaaaaa");
        if (IsOpened)
        {
            //LerpValue += Time.fixedDeltaTime;
            //child.sizeDelta = new Vector2(Mathf.Lerp(transform.GetComponentInChildren<RectTransform>().sizeDelta.x, 10, LerpValue / timeToMove), transform.GetComponentInChildren<RectTransform>().sizeDelta.y);
            transform.GetComponentInChildren<Image>().sprite = LargerBar;
            transform.GetComponentInChildren<Image>().SetNativeSize();
            child.sizeDelta = new Vector2(300, 680);
            IsOpened = false;

        }
        else
        {
            transform.GetComponentInChildren<Image>().sprite = TinyBar;
            transform.GetComponentInChildren<Image>().SetNativeSize();
            child.sizeDelta = new Vector2(transform.GetComponentInChildren<RectTransform>().sizeDelta.x, 680);
            IsOpened = true;
        }
    }
    public void OnSelected()
    {
        /*
        Animator tmpAnimator = gameObject.GetComponent<Animator>();
        tmpAnimator.SetFloat("AnimationSpeed", 1);
        tmpAnimator.Play("UIExpansion");
        */
        SL.OnSongSwitch(SongInside);
        Debug.Log(SongInside.TitleName);
    }

    public void OnDeselected()
    {
        /*
        Animator tmpAnimator = gameObject.GetComponent<Animator>();
        tmpAnimator.SetFloat("AnimationSpeed", -1);
        tmpAnimator.Play("UIExpansion");
        */
    }
    public void AssignStruc(TaikoSongContainer strucToSet, int CurrentSongIndex)
    {
        SongInside = strucToSet;

        text.text = SongInside.TitleName;
        foreach(TaikoSongStruc currentstruc in SongInside.AllSongDifficulty)
        {
            m_DropOptions.Add(currentstruc.Difficulty.ToString());
        }
        SongIndex = CurrentSongIndex;
    }

    public void OnClickedButton()
    {
        TGI.FindSongAndDifficulty(SongIndex, CurrentSelectedDif);
        SceneManager.LoadScene(1);
    }


    void DropdownValueChanged(Dropdown change)
    {
        /*
        Debug.Log(Drop.options[change.value].text);
        CurrentSelectedDif = ParseDifficultyByString(Drop.options[change.value].text);
        Debug.Log("SelectedDif = " + CurrentSelectedDif);
        */
        
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


}