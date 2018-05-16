using UnityEngine;
using System.Collections;
using YAMP;

public class Objective : MonoBehaviour {
	public int orderIndex = -1;
	public bool dynamic;
	public bool toggle;
	public float timeRequired = 0;
	public bool complete;
	public float completionTime;
	public bool triggering;
	public float triggerTime;
	public bool untriggerable;
	public float groundHeight;

	public SpriteRenderer gravityPointer;
	public TMPro.TextMeshPro centerText;
	public TMPro.TextMeshPro cornerText;
	public AudioClip completeSound;

	[HideInInspector] public Puzzle puzzle;
	[HideInInspector] public Vector3 startPosition;
	[HideInInspector] public Vector3 currentPosition;
	[HideInInspector] public bool selected;
	private SpriteRenderer spriteRenderer;
	private TrailRenderer trail;
	private CircleCollider2D dynamicGraphCollider;

	private Game game;

	void Start () {
		game = FindObjectOfType<Game>();
		if (transform.parent != null) puzzle = transform.parent.GetComponent<Puzzle>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		trail = GetComponent<TrailRenderer>();
		dynamicGraphCollider = GetComponentInChildren<CircleCollider2D>();
		startPosition = transform.position;
		if (dynamic) MakeDynamic();
	}
	
	void Update () {
		//if (game.graph.valid) groundHeight = game.graph.Get1(transform.position.x);
		if (game.graph.valid) groundHeight = game.graph.GetHeight(transform.position);
		else groundHeight = float.NegativeInfinity;
		
		if (!dynamic && GetComponent<Rigidbody2D>()) {
			transform.position = startPosition;
			transform.rotation = Quaternion.identity;
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			GetComponent<Rigidbody2D>().angularVelocity = 0;
		}
		
		if (!game.playing) {
			if (puzzle.currentOrderIndex < orderIndex) spriteRenderer.color = new Color(1, 0.5f, 0);
			else spriteRenderer.color = Color.white;

			if (toggle) centerText.text = "-";
			if (timeRequired > 0) centerText.text = timeRequired.ToString();
			if (dynamic) {
				transform.position = startPosition;
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				if (puzzle.polar) {
					if (game.graph.valid && !game.graph.swapped) groundHeight = game.graph.GetHeight(transform.position);
					else if (game.graph.inverted) groundHeight = float.PositiveInfinity;
					else groundHeight = 0;

					if (game.graph.inverted && transform.position.magnitude > groundHeight-0.9f) {
						game.graph.validHeightObjective = false;
						spriteRenderer.color = Color.red;
					}
					else if (!game.graph.inverted && transform.position.magnitude < groundHeight+0.9f) {
						game.graph.validHeightObjective = false;
						spriteRenderer.color = Color.red;
					}
				}
				else if (transform.position.y-0.9f < groundHeight) {
					game.graph.validHeightObjective = false;
					spriteRenderer.color = Color.red;
				}
//				if (puzzle.currentOrderIndex < orderIndex) spriteRenderer.color = new Color(1, 0.5f, 0);
//				else spriteRenderer.color = Color.white;
			}

			triggerTime = Time.time;
		}
		else if (untriggerable) {
			spriteRenderer.color = Color.red;
			triggerTime = Time.time;
		}
		else if (puzzle.currentOrderIndex < orderIndex && triggering) {
			spriteRenderer.color = Color.red;
			puzzle.MakeUnwinnable();
		}
		else if (puzzle.currentOrderIndex < orderIndex) {
			spriteRenderer.color = new Color(1, 0.5f, 0);
		}
		else if (triggering && Time.time-triggerTime > timeRequired && !complete) {
			complete = true;
			completionTime = Time.time;
			spriteRenderer.color = Color.green;
			if (toggle) centerText.text = "+";
			if (!puzzle.IsComplete()) GetComponent<AudioSource>().PlayOneShot(completeSound);
		}
		else if (triggering && timeRequired > 0) {
			//centerText.text = Mathf.Ceil(Mathf.Max(0, timeRequired-Time.time+triggerTime)).ToString();
			spriteRenderer.color = new Color(0.75f, 1, 0.75f);
		}
		else if (toggle) {
			complete = false;
			//spriteRenderer.color = new Color(0.5f, 1, 1);
			spriteRenderer.color = Color.white;
			centerText.text = "-";
		}
		else if (timeRequired > 0 && complete) {
			spriteRenderer.color = Color.green;
			centerText.text = "";
		}
		else if (timeRequired > 0) {
			spriteRenderer.color = Color.white;
			//centerText.text = timeRequired.ToString();
			triggerTime = Time.time;
		}
		else if (complete) {

		}
		else {
			spriteRenderer.color = Color.white;
		}

		if (orderIndex != -1 && timeRequired > 0) {
			centerText.text = Game.alphabet[orderIndex]+"\n"+(Mathf.Round(Mathf.Max(0, timeRequired-Time.time+triggerTime)*10)/10);
			centerText.fontSize = 8;
		}
		else if (orderIndex != -1) {
			centerText.text = Game.alphabet[orderIndex].ToString();
			centerText.fontSize = 12;
		}
		else if (timeRequired != 0) {
			centerText.text = (Mathf.Round(Mathf.Max(0, timeRequired-Time.time+triggerTime)*10)/10).ToString();
			centerText.fontSize = 12;
		}
		else centerText.text = "";

		if (game.playing && dynamic) {
			if (puzzle.polar) {
//				if (polar && inverted && transform.position.y-0.9f < groundHeight) transform.position = new Vector3(transform.position.x, groundHeight+1, transform.position.z);
//				else if (polar && transform.position.y-0.9f < groundHeight) transform.position = new Vector3(transform.position.x, groundHeight+1, transform.position.z);
			}
			else if (transform.position.y-0.9f < groundHeight) transform.position = new Vector3(transform.position.x, groundHeight+1, transform.position.z);

			if (puzzle.currentOrderIndex != orderIndex) {
				spriteRenderer.sortingOrder = 0;
				centerText.sortingOrder = 0;
			}
			else {
				spriteRenderer.sortingOrder = 1;
				centerText.sortingOrder = 1;
			}
		}

		if (dynamic && puzzle.polar) {
			gravityPointer.enabled = true;
			if (puzzle.inverted) gravityPointer.transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position);
			else gravityPointer.transform.rotation = Quaternion.LookRotation(Vector3.forward, -transform.position);
		}
		else gravityPointer.enabled = false;
			
		centerText.transform.rotation = transform.rotation;
		//centerText.transform.rotation = Quaternion.identity;
		cornerText.transform.rotation = Quaternion.identity;
		centerText.transform.localPosition = new Vector3(0, -0.09f, -0.5f);
		cornerText.transform.position = transform.position + new Vector3(1, 1, -2);

		//centerText.characterSize = game.windowSize/100;
		if (GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition))) cornerText.gameObject.SetActive(true);
		else cornerText.gameObject.SetActive(false);
		cornerText.fontSize = game.windowSize*0.4f;
		cornerText.text = "("+(Mathf.Round(transform.position.x*10)/10)+", "+(Mathf.Round(transform.position.y*10)/10)+")";

		transform.position = new Vector3(transform.position.x, transform.position.y, -1);

		if (game.playing) trail.time = 8;
		else trail.time = 0;
	}

	public void MakeDynamic () {
		dynamic = true;
		if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Graphics.instance.dynamicObjectiveSprite;
		Destroy(GetComponent<Collider2D>());
		CircleCollider2D newCollider = gameObject.AddComponent<CircleCollider2D>();
//		newCollider.radius = 1;
		newCollider.isTrigger = true;
		if (!dynamicGraphCollider) dynamicGraphCollider = GetComponentInChildren<CircleCollider2D>();
		dynamicGraphCollider.GetComponent<Collider2D>().enabled = true;
	}
	
	public void MakeStatic () {
		dynamic = false;
		if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Graphics.instance.staticObjectiveSprite;
		Destroy(GetComponent<Collider2D>());
		BoxCollider2D newCollider = gameObject.AddComponent<BoxCollider2D>();
//		newCollider.size = Vector2.one*2;
		newCollider.isTrigger = true;
		if (!dynamicGraphCollider) dynamicGraphCollider = GetComponentInChildren<CircleCollider2D>();
		dynamicGraphCollider.GetComponent<Collider2D>().enabled = false;
	}

	public void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			triggering = true;
			//triggerTime = Time.time;
		}
	}
	
	public void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			triggering = false;
		}
	}

	public void Reset () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		transform.position = startPosition;
		transform.rotation = Quaternion.identity;
		triggering = false;
		untriggerable = false;
		complete = false;
		spriteRenderer.color = Color.white;
	}

	public void FixedUpdate () {
		if (dynamic) {
			if (puzzle.polar && puzzle.inverted) GetComponent<Rigidbody2D>().AddForce(9.8f*((Vector2)transform.position).normalized);
			else if (puzzle.polar) GetComponent<Rigidbody2D>().AddForce(-9.8f*((Vector2)transform.position).normalized);
			else GetComponent<Rigidbody2D>().AddForce(-9.8f*Vector2.up);
//			rigidbody2D.AddForce(-4.9f*Vector2.up);
		}
	}

	public void Complete () {
		
	}

	public void Uncomplete () {
		
	}
}
