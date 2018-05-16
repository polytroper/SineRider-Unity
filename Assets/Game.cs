using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	public int objectiveCount = 2;
	public float resolution = 8;
	public float playTime;
	public float time;

	[HideInInspector] public Transform window;
	[HideInInspector] public float windowSize = 10;
	[HideInInspector] public float windowZoom = 0;

	[HideInInspector] public Rect cameraRect;
	[HideInInspector] public Rect targetRect;
	[HideInInspector] public Rect graphRect;
	[HideInInspector] public bool cameraSmooth = false;

	public bool complete;
	public bool playing;
	public GameObject guiStack;
	public Color skyColor;
	
	public ParticleSystem snow;
	public GameObject objectivePrefab;
	public GameObject staticObjectivePrefab;
	public GameObject dynamicObjectivePrefab;
    public GameObject blockPrefab;
    public GameObject textPrefab;

//	[HideInInspector] public float windowX0;
//	[HideInInspector] public float windowX1;
	[HideInInspector] public bool mobileZooming = false;
	[HideInInspector] public float mobileZoomStartValue = 0;
	[HideInInspector] public float mobileZoomStartDistance = 0;

	public Puzzle puzzle;
	public PuzzleGroup puzzleGroup;
	public PuzzleGroup[] puzzleGroups;
	public int puzzleIndex = 0;
	public int puzzleGroupIndex = 0;

	public bool record;
	public bool canRecord = true;
	public int frames = 300;
    public float frameRate = 30f;
    public string recordingDir = "gifs";
    public string recordingName = "gif";

	public string queryString = "";
	public string startUrl;

	public int playState = 0;
    public bool mobile;

	public Rider rider;
	public Rider[] riders;
	public Menu menu;
	public Graph graph;
    public Graphics graphics;
    public static Game instance;

	public static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	void Awake () {

	}

	void Start () {
		int i;
		instance = this;
		window = Camera.main.transform;
		menu = FindObjectOfType<Menu>();
		rider = FindObjectOfType<Rider>();
		riders = FindObjectsOfType<Rider>();
		graphics = FindObjectOfType<Graphics>();
		puzzleGroups = FindObjectsOfType<PuzzleGroup>();

		puzzleGroupIndex = 0;
		Array.Sort(puzzleGroups, new PuzzleGroupSorter());
		puzzleGroup = puzzleGroups[puzzleGroupIndex];
		puzzle = puzzleGroup.puzzles[puzzleGroup.puzzleIndex];
		for (i = 0; i < puzzleGroups.Length; i++) puzzleGroups[i].gameObject.SetActive(false);
		puzzleGroup.gameObject.SetActive(true);
		puzzle.Activate();

		switch (Application.platform) {
			case RuntimePlatform.WindowsPlayer:
				break;
			case RuntimePlatform.OSXPlayer:
				break;
			case RuntimePlatform.LinuxPlayer:
				break;
		}

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			mobile = true;
			menu.touchKeyboard = TouchScreenKeyboard.Open(graph.string1, TouchScreenKeyboardType.NumberPad, false);
			menu.showKeypad = true;
		}
		else mobile = false;

		if (Application.isWebPlayer) Application.ExternalEval(String.Format("u.getUnity().SendMessage(\"{0}\", \"OnQueryString\", window.location.search);", gameObject.name));
		else if (Application.isEditor) OnQueryString(startUrl);

		Application.targetFrameRate = 60;
	}
	
	void Update () {
		if (playing) {
			if (graph.graphColliderEdge != null) graph.graphColliderEdge.isTrigger = false;
			if (graph.graphColliderPolygon != null) graph.graphColliderPolygon.isTrigger = false;
			time = Time.time-playTime;
		}
		else {
			if (graph.graphColliderEdge != null) graph.graphColliderEdge.isTrigger = true;
			if (graph.graphColliderPolygon != null) graph.graphColliderPolygon.isTrigger = true;
			playTime = Time.time;
			time = 0;
		}

		graph.polar = puzzle.polar;
		graph.inverted = puzzle.inverted;
		graph.parametric = puzzle.parametric;
		graph.swapped = puzzle.swapped;
		if (puzzle.inverted && puzzle.polar && !puzzle.swapped) {
			if (graph.valid) {
				Camera.main.backgroundColor = Color.white;
				graph.meshRenderer.material.color = skyColor;
			}
			else Camera.main.backgroundColor = skyColor;
		}
		else {
			Camera.main.backgroundColor = skyColor;
			graph.meshRenderer.material.color = Color.white;
		}

		UpdateCamera();

		snow.transform.position = new Vector3(window.position.x, window.position.y+cameraRect.height/2+2, 8);
		snow.transform.localScale = Vector3.one*Mathf.Pow(6, 1+Mathf.Ceil(Mathf.Log(1+cameraRect.width, 12)));
		snow.emissionRate = snow.transform.localScale.x/32;
		snow.startLifetime = cameraRect.height;

		if (playState == 0 || playState == 1) snow.gameObject.SetActive(true);
		else snow.gameObject.SetActive(false);

		if (puzzleGroup.index == 0 && puzzleIndex == 0) {
			if (playing) playState = 1;
			else playState = 0;
		}
		else if (puzzleGroup.puzzleGroupName.StartsWith("Tutorials") && puzzleIndex != 0 && playState != 2) playState = 3;
		else if (playState != 2) playState = 3;
	}
	
	public void UpdateCamera () {
		int i;
		Vector3 midPoint = Vector3.zero;
		midPoint += rider.bodyCenter;
		for (i = 0; i < puzzle.objectives.Length; i++) midPoint += puzzle.objectives[i].transform.position;
		midPoint /= puzzle.objectives.Length+2;
		if (puzzle.polar) midPoint = Vector3.zero;

		windowSize = 0;
		for (i = 0; i < riders.Length; i++) windowSize = Mathf.Max(Mathf.Max(Mathf.Abs((midPoint-riders[i].transform.position).x), Mathf.Abs((midPoint-riders[i].transform.position).y)), windowSize);
		for (i = 0; i < puzzle.objectives.Length; i++) windowSize = Mathf.Max(Mathf.Max(Mathf.Abs((midPoint-puzzle.objectives[i].transform.position).x), Mathf.Abs((midPoint-puzzle.objectives[i].transform.position).y)), windowSize);

		if (Input.touchCount == 2 && mobileZooming) windowZoom = mobileZoomStartValue*(Input.touches[0].position-Input.touches[1].position).magnitude/mobileZoomStartDistance;
		else if (Input.touchCount == 2) {
			mobileZooming = true;
			mobileZoomStartValue = windowZoom;
			mobileZoomStartDistance = (Input.touches[0].position-Input.touches[1].position).magnitude;
		}
		else if (mobileZooming) {
			mobileZoomStartValue = 0;
			mobileZoomStartDistance = 0;
			mobileZooming = false;
		}

		windowSize = Mathf.Max(4, windowSize);
		windowZoom = Mathf.Max(0, windowZoom-Input.GetAxis("Mouse ScrollWheel")*4);
		if (Input.GetAxis("Mouse ScrollWheel") != 0) cameraSmooth = true;
		windowSize += windowZoom;
		windowSize += puzzle.zoomPad;
		if (graphics.portrait) window.GetComponent<Camera>().rect = new Rect(0, 1-graphics.aspect, 1, graphics.aspect);
		else window.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
//		window.camera.orthographicSize = windowSize;
//		windowX0 = window.position.x-windowSize*Mathf.Max(1, graphics.aspect);
//		windowX1 = window.position.x+windowSize*Mathf.Max(1, graphics.aspect);

		cameraRect = new Rect(0, 0, 0, 0);
		cameraRect.height = Camera.main.orthographicSize*2;
		cameraRect.width = Camera.main.orthographicSize*graphics.aspect*2;
		cameraRect.center = (Vector2)window.position;

		targetRect = new Rect(0, 0, 0, 0);
		targetRect.height = windowSize*2;
		targetRect.width = windowSize*graphics.aspect*2;
		targetRect.center = (Vector2)midPoint;
		
		graphRect = new Rect(0, 0, 0, 0);
		graphRect.xMin = Mathf.Min(cameraRect.xMin, targetRect.xMin);
		graphRect.yMin = Mathf.Min(cameraRect.yMin, targetRect.yMin);
		graphRect.xMax = Mathf.Max(cameraRect.xMax, targetRect.xMax);
		graphRect.yMax = Mathf.Max(cameraRect.yMax, targetRect.yMax);

//		if (puzzle.polar && puzzle.inverted) Camera.main.transform.rotation = Quaternion.LookRotation(Vector3.forward, -rider.bodyCenter);
//		else if (puzzle.polar) Camera.main.transform.rotation = Quaternion.LookRotation(Vector3.forward, rider.bodyCenter);
//		else Camera.main.transform.rotation = Quaternion.identity;

		if (graph.polar) {
//			window.transform.position = Vector3.back*16;
//			Camera.main.orthographicSize = 20;
//			windowSize = 20;
		}

		Camera.main.transform.rotation = Quaternion.identity;
		//Camera.main.transform.localScale = Vector3.one*windowSize/256;
		//guiStack.transform.position = Camera.main.transform.position+Vector3.forward*8;
	}

	public void SetGroup (int groupIndex) {
		puzzle.Deactivate();
		puzzleGroup.gameObject.SetActive(false);
		puzzleGroupIndex = groupIndex;
		puzzleGroup = puzzleGroups[puzzleGroupIndex];
		puzzleGroup.gameObject.SetActive(true);
		puzzle = puzzleGroup.puzzle;
		puzzleIndex = puzzleGroup.puzzleIndex;
		puzzle.Activate();

		graph.string1 = puzzle.string1;
		cameraSmooth = false;
		StopRiding();
	}

	public void StopRiding () {
		playing = false;
		if (graph.graphColliderEdge != null) graph.graphColliderEdge.isTrigger = true;
		if (graph.graphColliderPolygon != null) graph.graphColliderPolygon.isTrigger = true;
		puzzle.Reset();
		menu.showUrl = false;
		cameraSmooth = false;
	}

	public void StartRiding () {
		cameraSmooth = true;
		menu.showUrl = false;
		if (graph.ready) {
			playing = true;
			playTime = Time.time;
			if (graph.graphColliderEdge != null) graph.graphColliderEdge.isTrigger = false;
			if (graph.graphColliderPolygon != null) graph.graphColliderPolygon.isTrigger = false;
			if (record) {
				recordingName = puzzle.puzzleName;
 	        	CaptureTheGIF.Instance.Capture(frames, (int)menu.menuRect.width, (int)menu.menuRect.width, frameRate, recordingDir, recordingName, false);
			}
		}
	}
	public string GetURL () {
		string toReturn = "sineridergame.com/SineRider.html?";
		//toReturn += "f="+puzzleGroup.index+"p="+puzzle.index+"y="+System.Uri.EscapeDataString(graph.graphString);
		toReturn += "p="+puzzle.identifier+"y="+System.Uri.EscapeDataString(graph.string1);
		if (puzzle.editable) {
			toReturn += "e=";

			toReturn += "px="+puzzle.playerMarker.transform.position.x;
			toReturn += "py="+puzzle.playerMarker.transform.position.y;

			if (puzzle.polar) toReturn += "cp="+1;
			else toReturn += "cp="+0;

			if (puzzle.inverted) toReturn += "ci="+1;
			else toReturn += "ci="+0;

			if (puzzle.parametric) toReturn += "ca="+1;
			else toReturn += "ca="+0;

			if (puzzle.swapped) toReturn += "cs="+1;
			else toReturn += "cs="+0;

			foreach (Objective objective in puzzle.objectives) {
				toReturn += "o=";
				toReturn += "ox="+objective.transform.position.x;
				toReturn += "oy="+objective.transform.position.y;
				if (objective.dynamic) toReturn += "od=1";
				else toReturn += "od=0";
				toReturn += "ot="+objective.timeRequired;
				toReturn += "oi="+objective.orderIndex;
			}
		}
		return toReturn;
	}

	void OnQueryString (string value) {
		try {
			queryString = value;
			if (queryString != "") {
				queryString = queryString.Substring(1);

				//int queryFolderIndex = queryString.IndexOf("f=")+2;
				int queryPuzzleIndex = queryString.IndexOf("p=")+2;
				int queryFunctionIndex = queryString.IndexOf("y=")+2;
				int queryEditorIndex;
				if (queryString.Contains("e=")) queryEditorIndex = queryString.IndexOf("e=")+2;
				else queryEditorIndex = queryString.Length+2;

				//string queryFolder = queryString.Substring(queryFolderIndex, queryPuzzleIndex-queryFolderIndex-2);
				string queryPuzzle = queryString.Substring(queryPuzzleIndex, queryFunctionIndex-queryPuzzleIndex-2);
				string queryFunction = queryString.Substring(queryFunctionIndex, queryEditorIndex-queryFunctionIndex-2);
				string queryEditor;
				if (queryString.Contains("e=")) queryEditor = queryString.Substring(queryEditorIndex);
				else queryEditor = "";

				//Debug.Log ("p: "+queryString.IndexOf("p=")+" | y: "+queryString.IndexOf("y="));
				Debug.Log ("Puzzle: "+queryPuzzle+" | Function: "+System.Uri.UnescapeDataString(queryFunction));

				int i;
				int j;
				for (i = 0; i < puzzleGroups.Length; i++) {
					for (j = 0; j < puzzleGroups[i].puzzles.Length; j++) {
						if (puzzleGroups[i].puzzles[j].identifier == queryPuzzle) {
							SetGroup(i);
							puzzleGroup.SetPuzzle(j);
						}
					}
				}

				//SetGroup(int.Parse(queryFolder));
				//puzzleGroup.SetPuzzle(int.Parse(queryPuzzle));
				graph.string1 = System.Uri.UnescapeDataString(queryFunction);

				if (queryEditor != "" && puzzle.editable) {
					if (queryEditor.StartsWith("px=")) {
						int queryPlayerXIndex = queryEditor.IndexOf("px=")+3;
						int queryPlayerYIndex = queryEditor.IndexOf("py=")+3;
						int queryPolarIndex = queryEditor.IndexOf("cp=")+3;
						int queryInvertedIndex = queryEditor.IndexOf("ci=")+3;
						int queryParametricIndex = queryEditor.IndexOf("ca=")+3;
						int querySwappedIndex = queryEditor.IndexOf("cs=")+3;

						string queryPlayerX = queryEditor.Substring(queryPlayerXIndex, queryPlayerYIndex-queryPlayerXIndex-3);
						string queryPlayerY = queryEditor.Substring(queryPlayerYIndex, queryPolarIndex-queryPlayerYIndex-3);
						string queryPolar = queryEditor.Substring(queryPolarIndex, queryInvertedIndex-queryPolarIndex-3);
						string queryInverted = queryEditor.Substring(queryInvertedIndex, queryParametricIndex-queryInvertedIndex-3);
						string queryParametric = queryEditor.Substring(queryParametricIndex, querySwappedIndex-queryParametricIndex-3);
						string querySwapped = queryEditor.Substring(querySwappedIndex);

						puzzle.playerMarker.transform.position = new Vector3(float.Parse(queryPlayerX), float.Parse(queryPlayerY), 0);

						if (queryPolar == "1") puzzle.polar = true;
						if (queryInverted == "1") puzzle.inverted = true;
						if (queryParametric == "1") puzzle.parametric = true;
						if (querySwapped.StartsWith("1")) puzzle.swapped = true;
	
						queryEditor = queryEditor.Substring(querySwappedIndex+1);
					}

					puzzle.RemoveAll();
					Objective newObjective;

					int objectiveXIndex;
					int objectiveYIndex;
					int objectiveDynamicIndex;
					int objectiveTimerIndex;
					int objectiveOrderIndex;
					int nextObjectiveIndex;
					
					string objectiveX;
					string objectiveY;
					string objectiveDynamic;
					string objectiveTimer;
					string objectiveOrder;
					
					while (queryEditor.Contains("o=")) {
						queryEditor = queryEditor.Substring(2);
						objectiveXIndex = queryEditor.IndexOf("ox=")+3;
						objectiveYIndex = queryEditor.IndexOf("oy=")+3;
						objectiveDynamicIndex = queryEditor.IndexOf("od=")+3;
						objectiveTimerIndex = queryEditor.IndexOf("ot=")+3;
						objectiveOrderIndex = queryEditor.IndexOf("oi=")+3;
						if (queryEditor.Contains("o=")) nextObjectiveIndex = queryEditor.IndexOf("o=");
						else nextObjectiveIndex = queryEditor.Length;

						objectiveX = queryEditor.Substring(objectiveXIndex, objectiveYIndex-objectiveXIndex-3);
						objectiveY = queryEditor.Substring(objectiveYIndex, objectiveDynamicIndex-objectiveYIndex-3);
						objectiveDynamic = queryEditor.Substring(objectiveDynamicIndex, objectiveTimerIndex-objectiveDynamicIndex-3);
						objectiveTimer = queryEditor.Substring(objectiveTimerIndex, objectiveOrderIndex-objectiveTimerIndex-3);
						objectiveOrder = queryEditor.Substring(objectiveOrderIndex, nextObjectiveIndex-objectiveOrderIndex);

						Debug.Log(objectiveX+" | "+objectiveY+" | "+objectiveDynamic+" | "+objectiveTimer+" | "+objectiveOrder);

						newObjective = puzzle.AddObjective();
						newObjective.transform.position = new Vector3(float.Parse(objectiveX), float.Parse(objectiveY), 1);
						if (objectiveDynamic == "0") newObjective.MakeStatic();
						else newObjective.MakeDynamic();
						newObjective.timeRequired = float.Parse(objectiveTimer);
						newObjective.orderIndex = int.Parse(objectiveOrder);

						queryEditor = queryEditor.Substring(nextObjectiveIndex);
					}
				}
			}
		}
		catch (Exception ex) {
			Debug.Log(ex.Message);
			SetGroup(0);
			puzzleGroup.SetPuzzle(0);
		}
	}

	public void FixedUpdate () {
		if (cameraSmooth) {
			window.position = Vector3.Lerp(window.position, new Vector3(targetRect.center.x, targetRect.center.y, -16), 0.03f);
			window.GetComponent<Camera>().orthographicSize = Mathf.Lerp(window.GetComponent<Camera>().orthographicSize, windowSize, 0.05f);
		}
		else {
			window.position = new Vector3(targetRect.center.x, targetRect.center.y, -16);
			window.GetComponent<Camera>().orthographicSize = targetRect.height/2;
		}
	}
}