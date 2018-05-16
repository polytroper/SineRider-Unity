using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class TimeTravelGifRecorder : MonoBehaviour {

    public KeyCode key = KeyCode.P;


    public string dir = "gifs";
    public new string name = "gif";

    public int width = 640;
    public int height = 480;
    public int frames = 30;
    public int frameRate = 15;

    List<RenderTexture> snapshots = new List<RenderTexture>();

    float lastSnapshot;
    void Start() {
        lastSnapshot = Time.time;
    }
    void Update() {
        if (Time.time >= lastSnapshot + 1 / frameRate) {
            lastSnapshot = Time.time;
            Snapshot();
        }
        if (Input.GetKeyDown(key)) {
            var file = CaptureTheGIF.MakeFile(dir, name);
            CaptureTheGIF.Instance.MakeGif(snapshots.ToList(), 1 / frameRate, file, true);
            snapshots.Clear();
        }
    }

    void Snapshot() {
        snapshots.Add(CaptureTheGIF.Snapshot(width, height));
        if (snapshots.Count > frames) {
            RenderTexture.ReleaseTemporary(snapshots[0]);
            snapshots.RemoveAt(0);
        }
    }
}