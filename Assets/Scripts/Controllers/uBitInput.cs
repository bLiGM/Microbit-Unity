using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class uBitInput : MonoBehaviour {


	public static bool isSerialInit = false;

	//rotation + button presses from the serial input
	static private int[] baseRotations = new int[3];
	static private int[] rotation = new int[3];
	static private bool buttonAPressed = false;
	static private bool buttonBPressed = false;
	static private bool buttonAHeld = false;
	static private bool buttonBHeld = false;
	static private float buttonAStart;
	static private float buttonBStart;
	static private bool buttonAHoldCheck;
	static private bool buttonBHoldCheck;
	static private bool doReset = true;
	static private float delayInputTimer;


	private bool debug = false;



	// Use this for initialization
	void Awake () {

		//if an instance of this object already exists, destroy this.
		if (isSerialInit == true) {
			printDebug ("Destorying SerialToController");
			Destroy (this.gameObject);
		}
		isSerialInit = true;
	}

	// Update is called once per frame
	void Update () {
		updateInputs (SerialStream.getLine());
		printDebug (getRotations().ToString());
	}

	/// <summary>
	/// Updates the inputs.
	/// </summary>
	/// <param name="s">S.</param>
	private void updateInputs(string s){
		//updates the logic depending on the input of the form
		//button,x,y,z
		if (Time.realtimeSinceStartup < delayInputTimer) {
			rotation [0] = 0;
			rotation [1] = 0;
			rotation [2] = 0;
			buttonAPressed = false;
			buttonBPressed = false;
			buttonAHeld = false;
			buttonBHeld = false;
		} else {
			try {
				if (s.Equals ("Reset")) {
					//do resets
					doReset = true;
					return;
				}

				char[] seperator = { ',' };
				string[] splitString = s.Split (seperator, 4);
				// if any of them are null, exception will be thrown and the line will be ignored.
				if (splitString [0].Equals ("A")) {
					buttonAPressed = true;
					buttonBPressed = false;
					buttonBHeld = false;
					buttonBHoldCheck = false;
					checkButtonA ();
				} else if (splitString [0].Equals ("B")) {
					buttonBPressed = true;
					buttonAPressed = false;
					buttonAHeld = false;
					buttonAHoldCheck = false;
					checkButtonB ();
				} else if (splitString [0].Equals ("C")) {
					buttonAPressed = true;
					buttonBPressed = true;

					checkButtonA ();
					checkButtonB ();

				} else {
					buttonAPressed = false;
					buttonBPressed = false;
					buttonAHoldCheck = false;
					buttonBHoldCheck = false;
					buttonAHeld = false;
					buttonBHeld = false;


				}

				string roll = splitString [1];
				string pitch = splitString [2];
				string yaw = splitString [3];

				rotation [0] = int.Parse (roll);
				rotation [1] = int.Parse (pitch);
				rotation [2] = int.Parse (yaw);


				if (doReset) {
					baseRotations [0] = int.Parse (roll);
					baseRotations [1] = int.Parse (pitch);
					baseRotations [2] = int.Parse (yaw);
					doReset = false;
				}


			} catch (System.Exception) {
				return;
			}
		}
	}

	/// <summary>
	/// Checks the button a.
	/// </summary>
	private void checkButtonA(){
		if(buttonAHoldCheck == false){
			buttonAHoldCheck = true;
			buttonAStart = Time.realtimeSinceStartup;
		}
		else if(buttonAHoldCheck == true){
			if((Time.realtimeSinceStartup - buttonAStart)<0.25){
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
			buttonBStart = Time.realtimeSinceStartup;
		}
		else if(buttonBHoldCheck == true){
			if((Time.realtimeSinceStartup - buttonBStart)<0.25){
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
	static public bool getAPressed(){
		return buttonAPressed;
	}


	/// <summary>
	/// Gets the B pressed.
	/// </summary>
	/// <returns><c>true</c>, if B pressed was gotten, <c>false</c> otherwise.</returns>
	static public bool getBPressed(){
		return buttonBPressed;
	}

	/// <summary>
	/// Gets A held.
	/// </summary>
	/// <returns><c>true</c>, if A held was gotten, <c>false</c> otherwise.</returns>
	static public bool getAHeld(){
		return buttonAHeld;
	}

	/// <summary>
	/// Gets the B held.
	/// </summary>
	/// <returns><c>true</c>, if B held was gotten, <c>false</c> otherwise.</returns>
	static public bool getBHeld(){
		return buttonBHeld;
	}

	//-------------------------------------------//
	//Returns the unadjusted roll/pitch/yaw values//
	//-------------------------------------------//
	private int getRoll(){
		return rotation [0];
	}

	private int getPitch(){
		return rotation [1];
	}

	private int getYaw(){
		return rotation [2];
	}
	//-------------------------------------------//
	//-------------------------------------------//
	//-------------------------------------------//

	//-------------------------------------------//
	//Returns the adjusted roll/pitch/yaw values, adjusted with respect to the base rotation//
	//Adds a deadzone at the extreme rotations to prevent accidental overrotating//
	//-------------------------------------------//
	static private int getAdjustedRoll(){
		//Roll goes from -180 degrees to +180
		return adjustAngle(rotation[0] - baseRotations[0], 180);
	}

	static private int getAdjustedPitch(){
		//Pitch goes from -90 degrees to +90
		return adjustAngle(rotation[1] - baseRotations[1], 90);
	}

	static private int getAdjustedYaw(){
		//Yaw goes from 0 degrees to 360
		//Since the yaw is VERY sensitive, I decided to put a deadzone in the neutral position aswell//
		//I also needed to dampen the yaw, since the yaw is extremely noisy
		if (Mathf.Abs (rotation [2] - baseRotations [2]) < 8) {
			return 0;
		} else if (rotation [2] - baseRotations [2] < -8) {
			return adjustAngle(rotation[2] - baseRotations[2] + 8, 90);
		}
		else{
			return adjustAngle(rotation[2] - baseRotations[2] - 8, 90);
		}
	}

	static private int adjustAngle(int a, int limit){
		int returnVal;
		if (a > limit) {
			returnVal = -limit + (limit - a);
		} else if (a < -limit) {
			returnVal = limit - (-limit - a);
		} else {
			returnVal = a;
		}

		//----------Deadzone------------//
		if (returnVal > limit - limit / 3) {
			return limit - limit / 3;
		} else if (returnVal < -limit + limit / 3) {
			return -limit + limit / 3;
		} else {
			return returnVal;
		}
	}

	//-------------------------------------------//
	//-------------------------------------------//
	//-------------------------------------------//


	//Returns the rotations as a Unity Vector3//
	//Adjusts the rotations to match the Unity axes//
	static public Vector3 getRotations(){
		//------The rotations must be slightly tweaked to match the Unity axes//
		return new Vector3 (-getAdjustedPitch(), getAdjustedYaw(), -getAdjustedRoll());
	}

	private void printDebug(string s){
		if (debug) {
			Debug.Log (s);
		}
	}

	static public bool isSerialTimeOut(){
		return SerialStream.isTimingOut ();
	}


	//Delays further input by s seconds
	static public void delayInput(float s){
		delayInputTimer = Time.realtimeSinceStartup + s;
	}

}
