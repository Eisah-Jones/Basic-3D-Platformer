using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	public GameObject headCast;
	public Animator anim;
	public float crouchSpeed = 3.0f;
	public float sneakSpeed = 3.5f;
	public GameObject punchCast;
	public Text promptText;
	public RawImage Note1Pic;
	public RawImage Note2Pic;
	public GameObject firstDoor;
	public GameObject secondDoor;
	public GameObject secondDoorSwitch1;
	public GameObject secondDoorSwitch2;
	public GameObject finalDoor;
	public Button resumeButton;
	public Button mainMenuButton;
	public GameObject explosion;
	public Image healthbar;

	private float runSpeed;
	private Vector3 moveDirection = Vector3.zero;
	private bool dead;
	private bool hanging = false;
	private WallHangDetection whd;
	private bool crouching = false;
	public bool canStand = true;
	private float oldSpeed;
	private bool sneaking;
	private float currentSpeed;
	private bool isRunning;
	private bool isStrafing;
	private bool isBackwards;
	private int health;
	private bool canPunch;
	private RaycastHit hit;
	private bool inNote1 = false;
	private bool readNote = false;
	private bool inNote2 = false;
	private bool isReading;
	private bool switchTrigger = false;
	private bool moveDoor = false;
	private bool inDoor1 = false;
	private bool inDoor2 = false;
	private bool switchDoor21 = false;
	private bool switchDoor22 = false;
	private bool outside = false;

	// Use this for initialization
	void Start () {
		resumeButton.gameObject.SetActive (false);
		mainMenuButton.gameObject.SetActive (false);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Note1Pic.gameObject.SetActive (false);
		Note2Pic.gameObject.SetActive (false);
		promptText.text = "";
		anim = transform.GetComponent<Animator> ();
		dead = false;
		whd = headCast.GetComponent<WallHangDetection> ();
		runSpeed = walkSpeed * 2;
		currentSpeed = walkSpeed;
		isRunning = false;
		isStrafing = false;
		isBackwards = false;
		health = 100;
		canPunch = true;
		isReading = false;
	}
	
	// Update is called once per frame
	void Update () {

		healthbar.transform.localScale = new Vector3 (health/100.0f, healthbar.transform.localScale.y, healthbar.transform.localScale.z);

		if(outside && Input.GetKeyDown(KeyCode.Escape)){
			//Go to main menu
			SceneManager.LoadScene("MainMenu");
		}

		if(Input.GetKeyDown(KeyCode.R)){
			//Go to main menu
			SceneManager.LoadScene("Main Scene");
		}

		if (!outside && Input.GetKeyDown (KeyCode.Escape)) {
			//Pause menu
			if (Time.timeScale == 1) {
				Time.timeScale = 0;
				resumeButton.gameObject.SetActive (true);
				mainMenuButton.gameObject.SetActive (true);
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			} else {
				Time.timeScale = 1;
				resumeButton.gameObject.SetActive (false);
				mainMenuButton.gameObject.SetActive (false);
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		if (health <= 0) {
			death ();
		}

		if (!dead && !hanging && !isReading) {
			CharacterController controller = GetComponent<CharacterController> ();
			if (controller.isGrounded) {
				moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= currentSpeed/3f;
				if (Input.GetButtonDown ("Jump") && !sneaking && !crouching) {
					moveDirection.y = jumpSpeed;
				}
			}

			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move (moveDirection * Time.deltaTime);
			// Set animations here
			//Walking
			if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.W)) {
				anim.SetBool ("Walking", true);
			} else if (Input.GetKeyUp (KeyCode.UpArrow) || Input.GetKeyUp (KeyCode.W)) {
				anim.SetBool ("Walking", false);
			}
			//Backward Walking
			if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.S)) {
				anim.SetBool ("Backward Walk", true);
				isBackwards = true;
			} else if (Input.GetKeyUp (KeyCode.DownArrow) || Input.GetKeyUp (KeyCode.S)) {
				anim.SetBool ("Backward Walk", false);
				isBackwards = false;
			}
			//Running
			if (Input.GetKeyDown (KeyCode.LeftShift) && !crouching && !sneaking) {
				anim.SetBool ("Running", true);
				isRunning = true;
				currentSpeed = runSpeed;
			} else if (Input.GetKeyUp (KeyCode.LeftShift) && !crouching && !sneaking) {
				anim.SetBool ("Running", false);
				isRunning = false;
				currentSpeed = walkSpeed;
			}
			//Left Strafe
			if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) {
				anim.SetBool ("Left Strafe", true);
				isStrafing = true;
			} else if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.A)) {
				anim.SetBool ("Left Strafe", false);
				isStrafing = false;
			}
			//Right Strafe
			if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D)) {
				anim.SetBool ("Right Strafe", true);
				isStrafing = true;
			} else if (Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.D)) {
				anim.SetBool ("Right Strafe", false);
				isStrafing = false;
			}
			//Jump
			if (Input.GetKeyDown (KeyCode.Space) && !sneaking && !crouching) {
				anim.SetBool ("Jump", true);
			} else if (Input.GetKeyUp (KeyCode.Space) && !sneaking && !crouching) {
				anim.SetBool ("Jump", false);
			}
			//Crouch
			if (Input.GetKeyDown (KeyCode.C) && !sneaking) {
				if (!crouching) {
					crouching = true;
					anim.SetBool ("Crouch", true);
					anim.SetBool ("Running", false);
					currentSpeed = crouchSpeed;
					controller.center = new Vector3 (0f, 0.63f, 0f);
					controller.height = 1.1f;
				} else if(crouching && canStand) {
					crouching = false;
					currentSpeed = walkSpeed;
					anim.SetBool ("Crouch", false);
					controller.center = new Vector3 (0f, 0.94f, 0f);;
					controller.height = 1.733011f;
				}
			} 
			//Sneaking
			if (Input.GetKeyDown(KeyCode.V) && !isRunning && !isStrafing && !isBackwards && !crouching){
				sneaking = true;
				anim.SetBool ("Sneak", true);
				currentSpeed = sneakSpeed;
			} else if (Input.GetKeyUp(KeyCode.V) && !isRunning  && !isStrafing && !isBackwards && !crouching){
				sneaking = false;
				anim.SetBool ("Sneak", false);
				currentSpeed = walkSpeed;
			}
			//Punching
			if (Input.GetMouseButtonDown (0) && canPunch) {
				canPunch = false;
				Invoke ("resetPunch", 1);
				Invoke ("resetPunchAnim", 0.1f);
				anim.SetBool ("Punch", true);
				Physics.Raycast (punchCast.transform.position, punchCast.transform.forward, out hit, 0.75f);
				Debug.DrawRay (punchCast.transform.position, punchCast.transform.forward * 0.75f, Color.green);
				try{
					if (hit.collider.tag == "Enemy") {
						NormalAI nai = hit.collider.gameObject.GetComponent<NormalAI>();
						nai.TakeDamage();
					} 
				} catch {
				}
			}

		}

		if (!dead && hanging) {
			animsFalse ();
			//change controlls to drop, get up, or left and right
			anim.SetBool ("Hang Up", false);
			anim.SetBool ("Hang Down", false);
			CharacterController controller = GetComponent<CharacterController> ();
			moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, 0);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= currentSpeed/5f;
			controller.Move (moveDirection * Time.deltaTime);
			//Left Strafe
			if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) {
				anim.SetBool ("Hang Left", true);
			} else if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.A)) {
				anim.SetBool ("Hang Left", false);
			}
			//Right Strafe
			if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D)) {
				anim.SetBool ("Hang Right", true);
			} else if (Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.D)) {
				anim.SetBool ("Hang Right", false);
			}
			//Climb Down
			if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) {
				anim.SetBool ("Hang Down", true);
				anim.SetBool ("Hanging", false);
				anim.SetBool ("Hang Right", false);
				anim.SetBool ("Hang Left", false);
				whd.stopRaycasting ();
				hanging = false;
			}
		}

		if (Input.GetMouseButtonUp(1) && inNote1) {
			readNote = !readNote;
			isReading = !isReading;
			Note1Pic.gameObject.SetActive (readNote);
		}

		if (Input.GetMouseButtonUp(1) && inNote2) {
			readNote = !readNote;
			isReading = !isReading;
			Note2Pic.gameObject.SetActive (readNote);
		}

		if (Input.GetMouseButtonUp(1) && switchTrigger) {
			moveDoor = true;
			promptText.text = "";
		}

		if (Input.GetMouseButtonUp(1) && inDoor1) {
			switchDoor21 = true;
			promptText.text = "";
		}

		if (Input.GetMouseButtonUp(1) && inDoor2) {
			switchDoor22 = true;
			promptText.text = "";
		}

		if (moveDoor) {
			firstDoor.transform.position = Vector3.MoveTowards (firstDoor.transform.position, new Vector3 (firstDoor.transform.position.x, 15, firstDoor.transform.position.z), 1 * Time.deltaTime);
		}

		if (switchDoor21 && switchDoor22) {
			secondDoor.transform.position = Vector3.MoveTowards (firstDoor.transform.position, new Vector3 (firstDoor.transform.position.x, 10, firstDoor.transform.position.z), 1 * Time.deltaTime);
		}
	}

	public bool getDead(){
		return dead;
	}

	public bool getHang(){
		return hanging;
	}

	public void setHang(bool b){
		hanging = b;
		anim.SetBool ("Hanging", b);
	}

	private void animsFalse(){
		anim.SetBool ("Jump", false);
		anim.SetBool ("Right Strafe", false);
		anim.SetBool ("Left Strafe", false);
		anim.SetBool ("Running", false);
		anim.SetBool ("Backward Walk", false);
		anim.SetBool ("Walking", false);
	}

	public void setStand(bool b){
		canStand = b;
	}

	public bool getCrouch(){
		return crouching;
	}

	public bool getSneaking(){
		return sneaking;
	}

	public void TakeHit(){
		health -= 5;
		Debug.Log (health);
	}

	public void TakeHitBoss(){
		health -= 20;
		Debug.Log (health);
	}

	public void resetPunch(){
		canPunch = true;
	}

	public void resetPunchAnim(){
		anim.SetBool ("Punch", false);
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Note1" && !readNote) {
			inNote1 = true;
			promptText.text = "Read Note";
		}

		if (other.tag == "Note2" && !readNote) {
			inNote2 = true;
			promptText.text = "Read Note";
		}

		if (other.tag == "Switch1" && !switchTrigger) {
			switchTrigger = true;
			promptText.text = "Flip Switch";
		}

		if (other.tag == "Door21") {
			inDoor1 = true;
			promptText.text = "Flip Switch";
		}

		if (other.tag == "Door22") {
			inDoor2 = true;
			promptText.text = "Flip Switch";
		}

		if (other.tag == "final") {
			finalDoor.SetActive (true);
			outside = true;
			promptText.text = "Congrats!! Press Esc to go to Main Menu.";
		}
	}

	void OnTriggerStay(Collider other){
		if (other.tag == "Note1" && readNote) {
			inNote1 = true;
			promptText.text = "";
		}

		if (other.tag == "Note2" && readNote) {
			inNote2 = true;
			promptText.text = "";
		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Note1") {
			inNote1 = false;
			promptText.text = "";
		}

		if (other.tag == "Note2") {
			inNote2 = false;
			promptText.text = "";
		}

		if (other.tag == "Switch1" && switchTrigger) {
			switchTrigger = false;
			promptText.text = "";
		}

		if (other.tag == "Door21") {
			inDoor1 = false;
			promptText.text = "";
		}

		if (other.tag == "Door22") {
			inDoor2 = false;
			promptText.text = "";
		}
	}

	public bool getReading(){
		return isReading;
	}

	public bool getOutside(){
		return outside;
	}

	public void resume(){
		Time.timeScale = 1;
		resumeButton.gameObject.SetActive (false);
		mainMenuButton.gameObject.SetActive (false);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void mainMenu(){
		Time.timeScale = 1;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		SceneManager.LoadScene ("MainMenu");
	}

	private void death(){
		dead = true;
		Instantiate (explosion, transform.position, transform.rotation);
		mainMenuButton.gameObject.SetActive (true);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		gameObject.SetActive (false);
	}
}



