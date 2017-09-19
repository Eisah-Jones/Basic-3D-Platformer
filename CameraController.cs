using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;
	public float rotateSpeed = 5;

	private Vector3 offset;
	private PlayerController pc;
	private RaycastHit[] hits;

	// Use this for initialization
	void Start () {
		offset = target.transform.position - transform.position;
		pc = target.GetComponent<PlayerController> ();
	
	}
	
	// Update is called once per frame
	void LateUpdate () {

		if (hits != null) {
			foreach (RaycastHit hit in hits) {
				Renderer r = hit.collider.GetComponent<Renderer> ();
				if (r && hit.collider.tag == "wall") {
					r.enabled = true;
				}
			}
		}

		Debug.DrawRay(this.transform.position, (this.target.transform.position - this.transform.position), Color.magenta);

		hits = Physics.RaycastAll(this.transform.position, (this.target.transform.position - this.transform.position), Vector3.Distance(this.transform.position, this.target.transform.position));
		foreach (RaycastHit hit in hits) {
			Renderer r = hit.collider.GetComponent<Renderer> ();
			if (r && hit.collider.tag == "wall") {
				r.enabled = false;
			}
		}
			

		if (!pc.getDead () && !pc.getHang () && !pc.getReading() && Time.timeScale == 1) {
			float horizontal = Input.GetAxis ("Mouse X") * rotateSpeed;
			target.transform.Rotate (0, horizontal, 0);

			float desiredAngle = target.transform.eulerAngles.y;
			Quaternion rotation = Quaternion.Euler (0, desiredAngle, 0);
			transform.position = target.transform.position - (rotation * offset);

			transform.LookAt (new Vector3 (target.transform.position.x, (target.transform.position.y + 1.9f), target.transform.position.z));
		} else if (!pc.getDead () && pc.getHang () && !pc.getReading()) {
			float desiredAngle = target.transform.eulerAngles.y;
			Quaternion rotation = Quaternion.Euler (0, desiredAngle, 0);
			transform.position = target.transform.position - (rotation * offset);
		}
	
	}
}
