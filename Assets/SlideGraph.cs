using UnityEngine;
using System.Collections;

public class SlideGraph : MonoBehaviour {
	public SpriteRenderer slider;
	public Graph graph;
	public LineRenderer slideLine;
	public TMPro.TextMeshPro graphText;
	public TMPro.TextMeshPro sliderText;

	public float c0;
	public float c1;
	public float c;

	public Vector3 slide0 = -Vector3.up;
	public Vector3 slide1 = Vector3.up;

	public SlideGraph twinGraph;

	private Vector3 mousePosition;
	private string cString;
	private bool dragging;
	private Vector3 dragMousePosition;
	private Vector3 dragSliderPosition;

	// Use this for initialization
	void Start () {
		cString = graph.string1;
		slide0 = slider.transform.position+slide0+transform.position.z*Vector3.forward;
		slide1 = slider.transform.position+slide1+transform.position.z*Vector3.forward;

		slider.transform.position = Vector3.Lerp(slide0, slide1, Mathf.InverseLerp(c0, c1, c));

		slideLine.SetPosition(0, slide0+Vector3.forward);
		slideLine.SetPosition(1, slide1+Vector3.forward);
		slideLine.SetWidth(slider.transform.localScale.x/10, slider.transform.localScale.x/10);
	}
	
	// Update is called once per frame
	void Update () {
		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (dragging) {
			slider.transform.position = dragSliderPosition+mousePosition-dragMousePosition;
			if (Input.GetMouseButtonUp(0)) {
				dragging = false;
			}
		}
		else if (slider.GetComponent<Collider2D>().OverlapPoint(mousePosition)) {
			if (Input.GetMouseButtonDown(0)) {
				dragging = true;
				dragMousePosition = mousePosition;
				dragSliderPosition = slider.transform.position;
			}
			slider.color = Color.green;
		}
		else {
			slider.color = Color.white;
		}

		c = Mathf.InverseLerp(slide0.y, slide1.y, slider.transform.position.y);

		if (twinGraph && dragging) twinGraph.slider.transform.position = Vector3.Lerp(twinGraph.slide0, twinGraph.slide1, c);

		slider.transform.position = Vector3.Lerp(slide0, slide1, c);
		c = Mathf.Lerp(c0, c1, c);
		graph.string1 = cString.Replace("<C>", c.ToString());

		string textString = "Y="+cString;
		if (c < 0) textString = textString.Replace("+<C>", "-<#FF2391>"+Mathf.Abs(Mathf.Round(c*10)/10).ToString()+"</color>");
		textString = textString.Replace("<C>", "<#FF2391>"+(Mathf.Round(c*10)/10).ToString()+"</color>");
		graphText.text = textString;
	}
}
