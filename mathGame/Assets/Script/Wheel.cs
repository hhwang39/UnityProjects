//#define VERSION1
//#define VERSION2
//#define DEBUG
using UnityEngine;
//using System;
using System.Collections;
using TouchScript.Gestures;
using System.Collections.Generic;
public class Wheel : MonoBehaviour {
	private enum State {SPIN, NOTSPIN};
	private State st = State.NOTSPIN;
	public int numToAdd;
	public int count;
	public bool finBool = false;
	private bool blueBool;
	private string newStr;
	private char op;
	//#if VERSION1
	List<int> randRotList = new List<int>(new int[] {432, 504, 576, 648, 720});
	//#endif
	//#if VERSION2
	public GameObject arrow;
	//#endif
	// Use this for initialization
	void Start () {
		//blueBool = false;
		newStr = "";
		//op = '+';
		arrow = gameObject.transform.Find("arr").gameObject;
		count = 0;
		if (op == '+') {
			gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/WheelImg") as Texture2D;
		} else if (op == '-') {
			gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/Wheelminus") as Texture2D;
		}
		//arrow.renderer.material.mainTexture = Resources.Load("Images/arrow") as Texture2D;
		//Color32 color2 = new Color32(231, 210, 200, 255);
		//gameObject.renderer.material.color = Color.green;
		//arrow.transform.eulerAngles = new Vector3(0, 235, 0);
	}
	
	// Update is called once per frame
	void Update () {
		//gameObject.animation.Play("spinning");
		if (blueBool && count == 1 && !finBool && st == State.NOTSPIN) {
			if (newStr == "1") {
				startRotate();
			}
		}
	}
	public void setOP(char o) {
		op = o;
	}
	public void bluetoothEnable(bool b) {
		if (b) {
			blueBool = true;
		}
	}
	void OnEnable() {
		gameObject.GetComponent<FlickGesture>().Flicked += flickHandler;
	}	
	void OnDisable() {
		gameObject.GetComponent<FlickGesture>().Flicked -= flickHandler;
	}
	void flickHandler(object sender, System.EventArgs e) {
		finBool = false;
		//float randNum = Random.Range(0, 360);
		//rotate();
		if ((!blueBool || count != 1) && st == State.NOTSPIN) {
			startRotate();
		}

		//gameObject.animation.Play("newSpin");
	}
	void startRotate() {
		StartCoroutine("rotate");
	}
	IEnumerator rotate() {
		st = State.SPIN;
//#if VERSION1
		//creating random number from predefined rotation
		int randNum = randRotList[Random.Range(0, randRotList.Count)];
		for (int i = 0; i < randNum; i+=12) {
			//gameObject.transform.Rotate(new Vector3(0, 10, 0)); //version 1
			arrow.transform.Rotate(new Vector3(0, 12, 0)); // version 2
			yield return new WaitForSeconds(0.01f);
		}
		yield return new WaitForSeconds(0.5f);
		//numToAdd = calcNum(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y);
		//numToAdd = calcNum(arrow.transform.eulerAngles.x, gameObject.transform.eulerAngles.y); //version 1
		numToAdd = calArrAngle(arrow.transform.localEulerAngles.y); //version 2
		yield return new WaitForSeconds(0.1f);
//#endif
		++count;
		finBool = true;
		st = State.NOTSPIN;
		if (count == 1 && op == '-') {
			gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/WheelImg") as Texture2D;
			yield return new WaitForSeconds(0.1f);
		}
#if DEBUG
		Debug.Log("Count: " + count);
		Debug.Log("My char is " + op);
		Debug.Log("arrow angle " + arrow.transform.localEulerAngles.y);
		Debug.Log("numToAdd " + numToAdd);
#endif
	}
	//calculate number that arrow points based on the arrow's local angle
	public int calArrAngle(float y) {
		if (y < 0) {
			y += 360;
		}
		if (op == '+' || (op == '-' && count == 1) || (op == '*')) {
			if (y >=234 && y < 306) {
				return 1;
			} else if ((y >= 0 && y < 18) || (y >= 306 && y <= 360)) {
				return 2;
			} else if (y >= 18 && y < 90) {
				return 3;
			} else if (y >= 90 && y < 162) {
				return 4;
			} else if (y >= 162 && y < 234) {
				return 5;
			}
		} else if (op == '-') {
			if (y >=234 && y < 306) {
				return 6;
			} else if ((y >= 0 && y < 18) || (y >= 306 && y <= 360)) {
				return 7;
			} else if (y >= 18 && y < 90) {
				return 8;
			} else if (y >= 90 && y < 162) {
				return 9;
			} else if (y >= 162 && y < 234) {
				return 10;
			}
		}

		return 0;
	}
	public void OnDestry() {
		Destroy(arrow);
	}
	public void setWheelStr(string str) {
		newStr = str;
	}
	//void OnMouseDown() {
		//gameObject.animation.Play("newSpin");
	//}
}
