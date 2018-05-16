using UnityEngine;
//using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Puzzle : MonoBehaviour {
	public int index;
	public string puzzleName;
	public string string0;
	public string string1;
	public float zoomPad = 4;

	public bool polar;
	public bool inverted;
	public bool parametric;
	public bool swapped;
	public Vector3 playerOrigin;
	public GameObject playerMarker;

	public bool complete;
	public bool canComplete;
	public float completionTime;
	public int completionCharacterCount;
	public Objective[] objectives;
	public bool isActivePuzzle;
	public bool editable;
	[HideInInspector] public int currentOrderIndex;
	[HideInInspector] public string string0Start;
	[HideInInspector] public string string1Start;

	public string identifier;

	private Game game;
	[HideInInspector]
	public Instructions instructions;
	private Objective selectedObjective;
	private Vector3 selectionPosition;

	void Start () {
		if (Application.isPlaying) {
			instructions = GetComponentInChildren<Instructions>();
			string0Start = string0;
			string1Start = string1;
			game = FindObjectOfType<Game>();
			objectives = GetComponentsInChildren<Objective>();
			if (playerMarker != null) playerOrigin = playerMarker.transform.position;
			else if (polar) playerOrigin = Vector3.up;
			//Deactivate();
		}
	}
	
	void Update () {
		if (Application.isPlaying) {
			int i;
			if (playerMarker != null) playerOrigin = playerMarker.transform.position;
			string0 = game.graph.string0;
			string1 = game.graph.string1;
			if (game.playing && !complete) {
				complete = IsComplete();
				if (complete) completionTime = Time.time-game.playTime;
			}
			currentOrderIndex = 100;
			for (i = 0; i < objectives.Length; i++) {
				if (!objectives[i].complete && objectives[i].orderIndex < currentOrderIndex && objectives[i].orderIndex >= 0) currentOrderIndex = objectives[i].orderIndex;
			}
		}
		else if (identifier == null || identifier == "") {
			identifier = "";
			while (identifier.Length < 5) identifier += Game.alphabet[Random.Range(0, 26)];
		}
	}

	public bool IsComplete () {
		if (objectives.Length == 0) return false;
		for (int i = 0; i < objectives.Length; i++) {
			if (!objectives[i].complete) return false;
		}
		return true;
	}

	public void GenerateRandomObjectives(int objectiveCount){
		int i;
		Debug.Log("Generating "+objectiveCount+" Random Objectives");
		for (i = 0; i < objectives.Length; i++) Destroy(objectives[i].gameObject);
		objectives = new Objective[objectiveCount];
		for (i = 0; i < objectives.Length; i++) {
			objectives[i] = (Instantiate(game.objectivePrefab) as GameObject).GetComponent<Objective>();
			objectives[i].puzzle = this;
			objectives[i].transform.parent = transform;
			objectives[i].startPosition = new Vector3((UnityEngine.Random.value*2-1)*16, (UnityEngine.Random.value*2-1)*16, 1);
			objectives[i].transform.position = objectives[i].startPosition;
		}
		complete = false;
	}

	public void Reset () {
		complete = false;
		canComplete = true;
		for (int i = 0; i < objectives.Length; i++) objectives[i].Reset();
	}

	public void DestroyObjectives () {
		complete = false;
		for (int i = 0; i < objectives.Length; i++) Destroy(objectives[i]);
	}

	public void Activate () {
		canComplete = true;
		game.windowZoom = 0;
		isActivePuzzle = true;
		for (int i = 0; i < objectives.Length; i++) {
			objectives[i].gameObject.SetActive(true);
			objectives[i].Reset();
		}
		if (instructions != null) instructions.gameObject.SetActive(true);
	}

	public void Deactivate () {
		isActivePuzzle = false;
		if (instructions != null) instructions.gameObject.SetActive(false);
		for (int i = 0; i < objectives.Length; i++) objectives[i].gameObject.SetActive(false);
	}

	public Objective AddObjective () {
		return AddObjective(game.objectivePrefab);
	}

	public Objective AddObjective (GameObject prefab) {
		Objective newObjective = (Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Objective>();
		newObjective.puzzle = this;
		newObjective.transform.parent = transform;
		newObjective.gameObject.name = "Objective";
		Objective[] newObjectives = new Objective[objectives.Length+1];
		for (int i = 0; i < objectives.Length; i++) newObjectives[i] = objectives[i];
		newObjectives[objectives.Length] = newObjective;
		objectives = newObjectives;
		return newObjective;
	}

	public void RemoveAll () {
		while (objectives.Length > 0) RemoveObjective(objectives[0]);
	}

	public void RemoveObjective (Objective toRemove) {
		Objective[] newObjectives = new Objective[objectives.Length-1];
		int offset = 0;
		for (int i = 0; i < objectives.Length; i++) {
			if (objectives[i] == toRemove) offset = -1;
			else newObjectives[i+offset] = objectives[i];
		}
		Destroy(toRemove.gameObject);
		objectives = newObjectives;
	}

	public void MakeUnwinnable () {
		canComplete = false;
		game.menu.GetComponent<AudioSource>().PlayOneShot(game.menu.errorSound);
		foreach (Objective objective in objectives) objective.untriggerable = true;
	}
}

class PuzzleSorter : IComparer<Puzzle> {
	public int Compare(Puzzle a, Puzzle b) {
		if (a.index < b.index) return -1;
		return 1;
	}
}