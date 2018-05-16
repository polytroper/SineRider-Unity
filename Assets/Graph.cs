using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YAMP;

public class Graph : MonoBehaviour {
	public string string0;
	public string string1;
	public string variable0 = "x";
	public string variable1 = "y";
	public float resolution = 8;

	public bool polar;
	public bool inverted;
	public bool parametric;
	public bool swapped;
	public bool drawMesh;
	public bool solid;

	public bool ready;
	public bool valid;
	public bool valid0;
	public bool valid1;
	public string error;
	public string error0;
	public string error1;
	[HideInInspector] public bool validHeightPlayer = true;
	[HideInInspector] public bool validHeightObjective = true;

	public float[] values0;
	public float[] values1;
	public Vector2[] points;
	public float length;
	public float thickness = 1;
	public float dashLength = 4;

	public Parser parser;
	public Parser parser0;
	public Parser parser1;
	public ParseContext parseContext;

	//[HideInInspector]
	public Mesh graphMesh;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;
	//public bool local;
	private Vector2[] edgePoints;
	private Vector2[] polygonPoints;

	public LineRenderer graphLine;
	public EdgeCollider2D graphColliderEdge;
	public PolygonCollider2D graphColliderPolygon;

	public Graph shadowGraph;
	public float shadowLimit;
	public float shadowOffset;

	private Game game;
	private Rider rider;
	private Rider[] riders;
	
	private string[] implicitStrings = {"0x", "1x", "2x", "3x", "4x", "5x", "6x", "7x", "8x", "9x", ")x", "x0", "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9", "x(",
	                                    "0t", "1t", "2t", "3t", "4t", "5t", "6t", "7t", "8t", "9t", ")t", "t0", "t1", "t2", "t3", "t4", "t5", "t6", "t7", "t8", "t9", "t(",
	                                    "0(", "1(", "2(", "3(", "4(", "5(", "6(", "7(", "8(", "9(", ")0", ")1", ")2", ")3", ")4", ")5", ")6", ")7", ")8", ")9", ")("};
	private string[] explicitStrings = {"0*x", "1*x", "2*x", "3*x", "4*x", "5*x", "6*x", "7*x", "8*x", "9*x", ")*x", "x*0", "x*1", "x*2", "x*3", "x*4", "x*5", "x*6", "x*7", "x*8", "x*9", "x*(",
	                                    "0*t", "1*t", "2*t", "3*t", "4*t", "5*t", "6*t", "7*t", "8*t", "9*t", ")*t", "t*0", "t*1", "t*2", "t*3", "t*4", "t*5", "t*6", "t*7", "t*8", "t*9", "t*(",
	                                    "0*(", "1*(", "2*(", "3*(", "4*(", "5*(", "6*(", "7*(", "8*(", "9*(", ")*0", ")*1", ")*2", ")*3", ")*4", ")*5", ")*6", ")*7", ")*8", ")*9", ")*("};


	void Start () {
		game = Game.instance;
		rider = game.rider;
		riders = game.riders;

		graphLine = GetComponent<LineRenderer>();
		meshRenderer = GetComponent<MeshRenderer>();
		graphColliderEdge = GetComponent<EdgeCollider2D>();
		graphColliderPolygon = GetComponent<PolygonCollider2D>();
		if (graphColliderEdge != null) {
			if (!graphColliderEdge.enabled) graphColliderEdge = null;
		}
		if (graphColliderPolygon != null) {
			if (!graphColliderPolygon.enabled) graphColliderPolygon = null;
		}
		if (!solid) GetComponent<Collider2D>().enabled = false;

		if (drawMesh) {
			graphMesh = new Mesh();
			meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = graphMesh;
		}

		parseContext = Parser.Load();

		Parser.AddVariable("x", new ScalarValue(0));
		Parser.AddVariable("a", new ScalarValue(0));
		Parser.AddVariable("t", new ScalarValue(0));
		Parser.AddVariable("px", new ScalarValue(0));
		Parser.AddVariable("py", new ScalarValue(0));
	}

	void Update () {
		int i;
		if (shadowGraph != null) {
			string0 = shadowGraph.string0;
			string1 = shadowGraph.string1;
		}
		if (game.playing) {

		}
		else {
			if (polar) {
				variable0 = "a";
				variable1 = "r";
			}
			else {
				variable0 = "x";
				variable1 = "y";
			}
			if (swapped) {
				string swapVariable = variable0;
				variable0 = variable1;
				variable1 = swapVariable;
			}
				
			if (!validHeightPlayer) error1 = "The sled must be above the hill";
			else if (!validHeightObjective) error1 = "All rolling objectives must be above the hill";
			
			valid = false;
			valid0 = true;
			valid1 = true;

			if (parametric) {
				if (string0 == "" || string0.Contains("=") || string0.Contains("/+") || string0.Contains("/*") || (parametric && (string0.Contains(variable0) || string0.Contains(variable1)))) {
					valid0 = false;
				}
				else {
					try {
						parseContext.AssignVariable("t", new ScalarValue(0));
						parseContext.AssignVariable("T", new ScalarValue(0));
						parseContext.AssignVariable("pa", new ScalarValue(0));
						parseContext.AssignVariable("pr", new ScalarValue(0));
						valid0 = true;		
						parser0 = Parser.Parse(string0);
					}
					catch (YAMPParseException ex0) {
						valid0 = false;
					}
					catch (YAMPSymbolMissingException ex1) {
						valid0 = false;
					}
				}
			}
			for (i = 0; i < implicitStrings.Length; i++) {
				if (string1.Contains(implicitStrings[i])) {
					valid1 = false;
					error = "Multiplication must be explicit! Change '"+implicitStrings[i]+"' to '"+explicitStrings[i]+"'";
				}
			}

			if (!valid1) {

			}
			else if (string1 == "") {
				valid1 = false;
				error1 = "Something's wrong! Your function cannot be empty";
			}
			else if (string1.Contains("=")) {
				valid1 = false;
				error1 = "Something's wrong! You cannot use '=' in your function";
			}
			else if (string1.Contains("--")) {
				valid1 = false;
				error1 = "Something's wrong! You cannot use '--' in your function";
			}
//			else if (string1.Contains(variable1)) {
//				valid1 = false;
//				error1 = "Something's wrong! You cannot use the variable '"+variable1.ToUpper()+"' in your function";
//			}
			else {
				try {
					valid1 = true;
					parseContext.AssignVariable(variable0, new ScalarValue(0));
					parseContext.AssignVariable(variable0.ToUpper(), new ScalarValue(0));
					parseContext.AssignVariable("t", new ScalarValue(0));
					parseContext.AssignVariable("T", new ScalarValue(0));
					parseContext.AssignVariable("px", new ScalarValue(0));
					parseContext.AssignVariable("py", new ScalarValue(0));
					parseContext.AssignVariable("pa", new ScalarValue(0));
					parseContext.AssignVariable("pr", new ScalarValue(0));
					parser1 = Parser.Parse(string1);
					float testValue = (float)((ScalarValue)parser1.Execute()).Value;
				}
				catch (YAMPParseException ex0) {
					valid1 = false;
					error1 = "Something's wrong! Double-check your function for problems";
				}
				catch (YAMPSymbolMissingException ex1) {
					valid1 = false;
					error1 = "Something's wrong! Looks like you used an invalid symbol";
				}
				catch (YAMPArgumentNumberException ex2) {
					valid1 = false;
					error1 = "Something's wrong! One of your operations has too many inputs";
				}
				catch (Exception ex) {
					valid1 = false;
					if (Application.isEditor) error = ex.Message;
					else error1 = "Something's wrong! Double-check your function for problems";
				}
			}
			if (valid0 && valid1) valid = true;
		}

		if (graphColliderPolygon != null) graphColliderPolygon.isTrigger = true;

		if (shadowGraph != null && game.playing) {
			graphLine.enabled = false;
			if (meshRenderer) meshRenderer.enabled = false;
		}
		else if (valid) {
			GenerateGraph();
			graphLine.enabled = true;
			if (meshRenderer) {
				if (drawMesh && !(swapped && polar)) meshRenderer.enabled = true;
				else meshRenderer.enabled = false;
			}
		}
		else {
			graphLine.enabled = false;
			if (meshRenderer) meshRenderer.enabled = false;
		}

		if (valid && validHeightPlayer && validHeightObjective) ready = true;
		else ready = false;

		if (shadowGraph != null) graphLine.SetWidth((1-(Time.time+shadowOffset)%shadowLimit/shadowLimit)*0.9f*thickness*game.windowSize/100, (1-(Time.time+shadowOffset)%shadowLimit/shadowLimit)*0.9f*thickness*game.windowSize/100);
		else graphLine.SetWidth(thickness*game.windowSize/100, thickness*game.windowSize/100);
		validHeightPlayer = true;
		validHeightObjective = true;
	}

	public bool IsValid () {
		bool toReturn = true;
		//expressionContext.Variables["t"] = (double)(Time.time-game.playTime);
		if (shadowGraph != null) parseContext.AssignVariable("t", new ScalarValue((double)((Time.time+shadowOffset)%shadowLimit)));
		else parseContext.AssignVariable("t", new ScalarValue((double)game.time));
		if (string1 == "" || string1.Contains("=") || string1.Contains("/+") || string1.Contains("/*") || string1.Contains("--")) toReturn = false;
		return toReturn;
	}

	void GenerateGraph () {
		int i;
		if (shadowGraph != null) {
			graphLine.material.color = new Color(1, 0.137f, 0.569f, (1-(Time.time+shadowOffset)%shadowLimit/shadowLimit)*0.8f);
			parseContext.AssignVariable("t", new ScalarValue((double)((Time.time+shadowOffset)%shadowLimit)));
			parseContext.AssignVariable("T", new ScalarValue((double)((Time.time+shadowOffset)%shadowLimit)));
		}
		else parseContext.AssignVariable("t", new ScalarValue((double)(Time.time-game.playTime)));
//		parseContext.AssignVariable("T", (double)(Time.time-game.playTime));
		parseContext.AssignVariable("px", new ScalarValue((double)rider.transform.position.x));
		parseContext.AssignVariable("py", new ScalarValue((double)rider.transform.position.y));
		parseContext.AssignVariable("pa", new ScalarValue((double)Mathf.Atan2(rider.transform.position.y, rider.transform.position.x)));
		parseContext.AssignVariable("pr", new ScalarValue((double)(rider.transform.position.magnitude)));

		if (polar) {
			float value0;
			float value1;
			Vector2 value;
			Vector2 xy;
			length = 0;
			
			int swapOffset;
			if (swapped) swapOffset = 1;
			else swapOffset = 0;

			int sampleCount;
			if (swapped) sampleCount = Mathf.CeilToInt((float)Screen.width*2/resolution);
			else sampleCount = Mathf.CeilToInt(Mathf.Pow((float)Screen.width/resolution, 0.5f)*24);
			
			if (swapped) edgePoints = new Vector2[sampleCount+1];
			else edgePoints = new Vector2[sampleCount+2];
			graphLine.SetVertexCount(sampleCount+1);

			for (i = 0; i < sampleCount+1; i++) {
				if (swapped) value0 = Mathf.Lerp(0, game.graphRect.size.magnitude, (float)i/sampleCount);
				else value0 = Mathf.Lerp(0, 2*Mathf.PI, (float)i/sampleCount);
				value1 = Get1(value0);
				
				if (swapped) value = new Vector2(value1, value0);
				else value = new Vector2(value0, value1);
				xy = new Vector2(Mathf.Cos(value.x), Mathf.Sin(value.x))*value.y;

				edgePoints[i] = xy;
				graphLine.SetPosition(i, xy);
				if (i > 0) length += Vector2.Distance(xy, edgePoints[i-1]);
			}
			if (!swapped) edgePoints[edgePoints.Length-1] = edgePoints[0];

			if (solid) {
				if (graphColliderEdge != null) graphColliderEdge.points = edgePoints;
				if (graphColliderPolygon != null) graphColliderPolygon.points = edgePoints;
			}
			if (drawMesh && !swapped) {
		        int[] indices = new int[(sampleCount+1)*3];
		        for (i = 0; i < sampleCount+1; i++) {
		        	indices[i*3] = i+1;
		        	indices[i*3+1] = 0;
		        	indices[i*3+2] = (i+2)%(sampleCount+1);
		        }
		 
		        Vector3[] vertices = new Vector3[sampleCount+2];
		        Vector2[] uv = new Vector2[sampleCount+2];
		        for (i = 1; i < sampleCount+2; i++) {
		        	vertices[i] = new Vector3(edgePoints[i].x, edgePoints[i].y, transform.position.z+4);
		        	uv[i] = edgePoints[i-1];
		        }
		        vertices[0] = transform.position+Vector3.forward*4;
		        uv[0] = Vector2.zero;
		 	
		 		graphMesh.Clear();
		        graphMesh.vertices = vertices;
		        graphMesh.triangles = indices;
				graphMesh.uv = uv;
		        graphMesh.RecalculateNormals();
		        graphMesh.RecalculateBounds();
			}
		}
		else {
			int sampleCount = Mathf.CeilToInt((float)Screen.width/resolution);
			float value0;
			float value1;
			Vector2 xy;
			edgePoints = new Vector2[sampleCount+1];
			polygonPoints = new Vector2[sampleCount+3];
			graphLine.SetVertexCount(sampleCount+1);
			length = 0;
			float lowestPoint = float.PositiveInfinity;
			for (i = 0; i < sampleCount+1; i++) {
				if (swapped) value0 = Mathf.Lerp(game.graphRect.yMin, game.graphRect.yMax, (float)i/sampleCount);
				else value0 = Mathf.Lerp(game.graphRect.xMin, game.graphRect.xMax, (float)i/sampleCount);
				value1 = Get1(value0);
				
				if (swapped) xy = new Vector2(value1, value0);
				else xy = new Vector2(value0, value1);

				edgePoints[i] = xy;
				polygonPoints[i] = xy;
				graphLine.SetPosition(i, xy);
				if (i > 0) length += Vector2.Distance(xy, edgePoints[i-1]);
				if (xy.y < lowestPoint) lowestPoint = xy.y;
			}

			if (swapped) {
				polygonPoints[sampleCount+1] = new Vector2(Mathf.Min(game.graphRect.xMin, lowestPoint-1), game.graphRect.yMax);
				polygonPoints[sampleCount+2] = new Vector2(Mathf.Min(game.graphRect.yMin, lowestPoint-1), game.graphRect.yMin);
			}
			else {
				polygonPoints[sampleCount+1] = new Vector2(game.graphRect.xMax, Mathf.Min(game.graphRect.yMin, lowestPoint-1));
				polygonPoints[sampleCount+2] = new Vector2(game.graphRect.xMin, Mathf.Min(game.graphRect.yMin, lowestPoint-1));
			}

			if (solid) {
				if (graphColliderEdge != null) graphColliderEdge.points = edgePoints;
				if (graphColliderPolygon != null) graphColliderPolygon.points = polygonPoints;
			}
			else {
				graphLine.material.mainTextureScale = new Vector2(length/dashLength, 1);
				//graphLine.material.mainTextureOffset = new Vector2(game.window.position.x, game.window.position.x*0);
			}
			if (drawMesh) {
				Triangulator triangulator = new Triangulator(polygonPoints);
		        int[] indices = triangulator.Triangulate();

		        Vector3[] vertices = new Vector3[polygonPoints.Length];
		        for (i = 0; i < polygonPoints.Length; i++) vertices[i] = new Vector3(polygonPoints[i].x, polygonPoints[i].y, transform.position.z+4);

		 		graphMesh.Clear();
		        graphMesh.vertices = vertices;
		        graphMesh.triangles = indices;
				graphMesh.uv = polygonPoints;
		        graphMesh.RecalculateNormals();
		        graphMesh.RecalculateBounds();
			}
		}
	}

	public float Get1 (float value0, float valueT, float valuePX, float valuePY) {
		parseContext.Clear();
		parseContext.AssignVariable(variable0, new ScalarValue((double)value0));
		parseContext.AssignVariable("t", new ScalarValue((double)valueT));
		parseContext.AssignVariable("px", new ScalarValue((double)valuePX));
		parseContext.AssignVariable("py", new ScalarValue((double)valuePY));
		return Get1();
	}

	public float Get1 (float value0) {
		parseContext.AssignVariable(variable0, new ScalarValue((double)value0));
		return Get1();
	}

	public float Get1 () {
		float toReturn = 0;
		if (!IsValid()) {

		}
		try	{
			toReturn = (float)((ScalarValue)parser1.Execute()).Value;
		}
		catch (YAMPParseException ex0) {
			toReturn = 0;
		}
		catch (YAMPSymbolMissingException ex1) {
			toReturn = 0;
		}
		return toReturn;
	}

	public float Get0 (float valueT, float valuePX, float valuePY) {
		float toReturn = 0;
		parseContext.AssignVariable("t", new ScalarValue((double)valueT));
		parseContext.AssignVariable("px", new ScalarValue((double)valuePX));
		parseContext.AssignVariable("py", new ScalarValue((double)valuePY));
		try	{
			toReturn = (float)((ScalarValue)parser0.Execute()).Value;
		}
		catch (YAMPParseException ex0) {
			toReturn = 0;
		}
		catch (YAMPSymbolMissingException ex1) {
			toReturn = 0;
		}
		return toReturn;
	}

	public float[] GetChain1 (float valueT0, float valueT1, float valueTStep, float valuePX, float valuePY, float[] chain0, float[] chain1) {
		int i;
		float[] toReturn = new float[chain0.Length];
		if (IsValid()) {
			float valueT;
			for (i = 0; i < chain1.Length; i++) {
				toReturn[i] = chain1[i];
			}
			for (i = chain1.Length; i < toReturn.Length; i++) {
				valueT = Mathf.Lerp(valueT0, valueT1, (float)(i+1)/(toReturn.Length-chain1.Length));
				toReturn[i] = Get1(chain0[i], valueT, valuePX, valuePY);
			}
		}
		return toReturn;
	}

	public float[] GetChain1 (float valueT, float valueTStep, float valuePX, float valuePY, float[] chain0) {
		float[] toReturn = new float[0];
		return GetChain1(0, valueT, valueTStep, valuePX, valuePY, chain0, toReturn);
	}

	public float[] GetChain0 (float valueT0, float valueT1, float valueTStep, float valuePX, float valuePY, float[] chain0) {
		int i;
		float[] toReturn = new float[chain0.Length+Mathf.CeilToInt((valueT1-valueT0)/valueTStep)];
		if (IsValid()) {
			float valueT;
			for (i = 0; i < chain0.Length; i++) {
				toReturn[i] = chain0[i];
			}
			for (i = chain0.Length; i < toReturn.Length; i++) {
				valueT = Mathf.Lerp(valueT0, valueT1, (float)(i+1)/(toReturn.Length-chain0.Length));
				toReturn[i] = Get0(valueT, valuePX, valuePY);
			}
		}
		return toReturn;
	}

	public float[] GetChain0 (float valueT, float valueTStep, float valuePX, float valuePY) {
		float[] toReturn = new float[0];
		return GetChain0(0, valueT, valueTStep, valuePX, valuePY, toReturn);
	}

	public float GetHeight (Vector2 testPoint) {
		float toReturn = 0;
		//Debug.Log("0: "+Get1(Mathf.Atan2(-testPoint.y, -testPoint.x)+Mathf.PI, game.time, rider.transform.position.x, rider.transform.position.y)+", 1: "+(-Get1(Mathf.Atan2(testPoint.y, testPoint.x)+Mathf.PI, game.time, rider.transform.position.x, rider.transform.position.y)));
		//Debug.Log("0: "+Mathf.Atan2(testPoint.y, testPoint.x)+", 1: "+Mathf.Atan2(-testPoint.y, -testPoint.x));
		if (polar) toReturn = Mathf.Max(0, Mathf.Max(-Get1(Mathf.Atan2(testPoint.y, testPoint.x)+Mathf.PI, game.time, rider.transform.position.x, rider.transform.position.y), Get1(Mathf.Atan2(-testPoint.y, -testPoint.x)+Mathf.PI, game.time, rider.transform.position.x, rider.transform.position.y)));
		else {
			if (swapped) toReturn = Get1(testPoint.y, game.time, rider.transform.position.x, rider.transform.position.y);
			else toReturn = Get1(testPoint.x, game.time, rider.transform.position.x, rider.transform.position.y);
		}
		return toReturn;
	}
}