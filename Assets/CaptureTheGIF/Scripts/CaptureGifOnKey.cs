using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class CaptureGifOnKey : MonoBehaviour {

    public enum ButtonType {
        KeyCode,
        InputManager,
        Mobile
    }


    public string buttonName = "p";
    public ButtonType buttonType = ButtonType.KeyCode;

    public string dir = "gifs";
    public new string name = "gif";

    public int frames = 30;
    public float frameRate = 10f;

    public int height = 480;
    public float aspect = 4.0f / 3.0f;

    public bool lockFramerate = false;

    public bool restrictDuplicateCaptures = true;

    void Update() {
        bool button = false;
        switch (buttonType) {
#if UNITY_ANDROID || UNITY_IOS
            case ButtonType.Mobile:
                button = Input.touches.Count() > 0;
                break;
#endif
            case ButtonType.KeyCode:
                var code = (KeyCode)Enum.Parse(typeof(KeyCode), buttonName, true);
                button = Input.GetKeyUp(code);
                break;
            case ButtonType.InputManager:
                button = Input.GetButtonUp(buttonName);
                break;
            default:
                throw new NotImplementedException("unknown button type: " + buttonType);
        }
        if (button) {
            if (restrictDuplicateCaptures && CaptureTheGIF.running)
            {
                return;
            }
            CaptureTheGIF.Instance.Capture(frames, (int)(aspect * height), height, frameRate, dir, name, lockFramerate);
        }
    }
}