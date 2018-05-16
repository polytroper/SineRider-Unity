using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	public LineRenderer xAxis;
	public LineRenderer yAxis;

	private Game game;
	private Graphics graphics;

	void Start () {
		game = FindObjectOfType<Game>();
		graphics = FindObjectOfType<Graphics>();
	}
	
	void Update () {
		Vector3 x0 = new Vector3(game.cameraRect.xMin, 0, 4);
		Vector3 x1 = new Vector3(game.cameraRect.xMax, 0, 4);
		Vector3 y0 = new Vector3(0, game.cameraRect.yMin, 4);
		Vector3 y1 = new Vector3(0, game.cameraRect.yMax, 4);
		xAxis.SetPosition(0, x0);
		xAxis.SetPosition(1, x1);
		yAxis.SetPosition(0, y0);
		yAxis.SetPosition(1, y1);
		xAxis.SetWidth(game.windowSize/200, game.windowSize/200);
		yAxis.SetWidth(game.windowSize/200, game.windowSize/200);
	}
}
