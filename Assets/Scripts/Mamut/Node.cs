using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public string ID;
	public string Name;
	public bool Visibility = true;
	public float Radius = 10;
	public float Quality = 1;
	public float T;
	public float X;
	public float Y;
	public float Z;
	public int Frame;
	public Color Color = Color.cyan;
	public List<string> Parents = new List<string>();
	public List<string> Children = new List<string>();
	public GameObject Spot;
	public GameObject Sphere;

	public Node(string ID, string Name, float X, float Y, float Z, int Frame) {
		this.ID = ID;
		this.Name = Name;
		this.X = X;
		this.Y = Y;
		this.Z = Z;
		this.Frame = Frame;
		this.T = Frame;
	}

	public Node(string ID, string Name, float X, float Y, float Z, int Frame, float Radius) {
		this.ID = ID;
		this.Name = Name;
		this.X = X;
		this.Y = Y;
		this.Z = Z;
		this.Frame = Frame;
		this.Radius = Radius;
		this.T = Frame;
	}

	public Node(string ID, string Name, float X, float Y, float Z, int Frame, float Radius, float deltaT) {
		this.ID = ID;
		this.Name = Name;
		this.X = X;
		this.Y = Y;
		this.Z = Z;
		this.Frame = Frame;
		this.T = Frame * deltaT;
		this.Radius = Radius;
	}

	// <Spot ID="279952" name="ID279952" VISIBILITY="1" RADIUS="10.0" QUALITY="-1.0" SOURCE_ID="0" POSITION_T="18.0" POSITION_X="470.8245898642763" POSITION_Y="219.1190193300717" FRAME="18" POSITION_Z="461.7333318100787" />


}
