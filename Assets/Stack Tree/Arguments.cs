using UnityEngine;
using System.Collections;

public class Arguments : MonoBehaviour {
	public static float xValue;
	public static float tValue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static float Parse (string argument) {
		argument = argument.ToLower();
		if (argument == "x") return xValue;
		if (argument == "t") return tValue;
		return float.Parse(argument);
	}
}
