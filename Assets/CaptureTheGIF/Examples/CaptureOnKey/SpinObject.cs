using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class SpinObject : MonoBehaviour {

    public float speed = 1;
    float time;
    void Update() {
        time += Time.deltaTime * speed;
        transform.localRotation = Quaternion.AngleAxis(360 * time, Vector3.up);
    }
}