using UnityEngine;
using System.Collections;

public class RoundTo : MonoBehaviour {
	public float input = 5;

	// Use this for initialization
	void Start () {



	}
	
	// Update is called once per frame
	void Update () {
		RoundTo45 (input);
	}

	public float RoundTo45(float Decimal){
		float i = Decimal%45;
		//Debug.Log(i);
		if (i < 45 / 2) {
			Decimal -= i;
			//Debug.Log (Decimal);
			return Decimal;
		}
		else if (i>45/2){
			Decimal += (45-i);
			//Debug.Log(Decimal);
			return Decimal;
		}
		else{	
			return 0f;
		}
	}
}
