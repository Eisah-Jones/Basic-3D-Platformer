using UnityEngine;
using System.Collections;

public class StandDetection : MonoBehaviour {

	public GameObject player;

	private RaycastHit hit;
	private PlayerController pc;

	// Use this for initialization
	void Start () {
		pc = player.GetComponent<PlayerController>();
	}

	// Update is called once per frame
	void Update () {
		Physics.Raycast (transform.position, transform.up, out hit, 0.5f);
		Debug.DrawRay (transform.position, transform.up * 0.5f, Color.green);
		try{
		if (hit.collider.tag == "wall") {
			pc.setStand (false);
		} 
		}catch {
			pc.setStand (true);
		}
	}
}
