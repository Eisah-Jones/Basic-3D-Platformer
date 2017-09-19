using UnityEngine;
using System.Collections;

public class NormalAI : MonoBehaviour {

	public Transform target;
	public GameObject punchDetect;
	public int health;
	public GameObject explosion;

	private NavMeshAgent agent;
	private Animator anim;
	private Vector3 previousPosition;
	private float curSpeed;
	private RaycastHit hit;
	private PlayerController pc;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		previousPosition = transform.position;
		InvokeRepeating ("DoDamage", 0.0f, 1.0f);
		pc = target.GetComponent<PlayerController> ();
		health = 100;
	}
	
	// Update is called once per frame
	void Update () {
		Physics.Raycast (punchDetect.transform.position, punchDetect.transform.forward, out hit, 1f);
		Debug.DrawRay (punchDetect.transform.position, punchDetect.transform.forward * 1f, Color.green);
		transform.LookAt (new Vector3(target.position.x, transform.position.y, target.position.z));
		float distance = Vector3.Distance (transform.position, target.position);
		if (distance < 10) {
			agent.SetDestination (target.position);
		}
		SetRunAnim (distance);
		if (health < 1) {
			Invoke ("death", 0.5f);
		}
	}

	private void SetRunAnim(float d){
		Vector3 curMove = transform.position - previousPosition;
		curSpeed = curMove.magnitude / Time.deltaTime;
		previousPosition = transform.position;
		if (d > 10){
			anim.SetBool ("Running", false);
			anim.SetBool ("Punching", false);
		}
		else if (curSpeed > 0) {
			anim.SetBool ("Running", true);
			anim.SetBool ("Punching", false);
		} else if (curSpeed < 1){
			anim.SetBool ("Running", false);
			anim.SetBool ("Punching", true);
		}
	}

	private void DoDamage(){
		try{
			if (hit.collider.tag == "Player") {
				pc.TakeHit();
			} 
		} catch {}
	}

	public void TakeDamage(){
		health -= 50;
	}

	private void death(){
		StartCoroutine (startExplosion());
	}

	private IEnumerator startExplosion(){
		Object temp = Instantiate (explosion, transform.position, transform.rotation);
		Destroy(transform.gameObject);
		yield return new WaitForSeconds(2);
		Destroy (temp);
	}
}
