using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeNode : MonoBehaviour {
	public int nodeType = 0;
	public bool open = false;
	public bool visible = false;
	public bool vertical = false;
	public string argument;

	public int depth;
	public Vector3 currentPosition;
	public Vector3 visiblePosition;
	public Vector3 invisiblePosition;

	public TMPro.TextMeshPro textMesh;
	public string textString;
	public SpriteRenderer spriteRenderer;
	public int index;
	public TreeNode parentNode;
	public List<TreeNode> subNodes;
	public bool selected = false;

	public static GameObject prefab;
	public string[] typeStrings = {"+", "-", "*", "/", "^", "sin", "cos", "tan"};
	public string functionString;
	public NodeGraph graph;

	public StackTreeEditor editor;

	void Start () {
		editor = StackTreeEditor.editor;
	}
	
	void Update () {
		visible = GetVisible();
		GetComponent<Renderer>().enabled = visible;
		textMesh.enabled = visible;
		GetComponent<Collider>().enabled = visible;
		currentPosition = GetPosition();
		transform.position = currentPosition;

		if (nodeType == 0) spriteRenderer.color = Color.blue;
		else if (nodeType == 1) spriteRenderer.color = Color.white;
		else spriteRenderer.color = Color.gray;

		bool lowLight = true;
		if (parentNode) {
			if (parentNode.selected) lowLight = false;
		}
		if (selected) lowLight = false;
		if (lowLight) spriteRenderer.color *= 0.5f;

		if (nodeType == 0) textString = "()";
		else if (nodeType == 1) textString = argument;
		else if (nodeType > 1) textString = typeStrings[nodeType-1];
		textMesh.text = textString;

		functionString = GetString();

		if (nodeType == 0) graph.graphString = functionString;
		else graph.graphString = "";
	}

	public void SetOpen (bool state) {
		if (state) {
			if (parentNode)	parentNode.CloseAll();
			open = true;
		}
		else open = false;
	}

	void OnGUI () {
		if (!parentNode) {
			float width = Screen.width;
			float height = Screen.height;
			GUI.Box(new Rect(width/2-height/2, 19*height/20, height, height/20), functionString, Graphics.inputStyle);
		}
		else if (selected) {
			float width = Screen.width;
			float height = Screen.height;
			GUI.Box(new Rect(width/2-height/2, 9*height/10, height, height/20), functionString, Graphics.inputStyle);
		}
	}
	
//	void SetVisible (bool state) {
//		int i;
//		for (i = 0; i < subNodes.Count; i++) {
//			
//		}
//	}

	public bool GetVisible () {
		if (parentNode == null) return true;
		if (!parentNode.open) return false;
		return parentNode.GetVisible();
	}

	public Vector3 GetPosition () {
		if (parentNode == null) return transform.position;
		if (editor.horizontal) {
			if (visible) {
				return parentNode.GetPosition()+new Vector3(index, -1, 0);
			}
			return parentNode.GetPosition()+new Vector3(0, 0, 1);
		}
		if (visible) {
			if (vertical) return parentNode.GetPosition()+new Vector3(0, -(index+1), 0);
			return parentNode.GetPosition()+new Vector3(index+1, 0, 0);
		}
		return parentNode.GetPosition()+new Vector3(0, 0, 1);
	}

	public float Evaluate (float input) {
		float toReturn = input;
		if (index == 0) return Arguments.Parse(argument);
		switch (nodeType) {
			case 0:
				for (int i = 0; i < subNodes.Count; i++) toReturn = subNodes[i].Evaluate(toReturn);
				break;
			case 1:
				toReturn += Arguments.Parse(argument);
				break;
			case 2:
				toReturn += Arguments.Parse(argument);
				break;
			case 3:
				toReturn -= Arguments.Parse(argument);
				break;
			case 4:
				toReturn *= Arguments.Parse(argument);
				break;
			case 5:
				toReturn /= Arguments.Parse(argument);
				break;
			case 6:
				toReturn = Mathf.Pow(toReturn, Arguments.Parse(argument));
				break;
			case 7:
				toReturn = Mathf.Sin(toReturn);
				break;
			case 8:
				toReturn = Mathf.Cos(toReturn);
				break;
			case 9:
				toReturn = Mathf.Tan(toReturn);
				break;
		}
		return toReturn;
	}

	public string GetString () {
		return GetString("");
	}

	public string GetString (string input) {
		string toReturn = input;
		switch (nodeType) {
			case 0:
				toReturn += "(";
				for (int i = 0; i < subNodes.Count; i++) toReturn = ""+subNodes[i].GetString(toReturn)+"";
				toReturn += ")";
				break;
			case 1:
				toReturn += argument;
				break;
			case 2:
				toReturn += "+";
				break;
			case 3:
				toReturn += "-";
				break;
			case 4:
				toReturn += "*";
				break;
			case 5:
				toReturn += "/";
				break;
			case 6:
				toReturn += "^";
				break;
			case 7:
				toReturn = "sin("+toReturn+")";
				break;
			case 8:
				toReturn = "cos("+toReturn+")";
				break;
			case 9:
				toReturn = "tan("+toReturn+")";
				break;
		}
		return toReturn;
	}

	public void Randomize () {
		int i;
		if (nodeType == 0) {
			DeleteNodes();
			subNodes.Clear();
			TreeNode newNode;
			int randomSize = 2*Random.Range(1, 4)+1;
			for (i = 0; i < randomSize; i++) {
				newNode = (Instantiate(prefab) as GameObject).GetComponent<TreeNode>();
				newNode.depth = depth+1;
				newNode.vertical = !vertical;
				newNode.parentNode = this;
				newNode.index = i;
				if (i%2 == 0) {
					if (Random.value < 0.2f) newNode.nodeType = 0;
					else newNode.nodeType = 1;
				}
				else newNode.nodeType = 2;
				newNode.Randomize();
				subNodes.Add(newNode);
			}
		}
		else if (nodeType == 1) {
			argument = Random.Range(0, 10).ToString();
		}
		else {
			nodeType = Random.Range(2, 7);
		}
	}

	public void Delete () {
		DeleteNodes();
		Destroy(gameObject);
	}

	public void DeleteNodes () {
		for (int i = 0; i < subNodes.Count; i++) {
			subNodes[i].Delete();
			Destroy(subNodes[i].gameObject);
		}
	}

	public void CloseAll () {
		for (int i = 0; i < subNodes.Count; i++) {
			subNodes[i].open = false;
		}
	}
}
