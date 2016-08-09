using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SerialToInput : MonoBehaviour {
	public SerialStream serialInput;
	public static bool isSerialInit = false;

	//rotation + button presses from the serial input
	private Vector3 rotation;
	private bool buttonAPressed;
	private bool buttonBPressed;
	private bool buttonAHeld;
	private bool buttonBHeld;
	private float buttonAStart;
	private float buttonBStart;
	private bool buttonAHoldCheck;
	private bool buttonBHoldCheck;


	private bool debug;



	// Use this for initialization
	void Start () {
		
		//if an instance of this object already exists, destroy this.
		if (isSerialInit == true) {
			printDebug ("Destorying SerialToController");
			Destroy (this.gameObject);
		}
		isSerialInit = true;
	}
	
	// Update is called once per frame
	void Update () {
		updateInputs (serialInput.getLine());
		/*printDebug (buttonA);
		printDebug (buttonB);
		printDebug (rotation.ToString());*/
	}

	/// <summary>
	/// Updates the inputs.
	/// </summary>
	/// <param name="s">S.</param>
	private void updateInputs(string s){
		//updates the logic depending on the input of the form
		//button,x,y,z
		try{
			char[] seperator = {','};
			string[] splitString = s.Split(seperator,4);
		// if any of them are null, exception will be thrown and the line will be ignored.
			if(splitString[0].Equals("A")){
				buttonAPressed = true;
				buttonBPressed = false;
				buttonBHeld = false;
				buttonBHoldCheck = false;
				checkButtonA();
			}
			else if(splitString[0].Equals("B")){
				buttonBPressed = true;
				buttonAPressed = false;
				buttonAHeld = false;
				buttonAHoldCheck = false;
				checkButtonB();
			}
			else if(splitString[0].Equals("C")){
				buttonAPressed = true;
				buttonBPressed = true;

				checkButtonA();
				checkButtonB();

			}
			else{
				buttonAPressed = false;
				buttonBPressed = false;
				buttonAHoldCheck = false;
				buttonBHoldCheck = false;
				buttonAHeld = false;
				buttonBHeld = false;
				
				
			}
			string X = splitString[1];
			string Y = splitString[2];
			string Z = splitString[3];

			rotation.Set((float) int.Parse(X),(float) int.Parse(Y), (float) int.Parse(Z));


		}catch(System.Exception){
			return;
		}
	}

	/// <summary>
	/// Checks the button a.
	/// </summary>
	private void checkButtonA(){
		if(buttonAHoldCheck == false){
			buttonAHoldCheck = true;
			buttonAStart = Time.time;
		}
		else if(buttonAHoldCheck == true){
			if((Time.time - buttonAStart)<0.25){
				//Do nothing
				buttonAHeld = false;
			}
			else{
				buttonAHeld = true;
			}
		}
	}

	/// <summary>
	/// Checks the button b.
	/// </summary>
	private void checkButtonB(){
		if(buttonBHoldCheck == false){
			buttonBHoldCheck = true;
			buttonBStart = Time.time;
		}
		else if(buttonBHoldCheck == true){
			if((Time.time - buttonBStart)<0.25){
				//Do nothing
				buttonBHeld = false;
			}
			else{
				buttonBHeld = true;
			}
		}
	}


	/// <summary>
	/// Gets A pressed.
	/// </summary>
	/// <returns><c>true</c>, if A pressed was gotten, <c>false</c> otherwise.</returns>
	public bool getAPressed(){
		return this.buttonAPressed;
	}


	/// <summary>
	/// Gets the B pressed.
	/// </summary>
	/// <returns><c>true</c>, if B pressed was gotten, <c>false</c> otherwise.</returns>
	public bool getBPressed(){
		return this.buttonBPressed;
	}

	/// <summary>
	/// Gets A held.
	/// </summary>
	/// <returns><c>true</c>, if A held was gotten, <c>false</c> otherwise.</returns>
	public bool getAHeld(){
		return this.buttonAHeld;
	}
		
	/// <summary>
	/// Gets the B held.
	/// </summary>
	/// <returns><c>true</c>, if B held was gotten, <c>false</c> otherwise.</returns>
	public bool getBHeld(){
		return this.buttonBHeld;
	}

	/// <summary>
	/// Gets the rotation.
	/// </summary>
	/// <returns>The rotation.</returns>
	public Vector3 getRotation(){
		return this.rotation;
	}

	private void printDebug(string s){
		if (debug) {
			Debug.Log (s);
		}
	}

}
