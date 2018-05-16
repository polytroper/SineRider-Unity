using UnityEngine;
using System.Collections;
using YAMP;

public class Rider : MonoBehaviour {
	public Collider2D riderCollider;
	public TMPro.TextMeshPro positionText;
	public SpriteRenderer gravityPointer;
	
	public bool onFoot;
	public float walkSpeed = 4;

	private Game game;
	private Graph graph;
	private TrailRenderer trail;
	private Animator animator;
	private Vector3 inputVector;
	private Vector3 moveVector;
	private Vector3 position;
	private Vector3 floor;
	public float floorAngle;
	public float moveCoefficient;
	public float groundHeight;
	public Vector3 bodyCenter;
	private bool grounded = true;

	public Vector2 velocity;
	public Vector2 inferredVelocity;
	private Vector2 lastPosition;
	private int frames = 0;

    private Rigidbody2D rigidbody2D;
	
	void Start () {
        rigidbody2D = GetComponent<Rigidbody2D>();
		trail = GetComponentInChildren<TrailRenderer>();
		animator = GetComponent<Animator>();
		game = FindObjectOfType<Game>();
		graph = game.graph;
	}
	
	void Update () {
		bodyCenter = transform.TransformPoint(Vector3.up*0.6f);
		if (graph.polar) {
			if (graph.valid && !graph.swapped) groundHeight = graph.GetHeight(bodyCenter);
			else if (graph.inverted) groundHeight = float.PositiveInfinity;
			else groundHeight = 0;

			if (game.playing) {
				trail.time = 8;
				if ((graph.inverted && bodyCenter.magnitude > groundHeight) || (!graph.inverted && bodyCenter.magnitude < groundHeight)) {
					Vector3 oldPosition = transform.position;
					transform.position = bodyCenter.normalized*groundHeight+transform.position-bodyCenter;
					Debug.Log ("Resetting from "+oldPosition+" to "+transform.position);
				}
			}
			else {
				trail.time = 0;
				transform.position = game.puzzle.playerOrigin;
				rigidbody2D.velocity = Vector2.zero;
				rigidbody2D.angularVelocity = 0;
				if (graph.inverted) transform.rotation = Quaternion.LookRotation(Vector3.forward, -game.puzzle.playerOrigin);
				else transform.rotation = Quaternion.LookRotation(Vector3.forward, game.puzzle.playerOrigin);

				if (graph.inverted && transform.position.magnitude > groundHeight) graph.validHeightPlayer = false;
				else if (!graph.inverted && transform.position.magnitude < groundHeight) graph.validHeightPlayer = false;
			}

			gravityPointer.enabled = true;
			if (graph.inverted) gravityPointer.transform.rotation = Quaternion.LookRotation(Vector3.forward, bodyCenter);
			else gravityPointer.transform.rotation = Quaternion.LookRotation(Vector3.forward, -bodyCenter);
		}
		else {
			if (graph.valid) groundHeight = graph.GetHeight(bodyCenter);
			else groundHeight = float.NegativeInfinity;
			
			if (game.playing) {
				trail.time = 8;
				if ((!graph.swapped && bodyCenter.y < groundHeight) || (graph.swapped && bodyCenter.x < groundHeight)) {
					Vector3 oldPosition = transform.position;
					transform.position = new Vector3(bodyCenter.x, groundHeight, 0)+transform.position-bodyCenter;
					Debug.Log ("Resetting from "+oldPosition+" to "+transform.position);
				}
			}
			else {
				trail.time = 0;
				transform.position = game.puzzle.playerOrigin;
				rigidbody2D.velocity = Vector2.zero;
				rigidbody2D.angularVelocity = 0;
				transform.rotation = Quaternion.identity;
				if ((!graph.swapped && bodyCenter.y-0.3f < groundHeight) || (graph.swapped && bodyCenter.x-0.3f < groundHeight)) graph.validHeightPlayer = false;
			}
			gravityPointer.enabled = false;
		}

		positionText.text = "("+Mathf.Round(transform.position.x*10)/10+", "+Mathf.Floor(transform.position.y*10)/10+")";
		positionText.fontSize = game.windowSize*0.4f;
		positionText.transform.position = transform.position+Camera.main.transform.rotation*(new Vector3(0.75f, 0.75f, 0));
		positionText.transform.rotation = Camera.main.transform.rotation;
	}

	void FixedUpdate () {
		if (game.playing) {
			if (graph.polar && graph.inverted) rigidbody2D.AddForce(4.9f*transform.position.normalized);
			else if (graph.polar) rigidbody2D.AddForce(-4.9f*transform.position.normalized);
			else rigidbody2D.AddForce(-4.9f*Vector2.up);
//			rigidbody2D.AddForce(-4.9f*Vector2.up);
		}

		if (frames > 100) {
			velocity = rigidbody2D.velocity;
			inferredVelocity = rigidbody2D.position-lastPosition;
			lastPosition = rigidbody2D.position;
			velocity = new Vector2(Mathf.Round(velocity.x*100)/100, Mathf.Round(velocity.y*100)/100);
			inferredVelocity = new Vector2(Mathf.Round(inferredVelocity.x*100)/100, Mathf.Round(inferredVelocity.y*100)/100);
			frames = 0;
		}
		else frames++;
	}

	void OnTriggerEnter2D (Collider2D trigger) {
		if (trigger.gameObject.tag == "Objective") {
			//game.puzzles[game.puzzleIndex].CompleteObjective(trigger.gameObject);
		}
	}
}
