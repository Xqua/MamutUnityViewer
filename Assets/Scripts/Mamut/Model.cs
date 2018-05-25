using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {

	public List<List<string>> NodeFrames;
	public List<List<string>> EdgeFrames;
	public Dictionary<string, Track> Tracks;
	public Dictionary<string, Node> Nodes;
	public Dictionary<string, Edge> Edges;
	public Dictionary<string, string> EdgesToTrack;
	public Dictionary<string, string> NodesToTrack;
	public List<string> Checked;
	public List<string> Roots;
	public float Scaling = 10f;
	public float RadiusScale = 10f;
	public bool Ready = false;
	public Graph G;

	void Start() {
		NodeFrames = new List<List<string>>();
		EdgeFrames = new List<List<string>>();
		Tracks = new Dictionary<string, Track>();
		Nodes = new Dictionary<string, Node>();
		Edges = new Dictionary<string, Edge>();
	}

	public Edge GetEdge(string ID1, string ID2) {
		string key = string.Format("{0},{1}", ID1, ID2);
		if (Edges.ContainsKey(key)) {
			return Edges[key];
		}
		else {
			return null;
		}
	}

	public void AddNode(string ID, string Name, float X, float Y, float Z, int Frame) {
		Node N = new Node(ID, Name, X, Y, Z, Frame);
		Nodes[ID] = N;
		if (Frame > NodeFrames.Count) {
			NodeFrames.Add(new List<string>());
		}
		NodeFrames[Frame].Add(ID);
		G.AddNode(ID);
	}

	public void AddNode(string ID, string Name, float X, float Y, float Z, int Frame, float Radius) {
		Node N = new Node(ID, Name, X, Y, Z, Frame, Radius);
		Nodes[ID] = N;
		while (Frame >= NodeFrames.Count) {
			NodeFrames.Add(new List<string>());
		}
		NodeFrames[Frame].Add(ID);
		G.AddNode(ID);
	}

	public void AddEdge(string SourceID, string TargetID) {
		Debug.Log("AddingEdge0");
		Node Source = Nodes[SourceID];
		Node Target = Nodes[TargetID];
		Edge E = new Edge(Source, Target);
		Edges[E.ID] = E;
		while (Source.Frame >= EdgeFrames.Count) {
			EdgeFrames.Add(new List<string>());
		}
		EdgeFrames[E.Source.Frame].Add(E.ID);
	 	Source.Children.Add(TargetID);
		Target.Parents.Add(SourceID);
		G.AddEdge(SourceID, TargetID);
	}

	public void RemoveEdge(string edgeID) {
		EdgeFrames[Edges[edgeID].Source.Frame].Remove(edgeID);
		Edges[edgeID].Source.Children.Remove(Edges[edgeID].Target.ID);
		Edges[edgeID].Target.Parents.Remove(Edges[edgeID].Source.ID);
		string TrackID = EdgesToTrack[edgeID];
		Tracks[TrackID].RemoveEdge(edgeID);
		G.RemoveEdge(Edges[edgeID].Source.ID, Edges[edgeID].Target.ID);
		Edges.Remove(edgeID);
	}

	public void RemoveNode(string nodeID) {
		NodeFrames[Nodes[nodeID].Frame].Remove(nodeID);
		string edgeID;
		foreach(string parentID in Nodes[nodeID].Parents) {
			edgeID = string.Format("{0},{1}", parentID, nodeID);
			RemoveEdge(edgeID);
			Nodes[parentID].Children.Remove(nodeID);
		}
		foreach(string childID in Nodes[nodeID].Children) {
			edgeID = string.Format("{0},{1}", nodeID, childID);
			RemoveEdge(edgeID);
			Nodes[childID].Parents.Remove(nodeID);
		}
		G.RemoveNode(nodeID);
	}

	public void MakeTracks() {
		G.ComputeComponents();
		G.FindRoots()
		foreach(string root in G.Roots) {

		}
	}



	public IEnumerator ComputeComponents() {
		Debug.Log("Finding All Roots");
		Checked = new List<string>();
		Roots = new List<string>();
		string Root;
		foreach(string nodeID in Nodes.Keys) {
			Node N = Nodes[nodeID];
			if (!Checked.Contains(nodeID)) {
				// Root = FindRoot(N);
				while (N.Parents.Count > 0) {
					Checked.Add(N.ID);
					N = Nodes[N.Parents[0]];
					yield return null;
				}
				Root = N.ID;
				Debug.Log("Found a Root !");
				if (!Roots.Contains(Root)) {
					Roots.Add(Root);
				}
			}
			yield return null;
		}
		foreach(string R in Roots) {
			Debug.Log("Adding Track");
			AddTrack(R);
			yield return null;
		}
	}

	public string FindRoot(Node N) {
		while (N.Parents.Count > 0) {
			Checked.Add(N.ID);
			N = Nodes[N.Parents[0]];
		}
		return N.ID;
	}

	public void AddTrack(int TrackID, string rootID) {
		Track T = new Track(TrackID, this, rootID);
		Tracks[rootID] = T;
	}

	public void RemoveTrack(string trackID) {
		Tracks.Remove(trackID);
	}

}
