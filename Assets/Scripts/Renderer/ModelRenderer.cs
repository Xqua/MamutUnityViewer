using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRenderer : MonoBehaviour {

	public Model Model;
	public GameObject Spot;
	public GameObject Sphere;
	public GameObject Edge;
	public GameObject LineEdge;
	public Dictionary<string, GameObject> RenderedNodes;
	public Dictionary<string, GameObject> RenderedEdges;
	public ModelControler ModelControler;

	// Use this for initialization
	void Start () {
		RenderedNodes = new Dictionary<string, GameObject>();
		RenderedEdges = new Dictionary<string, GameObject>();
	}


	private void EdgeChangeState() {
		// Erasing depending on Last State (this should save some execution time !)
		int d = 0;
		switch (ModelControler.LastEdgeRenderType) {
			case ModelControler.EdgeRenderTypes.LocalCenter:
				d = ModelControler.TrackDepth/2;
				for (int i = -d; i <= d; i++) {
					if (ModelControler.LastFrame+i >= 0 && ModelControler.LastFrame+i < Model.EdgeFrames.Count) {
						foreach (string edgeID in Model.EdgeFrames[ModelControler.LastFrame+i]) {
							HideEdge(edgeID);
						}
					}
				}
				break;
			case ModelControler.EdgeRenderTypes.LocalForward:
				d = ModelControler.TrackDepth;
				for (int i = 0; i <= d; i++) {
					if (ModelControler.LastFrame+i < Model.EdgeFrames.Count) {
						foreach (string edgeID in Model.EdgeFrames[ModelControler.LastFrame+i]) {
							HideEdge(edgeID);
						}
					}
				}
				break;
			case ModelControler.EdgeRenderTypes.LocalBackward:
				d = ModelControler.TrackDepth;
				for (int i = 1; i <= d; i++) {
					if (ModelControler.LastFrame-i >= 0) {
						foreach (string edgeID in Model.EdgeFrames[ModelControler.LastFrame-i]) {
							HideEdge(edgeID);
						}
					}
				}
				break;
			case ModelControler.EdgeRenderTypes.All:
				for (int i = 0; i < Model.EdgeFrames.Count; i++) {
					foreach (string edgeID in Model.EdgeFrames[i]) {
						HideEdge(edgeID);
					}
				}
				break;
		}
		// Redraw based on new state
		switch (ModelControler.EdgeRenderType) {
			case ModelControler.EdgeRenderTypes.LocalCenter:
				d = ModelControler.TrackDepth/2;
				for (int i = -d; i <= d; i++) {
					if (ModelControler.Frame+i >= 0 && ModelControler.Frame+i < Model.EdgeFrames.Count) {
						foreach (string edgeID in Model.EdgeFrames[ModelControler.Frame+i]) {
							RenderEdge(edgeID);
						}
					}
				}
				break;
			case ModelControler.EdgeRenderTypes.LocalForward:
				d = ModelControler.TrackDepth;
				for (int i = 0; i <= d; i++) {
					if (ModelControler.Frame+i < Model.EdgeFrames.Count) {
						foreach (string edgeID in Model.EdgeFrames[ModelControler.Frame+i]) {
							RenderEdge(edgeID);
						}
					}
				}
				break;
			case ModelControler.EdgeRenderTypes.LocalBackward:
				d = ModelControler.TrackDepth;
				for (int i = 1; i <= d; i++) {
					if (ModelControler.Frame-i >= 0) {
						foreach (string edgeID in Model.EdgeFrames[ModelControler.Frame-i]) {
							RenderEdge(edgeID);
						}
					}
				}
				break;
			case ModelControler.EdgeRenderTypes.All:
				for (int i = 0; i < Model.EdgeFrames.Count; i++) {
					foreach (string edgeID in Model.EdgeFrames[i]) {
						RenderEdge(edgeID);
					}
				}
				break;
		}
	}

	private void RenderEdge(string edgeID) {
		if (RenderedEdges.ContainsKey(edgeID)) {
			RenderedEdges[edgeID].SetActive(true);
		}
		else {
			GameObject obj = MakeEdge(edgeID);
			RenderedEdges[edgeID] = obj;
		}
	}

	private void HideEdge(string edgeID) {
		// Debug.Log("Disappear !");
		if (RenderedEdges.ContainsKey(edgeID)) {
			// Debug.Log("THIS EDGE !");
			RenderedEdges[edgeID].SetActive(false);
		}
	}

	private void NewFrameRedrawNodes() {
		if (ModelControler.LastFrame >= 0) {
			foreach (string nodeID in Model.NodeFrames[ModelControler.LastFrame]) {
				RenderedNodes[nodeID].SetActive(false);
			}
		}
		if (ModelControler.Frame < Model.NodeFrames.Count) {
			foreach (string nodeID in Model.NodeFrames[ModelControler.Frame]) {
				if (RenderedNodes.ContainsKey(nodeID)) {
					// Debug.Log("Node Exist");
					RenderedNodes[nodeID].SetActive(true);
				}
				else {
					// Debug.Log("Creating Node");
					GameObject obj = MakeNuclei(nodeID);
					RenderedNodes[nodeID] = obj;
				}
			}
		}
	}


	// Update is called once per frame
	void Update () {

		// First if we change the Mode of Edge display, we need to Erase the current ones
		// And then Redraw the new ones.
		if (Model.Ready) {

			// Now if the Frame has changed, we need to erase the last Spots, and draw the new one
			// We also need to add or remove edges if we are on the Local mode.


			if (ModelControler.LastFrame != ModelControler.Frame) {
				NewFrameRedrawNodes();
			}

			if (RenderedEdges.Keys.Count == 0) {
				// Debug.Log("Init Render Edge");
				EdgeChangeState();
			}

			if (ModelControler.LastEdgeRenderType != ModelControler.EdgeRenderType) {
				// Debug.Log("Change Type Trigger");
				EdgeChangeState();
				ModelControler.LastEdgeRenderType = ModelControler.EdgeRenderType;
			}

			if (ModelControler.LastTrackDepth != ModelControler.TrackDepth) {
				EdgeChangeState();
				ModelControler.LastTrackDepth = ModelControler.TrackDepth;
			}


			if (ModelControler.LastFrame != ModelControler.Frame) {
				EdgeChangeState();
				ModelControler.LastFrame = ModelControler.Frame;
			}

		}
	}

	public GameObject MakeSpot(string ID) {
		Node N = Model.Nodes[ID];
		GameObject spot = (GameObject) Instantiate(Spot, transform.position, transform.rotation);
		N.Spot = spot;
    spot.transform.position = new Vector3(N.X/Model.Scaling, N.Y/Model.Scaling, N.Z/Model.Scaling);
		spot.transform.localScale = new Vector3(N.Radius/Model.RadiusScale, N.Radius/Model.RadiusScale, N.Radius/Model.RadiusScale);
		spot.GetComponent<Renderer>().material.color = N.Color;
		return spot;
	}

	public GameObject MakeNuclei(string ID) {
		Node N = Model.Nodes[ID];
		GameObject sphere = (GameObject) Instantiate(Sphere, transform.position, transform.rotation);
		Model.Nodes[ID].Sphere = sphere;
    sphere.transform.position = new Vector3(N.X/Model.Scaling, N.Y/Model.Scaling, N.Z/Model.Scaling);
		sphere.transform.localScale = new Vector3(N.Radius/Model.RadiusScale, N.Radius/Model.RadiusScale, N.Radius/Model.RadiusScale);
		sphere.GetComponent<Renderer>().material.color = N.Color;
		return sphere;
	}

	public GameObject MakeEdge(string ID) {
		Edge E = Model.Edges[ID];
		GameObject edge = (GameObject) Instantiate(Edge, transform.position, transform.rotation);
		Vector3 SourcePosition = new Vector3(E.Source.X/Model.Scaling, E.Source.Y/Model.Scaling, E.Source.Z/Model.Scaling);
		Vector3 TargetPosition = new Vector3(E.Target.X/Model.Scaling, E.Target.Y/Model.Scaling, E.Target.Z/Model.Scaling);


		Vector3 offset = TargetPosition - SourcePosition;
		Vector3 position = SourcePosition + (offset / 2.0f);

		edge.transform.position = position;
		edge.transform.LookAt(SourcePosition);
		Vector3 localScale = edge.transform.localScale;
		localScale.z = (TargetPosition - SourcePosition).magnitude;
		edge.transform.localScale = localScale;

		// Vector3 position = (TargetPosition - SourcePosition)/2.0f + SourcePosition;
		// Vector3 scale = transform.localScale;
		// scale.y = (TargetPosition - SourcePosition).magnitude;
		// edge.transform.position = position;
		// edge.transform.localScale = scale;
		// edge.transform.rotation = Quaternion.FromToRotation(Vector3.up, (TargetPosition - SourcePosition));
		//
		// float dx = E.Target.X - E.Source.X;
		// float dy = E.Target.Y - E.Source.Y;
		// float dz = E.Target.Z - E.Source.Z;
		// float dist = E.Distance;
		// float phi = (float)(Math.Atan2(dy, dx) * (180/Math.PI));
		// float theta = (float)(Math.Acos(dz/dist) * (180/Math.PI));
		// GameObject edge = (GameObject) Instantiate(Edge, transform.position, transform.rotation);
		// edge.transform.position = new Vector3(dx/2 + E.Source.X, dy/2 + E.Source.Y, dz/2 + E.Source.Z);
		// edge.transform.eulerAngles = new Vector3(0, theta, phi);
		// edge.transform.localScale = new Vector3(edge.transform.localScale.x/Model.Scaling, edge.transform.localScale.y*(dist/2), edge.transform.localScale.z/Model.Scaling);
		return edge;
	}

	public GameObject MakeLineEdge(string ID) {
		Edge E = Model.Edges[ID];
		float dx = E.Target.X - E.Source.X;
		float dy = E.Target.Y - E.Source.Y;
		float dz = E.Target.Z - E.Source.Z;
		float dist = E.Distance;
		float phi = (float)(Math.Atan2(dy, dx) * (180/Math.PI));
		float theta = (float)(Math.Acos(dz/dist) * (180/Math.PI));
		GameObject edge = (GameObject) Instantiate(LineEdge, transform.position, transform.rotation);
		edge.transform.position = new Vector3(dx/2 + E.Source.X, dy/2 + E.Source.Y, dz/2 + E.Source.Z);
		edge.transform.eulerAngles = new Vector3(0, theta, phi);
		return edge;
	}
}
