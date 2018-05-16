using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PuzzleGroup : MonoBehaviour {
	public int index;
	public string puzzleGroupName;
	public int puzzleIndex;
	public Puzzle[] puzzles;
	public Puzzle puzzle;

	private Game game;

	public GUIContent[] puzzleListContent;
	public bool showPuzzleList = false;
	public bool puzzleListPicked = true;
	public int puzzleListSelection = 0;
	[HideInInspector] public GUIContent puzzleListButtonContent;

	void Start () {
		int i;
		game = FindObjectOfType<Game>();
		puzzles = GetComponentsInChildren<Puzzle>();
		Array.Sort(puzzles, new PuzzleSorter());
		puzzleIndex = 0;
		puzzleListSelection = puzzleIndex;
		puzzle = puzzles[puzzleIndex];

		puzzleListContent = new GUIContent[puzzles.Length+1];
		for (i = 0; i < puzzles.Length; i++) {
			puzzleListContent[i] = new GUIContent(puzzles[i].index+" - "+puzzles[i].puzzleName);
			puzzles[i].gameObject.SetActive(false);
		}
		puzzle.gameObject.SetActive(true);

		puzzleListContent[puzzles.Length] = new GUIContent(puzzle.puzzleName);
		puzzleListButtonContent = new GUIContent(puzzle.puzzleName);
		if (puzzle.editable) {			
			//puzzle.GenerateRandomObjectives(2);
		}
	}
	
	void Update () {
	
	}

	public void SetPuzzle (int newPuzzleIndex) {
		Debug.Log("Setting Puzzle "+newPuzzleIndex+" from "+puzzleIndex);
		//puzzle.Deactivate();
		puzzle.gameObject.SetActive(false);
		puzzleIndex = newPuzzleIndex;
		puzzle = puzzles[puzzleIndex];
		puzzle.gameObject.SetActive(true);
		puzzle.Activate();
		game.puzzle = puzzle;
		game.puzzleIndex = puzzleIndex;
		puzzleListSelection = puzzleIndex;
		puzzleListContent[puzzleListContent.Length-1] = new GUIContent(puzzle.puzzleName);
		puzzleListButtonContent = new GUIContent(puzzle.puzzleName);
		
		game.graph.string0 = puzzle.string0;
		game.graph.string1 = puzzle.string1;
		game.cameraSmooth = false;
		game.StopRiding();
		Debug.Log("Current Puzzle: "+puzzleIndex);
	}
}

class PuzzleGroupSorter : IComparer<PuzzleGroup> {
	public int Compare(PuzzleGroup a, PuzzleGroup b) {
		if (a.index < b.index) return -1;
		return 1;
	}
}