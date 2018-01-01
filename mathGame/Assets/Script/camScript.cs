using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
public class camScript : MonoBehaviour {
	Vector3 newPos;
	Vector3 oldPos;
	Camera cam;
	// Use this for initialization
	void Start () {
		newPos = new Vector3(370, -60, 110);
		cam = GameObject.Find ("Cam2").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnEnable() {
		gameObject.GetComponent<FlickGesture>().Flicked += flickHandler;
	}	
	void OnDisable() {
		gameObject.GetComponent<FlickGesture>().Flicked -= flickHandler;
	}
	void flickHandler(object sender, System.EventArgs e) {
		//float randNum = Random.Range(0, 360);
		//rotate();
		//startRotate();
		oldPos = cam.transform.position;
		cam.transform.position = newPos;
		newPos = oldPos;
		//gameObject.animation.Play("newSpin");
	}
}
