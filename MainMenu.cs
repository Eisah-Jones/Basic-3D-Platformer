using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public Button play;
	public Button controls;
	public Button back;

	public Text title;

	public RawImage control;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		title.gameObject.SetActive (true);
		play.gameObject.SetActive (true);
		controls.gameObject.SetActive (true);
		back.gameObject.SetActive (false);
		control.gameObject.SetActive (false);
	}
	
	public void playClick(){
		SceneManager.LoadScene ("Main Scene");
	}

	public void controlsClick(){
		title.gameObject.SetActive (false);
		play.gameObject.SetActive (false);
		controls.gameObject.SetActive (false);
		back.gameObject.SetActive (true);
		control.gameObject.SetActive (true);
	}

	public void backClick(){
		title.gameObject.SetActive (true);
		play.gameObject.SetActive (true);
		controls.gameObject.SetActive (true);
		back.gameObject.SetActive (false);
		control.gameObject.SetActive (false);
	}
}
