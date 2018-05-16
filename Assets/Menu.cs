using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public AudioClip startSound;
	public AudioClip stopSound;
	public AudioClip createSound;
	public AudioClip deleteSound;
	public AudioClip selectSound;
	public AudioClip deselectSound;
	public AudioClip resetSound;
	public AudioClip openSound;
	public AudioClip closeSound;
	public AudioClip openMenuSound;
	public AudioClip closeMenuSound;
	public AudioClip buttonSound;
	public AudioClip errorSound;
	public AudioClip completeSound;

	public TMPro.TextMeshPro timeText;

	private bool showPuzzleList = false;
	private bool showPuzzleGroupList = false;
	private bool puzzleGroupListPicked = true;
	public int puzzleGroupListSelection = 0;
	public GUIContent[] puzzleGroupListContent;
	public GUIContent puzzleGroupListButtonContent;

	public SpriteRenderer highlighter;
	public SpriteRenderer staticHighlighter;
	public SpriteRenderer dynamicHighlighter;

	private bool movingPlayer;
	private bool movingSelectedObjective;
	private Vector3 selectionPosition;
	private Vector3 selectionOffset;
	public GameObject selectionBox;
	private Vector3 selectionOrigin;
	private Objective selectedObjective;

	public bool mute;
	public bool showKeypad;
	public bool showUrl;
	private string urlString;
	
	private Vector2 mouseGuiPosition;
	private Vector3 worldMousePosition;

	private int topMenuState = 0;
	private int bottomMenuState = 1;
	private float topMenuTime = -10;
	private float bottomMenuTime = 1;

	[HideInInspector] public Rect menuRect;
	private Rect keypadRect;
    private Graphics graphics;
	private Game game;
	private Graph graph;
	private Victory victory;

	[HideInInspector] public TouchScreenKeyboard touchKeyboard;

	public static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	void Start () {
		game = Game.instance;
		graph = game.graph;
		graphics = game.graphics;
		victory = FindObjectOfType<Victory>();

		staticHighlighter.gameObject.SetActive(false);
		dynamicHighlighter.gameObject.SetActive(false);

		puzzleGroupListContent = new GUIContent[game.puzzleGroups.Length+1];
		for (int i = 0; i < game.puzzleGroups.Length; i++) puzzleGroupListContent[i] = new GUIContent(game.puzzleGroups[i].puzzleGroupName);
		puzzleGroupListContent[game.puzzleGroups.Length] = new GUIContent(game.puzzleGroup.puzzleGroupName);
		puzzleGroupListButtonContent = new GUIContent(game.puzzleGroup.puzzleGroupName);
	}
	
	void Update () {
		if (game.playing) {
			if (game.puzzle.complete) victory.gameObject.SetActive(true);
			else victory.gameObject.SetActive(false);
		}
		else {
			if (game.puzzle.editable) {
				if (Input.GetMouseButton(0)) game.cameraSmooth = false;
				Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				currentPosition = new Vector3(currentPosition.x, currentPosition.y, -4);
				if (Input.GetMouseButtonDown(0) && !game.puzzle.playerMarker.GetComponent<Collider2D>().OverlapPoint(currentPosition)) {
					bool muteSelection = false;
					foreach (Objective ob in game.puzzle.objectives) {
						if (ob.GetComponent<Collider2D>().OverlapPoint(currentPosition)) muteSelection = true;
					}
					if (!muteSelection) {
						selectionOrigin = currentPosition;
						selectionBox.SetActive(true);
					}
				}
				if (Input.GetMouseButton(0) && selectionBox.activeSelf) {
					selectionBox.transform.position = (selectionOrigin+currentPosition)/2;
					selectionBox.transform.localScale = new Vector3(Mathf.Abs(currentPosition.x-selectionOrigin.x), Mathf.Abs(currentPosition.y-selectionOrigin.y), 0);
				}
				else if (selectionBox.activeSelf) {
					foreach (Objective o in game.puzzle.objectives) {
						if (o.startPosition.x > Mathf.Min(currentPosition.x, selectionOrigin.x) &&
						    o.startPosition.x < Mathf.Max(currentPosition.x, selectionOrigin.x) &&
						    o.startPosition.y > Mathf.Min(currentPosition.y, selectionOrigin.y) &&
						    o.startPosition.y < Mathf.Max(currentPosition.y, selectionOrigin.y)) {
							if (selectedObjective) selectedObjective.selected = false;
							selectedObjective = o;
							o.selected = true;
						}
					}
					selectionBox.SetActive(false);
				}
				game.puzzle.playerMarker.transform.rotation = game.rider.transform.rotation;
			}
			else {
				highlighter.gameObject.SetActive(false);
				staticHighlighter.gameObject.SetActive(false);
				dynamicHighlighter.gameObject.SetActive(false);
			}

			puzzleGroupListSelection = game.puzzleGroupIndex;
			puzzleGroupListContent[puzzleGroupListContent.Length-1] = new GUIContent(game.puzzleGroup.puzzleGroupName);
			puzzleGroupListButtonContent = new GUIContent(game.puzzleGroup.puzzleGroupName);

			victory.gameObject.SetActive(false);
		}

		timeText.text = "T = "+Mathf.Round((Time.time-game.playTime)*10)/10;
		timeText.fontSize = game.cameraRect.height/2;
		timeText.transform.position = game.window.TransformPoint(Vector3.left*game.cameraRect.height*0.49f+Vector3.down*game.cameraRect.height/2+Vector3.back*(game.window.position.z+4));
		timeText.transform.rotation = game.window.rotation;
	}
	
	void OnGUI () {
		int i;
		mouseGuiPosition = new Vector2(Input.mousePosition.x, Screen.height-Input.mousePosition.y);
		worldMousePosition = game.window.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
		int textFieldType;
		string textFieldString;

		//Tutorial Victory Prompts

		if (victory.gameObject.activeSelf) victory.DrawGUI();

		// Window
		
		if (graphics.portrait) {
			menuRect = new Rect(0, 0, graphics.width, graphics.width);
			keypadRect = new Rect(0, graphics.width, graphics.width, graphics.height-graphics.width);
		}
		else {
			menuRect = new Rect(graphics.width/2f-graphics.height/2f, 0, graphics.height, graphics.height);
			keypadRect = new Rect(graphics.width/2f-graphics.height/2f, 0, graphics.height, 9*graphics.height/10f);
		}

		Rect topMenuRect;
		Rect bottomMenuRect;
		Rect editorMenuRect;

		Rect keypadButtonRect;
		Rect variable1Rect;
		Rect graphStringRect;
		Rect function0Rect;
		Rect function1Rect;
		Rect puzzleGroupListRect;
		Rect puzzleListRect;
		Rect puzzleGroupLabelRect;
		Rect puzzleLabelRect;
		Rect muteButtonRect;
		Rect urlButtonRect;
		Rect recordButtonRect;
		Rect goButtonRect;
		Rect resetButtonRect;
		Rect victoryBoxRect;
		//keypadButtonRect = new Rect(menuRect.x, 19*menuRect.height/20f, menuRect.height/20f, menuRect.height/20f);

    	Rect coordinatesButtonRect;
    	Rect swapButtonRect;
    	Rect addButtonRect;
    	Rect deleteButtonRect;
    	Rect staticButtonRect;
    	Rect dynamicButtonRect;
    	Rect timerLabelRect;
    	Rect timerFieldRect;
    	Rect orderLabelRect;
    	Rect orderFieldRect;
    	Rect positionLabelRect;

		if (game.playState == 1) {
			topMenuState = 1;
			topMenuTime = game.playTime+30;
			topMenuRect = new Rect(menuRect.x, menuRect.y-Mathf.Lerp(menuRect.height/20, 0, (Time.time-topMenuTime)*4), menuRect.width, menuRect.height/20);
		}
		else {
			if (topMenuState == 0) {
				topMenuRect = new Rect(menuRect.x, menuRect.y-Mathf.Lerp(0, menuRect.height/20, (Time.time-topMenuTime)*4), menuRect.width, menuRect.height/20);
				if (((mouseGuiPosition.y < menuRect.height/10 && mouseGuiPosition.y > menuRect.y) && (!game.playing || game.puzzle.complete) && !Input.GetMouseButton(0)) || game.puzzle.editable) {
					topMenuState = 1;
					topMenuTime = Time.time-Mathf.Max((0.25f-(Time.time-topMenuTime)), 0);
				}
			}
			else {
				topMenuRect = new Rect(menuRect.x, menuRect.y-Mathf.Lerp(menuRect.height/20, 0, (Time.time-topMenuTime)*4), menuRect.width, menuRect.height/20);
				if ((mouseGuiPosition.y > menuRect.height/10 || mouseGuiPosition.y < menuRect.y) && !showPuzzleList && !showPuzzleGroupList && !showUrl && !game.puzzle.editable) {
					topMenuState = 0;
					topMenuTime = Time.time-Mathf.Max((0.25f-(Time.time-topMenuTime)), 0);
				}
			}
		}

		if (bottomMenuState == 0) {
			bottomMenuRect = new Rect(menuRect.x, menuRect.y+menuRect.height-Mathf.Lerp(menuRect.height/20, 0, (Time.time-bottomMenuTime)*4), menuRect.width, menuRect.height/10);
			if ((((mouseGuiPosition.y > 9*menuRect.height/10 && mouseGuiPosition.y < menuRect.height) || !game.playing) || game.puzzle.complete || !game.puzzle.canComplete) && !(showPuzzleList || showPuzzleGroupList)) {
				bottomMenuState = 1;
				if (game.puzzle.complete) bottomMenuTime = Time.time-Mathf.Max((0.25f-(Time.time-bottomMenuTime)), 0)+1;
				else bottomMenuTime = Time.time-Mathf.Max((0.25f-(Time.time-bottomMenuTime)), 0);
			}
		}
		else {
			bottomMenuRect = new Rect(menuRect.x, menuRect.y+menuRect.height-Mathf.Lerp(0, menuRect.height/20, (Time.time-bottomMenuTime)*4), menuRect.width, menuRect.height/10);
			if ((((mouseGuiPosition.y < 9*menuRect.height/10 || mouseGuiPosition.y > menuRect.height) && game.playing) || showPuzzleList || showPuzzleGroupList) && !game.puzzle.complete && game.puzzle.canComplete) {
				bottomMenuState = 0;
				bottomMenuTime = Time.time-Mathf.Max((0.25f-(Time.time-bottomMenuTime)), 0);
			}
		}

		puzzleGroupLabelRect = new Rect(menuRect.x, topMenuRect.y, topMenuRect.width/20, topMenuRect.height);
		if (game.puzzle.editable) puzzleGroupListRect = new Rect(puzzleGroupLabelRect.xMax, topMenuRect.y, topMenuRect.width/2-topMenuRect.width/30-2*topMenuRect.width/20, topMenuRect.height);
		else puzzleGroupListRect = new Rect(puzzleGroupLabelRect.xMax, topMenuRect.y, 48*topMenuRect.width/120, topMenuRect.height);
		puzzleLabelRect = new Rect(puzzleGroupListRect.xMax, topMenuRect.y, topMenuRect.width/20, topMenuRect.height);
		puzzleListRect = new Rect(puzzleLabelRect.xMax, topMenuRect.y, 48*topMenuRect.width/120, topMenuRect.height);
//		recordButtonRect = new Rect(topMenuRect.xMax-topMenuRect.height, topMenuRect.y, topMenuRect.height, topMenuRect.height);
		muteButtonRect = new Rect(topMenuRect.xMax-topMenuRect.height, topMenuRect.y, topMenuRect.height, topMenuRect.height);
		urlButtonRect = new Rect(muteButtonRect.x-topMenuRect.height, topMenuRect.y, topMenuRect.height, topMenuRect.height);
			
		//Keypad

//		DrawKeypad();

	    //Bottom Menu

	    //if (graph.parametric) bottomMenuRect = new Rect(menuRect.x, menuRect.y+menuRect.height-menuRect.height/10, menuRect.width, menuRect.height/10);
		//else bottomMenuRect = new Rect(menuRect.x, menuRect.y+menuRect.height-menuRect.height/20, menuRect.width, menuRect.height/20);

		GUI.Box(bottomMenuRect, "", Graphics.infoStyle);
		
		if ((game.puzzle.string1Start != "0" && !(game.puzzle.polar && game.puzzle.string1Start == "1")) && !showUrl && !(game.puzzleGroup.index == 0 && game.puzzle.index == 0)) {
			resetButtonRect = new Rect(menuRect.x, bottomMenuRect.y, menuRect.height/20f, menuRect.height/20f);
			variable1Rect = new Rect(resetButtonRect.xMax, resetButtonRect.y, menuRect.height/20f, menuRect.height/20f);
			function1Rect = new Rect(variable1Rect.xMax, bottomMenuRect.y, 48*menuRect.height/60f, menuRect.height/20f);
		}
		else {
			resetButtonRect = new Rect(menuRect.x, bottomMenuRect.y, 0, menuRect.height/20f);
			variable1Rect = new Rect(resetButtonRect.xMax, resetButtonRect.y, menuRect.height/20f, menuRect.height/20f);
			function1Rect = new Rect(variable1Rect.xMax, bottomMenuRect.y, 51*menuRect.height/60f, menuRect.height/20f);
		}
		goButtonRect = new Rect(function1Rect.xMax, function1Rect.y, menuRect.height/10, menuRect.height/20);

	    //Function 0 Box

	    //if (graph.parametric) {
		//	variable1Rect = new Rect(menuRect.x, bottomMenuRect.y, menuRect.height/20f, menuRect.height/20f);
		//	function0Rect = new Rect(variable1Rect.x+variable1Rect.width, 18*menuRect.height/20f, 45*menuRect.height/60f, menuRect.height/20f);
		//
		//	GUI.Box(variable1Rect, "X=", Graphics.variableStyle);
		//	//GUI.Label(new Rect(width/2-height/2, 19*height/20, height/20, height/20), guiContentY, guiStyleY);
		//	if (game.playing) GUI.Box(function0Rect, graph.string1, Graphics.inputDisabledStyle);
		//	else {
		//		GUI.SetNextControlName("Y=");
		//		graph.string1 = GUI.TextField(function0Rect, graph.string1, Graphics.inputStyle);
		//	}
	    //}

		//Play Button

		if (game.playing) {
			if (GUI.Button(goButtonRect, graphics.stopButtonIcon, Graphics.buttonRedStyle)) {
				game.StopRiding();
				GetComponent<AudioSource>().PlayOneShot(stopSound);
			}
		}
		else if (showUrl) {
			if (GUI.Button(goButtonRect, "OPEN", Graphics.buttonStyle)) {
				if (Application.isWebPlayer) Application.ExternalEval("game.window.open('"+urlString.Remove(0, 18)+"','_blank')");
				else Application.OpenURL("http://"+urlString);
				GetComponent<AudioSource>().PlayOneShot(buttonSound);
			}
		}
		else {
			if (!graph.ready) {
				if (GUI.Button(goButtonRect, graphics.goButtonIcon, Graphics.buttonRedStyle)) GetComponent<AudioSource>().PlayOneShot(errorSound);
			}
			else if (GUI.Button(goButtonRect, graphics.goButtonIcon, Graphics.buttonGreenStyle)) {
				GetComponent<AudioSource>().PlayOneShot(startSound);
				game.StartRiding();
			}
		}

		//Reset Button
	    
		if (game.playing) GUI.Box(resetButtonRect, graphics.resetButtonIcon, Graphics.toggleDisabledStyle);
		else {
			if (GUI.Button(resetButtonRect, graphics.resetButtonIcon, Graphics.toggleStyle)) {
				GetComponent<AudioSource>().PlayOneShot(resetSound);
				graph.string1 = game.puzzle.string1Start;
			}
		}

        //Keypad Button

		//if (game.playing && !graphics.portrait) GUI.Box(keypadButtonRect, "", Graphics.infoStyle);
		//else showKeypad = GUI.Toggle(keypadButtonRect, showKeypad, Graphics.instance.keypadButtonTexture, Graphics.toggleStyle);

		//Top Menu

		bool showGraphString = false;

		if (game.playState == 1) {
			if (GUI.Button(topMenuRect, "New to SineRider? Click here to get started!", Graphics.buttonGreenStyle)) {
				game.puzzleGroup.SetPuzzle(1);	
				GetComponent<AudioSource>().PlayOneShot(buttonSound);
				game.playState = 2;
			}

			if (game.playing) {
				if (function1Rect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Functions cannot be changed while sledding. To edit, hit STOP", Graphics.inputErrorStyle);
				else if (goButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "That's enough for now (it's cold out here!)", Graphics.inputInfoStyle);
				else GUI.Box(function1Rect, graph.string1, Graphics.inputDisabledStyle);
			}
			else showGraphString = true;
		}
		else if (game.playState == 2) {
			Rect previousButtonRect = new Rect(topMenuRect.x, topMenuRect.y, topMenuRect.height, topMenuRect.height);
			Rect nextButtonRect = new Rect(previousButtonRect.xMax, topMenuRect.y, topMenuRect.height, topMenuRect.height);
			Rect menuButtonRect = new Rect(nextButtonRect.xMax, topMenuRect.y, topMenuRect.width-4*topMenuRect.height, topMenuRect.height);

			if (game.puzzleGroupIndex == 0 && game.puzzleIndex == 1) GUI.Box(previousButtonRect, graphics.previousIcon, Graphics.buttonDisabledStyle);
			else if (GUI.Button(previousButtonRect, graphics.previousIcon, Graphics.buttonBlueStyle)) {
				if (game.puzzleIndex == 0) {
					game.SetGroup(game.puzzleGroupIndex-1);
					game.puzzleGroup.SetPuzzle(game.puzzleGroup.puzzles.Length-1);
				}
				else game.puzzleGroup.SetPuzzle(game.puzzleIndex-1);
				GetComponent<AudioSource>().PlayOneShot(buttonSound);
			}
			if (GUI.Button(menuButtonRect, "Return to Main Menu", Graphics.buttonStyle)) {
				game.puzzleGroup.SetPuzzle(0);
				game.SetGroup(0);
				game.puzzleGroup.SetPuzzle(0);
				GetComponent<AudioSource>().PlayOneShot(buttonSound);
			}
			if (game.puzzleIndex == game.puzzleGroup.puzzles.Length-1 && !game.puzzleGroups[game.puzzleGroupIndex+1].puzzleGroupName.StartsWith("Tutorials")) GUI.Box(nextButtonRect, graphics.nextIcon, Graphics.buttonDisabledStyle);
			else if (GUI.Button(nextButtonRect, graphics.nextIcon, Graphics.buttonBlueStyle)) {
				if (game.puzzleIndex == game.puzzleGroup.puzzles.Length-1) {
					game.SetGroup(game.puzzleGroupIndex+1);
					game.puzzleGroup.SetPuzzle(0);
				}
				else game.puzzleGroup.SetPuzzle(game.puzzleIndex+1);
				GetComponent<AudioSource>().PlayOneShot(buttonSound);
			}

			// URL

			if (game.playing) GUI.Box(urlButtonRect, graphics.linkButtonIcon, Graphics.toggleDisabledStyle);
			else {
				bool SHOWURL = GUI.Toggle(urlButtonRect, showUrl, graphics.linkButtonIcon, Graphics.toggleStyle);
				if (!SHOWURL && showUrl) {
					GUI.FocusControl("");
					GetComponent<AudioSource>().PlayOneShot(closeSound);
				}
				else if (SHOWURL && !showUrl) GetComponent<AudioSource>().PlayOneShot(openSound);
				showUrl = SHOWURL;
			}

			//Mute Button

			if (mute) mute = GUI.Toggle(muteButtonRect, mute, graphics.muteIcon, Graphics.toggleStyle);
			else mute = GUI.Toggle(muteButtonRect, mute, graphics.unmuteIcon, Graphics.toggleStyle);
			AudioListener.pause = mute;

			GUI.skin.settings.cursorColor = Color.black;

			if (game.playing) {
				if (function1Rect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Functions cannot be changed while sledding. To edit, hit STOP", Graphics.inputErrorStyle);
				else if (goButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "That's enough for now (it's cold out here!)", Graphics.inputInfoStyle);
				else if (resetButtonRect.Contains(mouseGuiPosition) && game.puzzle.string1Start != "0") GUI.Box(function1Rect, "Stuck? Reset to Y="+game.puzzle.string1Start, Graphics.inputInfoStyle);
				else GUI.Box(function1Rect, graph.string1, Graphics.inputDisabledStyle);
			}
			else if (showUrl) {
				if (goButtonRect.Contains(mouseGuiPosition)) {
					if (Application.isWebPlayer) GUI.Box(function1Rect, "Click here to open the link (watch out for popup blockers)", Graphics.inputInfoStyle);
					else GUI.Box(function1Rect, "Click here to open the link", Graphics.inputInfoStyle);
				}
				else {
					urlString = game.GetURL();
					TextEditor urlEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
					GUI.SetNextControlName("url");
					GUI.TextField(function1Rect, urlString, Graphics.inputStyle);
					GUI.FocusControl("url");
					urlEditor.pos = 0;
					urlEditor.selectPos = urlString.Length;
				}
			}
			else if (previousButtonRect.Contains(mouseGuiPosition) && !(game.puzzleGroupIndex == 0 && game.puzzleIndex == 1)) {
				if (game.puzzleIndex == 0) GUI.Box(function1Rect, "Go back to the previous puzzle ("+game.puzzleGroups[game.puzzleGroupIndex-1].puzzles[game.puzzleGroups[game.puzzleGroupIndex-1].puzzles.Length-1].puzzleName+")", Graphics.inputInfoStyle);
				else GUI.Box(function1Rect, "Go back to the previous puzzle ("+game.puzzleGroup.puzzles[game.puzzleIndex-1].puzzleName+")", Graphics.inputInfoStyle);
			}
			else if (muteButtonRect.Contains(mouseGuiPosition) && mute) GUI.Box(function1Rect, "Click here to unmute the sound", Graphics.inputInfoStyle);
			else if (muteButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here to mute the sound", Graphics.inputInfoStyle);
			else if (menuButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Remember that welcome screen? This will take you back there.", Graphics.inputInfoStyle);
			else if (nextButtonRect.Contains(mouseGuiPosition) && !(!game.puzzleGroups[game.puzzleGroupIndex+1].puzzleGroupName.StartsWith("Tutorials") && game.puzzleIndex == game.puzzleGroup.puzzles.Length-1)) {
				if (game.puzzleIndex == game.puzzleGroup.puzzles.Length-1) GUI.Box(function1Rect, "Skip to the next puzzle ("+game.puzzleGroups[game.puzzleGroupIndex+1].puzzles[0].puzzleName+")", Graphics.inputInfoStyle);
				else GUI.Box(function1Rect, "Skip to the next puzzle ("+game.puzzleGroup.puzzles[game.puzzleIndex+1].puzzleName+")", Graphics.inputInfoStyle);
			}
			else if (goButtonRect.Contains(mouseGuiPosition) && graph.ready) GUI.Box(function1Rect, "Let's go sledding!", Graphics.inputInfoStyle);
			else if (goButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, graph.error1, Graphics.inputErrorStyle);
			else if (resetButtonRect.Contains(mouseGuiPosition) && game.puzzle.string1Start != "0") GUI.Box(function1Rect, "Stuck? Reset to Y="+game.puzzle.string1Start, Graphics.inputInfoStyle);
			else if (showUrl) {
				if (goButtonRect.Contains(mouseGuiPosition)) {
					if (Application.isWebPlayer) GUI.Box(function1Rect, "Click here to open the link (watch out for popup blockers)", Graphics.inputInfoStyle);
					else GUI.Box(function1Rect, "Click here to open the link", Graphics.inputInfoStyle);
				}
				else {
					urlString = game.GetURL();
					TextEditor urlEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
					GUI.SetNextControlName("url");
					GUI.TextField(function1Rect, urlString, Graphics.inputStyle);
					GUI.FocusControl("url");
					urlEditor.pos = 0;
					urlEditor.selectPos = urlString.Length;
				}
			}
			else if (game.puzzle.editable && urlButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here for a link to your puzzle and function", Graphics.inputInfoStyle);
			else if (urlButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here for a link to this puzzle and function", Graphics.inputInfoStyle);
			else showGraphString = true;
		}
		else {
			GUI.Box(topMenuRect, "", Graphics.infoStyle);

			//Lists

			GUI.Label(puzzleGroupLabelRect, graphics.folderIcon, Graphics.infoStyle);
			if (showPuzzleGroupList) {
				showPuzzleList = false;
				int puzzleGroupSelectionIndex = PopupList(puzzleGroupListRect, game.puzzleGroupIndex, puzzleGroupListContent, false);
				if (puzzleGroupSelectionIndex > -1) {
					game.SetGroup(puzzleGroupSelectionIndex);
					showPuzzleGroupList = false;
					GetComponent<AudioSource>().PlayOneShot(openMenuSound);
				}
				else if (puzzleGroupSelectionIndex == -2) {
					showPuzzleGroupList = false;
					GetComponent<AudioSource>().PlayOneShot(closeMenuSound);
				}
			}
			bool SHOWPUZZLEGROUPLIST = showPuzzleGroupList;
			showPuzzleGroupList = GUI.Toggle(puzzleGroupListRect, showPuzzleGroupList, puzzleGroupListButtonContent, Graphics.listStyle);
			if (showPuzzleGroupList && !SHOWPUZZLEGROUPLIST) GetComponent<AudioSource>().PlayOneShot(openMenuSound);
			else if (!showPuzzleGroupList && SHOWPUZZLEGROUPLIST) GetComponent<AudioSource>().PlayOneShot(closeMenuSound);

			if (game.puzzleGroup.puzzleGroupName != "Custom") {
				GUI.Label(puzzleLabelRect, graphics.puzzleIcon, Graphics.infoStyle);
				if (showPuzzleList) {
					showPuzzleGroupList = false;
					int puzzleSelectionIndex = PopupList(puzzleListRect, game.puzzleGroup.puzzleIndex, game.puzzleGroup.puzzleListContent, false);
					if (puzzleSelectionIndex > -1) {
						game.puzzleGroup.SetPuzzle(puzzleSelectionIndex);
						showPuzzleList = false;
						GetComponent<AudioSource>().PlayOneShot(openMenuSound);
					}
					else if (puzzleSelectionIndex == -2) {
						showPuzzleList = false;
						GetComponent<AudioSource>().PlayOneShot(closeMenuSound);
					}
				}
				bool SHOWPUZZLELIST = showPuzzleList;
				showPuzzleList = GUI.Toggle(puzzleListRect, showPuzzleList, game.puzzleGroup.puzzleListButtonContent, Graphics.listStyle);
				if (showPuzzleList && !SHOWPUZZLELIST) GetComponent<AudioSource>().PlayOneShot(openMenuSound);
				else if (!showPuzzleList && SHOWPUZZLELIST) GetComponent<AudioSource>().PlayOneShot(closeMenuSound);
			}

			// URL

			if (game.playing) GUI.Box(urlButtonRect, graphics.linkButtonIcon, Graphics.toggleDisabledStyle);
			else {
				bool SHOWURL = GUI.Toggle(urlButtonRect, showUrl, graphics.linkButtonIcon, Graphics.toggleStyle);
				if (!SHOWURL && showUrl) {
					GUI.FocusControl("");
					GetComponent<AudioSource>().PlayOneShot(closeSound);
				}
				else if (SHOWURL && !showUrl) GetComponent<AudioSource>().PlayOneShot(openSound);
				showUrl = SHOWURL;
			}

			//Mute Button

			bool MUTE = mute;
			if (mute) mute = GUI.Toggle(muteButtonRect, mute, graphics.muteIcon, Graphics.toggleStyle);
			else mute = GUI.Toggle(muteButtonRect, mute, graphics.unmuteIcon, Graphics.toggleStyle);
			AudioListener.pause = mute;
			if (MUTE && !mute) GetComponent<AudioSource>().PlayOneShot(buttonSound);

			GUI.skin.settings.cursorColor = Color.black;

	        // Record Button

//			string recordString = "";
//			if (game.playing || Application.isWebPlayer || mobile || Application.platform == RuntimePlatform.OSXPlayer) {
//				if (record) {
//					string filmStrip = "[~]";
//					int filmIndex = Mathf.FloorToInt(Time.time*4)%3;
//					recordString += filmStrip.Substring(filmIndex, 3-filmIndex);
//					recordString += filmStrip.Substring(0, filmIndex);
//					GUI.Box(recordButtonRect, recordString, Graphics.infoStyle);
//					canRecord = false;
//				}
//				else {
//					GUI.Box(recordButtonRect, graphics.recordButtonIcon, Graphics.toggleDisabledStyle);
//				}
//			}
//			else if (canRecord) record = GUI.Toggle(recordButtonRect, record, Graphics.instance.recordButtonIcon, Graphics.toggleStyle);
//			else {
//				for (i = 0; i < Mathf.FloorToInt(Time.time*2)%4; i++) recordString += "-";
//				GUI.Box(recordButtonRect, recordString, Graphics.infoStyle);
//				record = false;
//			}

		    //Editor Menu

		    if (game.puzzle.editable) {
		    	editorMenuRect = new Rect(puzzleGroupListRect.xMax, topMenuRect.y, menuRect.width-puzzleGroupListRect.xMax-menuRect.height/10, menuRect.height/20);
		    	coordinatesButtonRect = new Rect(editorMenuRect.x+0*menuRect.height/160, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	//swapButtonRect = new Rect(coordinatesButtonRect.xMax, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	addButtonRect = new Rect(coordinatesButtonRect.xMax+menuRect.height/160, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	deleteButtonRect = new Rect(addButtonRect.xMax, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	staticButtonRect = new Rect(deleteButtonRect.xMax+menuRect.height/160, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	dynamicButtonRect = new Rect(staticButtonRect.xMax, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	timerLabelRect = new Rect(dynamicButtonRect.xMax+menuRect.height/160, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	timerFieldRect = new Rect(timerLabelRect.xMax, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	orderLabelRect = new Rect(timerFieldRect.xMax+menuRect.height/160, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);
		    	orderFieldRect = new Rect(orderLabelRect.xMax, editorMenuRect.y, menuRect.height/20, editorMenuRect.height);

		    	if (game.playing) {
			    	if (game.puzzle.polar && game.puzzle.inverted) GUI.Box(coordinatesButtonRect, graphics.invertedButtonIcon, Graphics.buttonDisabledStyle);
			    	else if (game.puzzle.polar) GUI.Box(coordinatesButtonRect, graphics.polarButtonIcon, Graphics.buttonDisabledStyle);
			    	else GUI.Box(coordinatesButtonRect, graphics.cartesianButtonIcon, Graphics.buttonDisabledStyle);

		    		//GUI.Box(swapButtonRect, graph.variable0+graph.variable1, Graphics.buttonDisabledStyle);

		    		GUI.Box(addButtonRect, graphics.addButtonIcon, Graphics.buttonDisabledStyle);
			    	GUI.Box(deleteButtonRect, graphics.deleteButtonIcon, Graphics.buttonDisabledStyle);

			    	GUI.Box(staticButtonRect, graphics.staticButtonIcon, Graphics.buttonDisabledStyle);
			    	GUI.Box(dynamicButtonRect, graphics.dynamicButtonIcon, Graphics.buttonDisabledStyle);

			    	GUI.Box(timerLabelRect, graphics.timerLabelIcon, Graphics.labelDisabledStyle);
			    	GUI.Box(timerFieldRect, "--", Graphics.inputCompactDisabledStyle);

			    	GUI.Box(orderLabelRect, graphics.orderLabelIcon, Graphics.labelDisabledStyle);
			    	GUI.Box(orderFieldRect, "--", Graphics.inputCompactDisabledStyle);

			    	if (selectedObjective) selectedObjective.selected = false;
			    	selectedObjective = null;
			    	highlighter.gameObject.SetActive(false);
		    	}
		    	else {
			    	//GUI.Box(editorMenuRect, "", Graphics.infoStyle);
			    	if (game.puzzle.polar && game.puzzle.inverted) {
				    	if (GUI.Button(coordinatesButtonRect, graphics.invertedButtonIcon, Graphics.buttonStyle)) {
				    		game.puzzle.polar = false;
				    		game.puzzle.inverted = false;
							GetComponent<AudioSource>().PlayOneShot(buttonSound);
							if (graph.string1 == "1") graph.string1 = "0";
							if (game.puzzle.playerMarker.transform.position == Vector3.down) game.puzzle.playerMarker.transform.position = Vector3.zero;
//							else graphString = graphString.Replace()
				    	}
			    	}
			    	else if (game.puzzle.polar) {
				    	if (GUI.Button(coordinatesButtonRect, graphics.polarButtonIcon, Graphics.buttonStyle)) {
				    		game.puzzle.inverted = true;
							GetComponent<AudioSource>().PlayOneShot(buttonSound);
							if (game.puzzle.playerMarker.transform.position == Vector3.up) game.puzzle.playerMarker.transform.position = Vector3.down;
				    	}
			    	}
			    	else {
				    	if (GUI.Button(coordinatesButtonRect, graphics.cartesianButtonIcon, Graphics.buttonStyle)) {
				    		game.puzzle.polar = true;
							GetComponent<AudioSource>().PlayOneShot(buttonSound);
							if (graph.string1 == "0") graph.string1 = "1";
							if (game.puzzle.playerMarker.transform.position == Vector3.zero) game.puzzle.playerMarker.transform.position = Vector3.up;
				    	}
			    	}

//			    	if (GUI.Button(swapButtonRect, graph.variable0+graph.variable1, Graphics.buttonStyle)) {
//			    		game.puzzle.swapped = !game.puzzle.swapped;
//						audio.PlayOneShot(buttonSound);
//			    	}

			    	if (GUI.Button(addButtonRect, graphics.addButtonIcon, Graphics.buttonGreenStyle)) {
			    		if (selectedObjective) {
			    			selectedObjective.selected = false;
							selectedObjective = game.puzzle.AddObjective(selectedObjective.gameObject);
			    		}
			    		else selectedObjective = game.puzzle.AddObjective();
						selectedObjective.selected = true;
						GetComponent<AudioSource>().PlayOneShot(createSound);
			    	}
			    	if (selectedObjective) {
				    	if (GUI.Button(deleteButtonRect, graphics.deleteButtonIcon, Graphics.buttonRedStyle)) {
							game.puzzle.RemoveObjective(selectedObjective);
							GetComponent<AudioSource>().PlayOneShot(deleteSound);
						}
				    	if (selectedObjective.dynamic) {
				    		if (GUI.Button(staticButtonRect, graphics.staticButtonIcon, Graphics.buttonStyle)) {
								selectedObjective.MakeStatic();
								GetComponent<AudioSource>().PlayOneShot(buttonSound);
							}
							GUI.Box(dynamicButtonRect, graphics.dynamicButtonIcon, Graphics.buttonDisabledStyle);
				    	}
				    	else {
				    		if (GUI.Button(dynamicButtonRect, graphics.dynamicButtonIcon, Graphics.buttonStyle)) {
								selectedObjective.MakeDynamic();
								GetComponent<AudioSource>().PlayOneShot(buttonSound);
							}
							GUI.Box(staticButtonRect, graphics.staticButtonIcon, Graphics.buttonDisabledStyle);
				    	}

				    	//if (!GUI.Toggle(staticButtonRect, !selectedObjective.dynamic, graphics.staticButtonIcon, Graphics.toggleStyle) && !selectedObjective.dynamic) selectedObjective.MakeStatic();
				    	//if (GUI.Toggle(dynamicButtonRect, selectedObjective.dynamic, graphics.dynamicButtonIcon, Graphics.toggleStyle) && selectedObjective.dynamic) selectedObjective.MakeDynamic();
			    		
			    		float parseAttempt;
						GUI.SetNextControlName("timer");
			    		if (float.TryParse(GUI.TextField(timerFieldRect, selectedObjective.timeRequired.ToString(), 2, Graphics.inputCompactStyle), out parseAttempt)) selectedObjective.timeRequired = Mathf.Max(0, parseAttempt);
			    		if (GUI.Button(timerLabelRect, graphics.timerLabelIcon, Graphics.buttonStyle)) {
			    			GUI.FocusControl("");
							GUI.FocusControl("timer");
							GetComponent<AudioSource>().PlayOneShot(buttonSound);
			    		}

			    		string orderString;
						GUI.SetNextControlName("order");
			    		if (selectedObjective.orderIndex == -1) orderString = GUI.TextField(orderFieldRect, "", 1, Graphics.inputStyle).ToUpper();
			    		else orderString = GUI.TextField(orderFieldRect, alphabet[selectedObjective.orderIndex].ToString(), 1, Graphics.inputCompactStyle).ToUpper();
			    		if (orderString == "") selectedObjective.orderIndex = -1;
			    		else if (alphabet.Contains(orderString)) selectedObjective.orderIndex = alphabet.IndexOf(orderString);
			    		else selectedObjective.orderIndex = -1;
			    		if (GUI.Button(orderLabelRect, graphics.orderLabelIcon, Graphics.buttonStyle)) {
			    			GUI.FocusControl("");
							GUI.FocusControl("order");
							GetComponent<AudioSource>().PlayOneShot(buttonSound);
			    		}
			    	}
			    	else {
				    	GUI.Box(deleteButtonRect, graphics.deleteButtonIcon, Graphics.buttonDisabledStyle);
				    	GUI.Box(staticButtonRect, graphics.staticButtonIcon, Graphics.toggleDisabledStyle);
				    	GUI.Box(dynamicButtonRect, graphics.dynamicButtonIcon, Graphics.toggleDisabledStyle);
			    		GUI.Box(timerLabelRect, graphics.timerLabelIcon, Graphics.labelDisabledStyle);
			    		GUI.Box(timerFieldRect, "--", Graphics.inputCompactDisabledStyle);
				    	GUI.Box(orderLabelRect, graphics.orderLabelIcon, Graphics.labelDisabledStyle);
				    	GUI.Box(orderFieldRect, "--", Graphics.inputCompactDisabledStyle);
			    	}

					if (Input.GetMouseButtonDown(0) && !topMenuRect.Contains(mouseGuiPosition)) {
						if (selectedObjective) selectedObjective.selected = false;
						selectedObjective = null;
						for (i = 0; i < game.puzzle.objectives.Length; i++) {
							if (game.puzzle.objectives[i].GetComponent<Collider2D>().OverlapPoint(worldMousePosition)) {
								selectedObjective = game.puzzle.objectives[i];
								selectedObjective.selected = true;
								selectionPosition = Input.mousePosition;
								selectionOffset = selectedObjective.transform.position-worldMousePosition;
								highlighter.gameObject.SetActive(true);
								if (selectedObjective.dynamic) highlighter.sprite = graphics.dynamicHighlighterSprite;
								else highlighter.sprite = graphics.staticHighlighterSprite;
								movingSelectedObjective = true;
							}
						}
						if (selectedObjective == null && game.puzzle.playerMarker.GetComponent<Collider2D>().OverlapPoint(worldMousePosition)) {
							movingPlayer = true;
							selectionPosition = Input.mousePosition;
							selectionOffset = game.puzzle.playerMarker.transform.position-worldMousePosition;
						}
					}
					else if (movingPlayer && Input.GetMouseButton(0)) {
						game.puzzle.playerMarker.transform.position = game.window.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition)+selectionOffset;
						game.puzzle.playerMarker.transform.position = new Vector3(Mathf.Clamp(game.puzzle.playerMarker.transform.position.x, -100, 100), Mathf.Clamp(game.puzzle.playerMarker.transform.position.y, -100, 100), 0);
					}
					else if (movingPlayer && Input.GetMouseButtonUp(0)) {
						game.puzzle.playerMarker.transform.position = new Vector3(Mathf.Round(game.puzzle.playerMarker.transform.position.x*10)/10, Mathf.Round(game.puzzle.playerMarker.transform.position.y*10)/10, 0);
						game.puzzle.playerMarker.GetComponent<BoxCollider2D>().size = Vector2.one*game.cameraRect.height/16;
						movingPlayer = false;
					}
					else if (selectedObjective && movingSelectedObjective && Input.GetMouseButton(0)) {
						//Debug.Log(game.window.camera.ScreenToWorldPoint(Input.mousePosition));
						selectedObjective.transform.position = game.window.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition)+selectionOffset;
						selectedObjective.transform.position = new Vector3(Mathf.Clamp(selectedObjective.transform.position.x, -100, 100), Mathf.Clamp(selectedObjective.transform.position.y, -100, 100), 1);
						selectedObjective.startPosition = selectedObjective.transform.position;
					}
					else if (selectedObjective && Input.GetMouseButtonUp(0)) {
						selectedObjective.transform.position = new Vector3(Mathf.Round(selectedObjective.transform.position.x*10)/10, Mathf.Round(selectedObjective.transform.position.y*10)/10, 1);
						selectedObjective.startPosition = selectedObjective.transform.position;
						movingSelectedObjective = false;
					}

					if (selectedObjective) {
						highlighter.gameObject.SetActive(true);
						highlighter.transform.position = selectedObjective.transform.position;
						if (selectedObjective.dynamic) highlighter.sprite = graphics.dynamicHighlighterSprite;
						else highlighter.sprite = graphics.staticHighlighterSprite;
					}
					else highlighter.gameObject.SetActive(false);
				}
			}
			else {
		    	editorMenuRect = new Rect(0, 0, 0, 0);
		    	coordinatesButtonRect = new Rect(0, 0, 0, 0);
		    	swapButtonRect = new Rect(0, 0, 0, 0);
		    	addButtonRect = new Rect(0, 0, 0, 0);
		    	deleteButtonRect = new Rect(0, 0, 0, 0);
		    	staticButtonRect = new Rect(0, 0, 0, 0);
		    	dynamicButtonRect = new Rect(0, 0, 0, 0);
		    	timerLabelRect = new Rect(0, 0, 0, 0);
		    	timerFieldRect = new Rect(0, 0, 0, 0);
		    	orderLabelRect = new Rect(0, 0, 0, 0);
		    	orderFieldRect = new Rect(0, 0, 0, 0);
			}

			//GUI.Label(new Rect(width/2-height/2, 19*height/20, height/20, height/20), guiContentY, guiStyleY);
			if (game.playing) {
				if (function1Rect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Functions cannot be changed while sledding. To edit, hit STOP", Graphics.inputErrorStyle);
				else if (goButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "That's enough for now (it's cold out here!)", Graphics.inputInfoStyle);
				else if (resetButtonRect.Contains(mouseGuiPosition) && game.puzzle.string1Start != "0") GUI.Box(function1Rect, "Stuck? Reset to Y="+game.puzzle.string1Start, Graphics.inputInfoStyle);
				else GUI.Box(function1Rect, graph.string1, Graphics.inputDisabledStyle);
			}
			else {
				if (game.puzzle.editable && coordinatesButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here to change the coordinate system", Graphics.inputInfoStyle);
				else if (game.puzzle.editable && editorMenuRect.Contains(mouseGuiPosition) && !selectedObjective) {
					if (addButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here to add an objective", Graphics.inputInfoStyle);
					else GUI.Box(function1Rect, "Select an objective to edit it", Graphics.inputInfoStyle);
				}
				else if (game.puzzle.editable && addButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here to duplicate the selected objective", Graphics.inputInfoStyle);
				else if (game.puzzle.editable && deleteButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here to remove the selected objective", Graphics.inputInfoStyle);
				else if (game.puzzle.editable && dynamicButtonRect.Contains(mouseGuiPosition)) {
					if (selectedObjective.dynamic) GUI.Box(function1Rect, "This objective is dynamic, meaning it can move", Graphics.inputInfoStyle);
					else GUI.Box(function1Rect, "Click here to make the selected objective dynamic", Graphics.inputInfoStyle);
				}
				else if (game.puzzle.editable && staticButtonRect.Contains(mouseGuiPosition)) {
					if (!selectedObjective.dynamic) GUI.Box(function1Rect, "This objective is static, meaning it cannot move", Graphics.inputInfoStyle);
					else GUI.Box(function1Rect, "Click here to make the selected objective static", Graphics.inputInfoStyle);
				}
				else if (game.puzzle.editable && timerFieldRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here to set the selected objective's timer", Graphics.inputInfoStyle);
				else if (game.puzzle.editable && orderFieldRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here to set the order in which objectives must be completed", Graphics.inputInfoStyle);
//				else if (recordButtonRect.Contains(mouseGuiPosition) && Application.platform != RuntimePlatform.OSXPlayer && !Application.isWebPlayer) GUI.Box(function1Rect, "Click here to record a solution", Graphics.inputInfoStyle);
//				else if (recordButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "GIF recording is not yet supported on web browsers or OSX :(", Graphics.inputErrorStyle);
				else if (muteButtonRect.Contains(mouseGuiPosition) && mute) GUI.Box(function1Rect, "Click here to unmute the sound", Graphics.inputInfoStyle);
				else if (muteButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here to mute the sound", Graphics.inputInfoStyle);
				else if (resetButtonRect.Contains(mouseGuiPosition) && game.puzzle.string1Start != "0") GUI.Box(function1Rect, "Stuck? Reset to Y="+game.puzzle.string1Start, Graphics.inputInfoStyle);
				else if (puzzleGroupListRect.Contains(mouseGuiPosition) && !showPuzzleGroupList) GUI.Box(function1Rect, "These are the puzzle folders", Graphics.inputInfoStyle);
				else if (puzzleListRect.Contains(mouseGuiPosition) && !showPuzzleList && !game.puzzle.editable) GUI.Box(function1Rect, "These are the puzzles in the "+game.puzzleGroup.puzzleGroupName+" folder", Graphics.inputInfoStyle);
				else if (showUrl) {
					if (goButtonRect.Contains(mouseGuiPosition)) {
						if (Application.isWebPlayer) GUI.Box(function1Rect, "Click here to open the link (watch out for popup blockers)", Graphics.inputInfoStyle);
						else GUI.Box(function1Rect, "Click here to open the link", Graphics.inputInfoStyle);
					}
					else {
						urlString = game.GetURL();
						TextEditor urlEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						GUI.SetNextControlName("url");
						GUI.TextField(function1Rect, urlString, Graphics.inputStyle);
						GUI.FocusControl("url");
						urlEditor.pos = 0;
						urlEditor.selectPos = urlString.Length;
					}
				}
				else if (game.puzzle.editable && urlButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here for a link to your puzzle and function", Graphics.inputInfoStyle);
				else if (urlButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, "Click here for a link to this puzzle and function", Graphics.inputInfoStyle);
				else if (goButtonRect.Contains(mouseGuiPosition) && graph.ready) GUI.Box(function1Rect, "Let's go sledding!", Graphics.inputInfoStyle);
				else if (goButtonRect.Contains(mouseGuiPosition)) GUI.Box(function1Rect, graph.error1, Graphics.inputErrorStyle);
				else showGraphString = true;
			}
		}

		GUI.Box(variable1Rect, graph.variable1.ToUpper()+"=", Graphics.variableStyle);
		if (showGraphString) {
			GUI.SetNextControlName("function1");
			graph.string1 = GUI.TextField(function1Rect, graph.string1, Graphics.inputStyle);
		}

	    //Function 1 Box
		
//		if (textFieldType == 0) {
//			if (game.playing) GUI.Box(function1Rect, graph.string1, Graphics.inputDisabledStyle);
//			else {
//				GUI.SetNextControlName("function1");
//				graph.string1 = GUI.TextField(function1Rect, graph.string1, Graphics.inputStyle);
//			}
//		}
//		else if (textFieldType == 1) GUI.Box(function1Rect, textFieldString, Graphics.inputInfoStyle);
//		else GUI.Box(function1Rect, textFieldString, Graphics.inputErrorStyle);

		//Keyboard Events

		Event e = Event.current;

        if (e.keyCode == KeyCode.Return && e.type == EventType.KeyUp) {
        	if (game.playing && game.puzzleGroup.puzzleGroupName.StartsWith("Tutorials") && game.puzzle.complete && game.puzzleIndex == game.puzzleGroup.puzzles.Length-1) {
				game.puzzleGroup.SetPuzzle(0);
				game.SetGroup(game.puzzleGroupIndex+1);
				GetComponent<AudioSource>().PlayOneShot(stopSound);
        	}
        	else if (game.playing && game.playState == 2 && game.puzzle.complete) {
        		game.puzzleGroup.SetPuzzle(game.puzzle.index+1);
				GetComponent<AudioSource>().PlayOneShot(stopSound);
        	}
        	else if (game.playing) {
        		game.StopRiding();
				GetComponent<AudioSource>().PlayOneShot(stopSound);
        	}
        	else {
        		game.StartRiding();
				if (graph.ready) GetComponent<AudioSource>().PlayOneShot(startSound);
				else GetComponent<AudioSource>().PlayOneShot(errorSound);
        	}
        }
	}

	public void DrawKeypad () {
		if (graphics.portrait) {
			if (game.playing) {
				if (showKeypad) {
	        		Graphics.DrawKeypad(keypadRect, false);
	        		if (game.mobile) touchKeyboard.active = false;
				}
				else {
					if (game.mobile) touchKeyboard = TouchScreenKeyboard.Open(graph.string1, TouchScreenKeyboardType.NumberPad, false);
				}
			}
			else {
				if (showKeypad) {
	        		Graphics.DrawKeypad(keypadRect, true);
				}
				else {
				}
			}
		}
		else {
			if (game.playing) {

			}
	        else if (showKeypad) {
	        	string keypadInput = Graphics.DrawKeypad(keypadRect, true);
		    	TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

		 		if (keypadInput == "DEL" && graph.string1 != "" && (editor.pos != 0 || editor.pos != editor.selectPos)) {
		 			if (editor.pos != editor.selectPos) {
		 				graph.string1 = graph.string1.Remove(Mathf.Min(editor.pos, editor.selectPos), Mathf.Abs(editor.pos-editor.selectPos));
		 				editor.pos = Mathf.Min(editor.pos, editor.selectPos);
		 			}
		 			else {
		 				graph.string1 = graph.string1.Remove(editor.pos-1, 1);
		 				editor.pos = Mathf.Min(editor.pos, editor.selectPos)-1;
		 			}
		 			editor.selectPos = editor.pos;
		 		}
		 		else if (keypadInput != "" && keypadInput != "DEL") {
		 			if (editor.pos != editor.selectPos) {
		 				graph.string1 = graph.string1.Remove(Mathf.Min(editor.pos, editor.selectPos), Mathf.Abs(editor.pos-editor.selectPos));
		 				editor.pos = Mathf.Min(editor.pos, editor.selectPos);
		 				graph.string1 = graph.string1.Insert(editor.pos, keypadInput);
		 			}
		 			else {
		 				graph.string1 = graph.string1.Insert(editor.pos, keypadInput);
		 			}
		 			editor.pos += keypadInput.Length;
		 			editor.selectPos = editor.pos;
		 		}
	        }
	    }
	}

	public int PopupList (Rect listRect, int currentSelection, GUIContent[] listContent, bool above) {
		int toReturn = -1;
		bool contained = false;
		Rect itemRect = listRect;
		GUIStyle displayStyle;

		if (listRect.Contains(mouseGuiPosition)) contained = true;
		for (int i = 0; i < listContent.Length-1; i++) {
			if (above) itemRect = new Rect(listRect.x, itemRect.y-listRect.height, listRect.width, listRect.height);
			else itemRect = new Rect(listRect.x, itemRect.yMax, listRect.width, listRect.height);
			
			if (listContent[i].text == "Custom") displayStyle = Graphics.listBlueStyle;
			else if (listContent[i].text.StartsWith("Tutorials")) displayStyle = Graphics.listGreenStyle;
			else displayStyle = Graphics.listStyle;

			if (GUI.Button(itemRect, listContent[i], displayStyle)) toReturn = i;
			else if (itemRect.Contains(mouseGuiPosition)) contained = true;
		}

		if (!contained && Input.GetMouseButtonDown(0)) return -2;
		return toReturn;
	}
}
