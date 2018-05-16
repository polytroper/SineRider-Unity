using UnityEngine;
using System.Collections;
using YAMP;

public class NodeGraph : MonoBehaviour {
	public string graphString;
	public int samples = 8;

	public bool drawMesh;
	public float length;
	public float scale = 0.1f;
	public float windowX0;
	public float windowX1;
	public float windowSize = 0.5f;

	private Parser parser;
	private LineRenderer graphLine;

	private Mesh graphMesh;
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;

	private Vector2[] edgePoints;
	private Vector2[] polygonPoints;

	// Use this for initialization
	void Start () {
		graphLine = GetComponent<LineRenderer>();
		meshRenderer = GetComponent<MeshRenderer>();
		Parser.Load();

		if (drawMesh) {
			graphMesh = new Mesh();
			meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = graphMesh;
		}
	}
	
	// Update is called once per frame
	void Update () {
		windowX0 = -1/scale;
		windowX0 = 1/scale;

		if (graphString != "") {
//			try {
				Parser.AddCustomConstant("x", 0);
				Parser.AddCustomConstant("X", 0);
				parser = Parser.Parse(graphString);
				//float testValue = (float)((ScalarValue)parser.Execute()).Value;
//			}
//			catch (YAMPParseException ex0) {
//			}
//			catch (YAMPSymbolMissingException ex1) {
//			}
//			catch (YAMPArgumentNumberException ex2) {
//			}

			GenerateGraph();
		}
	}

	void GenerateGraph () {
		int resolvedWidth = samples+1;
		float xValue;
		float yValue;
		Vector2 xy;
		edgePoints = new Vector2[resolvedWidth+1];
		polygonPoints = new Vector2[resolvedWidth+3];
		graphLine.SetVertexCount(resolvedWidth+1);
		length = 0;
		float lowestPoint = float.PositiveInfinity;
		for (int i = 0; i < resolvedWidth+1; i++) {
			xValue = Mathf.Lerp(windowX0, windowX1, (float)i/resolvedWidth);
			yValue = GetY(xValue);
			
			xy = new Vector2(xValue+transform.position.x, yValue+transform.position.y);
			edgePoints[i] = xy;
			polygonPoints[i] = xy+new Vector2(0, -0.0f);
			graphLine.SetPosition(i, xy);
			if (i > 0) length += Vector2.Distance(xy, edgePoints[i-1]);
			if (xy.y < lowestPoint) lowestPoint = xy.y;
		}
		polygonPoints[resolvedWidth+1] = new Vector2(windowX1, Mathf.Min(transform.position.y-windowSize, lowestPoint-1));
		polygonPoints[resolvedWidth+2] = new Vector2(windowX0, Mathf.Min(transform.position.y-windowSize, lowestPoint-1));

		if (drawMesh) {
			Triangulator triangulator = new Triangulator(polygonPoints);
 
	        // Use the triangulator to get indices for creating triangles
	        int[] indices = triangulator.Triangulate();
	 
	        // Create the Vector3 vertices
	        Vector3[] vertices = new Vector3[polygonPoints.Length];
	        for (int i = 0; i < polygonPoints.Length; i++) {
	            vertices[i] = new Vector3(polygonPoints[i].x, polygonPoints[i].y, transform.position.z+4);
	        }
	 
	        // Create the mesh
	        graphMesh.vertices = vertices;
			graphMesh.uv = polygonPoints;
	        graphMesh.triangles = indices;
	        graphMesh.RecalculateNormals();
	        graphMesh.RecalculateBounds();
		}
	}

	public float GetY (float xValue) {
		float yValue;

		Parser.AddCustomConstant("x", (double)xValue);
		Parser.AddCustomConstant("X", (double)xValue);
		try	{
			yValue = (float)((ScalarValue)parser.Execute()).Value;
		}
		catch (YAMPParseException ex0) {
			yValue = 0;
		}
		catch (YAMPSymbolMissingException ex1) {
			yValue = 0;
		}

		if (float.IsNaN(yValue) || float.IsInfinity(yValue)) yValue = 0;

		//yValue = expressionGeneric.Evaluate();
		return yValue;
	}
}
