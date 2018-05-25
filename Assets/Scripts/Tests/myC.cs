using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myC : MonoBehaviour {

	public string ID;
	public string name;
	public float X;
	public float Y;
	public float Z;

	public myC(string ID, string name, float X, float Y, float Z) {
		this.ID = ID;
		this.name = name;
		this.X = X;
		this.Y = Y;
		this.Z = Z;
	}

}
