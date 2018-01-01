//#define DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class gameCreator : MonoBehaviour {
	GameObject wheel;
	Wheel newWheel;
	const int numOfCard = 25;
	GameObject prefab = Resources.Load("Images/card") as GameObject;
	List<string> textArr = new List<string>();
	List<string> textCopy;
	Vector3 startPos = new Vector3(400, 70, 9);
	GameObject card1;
	GameObject card2;
	bool blueBool;
	int numChosen1;
	int numChosen2;
	char op;
	List<GameObject> objList;
	// Use this for initialization
	void Start () {
		op = '+';
		createList();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
//=====================================useful functions ===============================//
	public void revealCard1(int i){
		numChosen1 = i;
		card1.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/" + textArr[i - 1]) as Texture2D;
	}
	public void revealCard2(int i) {
		numChosen2 = i;
		card2.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/" + textArr[i - 1]) as Texture2D;
	}
	public int retCount() {
		return newWheel.count;
	}
	public int retNum() {
		return newWheel.numToAdd;
	}
	public bool retBool() {
		return newWheel.finBool;
	}
	public void setOperator(char c) {
		op = c;
	}
	private void createList() {
		for (int i = 1; i <= numOfCard; ++i) {
			textArr.Add(i.ToString());
		}
	}
//================================== Creating =============================================//
	//This creates a list of cards with numbers.
	//it will creat only five in random order
	public void createNumberList() {
		objList = new List<GameObject>();
		textCopy = new List<string>(textArr);
		int numTot = 0;
		if (op == '+') {
			numTot = numChosen1 + numChosen2;
		} else if (op == '-') {
			numTot = numChosen1 - numChosen2;
		} else if (op == '*') {
			numTot = numChosen1 * numChosen2;
		}
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0, 90, 80);
		GameObject temp = GameObject.Instantiate(prefab, new Vector3(0, 0, 0), rot) as GameObject;
		//startPos.x, startPos.y - i * 29, startPos.z + i * 5
		temp.name = "" + numTot.ToString();
		temp.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/" + textCopy[numTot - 1]) as Texture2D;
		objList.Add(temp);
		textCopy[numTot - 1] = "NaN";
		int count = 0;
		int randNum = 0;
		//choose random cards from 1 - 10
		while (count < 4) {
			randNum = Random.Range(0, numOfCard);
#if DEBUG
			Debug.Log("Random Number " + randNum);
#endif
			if (textCopy[randNum] != "NaN") {
				temp = GameObject.Instantiate(prefab, new Vector3(0, 0, 0), rot) as GameObject;
				temp.name = "" + (randNum + 1).ToString();
				temp.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/" + textCopy[randNum]) as Texture2D;
				textCopy[randNum] = "NaN";
				objList.Add(temp);
				count++;
			}
		}
		randomizePosition();
	}
	private void randomizePosition () {
		List<Vector3> posList = new List<Vector3>();
		for (int i = 0; i < 5; ++i) {
			posList.Add(new Vector3(startPos.x, startPos.y - i * 29, startPos.z + i * 5));
		}
		for (int i = 0; i < 5; ++i) {
			int randNum = Random.Range(0, posList.Count);
			objList[i].transform.position = posList[randNum];
			posList.RemoveAt(randNum);
		}
	}
	//creat wheel to play
	public void createWheel() {
		GameObject model = Resources.Load("Images/WheelV2") as GameObject;;
		if (op == '+') {
			model = Resources.Load("Images/WheelV2") as GameObject;
		} else if (op == '-') {
			model = Resources.Load("Images/WheelV2M") as GameObject;
		}
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0, 90, 90);
		wheel = GameObject.Instantiate(model, new Vector3(20, 0, 0), rot) as GameObject;
		wheel.transform.localScale = new Vector3(6, 1, 6);
		newWheel = wheel.GetComponent<Wheel>();
		newWheel.setOP(op);
		if (blueBool) {
			newWheel.bluetoothEnable(blueBool);
		}
	}
	//create "+" sign
	public void createOperator() {
		GameObject fab = Resources.Load("Images/op") as GameObject;
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0, 90, 80);
		GameObject temp = GameObject.Instantiate(fab, new Vector3(-30, 20, 10), rot) as GameObject;
		if (op == '+') {
			temp.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/plusImg") as Texture2D;
		} else if (op == '-') {
			temp.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/minusImg") as Texture2D;
		} else if (op == '*') {
			temp.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/multiImg") as Texture2D;
		}

		temp.name = "op";
	}
	//create "=" sign
	public void createEqual() {
		GameObject fab = Resources.Load("Images/equal") as GameObject;
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0, 90, 80);
		GameObject temp = GameObject.Instantiate(fab, new Vector3(-30, -40, 20), rot) as GameObject;
		temp.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/equalim") as Texture2D;
		temp.name = "equal";
	}
	//create card1 with default 12(should be empty or unrevealed)
	public void createCard1() {
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0, 90, 80);
		card1 = GameObject.Instantiate(prefab, new Vector3(-30, 50, 10), rot) as GameObject;
		card1.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/12") as Texture2D;
		card1.name = "cardT";
	}
	//create card2 with default 12(should be empty or unrevealed)
	public void createCard2() {
		Quaternion rot = Quaternion.identity;
		rot.eulerAngles = new Vector3(0, 90, 80);
		card2 = GameObject.Instantiate(prefab, new Vector3(-30, -10, 25), rot) as GameObject;
		card2.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/12") as Texture2D;
		card2.name = "cardT";
	}
	//<summary/>
	//create number of stars based on given number
	//</summary>
	public void createStars(int num, Vector3 starPos) {
		GameObject model = Resources.Load("Images/starFab") as GameObject;
		for (int i = 0; i < num; ++i) {
			Quaternion rot = Quaternion.identity;
			rot.eulerAngles = new Vector3(0, 90, 80);
			GameObject temp = GameObject.Instantiate(model, new Vector3(starPos.x, starPos.y - 15 * i, starPos.z), rot) as GameObject;
			temp.name = "star";
			temp.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/star") as Texture2D;
		}
	}
//	public void createBD() {
//		GameObject fab = Resources.Load("Images/bd") as GameObject;
//		Quaternion rot = Quaternion.identity;
//		rot.eulerAngles = new Vector3(0, -180, 89);
//		GameObject temp = GameObject.Instantiate(fab, new Vector3(15, -30, -40), rot) as GameObject;
//	}
//===================================Destroying======================================//
	public void destroyCards() {
		GameObject[] gTemp = GameObject.FindGameObjectsWithTag("card") as GameObject[];
		foreach (GameObject g in gTemp) {
			Destroy(g);
		}
	}
	public void destroyWheel() {
		Destroy(wheel);
	}
	public void destroyStars() {
		GameObject[] gTemp = GameObject.FindGameObjectsWithTag("starTag") as GameObject[];
		foreach (GameObject g in gTemp) {
			Destroy(g);
		}
	}
//===============================checking bluetooth enabled=========================//
	public void setBlueBool(bool b) {
		blueBool = b;
	}
	public void setStr(string str) {
		if (newWheel != null) {
			newWheel.setWheelStr(str);
		}
	}
}

//35 34 13 /35 6 18 /35 -23 23
