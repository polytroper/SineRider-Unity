using UnityEngine;
using System.Collections;

public class Dealer : MonoBehaviour {

	private Ray ray;
	private RaycastHit hit;
	private Card hoverCard;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit)) {
			Card hitCard = hit.transform.GetComponent<Card>();
			if (hitCard) {
				if (hitCard != hoverCard) {
					hoverCard = hitCard;
					hoverCard.OnHover();
				}
			}
			else {
				hoverCard.OffHover();
				hoverCard = null;
			}
		}
		else if (hoverCard) {
			hoverCard.OffHover();
			hoverCard = null;
		}
	}
}
