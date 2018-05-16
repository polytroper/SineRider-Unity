using UnityEngine;
using System.Collections;
using TMPro;

[ExecuteInEditMode]
public class TextEdit : MonoBehaviour {
	public bool swapText = false;
	public bool areYouSure = false;
	public string fromString;
	public string toString;
	public GameObject parentObject;
	private TextMeshPro[] textObjects;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (swapText && areYouSure) {
			Swap(fromString, toString);
			swapText = false;
			areYouSure = false;
		}
		else {
			areYouSure = false;
		}
	}

	void Swap (string fromString, string toString) {
		int i;
		if (fromString == "") return;
		if (parentObject) textObjects = parentObject.GetComponentsInChildren<TextMeshPro>();
		else textObjects = FindObjectsOfType<TextMeshPro>();
		for (i = 0; i < textObjects.Length; i++) {
			textObjects[i].text = textObjects[i].text.Replace(fromString, toString);
		}

	}
}
