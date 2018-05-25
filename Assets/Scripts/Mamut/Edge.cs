using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge {

	public Node Source;
	public Node Target;
	public string ID;
	public float Distance;
	public float Velocity;
	public Color Color;
	public GameObject Obj;
	public Track Track;

	// <Edge SPOT_SOURCE_ID="279888" SPOT_TARGET_ID="279889" LINK_COST="-1.0" VELOCITY="44.369362721790885" DISPLACEMENT="44.369362721790885" />

	public Edge(Node Source, Node Target) {
		this.Source = Source;
		this.Target = Target;
		this.ID = string.Format("{0},{1}", Source.ID, Target.ID);
		CalculateDistance();
		CalculateVelocity();
	}

	private void CalculateDistance() {
		Distance = (float)Math.Sqrt(Math.Pow((Target.X - Source.X),2) + Math.Pow((Target.Y - Source.Y),2) + Math.Pow((Target.Z - Source.Z),2));
	}

	private void CalculateVelocity() {
		Velocity = Distance / (Target.T - Source.T);
	}

}
