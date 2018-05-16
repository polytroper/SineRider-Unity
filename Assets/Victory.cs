using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {
    private Graphics graphics;
	private Game game;
	private Menu menu;
	private Graph graph;

	public TMPro.TextMeshPro victoryText;
	public TMPro.TextMeshPro victoryTimeText;
	public TMPro.TextMeshPro victoryCharacterText;
	public GameObject victoryPlane;

	private float progressBarProgress = 0;

	void Start () {
		game = Game.instance;
		menu = game.menu;
		graph = game.graph;
		graphics = game.graphics;
	}

	void OnEnable () {
		if (game) {
			progressBarProgress = 0;
			menu.GetComponent<AudioSource>().PlayOneShot(menu.completeSound);
		}
	}
	
	void Update () {
		victoryText.transform.position = new Vector3(game.window.position.x, game.window.position.y+game.cameraRect.height/6f, -3);
		victoryCharacterText.transform.position = new Vector3(game.window.position.x, game.window.position.y, -3);
		victoryTimeText.transform.position = new Vector3(game.window.position.x, game.window.position.y-game.cameraRect.height/6f, -3);
		victoryPlane.transform.position = new Vector3(game.window.position.x, game.window.position.y, -2);

		victoryText.transform.rotation = game.window.rotation;
		victoryCharacterText.transform.rotation = game.window.rotation;
		victoryTimeText.transform.rotation = game.window.rotation;
		victoryPlane.transform.rotation = Quaternion.Euler(270, 0, Camera.main.transform.rotation.eulerAngles.z);

		victoryTimeText.text = "T = "+Mathf.Round(game.puzzle.completionTime*10)/10;
		victoryCharacterText.text = game.puzzle.string1.Length+" Characters";
		
		victoryText.fontSize = game.cameraRect.height*3/2f;
		victoryTimeText.fontSize = game.cameraRect.height;
		victoryCharacterText.fontSize = game.cameraRect.height;

		victoryPlane.transform.localScale = game.cameraRect.height*Vector3.one/13f;
	}

	public void DrawGUI() {
		if (game.puzzleGroup.puzzleGroupName.StartsWith("Tutorials")) {
			GUIContent nextPuzzleContent;
			if (game.puzzle.index == game.puzzleGroup.puzzles.Length-1) {
				progressBarProgress = Mathf.Clamp01(progressBarProgress+Time.deltaTime/2);
				if (progressBarProgress < 1) {
					GUI.Box(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.height/5f, 8*menu.menuRect.height/11f, menu.menuRect.height/2.5f, menu.menuRect.height/20f), "", Graphics.infoStyle);
					GUI.Box(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.height/5f, 8*menu.menuRect.height/11f, Mathf.Lerp(game.puzzleIndex*(menu.menuRect.height/2.5f)/game.puzzleGroup.puzzles.Length, (game.puzzleIndex+1)*(menu.menuRect.height/2.5f)/game.puzzleGroup.puzzles.Length, progressBarProgress), menu.menuRect.height/20f), "", Graphics.progressBarStyle);
					GUI.Label(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.height/5f, 8*menu.menuRect.height/11f, menu.menuRect.height/2.5f, menu.menuRect.height/20f), "Section "+Mathf.Round(Mathf.Lerp(100*game.puzzleIndex/game.puzzleGroup.puzzles.Length, 100*(game.puzzleIndex+1)/game.puzzleGroup.puzzles.Length, progressBarProgress))+"% Complete", Graphics.progressLabelStyle);
				}
				else if (game.puzzleGroups[game.puzzleGroupIndex+1].puzzleGroupName.StartsWith("Tutorials")) {
					nextPuzzleContent = new GUIContent("You're done with this section!\nNext up: "+game.puzzleGroups[game.puzzleGroupIndex+1].puzzleGroupName.Substring(12));
					if (GUI.Button(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.width/4f, 3*menu.menuRect.height/4f-menu.menuRect.height/40f, menu.menuRect.height/2f, menu.menuRect.height/15f), nextPuzzleContent, Graphics.buttonGreenStyle)) {
						game.puzzleGroup.SetPuzzle(0);
						game.SetGroup(game.puzzleGroupIndex+1);
						menu.GetComponent<AudioSource>().PlayOneShot(menu.buttonSound);
					}
				}
				else {
					nextPuzzleContent = new GUIContent("That's all the tutorials for now! More coming soon\n(Click here to move on to the unguided puzzles)");
					if (GUI.Button(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.width/3f, 3*menu.menuRect.height/4f-menu.menuRect.height/40f, 2*menu.menuRect.height/3f, menu.menuRect.height/15f), nextPuzzleContent, Graphics.buttonGreenStyle)) {
						game.puzzleGroup.SetPuzzle(0);
						game.SetGroup(game.puzzleGroupIndex+1);
						menu.GetComponent<AudioSource>().PlayOneShot(menu.buttonSound);
						game.playState = 3;
					}
				}
			}
			else {
				progressBarProgress = Mathf.Clamp01(progressBarProgress+Time.deltaTime/2);
				GUI.Box(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.height/5f, 8*menu.menuRect.height/11f, menu.menuRect.height/2.5f, menu.menuRect.height/20f), "", Graphics.infoStyle);
				GUI.Box(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.height/5f, 8*menu.menuRect.height/11f, Mathf.Lerp(game.puzzleIndex*(menu.menuRect.height/2.5f)/game.puzzleGroup.puzzles.Length, (game.puzzleIndex+1)*(menu.menuRect.height/2.5f)/game.puzzleGroup.puzzles.Length, progressBarProgress), menu.menuRect.height/20f), "", Graphics.progressBarStyle);
				GUI.Label(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.height/5f, 8*menu.menuRect.height/11f, menu.menuRect.height/2.5f, menu.menuRect.height/20f), "Section "+Mathf.Round(Mathf.Lerp(100*game.puzzleIndex/game.puzzleGroup.puzzles.Length, 100*(game.puzzleIndex+1)/game.puzzleGroup.puzzles.Length, progressBarProgress))+"% Complete", Graphics.progressLabelStyle);

				if (progressBarProgress == 1) {
					nextPuzzleContent = new GUIContent("Next Up: "+game.puzzleGroup.puzzles[game.puzzleGroup.puzzleIndex+1].puzzleName);
					if (GUI.Button(new Rect(menu.menuRect.x+menu.menuRect.width/2f-menu.menuRect.height/5f, 4*menu.menuRect.height/5f, menu.menuRect.height/2.5f, menu.menuRect.height/20f), nextPuzzleContent, Graphics.buttonGreenStyle)) {
						game.puzzleGroup.SetPuzzle(game.puzzleGroup.puzzleIndex+1);
						menu.GetComponent<AudioSource>().PlayOneShot(menu.buttonSound);
					}
				}
			}
		}
	}
}
