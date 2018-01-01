using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
public class containerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnEnable() {
		gameObject.GetComponent<PanGesture>().Panned += panHandler;
	}
	void OnDisable() {
		gameObject.GetComponent<PanGesture>().Panned -= panHandler;
	}
	/// <summary>
	/// Handler for pan (any dragging or swiping)
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	void panHandler(object sender, System.EventArgs e) {
		//Debug.Log("Panned");
		gameObject.GetComponent<Animator>().Play("unsliding");
	}
}
