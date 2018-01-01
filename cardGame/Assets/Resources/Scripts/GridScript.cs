using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class GridScript : MonoBehaviour {
	//boolean value for checking if it grid should be cleared or created
	public bool clearBool = false;
	public bool	createBool = false;
	//register bool
	public bool regBool = false;
	//how many rows will we need
	public int numRows = 4;
	//how many cols will we need
	public int numCols = 5;
	//statring position of first card
	public Vector3 startingPos = new Vector3(5, 5, 0);
	//distance from one card to another in x direction
	public const int dx = -3;
	//public const int dy = 0;
	//distance from one card to another in z direction
	public const int dz = 4;
	//Model that will be copied over
	public GameObject defaultModel;
	//choose default rotation
	public Quaternion defaultRot = Quaternion.identity;
	//list that stores cards' positions and type
	public List<GameObject> cardLists = new List<GameObject>();
	// Use this for initialization
	void Start () {
		//initialze default rotation of cards
		defaultRot.eulerAngles = new Vector3(0, 90, 90);
	}
	
	// Update is called once per frame
	void Update () {
		clearGrid();
		createGrid();
	}

	// clear the grid
	void clearGrid() {
		if (clearBool) {
			for (int i = 0; i < cardLists.Count; ++i) {
				DestroyImmediate(cardLists[i]);	
			}
			cardLists.Clear();
			clearBool = false;
		}
	}

	// create the grid
	void createGrid() {
		if (createBool) {
			int index = 0;
			//create objects at certain positions
			for (int i = 0; i < numRows; ++i) {
				for (int j = 0; j < numCols; ++j) {
					GameObject temp = Instantiate(defaultModel, new Vector3(0, 0, 0), defaultRot) as GameObject;
					temp.name = "card" + index;
					//temp.transform.rotation = Quaternion.Euler(0, 90, 90);
					temp.transform.position = new Vector3(startingPos.x + dx*i, startingPos.y, startingPos.z + dz * j);
					cardLists.Add (temp);
					index++;
				}
			}
			createBool = false;
		}

	}
}
