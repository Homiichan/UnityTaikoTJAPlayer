using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum TaikoCharAction
{
    Normal,
    Static,
    GoGo,
    GoGoMax,
    GoGoStart,
    GoGoStartMax,
    Combo10,
    Comba10Max,
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
    public TaikoCharAction CurrentDonChanAction;
    public string SpriteSheetToLoad = "Taiko_GoGoStart";

    void Start()
    {
        frameArray = Resources.LoadAll<Sprite>(SpriteSheetToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frametate)
        {
            timer -= frametate;

            gameObject.GetComponent<SpriteRenderer>().sprite = frameArray[currentFrame];
            currentFrame = (currentFrame + 1) % frameArray.Length;
        }
    }
}
