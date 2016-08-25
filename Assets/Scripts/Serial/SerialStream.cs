using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;


public class SerialStream : MonoBehaviour {
	private SerialPort stream;
	private string inline;
	public string COM;
	public int BAUD;
	public int timeout = 0;
	public static bool isSerialStreamInit = false;

	private float lastRecievedTime;

	private bool timingOut = false;

	private bool debug = false;

	

	// Use this for initialization
	// Unity does not use constructors, but rather Start() and Awake()
	void Start () {


		//if an instance of this object already exists, destroy this.
		if (isSerialStreamInit == true) {
			printDebug ("Destorying duplicate SerialStream");
			Destroy (this.gameObject);
		}
		isSerialStreamInit = true;

		this.stream = new SerialPort (COM, BAUD);
		if (this.timeout == 0) {
			this.stream.ReadTimeout = 5;
		} else {
			this.stream.ReadTimeout = this.timeout;
		}
		openStream ();
	}
	
	// Update is called once per frame
	void Update () {
		if ((inline = this.readStream ()) != null) {
			printDebug (inline);
		}

		StartCoroutine(AsyncReaduBit(10f));

	}

	//Send and Read CoRoutines
	public IEnumerator AsyncReaduBit(float timeout, System.Action fail = null){
		//timeout should be very small (microseconds);
		float startTime = Time.time;
		float currentTime = 0;
		float diff = 0;

		pinguBit ();

		string readLine = null;
		do {
			try {
				readLine = readStream ();
			} catch (System.Exception) {
				//printDebug(e);
				readLine = null;
			}
			if(readLine != null){
				this.inline = readLine;
				this.stream.BaseStream.Flush();
				lastRecievedTime = Time.time;
				timingOut = false;
			}
			else{
				if((Time.time - lastRecievedTime)> 1f){
					//essentially a manual timeout
					printDebug ((lastRecievedTime-Time.time).ToString());
					timingOut = true;
					StartCoroutine(retryConnection());
				}
				yield return new WaitForSeconds(0.016f);
			}

			currentTime = Time.time;
			//difference in milliseconds
			diff = (currentTime - startTime)*1000;
		} while(diff < timeout);

		if(fail !=null){
			fail();
		}
		else{
			yield return null;
		}

	}
		

	//return inline
	public string getLine(){
		return this.inline;
	}

	//pings uBit to send infomation
	private void pinguBit(){
		try{
		//printDebug("Pinging uBit");
		this.stream.WriteLine ("SEND");
		this.stream.BaseStream.Flush ();
		}
		catch(System.Exception e){
			if (e is System.IO.IOException) {
				//Catches an unplogged exception, ignores timeouts
				printDebug ("catch");
				//StartCoroutine (retryConnection ());
			}
		}
	}


	//attempts 	to get the line from the buffer until the timeout value set
	private string readStream(){
		try{
			String line = stream.ReadLine();
			printDebug(line);
			return line;
		}catch(System.Exception e){
			printDebug ("Error with reading stream");
			printDebug (e.ToString());
			if (e.GetType() == typeof (System.TimeoutException)) {
				//Do nothing
			}
			//StartCoroutine (retryConnection ());
			return null;
		}
	}

	//Opens the stream
	private void openStream(){
		printDebug ("Initialised");
		try{
		this.stream.Open();
		}
		catch(System.IO.IOException e) {
			printDebug ("Something went wrong with opening stream");
			printDebug (e.ToString());

			//Attempt to reconnect
			StartCoroutine (retryConnection ());
		}
	}
	//Closing connection
	void OnDisable(){
		try{
		this.stream.Close ();
		printDebug("Closing Stream");
		}
		catch(System.IO.IOException){
		}
		catch(System.Exception){
		}
	}


	//On a disconnect, attempt to reopen the connection
	public IEnumerator retryConnection(){
		printDebug("retrying");
		if (this != null) {
			do {
				try {
					this.stream.Close ();
					this.stream.Open ();
				} catch (System.Exception) {
				
				}
				yield return new WaitForSeconds (0.016f);
			} while(!stream.IsOpen);
	
			printDebug ("Ending Retrys");

			yield break;
		}

	}

	//Returns if a timeout is occuring
	public bool isTimingOut(){
		return timingOut;
	}

	private void printDebug(String s){
		if (debug == true) {
			Debug.Log (s);
		}
	}



}
