using UnityEngine;
using System.Collections;

public class WallHangDetection : MonoBehaviour {

	public GameObject player;

	private RaycastHit hit;
	private PlayerController pc;
	private bool raycasting = true;

	// Use this for initialization
	void Start () {
		pc = player.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (raycasting) {
			Physics.Raycast (transform.position, player.transform.forward, out hit, 0.5f);
			Debug.DrawRay (transform.position, player.transform.forward * 0.5f, Color.green);
			try{
			if (hit.collider.tag == "hangable") {
				pc.setHang (true);
				setRotation (hit.collider.gameObject);
			} 
			} catch {
			}
		}
	}

	public void stopRaycasting(){
		raycasting = false;
		StartCoroutine (resetRayCast());
	}

	private IEnumerator resetRayCast(){
		yield return new WaitForSeconds (0.1f);
		raycasting = true;
	}

	private void setRotation(GameObject other){
		player.transform.rotation = other.transform.rotation;
		Camera.main.transform.rotation = other.transform.rotation;
	}
}
