using UnityEngine;
using System.Collections;

public class TextMeshProForceUpdate : MonoBehaviour {
	private TMPro.TextMeshPro text;

	void Start () {
		text = GetComponent<TMPro.TextMeshPro>();
		text.ForceMeshUpdate();
		text.UpdateFontAsset();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
