using UnityEngine;
using System.Collections;

public class StackTreeEditor : MonoBehaviour {
	public TreeNode selectedNode;
	public GameObject highLighter;

	public bool horizontal;

	private Ray ray;
	private RaycastHit hit;

	public GameObject treeNodePrefab;
	public static StackTreeEditor editor;

	void Start () {
		TreeNode.prefab = treeNodePrefab;
		editor = this;
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				TreeNode node = hit.transform.GetComponent<TreeNode>();
				if (node != null) {
					if (selectedNode != null) selectedNode.selected = false;
					selectedNode = node;
					selectedNode.selected = true;
				}
			}
			else selectedNode = null;
		}
		if (selectedNode != null) {
			highLighter.SetActive(true);
			highLighter.transform.position = new Vector3(selectedNode.transform.position.x, selectedNode.transform.position.y, -1);
			selectedNode.SetOpen(true);
		}
		else highLighter.SetActive(false);

		if (Input.GetKeyDown(KeyCode.R)) {
			if (selectedNode) selectedNode.Randomize();
		}
		if (Input.GetKeyDown(KeyCode.O)) {
			//if (selectedNode) selectedNode.SetOpen(!selectedNode.open);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			if (selectedNode) selectedNode.nodeType = Mathf.Min(selectedNode.nodeType+1, 8);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			if (selectedNode) selectedNode.nodeType = Mathf.Max(selectedNode.nodeType-1, 0);
		}
	}
}
