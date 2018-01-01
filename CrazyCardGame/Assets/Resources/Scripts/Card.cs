using UnityEngine;
using System.Collections;
public class Card : MonoBehaviour {
	//its card
	public GameObject cardView;
	//tag to be compared if they are same
	private string cardTag;
	//check if it is flipped
	private bool flipped;
	//number to be used
	private int num;
	public int index;
	//change texture based on given type
	public void setType(string type) {
		cardView.GetComponent<Renderer>().material.mainTexture = Resources.Load("Images/" + type) as Texture2D;
		setTag(type);
		flipped = false;
	}
	//setting number so that we can send message
	public void setNum(int n) {
		num = n;
	}
	public int getNum() {
		return num;
	}
	//assign tag to compare a match
	void setTag(string type) {
		cardTag = type;
	}
	public string getTag() {
		return cardTag;
	}
//	void OnMouseDown() {
//		//Destroy(cardView);
//		//Destroy(gameObject);
//		flipCard();
//	}
	public void flipCard() {
		Vector3 oldPos = gameObject.transform.position;
		GetComponent<Animation>().Play("flipping");
		//audio.Play();
		//cardView.animation.Play ("flipping");
		GetComponent<Animation>()["flipping"].speed = 1;
		//gameObject.transform.Rotate(new Vector3(0, 0, 0));
		gameObject.transform.position = oldPos;
	}
	public void unFlipCard() {
		Vector3 oldPos = gameObject.transform.position;
		GetComponent<Animation>().Play("unFlipping");
		GetComponent<Animation>()["unFlipping"].speed = 1;
		//audio.Play();
		//gameObject.transform.Rotate(new Vector3(0, 180, 0));
		gameObject.transform.position = oldPos;
	}
	public void destroyCard() {
		Destroy(gameObject);
		Destroy(this);
	}
//	public void setRotation() {
//		gameObject.transform.eulerAngles = new Vector3(0, 90, 0);
//	}
	public bool getFlipped() {
		return flipped;
	}
	public void setFlipped(bool f) {
		flipped = f;
	}
	public override string ToString ()
	{
		return "card is " + (flipped ? "Flipped" : "Not Flipped");
	}
	void OnMouseDown() {
		Debug.Log("this" + ToString());
	}
}