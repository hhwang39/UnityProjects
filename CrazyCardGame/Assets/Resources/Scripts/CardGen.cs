#define DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardGen {
	private const int dx = -5; 	//distance from one card to another in x direction
	public int numRows;
	public int numCols;
	//number of cards
	public int numCards;
	//distance from one card to another in z direction
	private const int dz = 5;
	//start pos
	private Vector3 startPos = new Vector3(9, 5, 1);
	//list of where positions goning to be
	private List <Vector3> posList = new List<Vector3>();
	//set rotation
	private Quaternion defaultRot = Quaternion.identity;
	//string of texture lists
	private ArrayList strArrTexture = new ArrayList(new string[] {"car1" ,"car2","car3","car4","car5", "car6","car7", "car8", "car9", "car10"});
	public GameObject newModel;
	//list that contains cards
	public List<Card> cardList = new List<Card>();
	Card tempCard;
	public List <Vector3> posCopyList = new List<Vector3>();
	// Use this for initialization
	void Start () {

	}
	public CardGen(int row, int col) {
		numRows = row;
		numCols = col;
		numCards = row * col;
	}
	public void Play() {
		//newModel = Resources.Load("model/Card") as GameObject;
		newModel = Resources.Load("model/model") as GameObject;
		createPos();
		creatCardList();
		generateCards();
		assignNumbers();
	}
	//assign each positions (for now it will be always rect shape)
	void createPos() {
		for (int i = 0; i < numRows; ++i) {
			for (int j = 0; j < numCols; ++j) {
				Vector3 tempVec = new Vector3(startPos.x + dx * i, startPos.y, startPos.z + dz * j);
				posList.Add(tempVec);
			}
		}
		posCopyList.AddRange(posList);
	}
	//create cards on lists
	void creatCardList() {
		defaultRot.eulerAngles = new Vector3(0, 180, 0);
		for (int i = 0; i < (numRows * numCols); ++i) {
#if DEBUG
			Debug.Log("total Count: " + (numRows * numCols));
#endif
			GameObject temp = GameObject.Instantiate(newModel, new Vector3(0, 0, 0), defaultRot) as GameObject;
			int numRand = Random.Range(0, posList.Count - 1);
			temp.transform.position = posList[numRand];
			posList.RemoveAt(numRand);
			cardList.Add(temp.GetComponent<Card>());
		}
	}
	//genenerate cards with random texture
	void generateCards() {
		#if DEBUG
		Debug.Log("CardList Count: " + cardList.Count.ToString());
		#endif
		for (int i = 0; i < (cardList.Count >> 1); ++i) {
			Card c1 = cardList[i];
			Card c2 = cardList[cardList.Count - 1 - i];
			string textType = randomTexture();
			c1.setType(textType);
			c2.setType(textType);
			c1.setNum(i);
			c2.setNum(i);
		}
	}
	//return random texture from one of strTexture
	string randomTexture() {
		#if DEBUG
		Debug.Log("Count: " + strArrTexture.Count.ToString());
		#endif
		int randNum = Random.Range(0, strArrTexture.Count);
		string s = (string) strArrTexture[randNum];
		strArrTexture.RemoveAt(randNum);
		return s;
	}
	//top 0 to number of cards bottom right
	// EX) 2X3 0 2 4
	//         1 3 5
	void assignNumbers() {
		int j =0;
		for (int i = 0; i < cardList.Count; ++i) {
			j = 0;
			while (cardList[j].transform.position != posCopyList[i]) {
				++j;
			}
			cardList[j].index = i;
		}
		for (int i = 0; i < cardList.Count; ++i) {
			cardList[i].name = "" + cardList[i].index;
		}
	}
	//destory every cards
	//public void destroyAll() {
	//	for (int i = 0; i < cardList.Count; ++i) {
	//		tempCard = cardList[i];
	//		tempCard.destroyCard();
	//	}
	//	cardList.Clear();
	//}
}
