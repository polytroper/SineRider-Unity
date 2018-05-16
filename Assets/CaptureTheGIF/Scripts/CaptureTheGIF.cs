using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.IO;
using System.Threading;


public class CaptureTheGIF : MonoBehaviour {

    static CaptureTheGIF instance;
    public static bool running = false;

    public static CaptureTheGIF Instance {
        get {
            if (!instance) {
                var go = new GameObject("CaptureTheGIF");
                go.AddComponent<CaptureTheGIF>();
            }
            return instance;
        }
    }

    void Awake() {
        if (instance) {
            Debug.LogError("there is already an instance of CaptureTheGIF");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
  
    public static FileStream MakeFile(string dirName, string name) {
        const string suffix = ".gif";
        var dir = 
            Path.Combine(
                #if UNITY_EDITOR
                        Application.dataPath,
                #else
                        Application.dataPath,
                #endif
                dirName);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var path = Path.Combine(dir, (name + suffix));
        for(int i = 0; File.Exists(path); ++i) {
            path = Path.Combine(dir, (name + i + suffix));
        }
        Debug.Log("Creating File @: " + path);
        var file = File.Create(path);
        return file;
    }

    public static RenderTexture Snapshot(int width, int height) {

        var result = RenderTexture.GetTemporary(width, height, 24);
        foreach(var cam in Camera.allCameras.OrderBy(c => c.depth)) {
            var old = cam.targetTexture;
            cam.targetTexture = result;
            cam.Render();
            cam.targetTexture = old;
            cam.ResetAspect();
        }
        return result;
    }

    public Coroutine Capture(int frames,int width, int height, float frameRate, string dir, string name, bool lockFramerate = false) {
        return StartCoroutine(CaptureRoutine(frames, width, height, frameRate, dir, name, lockFramerate));
    }
    IEnumerator CaptureRoutine(int frames, int width, int height, float frameRate, string dir, string name, bool lockFramerate) {
        running = true;
        var memStream = new MemoryStream();
        var capRoutine = CaptureRoutine(frames, width, height, frameRate, memStream, false, lockFramerate);
        while (capRoutine.MoveNext())
        {
            yield return capRoutine.Current;
        }

        using (var file = MakeFile(dir, name)) {
            var thread = new Thread(() => {

                memStream.WriteTo(file);
                file.Flush();
                file.Close();
            });
            thread.Start();
            while (thread.ThreadState == ThreadState.Running) {
                yield return null;
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            running = false;

        }
   
    }

    public Coroutine Capture(int frames, int width, int height, float frameRate, Stream stream, bool closeWhenDone, bool lockFramerate = false) {
        return StartCoroutine(CaptureRoutine(frames, width, height, frameRate, stream, closeWhenDone, lockFramerate));
    }
    IEnumerator CaptureRoutine(int frames, int width, int height, float frameRate, Stream stream, bool closeWhenDone, bool lockFramerate) {

        var textures = new List<RenderTexture>();
        float t = Time.time;
        float waitUntil = t;
        if (lockFramerate) {
            Time.captureFramerate = (int)frameRate;
        }
        while(Game.instance.playing) {
            if (!lockFramerate) {
                while (waitUntil > Time.time) yield return null;
            }
            yield return new WaitForEndOfFrame();
            waitUntil += 1 / frameRate;
            textures.Add(Snapshot(width, height));
        }

        Time.captureFramerate = 0;
        float period = 1/frameRate;

        yield return MakeGif(textures, period, stream, closeWhenDone);
    }

    public Coroutine MakeGif(List<RenderTexture> textures, float frameLength, Stream output, bool closeWhenDone) {
        return StartCoroutine(MakeGifRoutine(textures, frameLength, output,closeWhenDone));
    }

  
    static IEnumerator MakeGifRoutine(List<RenderTexture> textures, float frameLength, Stream output, bool closeWhenDone) {
        Game.instance.canRecord = false;
        var gifEncoder = new Gif.Components.AnimatedGifEncoder();
        gifEncoder.SetQuality(10);
        gifEncoder.SetRepeat(0);
        gifEncoder.SetDelay((int)(frameLength * 1000));

        gifEncoder.Start(output);
        int w = textures[0].width;
        int h = textures[0].height;
        var tex = new Texture2D(w,h, TextureFormat.ARGB32, false, true);

        var imageStart = new ManualResetEvent(false);
        Gif.Components.Image image = null;
        bool done = false;
        bool processed = false;
        var worker = new Thread(() => {
            while (!done) {
                imageStart.WaitOne();
                imageStart.Reset();
                gifEncoder.AddFrame(image);
                processed = true;
            }
        });
        worker.Start();

       
        for (int picCount = 0; picCount < textures.Count; picCount++) {
            var tempTex = textures[picCount];
            RenderTexture.active = tempTex;
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            RenderTexture.ReleaseTemporary(tempTex);
            image = new Gif.Components.Image(tex);
            processed = false;
            imageStart.Set();
            while (!processed) {
                yield return null;
            }
            
        }
        Game.instance.canRecord = true;

        done = true;

        textures.Clear();
        gifEncoder.Finish();

        DestroyImmediate(tex);
        output.Flush();
        if (closeWhenDone) {
            output.Close();
        }
    }
}