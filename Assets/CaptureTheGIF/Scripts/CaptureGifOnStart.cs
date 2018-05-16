using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class CaptureGifOnStart : MonoBehaviour {
    public string dir = "gifs";
    public new string name = "gif";

    public int frames = 30;
    public float frameRate = 10f;

    public int height = 640;
    public float aspect = 4.0f / 3.0f;

    void Start() {
        CaptureTheGIF.Instance.Capture(frames, (int)(aspect*height), height, frameRate, dir, name);
    }
}
