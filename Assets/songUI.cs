using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class songUI : MonoBehaviour
{
    // Start is called before the first frame update//
    bool StartScale = false;
    GameObject Layout;

    public ContentScrollSnapHorizontal CS;

    public Sprite SelectedSprite;
    public Sprite NotSelectedSprite;


    public Image Songbackground;

    public TaikoSongContainer currentSong;

    public GameObject DifficultyCanvas;
    Animator m_Animator;

    public List<Image> FadeComp;
    void Start()
    {
        Layout = GameObject.FindGameObjectWithTag("coucou");
        FadeComp = new List<Image>();

        for (int i = 0; i <= (gameObject.transform.childCount - 1); i++)
        {
            if(gameObject.transform.GetChild(i).tag == "SongBackground")
            {
                Songbackground = gameObject.transform.GetChild(i).GetComponent<Image>();
            }
        }

        for (int i = 0; i <= (DifficultyCanvas.transform.childCount - 1); i++)
        {
            if(DifficultyCanvas.transform.GetChild(i).gameObject.GetComponent<songDifficulty>())
            {
                songDifficulty tmpSongUI = DifficultyCanvas.transform.GetChild(i).gameObject.GetComponent<songDifficulty>();
                foreach(Image tmpImage in tmpSongUI.Stars)
                {
                    FadeComp.Add(tmpImage);
                }
            }
            else
            {
                if(DifficultyCanvas.transform.GetChild(i).tag == "FadeLevelComp")
                {
                    FadeComp.Add(DifficultyCanvas.transform.GetChild(i).GetComponent<Image>());
                }
            }
        }
        m_Animator = gameObject.GetComponent<Animator>();
        
    }


    public void ExpandUI()
    {
        
        Songbackground.sprite = SelectedSprite;
        m_Animator.SetBool("Expand", true);
    }

    public void RetracUI()
    {
        
        
        m_Animator.SetBool("Expand", false);
        DifficultyCanvas.SetActive(false);
        m_Animator.SetBool("Selected", false);
    }

    public void OnSongSelected()
    {
        m_Animator.SetBool("Selected", true);
        DifficultyCanvas.SetActive(false);
    }


    public void OnAnimFinished(string animPlayed)
    {
        //Debug.Log(animPlayed);
        switch (animPlayed)
        {
            case "Retract":
                Songbackground.sprite = NotSelectedSprite;
                break;
        }
    }
    public void OnEditorClick()
    {
        StartScale = true;
    }

    public void OnMidAnim()
    {
        DifficultyCanvas.SetActive(true);
        StartCoroutine(LerpAlpha(0, 1, .10f));
        
    }


    public IEnumerator LerpAlpha(float currentAlpha, float newAlpha, float timeToMove)
    {
        var currentPos = currentAlpha;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            foreach(Image tmpImage in FadeComp)
            {
                tmpImage.color = new Color(tmpImage.color.r, tmpImage.color.g, tmpImage.color.b, Mathf.Lerp(currentAlpha, newAlpha, t));
            }
            yield return null;
        }
    }


}
