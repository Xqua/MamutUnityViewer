using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour {

	public List<string> TrackEdges;
	public List<string> TrackNodes;
	public string Root;
	public float TrackDisplacement;
	public int TrackStart = 10000000;
	public int TrackStop= 0;
	public int TrackDuration = 0;
	public int NumberNodes = 0;
	public int NumberEdges = 0;
	public int NbDivisions = 0;
	public float DivisionTimeMean;
	public float DivisionTimeStd;
	public Dictionary<string, int> Divisions = new Dictionary<string, int>();
	public List<string> DivCheck = new List<string>();
	public int ID;
	public string Name;
	public Model Model;

	public IEnumerator CalculateTrackFeatures() {
		StartCoroutine("FindRoot");
		float distance = 0f;
		NumberEdges = TrackEdges.Count;
		NumberNodes = TrackNodes.Count;
		NbDivisions = 0;
		DivCheck = new List<string>();
		Divisions = new Dictionary<string, int>();

		foreach(string edgeID in TrackEdges) {
			Edge E = Model.Edges[edgeID];
			distance += E.Distance;
			if (E.Source.Frame < TrackStart) {
				TrackStart = E.Source.Frame;
			}
			if (E.Target.Frame > TrackStop) {
				TrackStart = E.Target.Frame;
			}
			if (DivCheck.Contains(E.Source.ID)) {
				NbDivisions += 1;
				Divisions[E.Source.ID] = E.Source.Frame;
			}
			else {
				DivCheck.Add(E.Source.ID);
			}
			yield return null;
		}

		float mu = 0f;
		float std = 0f;
		foreach(string key in Divisions.Keys) {
			mu += Divisions[key];
			yield return null;
		}
		mu /= Divisions.Keys.Count;
		foreach(string key in Divisions.Keys) {
			std += (float)Math.Pow((Divisions[key] - mu),2);
			yield return null;
		}
		std /= (Divisions.Keys.Count - 1);
		std = (float)Math.Sqrt(std);
		DivisionTimeMean = mu;
		DivisionTimeStd = std;
		TrackDisplacement = distance;
	}

	public void ReCalculateDivisions() {
		float mu = 0f;
		float std = 0f;
		foreach(string key in Divisions.Keys) {
			mu += Divisions[key];
		}
		mu /= Divisions.Keys.Count;
		foreach(string key in Divisions.Keys) {
			std += (float)Math.Pow((Divisions[key] - mu),2);
		}
		std /= (Divisions.Keys.Count - 1);
		std = (float)Math.Sqrt(std);
		DivisionTimeMean = mu;
		DivisionTimeStd = std;
	}

	public Track(int ID, Model Model, string rootID) {
		this.ID = ID;
		this.Name = ID;
		TrackEdges = new List<string>();
		TrackNodes = new List<string>();
		this.Model = Model;
		this.Root = rootID;

	}

	public Track(string ID, string Name, Model Model) {
		this.ID = ID;
		this.Name = Name;
		TrackEdges = new List<string>();
		TrackNodes = new List<string>();
		this.Model = Model;
	}


	public IEnumerator CreateTrack() {
		yield return StartCoroutine(PopulateTrack(Root));
		StartCoroutine("CalculateTrackFeatures");
	}

	public IEnumerator PopulateTrack() {
			string edgeID = string.Format("{0},{1}", nodeID, childID);
			if (!TrackEdges.Contains(edgeID)) {
				Model.Edges[edgeID].Target.TrackID = ID;
				AddEdge(edgeID);
			}
			yield return StartCoroutine(PopulateTrack(childID));
		}
	}

	public void OnEdgeFound(Edge<string> e) {
		string edgeID = string.Format("{0},{1}", e.Source, e.Target);
		TrackEdges.Add(edgeID);
	}

	public bool NodeIsIn(string nodeID) {
		if (TrackNodes.Contains(nodeID)) {
			return true;
		}
		return false;
	}

	public bool EdgeIsIn(string edgeID) {
		if (TrackEdges.Contains(edgeID)) {
			return true;
		}
		return false;
	}

	public void AddEdge(string edgeID) {
		Edge E = Model.Edges[edgeID];
		TrackDisplacement += E.Distance;
		if (DivCheck.Contains(E.Source.ID)) {
			Divisions[E.Source.ID] = E.Source.Frame;
			ReCalculateDivisions();
		}
		else {
			DivCheck.Add(E.Source.ID);
		}
		TrackEdges.Add(edgeID);
		Model.EdgesToTrack[edgeID] = ID;
		Model.NodesToTrack[E.Source.ID] = ID;
		Model.NodesToTrack[E.Target.ID] = ID;
		if (E.Source.TrackID == null) {
			E.Source.TrackID = ID;
		}
		if (E.Target.TrackID != ID) {
			MergeBranch(edgeID);
		}
		E.Target.TrackID = ID;
	}

	public void RemoveEdge(string edgeID) {
		Edge E = Model.Edges[edgeID];
		TrackEdges.Remove(edgeID);
		E.Target.TrackID = null;
		SplitBranch(edgeID);
		if (Divisions.ContainsKey(E.Source.ID)) {
			Divisions.Remove(E.Source.ID);
			ReCalculateDivisions();
		}
		if (TrackEdges.Count == 0) {
			Model.RemoveTrack(ID);
		}
	}

	public void SplitBranch(string edgeID) {
		Edge E = Model.Edges[edgeID];
		Model.AddTrack(E.Target.ID);
	}

	public void MergeBranch(string edgeID) {
		Edge E = Model.Edges[edgeID];
		PopulateTrack(E.Target.ID);
		CalculateTrackFeatures();
	}

}
