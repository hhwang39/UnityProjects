using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
//============================================================================//
//								Variables									  //
//===========================================================================//
	//cardName
	private string cardName1;
	private string cardName2;
	//number of cards left
	private int cardsLeft;
	//card that has been flipped
	private Card card1;
	private Card card2;
	private Bluetooth bluetooth;
	private SoundRecorder soundRec;
	public RaycastHit hit;
	private bool blueEnable;
	enum cardState {Ready, P1Start, P1Flip1, P1Flip2, Comparing, P2Start, P2Flip1, P2Flip2, Finish};
	private enum State {Start, Menu, Setting, Play, Playing, Done};
	private State state = State.Start;
	private cardState prevState;
	CardGen cGen;
	cardState cState = cardState.Ready;
	private int p1Score;
	private int p2Score;
	//score increase for each success on guess
	private int score;
	private bool blueBool;
	//bluetooth object
	private GameObject gO;
	//sound object
	private GameObject gO2;
	//data to send
	private string dataSend;
	private ArrayList sequence;
	//check if data is sent
	private bool sendBool;
	private int index1;
	private int index2;
	private int P1Count;
	private int P2Count;
	private int drCount;
	private bool strLogic;
	private bool soundBool;
//=============================================================================//
					// Use this for initialization
//============================================================================//
	void Start () {
		//check if we have sent the data once if so do not send it again
		sendBool = false;
		sequence = new ArrayList();
		//empty data to send in the beginning
		dataSend = "";
		//setting scores
		score = 10;
		P1Count = PlayerPrefs.GetInt("P1Count");
		P2Count = PlayerPrefs.GetInt("P2Count");
		drCount = PlayerPrefs.GetInt("drCount");
		increBool = true;
		//defaulting background color
		//Camera.main.backgroundColor = color2;
		//Using bluetooth gameobject
		gO = GameObject.Find("Bluetooth");
		gO2 = GameObject.Find("SoundRecorder");
		soundBool = false;
		strSound = "Enable Sound";
	}
//=================================================================================//
//									Update              						//
//=================================================================================//
	// Update is called once per frame
	void Update () {

	}
	//only updates when it is not comparing
	void FixedUpdate() {
		if (cState == cardState.P1Start || cState == cardState.P2Start || cState == cardState.P1Flip1 ||
		    cState == cardState.P1Flip2 || cState == cardState.P2Flip1 || cState == cardState.P2Flip2) {
			StartCoroutine(runGame());
		}
	}
	//actaul game playing cardFliping and choosing will be done here
	private IEnumerator runGame() {
		//Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		//see where ray game
		if (blueBool && !sendBool) {
			setDataFormToSend();
			sendMessage();
			sendBool = true;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//This is if bluetooth has been chosen
		// Robot mode
		if (blueBool && cState == cardState.P2Start) {
			strLogic = (soundRec.Data.Contains("turn")|| soundRec.Data.Contains("ahead"));
			if (!strLogic){
				soundRec.speak();
			}
			if (!bluetooth.isEmptyData() && strLogic) {
				prevState = cardState.P2Flip2;
				string[] names = parseForm();
				cardName1 = names[0];
				cardName2 = names[1];
				card1 = GameObject.Find(cardName1).GetComponent<Card>();
				card2 = GameObject.Find(cardName2).GetComponent<Card>();
				card1.flipCard();
				card2.flipCard();
				yield return new WaitForSeconds(card1.animation["flipping"].length);
				yield return new WaitForSeconds(2.0f);
				index1 = int.Parse(cardName1);
				index2 = int.Parse(cardName2);
				updateSequence(index1, index2, "f");
				cState = cardState.Comparing;
				if (compareCards()) {
					Destroy(card1, 0.5f);
					Destroy(card2, 0.5f);
					card1.destroyCard();
					card2.destroyCard();
					cardsLeft -= 2;
					increaseScore();
					updateSequence(index1, index2, "o");
					Debug.Log("cards left = " + cardsLeft);
					sendBool = false;
				} else {
					card1.unFlipCard();
					card2.unFlipCard();
					yield return new WaitForSeconds(card2.animation["unFlipping"].length);
					sendBool = false;
				}
				card1.setFlipped(false);
				card2.setFlipped(false);
				card1 = null;
				card2 = null;
				cState = cardState.P1Start;
				if (blueBool && !sendBool) {
					setDataFormToSend();
					sendMessage();
					sendBool = true;
					soundRec.getVoiceData("");
				}
			}
			//Human Mode
		} else {
			if (Input.GetMouseButtonDown (0))
				//if (Input.touchCount > 0)
			{
				if (Physics.Raycast(ray, out hit, 100))
					//if (hit.collider != null) 
				{
					//Choosing first Card
					if (card1 == null) {
						cardOneChosen();
						yield return new WaitForSeconds(card1.animation["flipping"].length);
						sendBool = false;
						index1 = int.Parse(card1.name);
						updateSequence(index1, -1, "f");
						if (blueBool && !sendBool) {
							setDataFormToSend();
							sendMessage();
							sendBool = true;
						}
					} else {
						//choosing second Card
						cardTwoChosen();
						if (card2 != null) {
							prevState = cState;
							sendBool = false;
							index2 = int.Parse(card2.name);
							updateSequence(index1, index2, "f");
							Debug.Log(card1.ToString());
							Debug.Log(card2.ToString());
							if (blueBool && !sendBool) {
								setDataFormToSend();
								sendMessage();
								sendBool = true;
							}
							cState = cardState.Comparing;
							yield return new WaitForSeconds(card2.animation["flipping"].length);
							yield return new WaitForSeconds(1.5f);
							if (compareCards()) {
								Destroy(card1, 0.5f);
								Destroy(card2, 0.5f);
								card1.destroyCard();
								card2.destroyCard();
								cardsLeft -= 2;
								increaseScore();
								updateSequence(index1, index2, "o");
								Debug.Log("cards left = " + cardsLeft);
								sendBool = false;
							} else {
								card1.unFlipCard();
								card2.unFlipCard();
								yield return new WaitForSeconds(card2.animation["unFlipping"].length);
								//yield return new WaitForSeconds(card2.animation["unFlipping"].length);
								//card1.setRotation();
								//card2.setRotation();
							}
							card1.setFlipped(false);
							card2.setFlipped(false);
							card1 = null;
							card2 = null;
							if (blueBool && !sendBool) {
								setDataFormToSend();
								sendMessage();
								sendBool = true;
							}
							yield return new WaitForSeconds(0.01f);
							if (prevState == cardState.P1Flip2) {
								cState = cardState.P2Start;
							} else if (prevState == cardState.P2Flip2) {
								cState = cardState.P1Start;
							}
						}
					}
				}
			}
		}
		//game ends when no more cards left
		if (cardsLeft == 0) {
			state = State.Done;
			cState = cardState.Finish;
		}
	}
	//change cardState based on states
	void changeState() {
		if (cState == cardState.P1Start) {
			cState = cardState.P1Flip1;
		} else if (cState == cardState.P1Flip1) {
			cState = cardState.P1Flip2;
		} else if (cState == cardState.P1Flip2) {
			cState = cardState.P2Start;
		}else if (cState == cardState.P2Start) {
			cState = cardState.P2Flip1;
		} else if (cState == cardState.P2Flip1) {
			cState = cardState.P2Flip2;
		} else if (cState == cardState.P2Flip2) {
			cState = cardState.P1Start;
		}
	}

	//compare cards based on its tag
	bool compareCards () {
		if ((card1.getTag() == card2.getTag()) && (card1 != null || card2 != null)) {
			return true;
		}
		return false;
	}
	//will increase score based on previous states
	void increaseScore() {
		if (prevState == cardState.P1Flip2) {
			p1Score += score;
		} else if (prevState == cardState.P2Flip2) {
			p2Score += score;
		}
	}
	//choose card one and flip it
	void cardOneChosen() {
		card1 = hit.transform.gameObject.GetComponent<Card>();
		card1.setFlipped(true);
		card1.flipCard();
		changeState();
		//Debug.Log("is it true" + (card1 == null ? "True": "False"));
		//Destroy(card1);
	}
	//choose card two and flip it
	void cardTwoChosen() {
		card2 = hit.transform.gameObject.GetComponent<Card>();
		if (!card2.getFlipped()) {
			card2.setFlipped(true);
			card2.flipCard();
			changeState();
		} else {
			card2 = null;
		}
	}
	//send message
	//This has to follow 
	//check if blueBool is true
	//setDataFormToSend
	//sendMessage
	void sendMessage() {
		bluetooth.write(dataSend);
	}
	//set the dataForm to send
	//Data From will follow State,numRows,numCols,p1Score,p2Score,seqence,card1ID,card2ID
	void setDataFormToSend() {
		dataSend = "" + cState + "," + cGen.numRows + "," + cGen.numCols + "," + p1Score + "," + p2Score +
			",";
		addSequence();
		int k1 = -1;
		int k2 = -1;
		if (card1 != null) {
			k1 = card1.getNum();
		}
		if (card2 != null) {
			k2 = card2.getNum();
		}
		dataSend += "," + k1 + "," + k2;
	}
	//add seqence to the dataSend
	void addSequence() {
		for (int i = 0; i < sequence.Count; ++i) {
			dataSend += (string) sequence[i];
		}
	}
	//initalize the sequence. It will be always xxxxxxxxxx (based on number of cards)
	void initSequence() {
		for (int i = 0; i < cGen.numCards; ++i) {
			sequence.Add("x");
		}
	}
	//update the card seqence o refers to removing Cards from seqence and f refers to flipping Cards
	void updateSequence(int i, int j, string Command) {
		if (Command == "o") {
			sequence[i] = "o";
			sequence[j] = "o";
		} else if (Command == "f") {
			if (card1 != null) {
				sequence[i] = "" + card1.getNum();
			}
			if (card2 != null && j != -1) {
				sequence[j] = "" + card2.getNum();	
			}
		}
	}
	//parse the string that we got from bluetooth
	string[] parseForm() {
		string[] words = bluetooth.readStr.Split(',');
		bluetooth.readStr = "";
		return words;
	}
//========================================================================================//
//	                                    GUI PARTS
//========================================================================================//
	//Variables Used in GUI
	private GUIStyle menuStyle;
	private GUIStyle labelStyle;
	//private GUIStyle sliderStyle;
	//private GUIStyle thumbStyle;
	//private GUIStyle butStyle;
	//private GUIStyle textStyle;
	//check if win count has been incremented
	private bool increBool;
	private string strSound;
	//Shown on GUI
	void OnGUI() {
		labelStyle = new GUIStyle(GUI.skin.label);
		labelStyle.fontSize = 35;
		menuStyle = new GUIStyle(GUI.skin.button);
		menuStyle.fontSize = 40;
		if (state == State.Start) {
			playButton();
			increBool = true;
			if (Application.platform == RuntimePlatform.Android) {
				if (isBackKeyPressed(KeyCode.Escape)) {
					Application.Quit();
				}
			}
		} else if (state == State.Menu) {
			menuChoice();
			if (Application.platform == RuntimePlatform.Android) {
				if (isBackKeyPressed(KeyCode.Escape)) {
					state = State.Start;
				}
			}
		} else if (state == State.Setting) {
			settingMenu();
			if (Application.platform == RuntimePlatform.Android) {
				if (isBackKeyPressed(KeyCode.Escape)) {
					state = State.Start;
				}
			}
		} else if (state == State.Play) {
			cGen.Play(); //generate cards
			initSequence();
			state = State.Playing;
			cState = cardState.P1Start;
			//cGen = null;
		} else if (state == State.Playing) {
			displayScores();
			if (soundBool) GUI.Label(new Rect(Screen.width - 100, Screen.height - 150, 50, 50), soundRec.Data);
			if (Application.platform == RuntimePlatform.Android) {
				if (isBackKeyPressed(KeyCode.Escape))
				{
					state = State.Menu;
					//cGen.destroyAll();
					deleteAll();
					cGen = null;
					cState = cardState.Ready;
					p1Score = 0;
					p2Score = 0;
					sequence.Clear();
					blueBool = false;
					bluetooth = null;
				}
			}
		} else if (state == State.Done) {
			cGen = null;
			doneGui();
			if (Application.platform == RuntimePlatform.Android) {
				if (isBackKeyPressed(KeyCode.Escape))
				{
					state = State.Start;
					cState = cardState.Ready;
					p1Score = 0;
					p2Score = 0;
					sequence.Clear();
					blueBool = false;
					bluetooth = null;
				}
			}
		}
	}
	//setName
	void setName(ref string player, string name) {
		player = name;
	}
	bool isBackKeyPressed(KeyCode key) {
		if (Event.current.type == EventType.KeyDown) {
			return (Event.current.keyCode == key);
		}
		return false;
	}
	void deleteAll() {
		GameObject[] gTemp = GameObject.FindGameObjectsWithTag("cardTag") as GameObject[];
		foreach (GameObject g in gTemp) {
			Destroy(g);
		}
	}
	void playButton() {
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.375f - Screen.height / 4, Screen.width/2, Screen.height/4),"Play Game", menuStyle)) {
			state = State.Menu;
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.625f - Screen.height / 4, Screen.width/2, Screen.height/4),"Bluetooth", menuStyle)) {
			blueEnable = true;
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.875f - Screen.height / 4, Screen.width/2, Screen.height/4),"Setting", menuStyle)) {
			//resetSetting();
			//soundRec.speak();
			state = State.Setting;
		}
		if (blueEnable) {
			bluetooth = gO.GetComponent<Bluetooth>();
			bluetooth.initialize();
			blueEnable = false;
			bluetooth.connect();
			blueBool = true;
		}
		if (blueBool && bluetooth.genStr == "can't connect") {
			blueBool = false;
		}
		if (blueBool) {
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 50, 50), bluetooth.genStr);
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 150, 50, 50), bluetooth.readStr);
		} else if (!blueBool) {
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 50, 50), "Person Mode");
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 150, 50, 50), soundRec.Data);
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 200, 50, 50), soundRec.SystemString);
		}
		//GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 50, 50), "Wrong2");
		
	}
	void menuChoice() {
		if (GUI.Button(new Rect(Screen.width/4, Screen.height / 8,Screen.width/2, Screen.height/4),"EASY", menuStyle)) {
			cGen = new CardGen(3, 2);
			cardsLeft = cGen.numCards;
			Camera.main.fieldOfView = 105;
			Camera.main.transform.position = new Vector3(3.5f, 10.0f, 4.0f);
			Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
			state = State.Play;
		} else if (GUI.Button(new Rect(Screen.width/4, 3*Screen.height/8, Screen.width/2,Screen.height/4),"MEDIUM", menuStyle)) {
			cGen = new CardGen(4, 3);
			cardsLeft = cGen.numCards;
			Camera.main.fieldOfView = 120;
			Camera.main.transform.position = new Vector3(2.0f, 10.0f, 6.0f);
			Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
			state = State.Play;
		} else if (GUI.Button(new Rect(Screen.width/4, 5*Screen.height / 8, Screen.width/2,Screen.height/4),"HARD", menuStyle)) {
			cGen = new CardGen(5, 4);
			cardsLeft = cGen.numCards;
			Camera.main.fieldOfView = 125;	
			Camera.main.transform.position = new Vector3(0.0f, 10.5f, 8.0f);
			Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
			state = State.Play;
		}
		
	}
	void resetSetting() {
		PlayerPrefs.SetInt("P1Count", 0);
		PlayerPrefs.SetInt("P2Count", 0);
		PlayerPrefs.SetInt("drCount", 0);
		P1Count = 0;
		P2Count = 0;
		drCount = 0;
	}
	void settingMenu() {
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.375f - Screen.height / 4, Screen.width/2, Screen.height/4),"Reset", menuStyle)) {
			resetSetting();
		}
		if (GUI.Button(new Rect(Screen.width / 4, Screen.height * 0.625f - Screen.height / 4, Screen.width/2, Screen.height/4), strSound, menuStyle)) {
			soundBool = !soundBool;
			if (soundBool) {
				soundRec = gO2.GetComponent<SoundRecorder>();
				soundRec.init();
				strSound = "Disable Sound";
			} else {
				soundRec = null;
				strSound = "Enable Sound";
			}
		}
	}
	void displayScores() {
		GUI.Label(new Rect(10, 10, 240, 60), "P1 Score: " + p1Score, labelStyle);
		GUI.Label(new Rect(Screen.width - 280, 10, 240, 60), "P2 Score: " + p2Score, labelStyle);
	}
	void doneGui() {
		GUIStyle largeFont = new GUIStyle();
		largeFont.fontSize = 60;
		largeFont.normal.textColor = Color.green;
		if (p1Score > p2Score) {
			GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height/2,100,50), "Congrats Player1 wins", largeFont);
		} else if (p2Score > p1Score) {
			GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height/2,100,50), "Congrats Player2 wins", largeFont);
		} else {
			GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height/2,100,50), "Congrats It's Tie", largeFont);
		}
		if (increBool) {
			incrementWinCount();
			increBool = false;
		}
		GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 + 100, 200, 50), "P1: " + P1Count + " P2: " + P2Count + " Draw: " + drCount, largeFont);
	}
	void incrementWinCount() {
		if (p1Score > p2Score) {
			P1Count++;
		} else if (p2Score > p1Score) {
			P2Count++;
		} else if (p1Score == p2Score) {
			drCount++;
		}
		PlayerPrefs.SetInt("P1Count", P1Count);
		PlayerPrefs.SetInt("P2Count", P2Count);
		PlayerPrefs.SetInt("drCount", drCount);
	}

}
