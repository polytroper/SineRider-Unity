using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour {
	public AudioClip windClip;
	public AudioSource audio0;
	public AudioSource audio1;
	private float currentVolume;
	private float targetVolume;

	// Use this for initialization
	void Start () {
		audio0.clip = windClip;
		audio1.clip = windClip;
		audio0.Play();
		audio1.PlayDelayed(windClip.length/2);
	}
	
	// Update is called once per frame
	void Update () {
		if (!Game.instance.playing) {
			audio0.volume = 0;
			audio1.volume = 0;
			currentVolume = 0;
		}
		else {
			targetVolume = 1-10/(10+transform.parent.GetComponent<Rigidbody2D>().velocity.magnitude);
			audio0.volume = Mathf.Abs(Mathf.Sin(Time.time*Mathf.PI/windClip.length))*currentVolume;
			audio1.volume = Mathf.Abs(Mathf.Cos(Time.time*Mathf.PI/windClip.length))*currentVolume;
			audio0.pitch = 0.7f+0.6f*targetVolume;
			audio1.pitch = 0.7f+0.6f*targetVolume;
		}
	}

	void FixedUpdate () {
		currentVolume = Mathf.Lerp(currentVolume, targetVolume, 0.02f);
	}
}
