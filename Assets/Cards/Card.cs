using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour {
	public int transitionType;
	public float transitionDuration = 0.5f;
	public float hoverDuration = 0.5f;

	public Card hoverCard;

	public Card[] subCards;

	private float transitionProgress;
	private float transitionTime;

	public bool hovering;
	public float hoverSpeed = 180;
	public float hoverOnTime;
	public float hoverOffTime;
	public float hoverOnProgress = 1;
	public float hoverOffProgress = 1;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transitionProgress = (Time.time-transitionTime)/transitionDuration;
		if (hovering) {
			hoverOnProgress = (Time.time-hoverOnTime)/hoverDuration;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(180, 0, 0), hoverSpeed*Time.deltaTime);
		}
		else {
			hoverOffProgress = (Time.time-hoverOffTime)/hoverDuration;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 0), hoverSpeed*Time.deltaTime);
		}
	}
	
	public void OnHover () {
		hovering = true;
		hoverOnTime = Time.time;//-(1-hoverOffProgress)*hoverDuration;
	}
	
	public void OffHover () {
		hovering = false;
		hoverOffTime = Time.time;//-(1-hoverOnProgress)*hoverDuration;
	}
	
	public void OnClick () {
		
	}

	public void OnClick0 () {
		
	}
	
	public void OnClick1 () {
		
	}

	public void OnClick2 () {
		
	}
}
