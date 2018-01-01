using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class playScript : MonoBehaviour {
	public GameObject choice;
	private enum State {Start, Menu, Basic, Record, Speak};
	private State state = State.Start;
	private GUIStyle menuStyle;
	private Texture2D upTexture;
	private Texture2D downTexture;
	private Texture2D leftTexture;
	private Texture2D rightTexture;
	private Font fontStyle;
	private bool createBool;
	private SliderMenu slider;
	private bool isReset;
	const uint threshold = 40000;
	private string moveText;
	private bool forwardBackward;
	GameObject bmObj;
	private string rotateText;
	private bool leftRight;
	private string speakText;
	private string repeatText;
	private BluetoothLE BLE;
	//private Bluetooth bluetooth;
	private bool scanBool;
	//become true when scan is done
	private bool scanDoneBool;
	public static int uid;
	//public GUISkin guiSkin;
	// Use this for initialization
	void Start () {
		uid = 0;
		BLE = null;
		//Camera.main.orthographic = true;
		choice.SetActive(false);
		//Screen.orientation = ScreenOrientation.Landscape; //will be changed
		createBool = true;
		scanBool = false;
		scanDoneBool = false;
		//Getting the font and textures needed
		fontStyle = Resources.Load("Fonts/LSANSDI") as Font;
		upTexture = Resources.Load("ArrowImg/arrow_up") as Texture2D;
		downTexture = Resources.Load("ArrowImg/arrow_down") as Texture2D;
		leftTexture = Resources.Load("ArrowImg/arrow_left") as Texture2D;
		rightTexture = Resources.Load("ArrowImg/arrow_right") as Texture2D;
		isReset = false;
	}
	
	// Update is called once per frame
	void Update () {

	}
	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI() {
		menuStyle = new GUIStyle(GUI.skin.button);
		//menuStyle = new GUIStyle(guiSkin.button);
		//GUI.skin = guiSkin;
		menuStyle.fontSize = 40;
		menuStyle.font = fontStyle;
		if (state == State.Start) {
			defaultCamera();
			startButton();
			if (isBackKeyPressed(KeyCode.Escape)) {
				//if (BLE != null) BLE.disonnect();
				Application.Quit();
			}
		} else if (state == State.Menu) {
			defaultCamera();
			menuButton();
			if (isBackKeyPressed(KeyCode.Escape)) {
				state = State.Start;
			}
		} else if (state == State.Basic) {
			basicSetup();
			if (isBackKeyPressed(KeyCode.Escape)) {
				//destroyObjects();
				state = State.Menu;
				createBool = true;
				//makeChoiceInvisible();
			}
		} else if (state == State.Record) {
			blockDiagramSetup();
			if (isReset) {
				checkStatus();
				Rect tempRect = new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2);
				tempRect = GUI.Window(1, tempRect, windowFunc, "ReSetting");
			}
			if (isBackKeyPressed(KeyCode.Escape)) {
				destroyObjects();
				state = State.Menu;
				createBool = true;
				makeChoiceInvisible();
			}
		}

	}
	/// <summary>
	/// buttons in the starts. Can't chnage states until bluetooth is enabled
	/// </summary>
	void startButton() {
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.375f - Screen.height / 4, Screen.width/2, Screen.height/4),"Start", menuStyle)) {
			if (BLE != null && BLE.connected) {
				state = State.Menu;
			}
			//state = State.Menu;
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.625f - Screen.height / 4, Screen.width/2, Screen.height/4),"Scan Arduino", menuStyle)) {
			scanBool = true;
		}
		//connect after scan
		GUI.enabled = scanDoneBool;
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.875f - Screen.height / 4, Screen.width/2, Screen.height/4),"Connect", menuStyle)) {
			BLE.connect();
		}
		//Android Only need IOS version
		if (scanBool) {
			BLE = GameObject.Find("BluetoothLE").GetComponent<BluetoothLE>();
			BLE.Initialize();
			BLE.scanBLE();
			scanBool = false;
		}
		if (BLE != null && BLE.scanned) {
			scanDoneBool = true;
		}
		GUI.enabled = true;
		if (scanDoneBool) {
			GUI.Label(new Rect(Screen.width - 150, Screen.height - 150, 100, 100), BLE.SystemStr);
		}
		if (BLE == null || !BLE.connected) {
			GUI.Label(new Rect(Screen.width - 150, Screen.height - 100, 100, 50), "BluetoothLE Not Connected");
			//GUI.Label(new Rect(Screen.width - 100, Screen.height - 150, 50, 50), soundRec.Data);
			//GUI.Label(new Rect(Screen.width - 100, Screen.height - 200, 50, 50), soundRec.SystemString);
		}
	}
	/// <summary>
	/// Checks the status.
	/// </summary>
	void checkStatus() {
		string[] strArr = bmObj.name.Split('-');
		if (strArr[0] == "0") {
			moveText = strArr[1];
			forwardBackward = (strArr[2] == "1") ? true : false;
		} else if (strArr[0] == "1") {
			rotateText = strArr[1];
			leftRight = (strArr[2] == "1") ? true : false;
		} else if (strArr[0] == "2") {
			strArr = bmObj.name.Split('\n');
			speakText = strArr[1];
		}
	}
	/// <summary>
	/// Windows the func.
	/// </summary>
	/// <param name="windowid">Windowid.</param>
	void windowFunc(int windowid) {
		GameObject.Find ("MoveBlock").GetComponent<Block>().setWindowClosed(false);
		//moving
		if (bmObj.name.StartsWith("0")) {
			moveText = GUI.TextField(new Rect(100, 50, 50, 20), moveText);
			forwardBackward = GUI.Toggle(new Rect(20, 20, 160, 50), forwardBackward, "Foward/Backward");
			if (!errorCheckStr(moveText, bmObj.name.Substring(0, 1))) {
				moveText = "1";
			}
			bmObj.name = "0-" + moveText + (forwardBackward ? "-1" : "-0");
			bmObj.transform.FindChild("Text").GetComponent<TextMesh>().text = "Move\n" + moveText +(forwardBackward ? "\nforward" : "\nbackward"); 
			//rotating
		} else if (bmObj.name.StartsWith("1")) {
			rotateText = GUI.TextField(new Rect(100, 50, 50, 20), rotateText);
			leftRight = GUI.Toggle(new Rect(20, 20, 100, 40), leftRight, "Left/Right");
			if (!errorCheckStr(rotateText, bmObj.name.Substring(0, 1))) {
				rotateText = "45";		
			}
			bmObj.name = "1-" + rotateText + (leftRight ? "-1" : "-0");
			bmObj.transform.FindChild("Text").GetComponent<TextMesh>().text = "Rotate\n" + rotateText +(leftRight ? "\nLeft" : "\nRight"); 
			//speak
		} else if (bmObj.name.StartsWith("2")) {
			speakText = GUI.TextField(new Rect(100, 50, 50, 20), speakText);
			if (!errorCheckStr(speakText, bmObj.name.Substring(0, 1))) {
				speakText = "Hello World";
			} else if (speakText.Length > 18) {
				speakText = speakText.Substring(0, 18);
			}
			bmObj.name = "2-" + speakText;
			bmObj.transform.FindChild("Text").GetComponent<TextMesh>().text = "Speak\n" + speakText;
		} else if (bmObj.name.StartsWith("F")) {
			repeatText = GUI.TextField(new Rect(100, 50, 50, 20), repeatText);
			bmObj.name = "F-" + repeatText;
		}
		//setting
		if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height /2 - 80, 40, 40), "Set")) {
			bmObj = null;
			isReset= false;
			GameObject.Find ("MoveBlock").GetComponent<Block>().setWindowClosed(true);
		}
	}
	/// <summary>
	/// check string.
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
	/// Containses the only digits.
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
	/// Defaults the camera.
	/// </summary>
	void defaultCamera() {
		Camera.main.clearFlags = CameraClearFlags.Skybox;
	}
	/// <summary>
	/// Menus the button.
	/// </summary>
	//default orientation is landscape
	void menuButton() {
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.375f - Screen.height / 4, Screen.width / 2, Screen.height / 4),"Basic", menuStyle)) {
			state = State.Basic;
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.625f - Screen.height / 4, Screen.width / 2, Screen.height / 4),"Record", menuStyle)) {
			state = State.Record;
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.875f - Screen.height / 4, Screen.width / 2, Screen.height / 4),"Sound", menuStyle)) {
			//sound
		}
	}
	//it will only have up left down right
	//0 - up
	//1 - down
	//2 - left
	//3 - right
	/// <summary>
	/// Basics the setup.
	/// </summary>
	void basicSetup() {
		//up arrow
		if (GUI.RepeatButton(new Rect(Screen.width / 2 - Screen.width / 8, Screen.height / 10, Screen.height / 4, Screen.height / 4), upTexture)) {
			//bluetooth.write("0");
			BLE.Send("0");
		}
		//down arrow
		if (GUI.RepeatButton(new Rect(Screen.width / 2 - Screen.width / 8, 3 * Screen.height / 4, Screen.height / 4, Screen.height / 4), downTexture)) {
			//bluetooth.write("1");
			BLE.Send("1");
		}
		//left arrow
		if (GUI.RepeatButton(new Rect(Screen.width / 4 - Screen.width / 8, Screen.height / 2 - Screen.width / 8, Screen.height / 4, Screen.height / 4), leftTexture)) {
			//bluetooth.write("2");
			BLE.Send("2");
		}
		//right arrow
		if (GUI.RepeatButton(new Rect(Screen.width / 2 + Screen.width / 8, Screen.height / 2 - Screen.width / 8, Screen.height / 4, Screen.height / 4), rightTexture)) {
			//bluetooth.write("3");
			BLE.Send("3");
			//send 3
		}
	}
	//block diagram where by connecting elements you can send commands.
	//
	/// <summary>
	/// Blocks the diagram setup.
	/// </summary>
	void blockDiagramSetup() {
		Camera.main.backgroundColor = Color.white;
		Camera.main.clearFlags = CameraClearFlags.SolidColor;
		createMenu();
	}
	/// <summary>
	/// Creates the menu.
	/// </summary>
	//create slide menu
	void createMenu() {
		if (createBool) {
			gameObject.AddComponent<GridCreator>();
			createQuads();

			//create menu button
			GameObject prefab = Resources.Load("Prefabs/menuCanvas2") as GameObject;
			Quaternion rot = Quaternion.identity;
			rot.eulerAngles = new Vector3(0, 0, 0);
			GameObject temp = GameObject.Instantiate(prefab, new Vector3(0, 0, 0), rot) as GameObject;
			slider = gameObject.AddComponent<SliderMenu>();
			temp.name = "CanvasBig";

			//assign event so that container can be dragged;
			temp.transform.Find("menuBut").gameObject.GetComponent<Button>().onClick.AddListener(() => {slider.slide();});
			//temp.transform.Find("Panel").transform.Find("Back").gameObject.GetComponent<Button>().onClick.AddListener(() => {slider.unSlide();});

			//creating menu screen
			prefab = Resources.Load("Prefabs/Container1") as GameObject;
			temp = GameObject.Instantiate(prefab, new Vector3(-40, 1.01f, 0), rot) as GameObject;
			temp.name = "Container";

			//creating trashcan
			rot.eulerAngles = new Vector3(0, 0, 180);
			prefab = Resources.Load("Prefabs/TrashCan") as GameObject;
			temp = GameObject.Instantiate(prefab, new Vector3(8, 6.05f, 0), rot) as GameObject;
			temp.name = "TrashCan";
			createBool  = false;
		}
		//send the data to the robot
		if (GUI.Button(new Rect(Screen.width / 2 - 20, 0, 40, 40), "Send")) {
			sendDataPacket();
		}
		//clear the screen
		if (GUI.Button(new Rect(Screen.width / 2 - 80, 0, 40, 40), "Clear")) {
			uid = 0;
			clearScreen();
			//resetGrid();
			choice.SetActive(false);
			bmObj = null;
		}
	}
//	void resetGrid() {
//		GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
//		foreach (GameObject g in grids) {
//			g.GetComponent<Grid>().isContain = false;
//		}
//	}
	/// <summary>
	/// Clears the screen.
	/// </summary>
	void clearScreen() {
		GameObject[] gTemp = GameObject.FindGameObjectsWithTag("objTag");
		foreach (GameObject g in gTemp) {
			if (g.GetComponent<BlockMoved>() != null) {
				Destroy(g);
			}
		}
	}
	/// <summary>
	/// create top with black plane
	/// </summary>
	void createQuads() {
		GameObject gT = GameObject.CreatePrimitive(PrimitiveType.Quad);
		gT.transform.localScale = new Vector3(gT.transform.localScale.x * 100, 1, 1);
		gT.transform.position = new Vector3(0, 6.3f, 0);
		gT.name = "top";
		gT.GetComponent<Renderer>().material.color = Color.black;
	}
	/// <summary>
	/// Sends the data packet.
	/// </summary>
	void sendDataPacket() {
		//GameObject[] gTemp = GameObject.Find("Map").GetComponentsInChildren<GameObject>();
		string newStr = "";
		GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
		foreach (GameObject g in grids) {
			if (g.GetComponent<Grid>().isContain) {

				newStr += g.transform.GetChild(0).name + ",";
			}
		}
		//newStr.Remove(newStr.Length - 1);
		//newStr.
		//send the data through bluetooth
		//bluetooth.write(newStr);
		BLE.Send(newStr);
		//Debug.Log(newStr);
	}
	bool isBackKeyPressed(KeyCode key) {
		if (Event.current.type == EventType.KeyDown) {
			return (Event.current.keyCode == key);
		}
		return false;
	}
	/// <summary>
	/// Destroies the objects.
	/// </summary>
	void destroyObjects() {
		GameObject[] gTemp = GameObject.FindGameObjectsWithTag("objTag");
		foreach (GameObject g in gTemp) {
			Destroy(g);
		}
		gTemp = GameObject.FindGameObjectsWithTag("Grid");
		foreach (GameObject g in gTemp) {
			Destroy(g);
		}
		Destroy(GameObject.Find("top").gameObject);
	}
	/// <summary>
	/// Sets the choice menu.
	/// </summary>
	/// <param name="bm">Bm.</param>
	public void setChoiceMenu(GameObject bm) {
		choice.SetActive(true);
		choice.transform.GetChild(0).GetComponent<RectTransform>().localPosition = bm.transform.position;//bm.GetComponent<RectTransform>().localPosition + bm.transform.parent.transform.localPosition;
		bmObj = bm;
	}
	/// <summary>
	/// Raises the click event. This is not fully implemented yet do not use Delete
	/// </summary>
	public void onClick(string buttonName) {
		if (buttonName == "Back") {
			backOn();
		} else if (buttonName == "Delete") {
			//Debug.Log("Deleted");
			//Debug.Log("Deleting" + bmObj.name);
			Destroy(bmObj);
			bmObj = null;
		}
		choice.SetActive(false);
		//Debug.Log("Clicked " + buttonName);
	}
	private void backOn() {
		isReset = true;
	}
	/// <summary>
	/// Makes the choice invisible.
	/// </summary>
	public void makeChoiceInvisible() {
		choice.SetActive(false);
		bmObj = null;
	}

}
