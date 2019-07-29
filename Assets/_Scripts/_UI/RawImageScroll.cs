
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageScroll : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed = 1;
    public bool UseXAxis = true;
    RawImage RI;
    void Start()
    {
        RI = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if(UseXAxis)
            RI.uvRect = new Rect(RI.uvRect.x + (Time.fixedDeltaTime * Speed), RI.uvRect.y, RI.uvRect.width, RI.uvRect.height);
        else
            RI.uvRect = new Rect(RI.uvRect.x, RI.uvRect.y + (Time.fixedDeltaTime * Speed), RI.uvRect.width, RI.uvRect.height);

    }
}
