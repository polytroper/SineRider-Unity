using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {
	public string blockString;
	public Vector4 blockBounds;

	public List<Block> subBlocks;
	public List<string> subStrings;
	public List<TMPro.TextMeshPro> subTexts;

	private Vector3 blockPosition;

	private Game game;

	// Use this for initialization
	void Start () {
		game = FindObjectOfType<Game>();
		subBlocks = new List<Block>();
		subStrings = new List<string>();
		subTexts = new List<TMPro.TextMeshPro>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool ParseString () {
		bool toReturn = false;
		try {
			int i;
			List<Block> newSubBlocks = new List<Block>();
			List<string> newSubStrings = new List<string>();
			List<TMPro.TextMeshPro> newSubTexts = new List<TMPro.TextMeshPro>();
			string subString = "";
			TMPro.TextMeshPro newSubText;
			for (i = 0; i < blockString.Length; i++) {
				if (blockString[i].ToString() == "_") {
					newSubStrings.Add(subString);
					newSubStrings.Add("_");
					newSubText = (Instantiate(game.textPrefab) as GameObject).GetComponent<TMPro.TextMeshPro>();
					newSubText.text = subString;
					newSubTexts.Add(newSubText);
					subString = "";
				}
				else {
					subString += blockString[i];
				}
			}
			if (subString.Length != 0) {
				newSubStrings.Add(subString);
				newSubStrings.Add("_");
				newSubText = (Instantiate(game.textPrefab) as GameObject).GetComponent<TMPro.TextMeshPro>();
				newSubText.text = subString;
				newSubTexts.Add(newSubText);
			}
			subBlocks = newSubBlocks;
			subStrings = newSubStrings;
			subTexts = newSubTexts;
			toReturn = true;
		}
		catch (Exception ex) {}
		return toReturn;
	}

	void BuildString () {
		List<Block> newSubBlocks = new List<Block>();
		Block newSubBlock;
		int i;
		for (i = 0; i < subStrings.Count; i++) {
			newSubBlock = (Instantiate(game.blockPrefab) as GameObject).GetComponent<Block>();
		}
	}
}
