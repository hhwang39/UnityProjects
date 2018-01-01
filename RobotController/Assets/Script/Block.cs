using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
public class Block : MonoBehaviour {
	private string moveText;
	private bool forwardBackward;
	private string rotateText;
	private bool leftRight;
	private string repeatText;
	private string speakText;
	enum State {MOVED, NOTMOVED};
	State state = State.NOTMOVED;
	private Vector3 screenPoint;
	private Vector3 offset;
	private GameObject blockCopy;
	// block copy for for loop
	private GameObject blockCopy2;
	private bool createBool;
	private bool windowBool;
	const uint threshold = 40000;
	private bool notifiyBool;
	public bool windowClosed;
	void Start () {
		createBool = true;
		notifiyBool = false;
		windowClosed = true;
	}
	/// <summary>
	/// Raises the mouse down event.
	/// Once pressed, add BlockMoved Componenets and make it move
	/// </summary>
	void OnMouseDown() {
		//Debug.Log(playScript.uid.ToString());
		Camera.main.gameObject.GetComponent<playScript>().makeChoiceInvisible();
		//check if window is closed and if it can create
		if (createBool && windowClosed) {
			notifiyBool = false;
			windowBool = true;
			Quaternion rot = Quaternion.identity;
			rot.eulerAngles = gameObject.transform.eulerAngles;
			if (gameObject.name == "ForBlock") {
				rot.eulerAngles = new Vector3(90, 180, 0);
				blockCopy = GameObject.Instantiate(gameObject, gameObject.transform.position, rot) as GameObject;
				blockCopy2 = GameObject.Instantiate(gameObject, gameObject.transform.position, rot) as GameObject;		
				blockCopy.GetComponent<Renderer>().material.mainTexture = Resources.Load("GeneralImg/forStart") as Texture2D;
				blockCopy2.GetComponent<Renderer>().material.mainTexture = Resources.Load("GeneralImg/forEnd") as Texture2D;
				Destroy(blockCopy2.GetComponent<Block>());
				blockCopy2.AddComponent<LongPressGesture>();
				blockCopy2.AddComponent<BlockMoved>();
				blockCopy2.transform.localScale = gameObject.transform.lossyScale;
				blockCopy2.name = "FE";
				blockCopy2.GetComponent<BlockMoved>().assignUniqueid(playScript.uid);
			} else {
				blockCopy = GameObject.Instantiate(gameObject, gameObject.transform.position, rot) as GameObject;		
			}
			Destroy(blockCopy.GetComponent<Block>());
			blockCopy.AddComponent<LongPressGesture>();
			blockCopy.AddComponent<BlockMoved>();
			blockCopy.transform.localScale = gameObject.transform.lossyScale;
			if (gameObject.name == "MoveBlock") {
				blockCopy.name = "0";
				blockCopy.GetComponent<BlockMoved>().assignUniqueid(-1);
			} else if (gameObject.name == "RotateBlock") {
				blockCopy.name = "1";
				blockCopy.GetComponent<BlockMoved>().assignUniqueid(-1);
			} else if (gameObject.name == "SpeakBlock") {
				blockCopy.name = "2";
				blockCopy.GetComponent<BlockMoved>().assignUniqueid(-1);
			} else if (gameObject.name == "ForBlock") {
				blockCopy.name = "F";
				blockCopy.GetComponent<BlockMoved>().assignUniqueid(playScript.uid);
				playScript.uid++;
			}
			createBool = false;
		}
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.localPosition);//gameObject.transform.position;
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}
	/// <summary>
	/// Raises the mouse drag event.
	/// Dragging objects with mouse or touch is pointing
	/// </summary>
	void OnMouseDrag() {
		if (windowClosed) {
			Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
			if (blockCopy != null) blockCopy.transform.position = curPosition;
			if (blockCopy2 != null)  {
				blockCopy2.transform.position = new Vector3(curPosition.x, curPosition.y - 2, curPosition.z);
			}
		}

	}
	/// <summary>
	/// Raises the mouse up event.
	/// once touch or mouse event is done, move the block to closet grid and open window function
	/// </summary>
	void OnMouseUp() {
		//Debug.Log(state.ToString() + "," + windowBool.ToString() + "," +(blockCopy == null).ToString() + "," + notifiyBool.ToString());
		//dafult values move 1 unit, Front, rotate 45, left, speakText ="Hello World" 
		if (windowClosed) {
			moveText = "1";
			forwardBackward = true;
			rotateText = "45";
			leftRight = true;
			speakText = "Hello World";
			repeatText = "3";
			//special case For loop destroy both.
			if (gameObject.name.StartsWith("F")) {
				int count = 0;
				GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
				foreach (GameObject g in grids) {
					if (g.GetComponent<Grid>().isContain) {
						count++;
					}
				}
				//Debug.Log("count: "+ count);
				if (count == grids.Length - 1 || count == grids.Length) {
					//Debug.Log("hello World");
					if (blockCopy2 != null) Destroy(blockCopy2);
					if (blockCopy != null) Destroy(blockCopy);
					blockCopy = null;
					blockCopy2 = null;
					createBool = true;
					notifiyBool = true;
				}
			}
			if (blockCopy != null) {
				blockCopy.GetComponent<BlockMoved>().checkCloestGrid(blockCopy.transform.position);
				blockCopy.GetComponent<BlockMoved>().checkIntegBlocks();
			}
			if (blockCopy2 != null) {
				blockCopy2.GetComponent<BlockMoved>().checkCloestGrid(blockCopy2.transform.position);
				blockCopy2.GetComponent<BlockMoved>().checkIntegBlocks();
			}
			//if (blockCopy != null) blockCopy.transform.SetParent(GameObject.Find("Map").transform);
			if (!notifiyBool) {
				state = State.MOVED;
				notifiyBool = false;
			}
		}
	}
	/// <summary>
	/// Raises the GUI event.
	/// Open GUI window that allows to set distance or rotation
	/// </summary>
	void OnGUI () {
		if (state == State.MOVED && windowBool && blockCopy != null) {
			Rect tempRect = new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2);
			tempRect = GUI.Window(0, tempRect, windowFunc, "Setting");
		}
	}
	/// <summary>
	/// Window function
	/// set the name of object so that data can be sent with names.
	/// allow user to set objects properties: distance, rotation or speech.
	/// </summary>
	/// <param name="windowid">Windowid.</param>
	//updates as types (can change when set)
	void windowFunc(int windowid) {
		setWindowClosed(false);
		//moving
		if (blockCopy.name.StartsWith("0")) {
			moveText = GUI.TextField(new Rect(100, 50, 50, 20), moveText);
			forwardBackward = GUI.Toggle(new Rect(20, 20, 160, 50), forwardBackward, "Foward/Backward");
			if (!errorCheckStr(moveText, blockCopy.name.Substring(0, 1))) {
				moveText = "1";
			}
			blockCopy.name = "0-" + moveText + (forwardBackward ? "-1" : "-0");
			blockCopy.transform.FindChild("Text").GetComponent<TextMesh>().text = "Move\n" + moveText +(forwardBackward ? "\nforward" : "\nbackward"); 
		//rotating
		} else if (blockCopy.name.StartsWith("1")) {
			rotateText = GUI.TextField(new Rect(100, 50, 50, 20), rotateText);
			leftRight = GUI.Toggle(new Rect(20, 20, 100, 40), leftRight, "Left/Right");
			if (!errorCheckStr(rotateText, blockCopy.name.Substring(0, 1))) {
				rotateText = "45";		
			}
			blockCopy.name = "1-" + rotateText + (leftRight ? "-1" : "-0");
			blockCopy.transform.FindChild("Text").GetComponent<TextMesh>().text = "Rotate\n" + rotateText +(leftRight ? "\nLeft" : "\nRight"); 
		//speak
		} else if (blockCopy.name.StartsWith("2")) {
			speakText = GUI.TextField(new Rect(100, 50, 50, 20), speakText);
			if (!errorCheckStr(speakText, blockCopy.name.Substring(0, 1))) {
				speakText = "Hello World";
			} else if (speakText.Length > 18) {
				speakText = speakText.Substring(0, 18);
			}
			blockCopy.name = "2-" + speakText;
			blockCopy.transform.FindChild("Text").GetComponent<TextMesh>().text = "Speak\n" + speakText;
		} else if (blockCopy.name.StartsWith("F")) {
			repeatText = GUI.TextField(new Rect(100, 50, 50, 20), repeatText);
			blockCopy.name = "F-" + repeatText;
		}
		if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height /2 - 80, 40, 40), "Set")) {
			blockCopy = null;
			blockCopy2 = null;
			createBool = true;
			state = State.NOTMOVED;
			windowBool= false;
			setWindowClosed(true);
		}
	}
	/// <summary>
	/// Sets the window closed.
	/// Don't allow user to move another object before setting all the properties of block.
	/// </summary>
	/// <param name="t">If set to <c>true</c> t.</param>
	public void setWindowClosed(bool t) {
		GameObject.Find("MoveBlock").GetComponent<Block>().windowClosed = t;
		GameObject.Find("RotateBlock").GetComponent<Block>().windowClosed = t;
		GameObject.Find("SpeakBlock").GetComponent<Block>().windowClosed = t;
		GameObject.Find("ForBlock").GetComponent<Block>().windowClosed = t;
	}


	/// <summary>
	/// check string integrity. Value should not be absurd and should only contains digits unless it is speech.
	/// </summary>
	/// <returns><c>true</c>, if check string wasn't errored, <c>false</c> otherwise.</returns>
	/// <param name="str">String.</param>
	/// <param name="mode">Mode.</param>
	bool errorCheckStr(string str, string mode) {
		if (mode == "0" || mode == "1") {
			if (str.Length == 0) {
				return false;
			} else if (!containsOnlyDigits(str)) {
				return false;
			} else if (int.Parse(str) > threshold) {
				return false;
			}
		} else if (mode == "2") {
			if (str.Length == 0) {
				return false;
			}
		}
		return true;
	}
	/// <summary>
	/// Check if string contains only digits.
	/// </summary>
	/// <returns><c>true</c>, if only digits was contained, <c>false</c> otherwise.</returns>
	/// <param name="str">String.</param>
	bool containsOnlyDigits(string str) {
		foreach (char c in str) {
			if (!(c <= '9' && c >= '0')) {
				return false;
			}
		}
		return true;
	}
	/// <summary>
	/// if notified, make blockCopy = null. This is needed in case user puts block in the trashcan without setting anything
	/// (while dragging).
	/// </summary>
	public void notified() {
		//Debug.Log("notified");
		blockCopy = null;
		blockCopy2 = null;
		createBool = true;
		state = State.NOTMOVED;
		notifiyBool = true;
	}
}
