using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public enum TaikoCharAction
{
    Normal,
    Static,
    GoGo,
    GoGoMax,
    GoGoStart,
    GoGoStartMax,
    Combo10,
    Combo10Max,
    BalloonBreaking,
    BalloonBroke,
    BalloonMiss,
    Clear,
    ClearMax,
    ClearIn,
    SoulIn

}

public class TaikoCharacter : MonoBehaviour
{
    public Sprite[] frameArray;
    private int currentFrame;
    private float timer;
    // Start is called before the first frame update
    public float frametate = .1f;
    public Sprite test;
    public Sprite[] spritelist;
    public int spritegetter;
    public TaikoCharAction CurrentDonChanAction  = TaikoCharAction.Normal;
    public TaikoCharAction LastAction = TaikoCharAction.Normal;
    public string SpriteSheetToLoad = "Taiko_GoGoStart";
    int CurrentFrame;
    public float BPM = 120;
    public bool Loop = false;

    void Start()
    {
        //frameArray = Resources.LoadAll<Sprite>(GetSpriteNameToLoad(CurrentDonChanAction));
        //PlayAnimation(true, CurrentDonChanAction);
        PlayAnimation(Loop, CurrentDonChanAction);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void EditorPlayAction()
    {
        PlayAnimation(Loop, CurrentDonChanAction);
    }
    void PlayAnimation(bool IsAnimationLooping, TaikoCharAction AnimToPlay)
    {
        Loop = IsAnimationLooping;
        if(CurrentDonChanAction == TaikoCharAction.Normal || CurrentDonChanAction == TaikoCharAction.Combo10 || CurrentDonChanAction == TaikoCharAction.Combo10Max)
        {
            LastAction = CurrentDonChanAction;
        }
        else
        {
            LastAction = TaikoCharAction.Normal;
        }
        StopCoroutine(PlayNextFrame(Loop));
        CurrentFrame = 0;
        frameArray = Resources.LoadAll<Sprite>(GetSpriteNameToLoad(AnimToPlay));
        StartCoroutine(PlayNextFrame(Loop));
    }
    string GetSpriteNameToLoad(TaikoCharAction targetAction)
    {
        switch(targetAction)
        {
            case TaikoCharAction.BalloonBreaking:
                CurrentDonChanAction = targetAction;
                return "Taiko_BalloonBreaking";
            case TaikoCharAction.BalloonBroke:
                CurrentDonChanAction = targetAction;
                return "Taiko_BalloonBroke";
            case TaikoCharAction.BalloonMiss:
                CurrentDonChanAction = targetAction;
                return "Taiko_BalloonMiss";
            case TaikoCharAction.Clear:
                CurrentDonChanAction = targetAction;
                return "Taiko_Clear";
            case TaikoCharAction.ClearIn:
                CurrentDonChanAction = targetAction;
                return "Taiko_ClearIn";
            case TaikoCharAction.ClearMax:
                CurrentDonChanAction = targetAction;
                return "Taiko_ClearMax";
            case TaikoCharAction.Combo10Max:
                CurrentDonChanAction = targetAction;
                return "Taiko_10comboMax";
            case TaikoCharAction.Combo10:
                CurrentDonChanAction = targetAction;
                return "Taiko_10combo";
            case TaikoCharAction.GoGo:
                CurrentDonChanAction = targetAction;
                return "Taiko_GoGo";
            case TaikoCharAction.GoGoMax:
                CurrentDonChanAction = targetAction;
                return "Taiko_GoGoMax";
            case TaikoCharAction.GoGoStart:
                CurrentDonChanAction = targetAction;
                return "Taiko_GoGoStart";
            case TaikoCharAction.GoGoStartMax:
                CurrentDonChanAction = targetAction;
                return "Taiko_GoGoStartMax";
            case TaikoCharAction.Normal:
                CurrentDonChanAction = targetAction;
                return "Taiko_Normal";
            case TaikoCharAction.SoulIn:
                CurrentDonChanAction = targetAction;
                return "Taiko_SoulIn";
        }
        return "Taiko_Normal";
    }

    IEnumerator PlayNextFrame(bool LoopAnimation)
    {
        yield return new WaitForSeconds((60000 / BPM) / 1000);
        
        if(CurrentFrame <= frameArray.Length -1)
        {
            gameObject.GetComponent<Image>().sprite = frameArray[CurrentFrame];
            CurrentFrame++;
            StartCoroutine(PlayNextFrame(Loop));
        }
        else if(Loop)
        {
            CurrentFrame = 0;
            Debug.Log("loop");
            StartCoroutine(PlayNextFrame(Loop));
        }
        else
        {
            Debug.Log(LastAction.ToString());
            PlayAnimation(true, LastAction);
        }
        
    }
}
