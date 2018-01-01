using UnityEngine;
using System.Collections;

public class SliderMenu : MonoBehaviour {
	private GameObject pauseMenuPanel;
	//animator reference
	private Animator anim;
	//public bool isSlided;
	//variable for checking if the game is paused
	//private bool isSlided = false;
	// Use this for initialization
	void Start () {
		pauseMenuPanel = GameObject.Find("Container").gameObject;
		anim = pauseMenuPanel.GetComponent<Animator>();
		//disable it on start to stop it from playing the default animation
		anim.enabled = false;
	}
	public void slide(){
		//enable the animator component
		anim.enabled = true;
		//play the Slide animation
		//if (!isSlided) {
			anim.Play("sliding");
		//	isSlided = true;
		//}
		//isSlided = true;
	}
	//function to unpause the game
	public void unSlide(){
		//set the isPaused flag to false to indicate that the game is not paused
		//isSlided = false;
		//play the unSlide animation
		//if (isSlided) {
			anim.Play("unsliding");
		//}
	}
}
