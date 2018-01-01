//#define DEBUG
using UnityEngine;
using System.Collections;
public class playScript : MonoBehaviour {
	enum playerState {Num1C, Num2C};
	RaycastHit hit;
	Camera cam;
	enum gameState {Start, Menu, Play, fakePlaying, realPlaying,End};
	gameState gs = gameState.Start;
	GUIStyle butGUI;
	int num1;
	int num2;
	GameObject cardChosen;
	Vector3 oldPos;
	bool cardBool;
	bool blueEnable;
	bool win;
	gameCreator creator;
	bool blueBool;
	Texture2D plusTexture;
	Texture2D minusTexture;
	Texture2D multiTexture;
	private Bluetooth bluetooth;
	bool showGUIWindow;
	string windowStr;
	char opcode;
	//count the number of star
	uint starCount;
	//count how many times player play
	uint playCount;
	bool playBool;
	bool updateBool;
	bool endBool;
	// Use this for initialization
	void Start () {
		endBool = false;
		playCount = 0;
		playBool = true;
		updateBool = true;
		opcode = '+';
		showGUIWindow = true;
		cam = GameObject.Find("Cam2").GetComponent<Camera>();
		cam.transform.position = new Vector3(370, 25, 100);
		cam.transform.eulerAngles = new Vector3(11, 180, 270);
		cardBool = false;
		num1 = 0;
		num2 = 0;
		Camera.main.transform.position = new Vector3(0, 25, 110);
		Camera.main.transform.eulerAngles = new Vector3(11, 180, 270);
		Camera.main.fieldOfView = 54;
		creator = gameObject.AddComponent<gameCreator>();
		cardChosen = null;
		win = false;
		plusTexture = Resources.Load("Images/plusImg") as Texture2D;
		minusTexture = Resources.Load("Images/minusImg") as Texture2D;
		multiTexture = Resources.Load("Images/multiImg") as Texture2D;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void FixedUpdate() {
		if (blueBool) {
			creator.setStr(bluetooth.readStr);
		}
		if (gs != gameState.End) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Ray ray2 = cam.ScreenPointToRay(Input.mousePosition);
			if (Input.GetMouseButtonDown (0)) {
				#if DEBUG
				Debug.Log("Mouse Down");
				#endif
				if (Physics.Raycast(ray, out hit) || Physics.Raycast(ray2, out hit)) {
					#if DEBUG
					Debug.Log("Hitted");
					#endif
					GameObject temp = hit.transform.gameObject;
					if (temp.tag == "card" && temp.name != "cardT" && temp.name != "equal" && temp.name != "op" && temp.name != "star"){
						chooseCard();
					}
				}
			}
		}
	}
	void chooseCard() {
		#if DEBUG
		Debug.Log("running fuc");
		Debug.Log("x pos: " + hit.transform.gameObject.transform.position.x.ToString());
		#endif
		if (cardChosen == null && !cardBool) {
			#if DEBUG
			Debug.Log("Choosing");
			#endif
			cardChosen = hit.transform.gameObject;
			oldPos = cardChosen.transform.position;
			cardChosen.transform.position = new Vector3(-30, -60, 35);
			showGUIWindow = true;
		}
		if (cardChosen != null && cardBool) {
			#if DEBUG
			Debug.Log("Unchoosing");
			#endif
			cardChosen.transform.position = oldPos;
			cardChosen = null;
		}
		cardBool = !cardBool;
	}
	void OnGUI() {
		butGUI = new GUIStyle(GUI.skin.button);
		butGUI.fixedHeight = Screen.height / 4;
		butGUI.fixedWidth = Screen.width / 2;
		butGUI.fontSize = 40;
		if (gs == gameState.Start) {
			endBool = false;
			starCount = 0;
			playBut();
			if (isAndroidAndBackPressed()) {
				Application.Quit();
			}
		} else if (gs == gameState.Menu) {
			menuBut();
			if (isAndroidAndBackPressed()) {
				gs = gameState.Start;
			}
		} else if (gs == gameState.Play) {
			createWorld();
			gs = gameState.fakePlaying;
		} else if (gs == gameState.fakePlaying) {
			//if gamecreator returns the first value assign it to num1
			if (creator.retCount() == 1 && creator.retBool()) {
				num1 = creator.retNum();
				creator.revealCard1(num1);
			} else if (creator.retCount() == 2 && creator.retBool()) {
				num2 = creator.retNum();
				creator.revealCard2(num2);
			}
			if (creator.retCount()>= 2) {
#if DEBUG
				//Debug.Log("Num1 = " + num1.ToString());
				//Debug.Log("Num2 = " + num2.ToString());
#endif
				creator.destroyWheel();
				creator.createNumberList();
				gs = gameState.realPlaying;

			}
			if (isAndroidAndBackPressed()) {
				destroyWorld();
				if (bluetooth != null) {
					bluetooth.genStr = "";
					bluetooth = null;
				}
				blueBool = false;
				playCount = 0;
				gs = gameState.Start;
			}

		} else if (gs == gameState.realPlaying) {
			win = checkAns();
			if (isAndroidAndBackPressed()) {
				destroyWorld();
				if (bluetooth != null) {
					bluetooth.genStr = "";
					bluetooth = null;
				}
				blueBool = false;
				playCount = 0;
				gs = gameState.Start;
			}
			if (showGUIWindow && cardChosen != null) {
				Rect windowRect = GUI.Window(0, new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), DoMyWindow, "RESULT");	
			}
//			if (win) {
//				//creator.destroyCards();
//				gs = gameState.End;
//			}
		} else if (gs == gameState.End) {
			//show based on number of stars
			if (starCount == 0) {
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), 
				          "You got " + starCount.ToString() + "stars\nYou can do better!");	
			} else if (starCount == 1) {
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), 
				          "You got " + starCount.ToString() + "star\nYou can do better!");	
			} else if (starCount == 2) {
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), 
				          "You got " + starCount.ToString() + "stars\nTry More!");	
			} else if (starCount == 3) {
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), 
				          "You got " + starCount.ToString() + "stars\nYou are almost there!");	
			} else if (starCount == 4) {
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), 
				          "You got " + starCount.ToString() + "stars\nGood Job!");	
			} else if (starCount == 5) {
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100), 
				          "You got " + starCount.ToString() + "stars\nExcellent Job!");	
			}
			if (!endBool) {
				destroyWorld();
				creator.createStars((int) starCount, new Vector3(-15, 35, 15));
				endBool = true;
			}

			if (isAndroidAndBackPressed()) {
				destroyWorld();
				if (bluetooth != null) {
					bluetooth.genStr = "";
					bluetooth = null;
				}
				playCount = 0;				
				blueBool = false;
				gs = gameState.Start;
			}
		}
	}
	void DoMyWindow(int windowID) {
		if (playBool) {
			playCount++;
			playBool = false;
		}
		if (win) {
			windowStr = "You are Correct!";
			//we need to update only once, otherwise it update every frame
			if (updateBool)  {
				starCount++;
				creator.createStars((int) starCount, new Vector3(-15, 35, 15));
				updateBool = false;
			} 
		} else {
			if (updateBool)	{
				creator.createStars((int) starCount, new Vector3(-15, 35, 15));
				updateBool = false;
			}
			windowStr = "You are Wrong!";
		}
		if (GUI.Button(new Rect(10, 70, Screen.width / 2 - 20, Screen.height / 2 - 80), windowStr)) {
			showGUIWindow = false;
			//make updatebool true so that we can update it again
			updateBool = true;
			if (playCount != 5) {
				returnWorld();
				creator.destroyStars();
				playBool = true;
				gs = gameState.Play;
			} else {
				playBool = true;
				playCount = 0;
				gs = gameState.End;
			}
		}
	}
	//creating card 1, 2 and wheel
	void createWorld() {
		creator.createWheel();
		creator.createCard1();
		creator.createCard2();
		creator.createOperator();
		creator.createEqual();
	}
	void destroyWorld() {
		creator.destroyWheel();
		creator.destroyCards();
		creator.destroyStars();
	}
	void returnWorld() {
		creator.destroyWheel();
		creator.destroyCards();
	}
	//display menu it will have three choices +, -, *, (might not /)
	void menuBut() {
		if (GUI.Button(new Rect(Screen.width - 7 * Screen.width / 8, Screen.height / 2 - Screen.height / 4, Screen.width/4, Screen.width/4), plusTexture)) {
			creator.setOperator('+');
			opcode = '+';
		}
		if (GUI.Button(new Rect(Screen.width - 5 * Screen.width / 8, Screen.height / 2 - Screen.height / 4, Screen.width/4, Screen.width/4),minusTexture)) {
			creator.setOperator('-');
			opcode = '-';
		}
		if (GUI.Button(new Rect(Screen.width - 3 * Screen.width / 8, Screen.height / 2 - Screen.height / 4, Screen.width/4, Screen.width/4),multiTexture)) {
			creator.setOperator('*');
			opcode = '*';
		}
	}
	//play button
	void playBut() {
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.375f - Screen.height / 4, Screen.width/2, Screen.height/4),"Play Game", butGUI)) {
			gs = gameState.Play;
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.625f - Screen.height / 4, Screen.width/2, Screen.height/4),"Bluetooth", butGUI)) {
			blueEnable = true;
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.875f - Screen.height / 4, Screen.width/2, Screen.height/4),"Setting", butGUI)) {
			gs = gameState.Menu;
		}
		if (blueEnable) {
			bluetooth = GameObject.Find ("Bluetooth").GetComponent<Bluetooth>();
			bluetooth.initialize();
			blueEnable = false;
			bluetooth.connect();
			blueBool = true;
		}
		if (blueBool && bluetooth.genStr == "can't connect") {
			blueBool = false;
		}
		creator.setBlueBool(blueBool);
		//indicate if it is connected
		if (blueBool) {
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 50, 50), bluetooth.genStr);
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 150, 50, 50), bluetooth.readStr);
		} else if (!blueBool) {
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 50, 50), "Person Mode");
			//GUI.Label(new Rect(Screen.width - 100, Screen.height - 150, 50, 50), soundRec.Data);
			//GUI.Label(new Rect(Screen.width - 100, Screen.height - 200, 50, 50), soundRec.SystemString);
		}
	}
	bool isBackKeyPressed(KeyCode key) {
		if (Event.current.type == EventType.KeyDown) {
			return (Event.current.keyCode == key);
		}
		return false;
	}
	bool isAndroidAndBackPressed() {
		if (Application.platform == RuntimePlatform.Android) {
			if (isBackKeyPressed(KeyCode.Escape))
			{
				return true;
			}
		}
		return false;
	}
	public bool checkAns() {
		if (cardChosen == null) {
			return false;
		}
#if DEBUG
		Debug.Log("cardChosen: " + cardChosen.name);
#endif
		int ans = int.Parse(cardChosen.name);
		if ((ans == (num1 + num2)) && opcode == '+') {
			return true;
		} else if ((ans == (num1 - num2)) && opcode == '-') {
			return true;
		} else if (ans == (num1 * num2) && opcode == '*') {
			return true;
		}
		return false;
		
	}
}