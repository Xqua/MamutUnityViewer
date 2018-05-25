using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ConnectedComponents;
using QuickGraph.Algorithms.Search;

public class Graph : MonoBehaviour {

	public BidirectionalGraph<string, Edge<string>> graph;
	public IDictionary<string, int> Node2Components;
	public List<List<string>> Components2Node;
	public int ComponentsCount;
	public List<string> Roots;


	void Start() {
		graph = new BidirectionalGraph<string, Edge<string>>(true);
		Node2Components = new Dictionary<string, int>();
		Components2Node = new List<List<string>>();
		ComponentsCount = 0;
		Roots = new List<string>();
	}

	public void AddNode(string nodeID) {
		graph.AddVertex(nodeID);
	}

	public void AddEdge(string Source, string Target) {
		if (!graph.ContainsVertex(Source)) {
			AddNode(Source);
		}
		if (!graph.ContainsVertex(Target)) {
			AddNode(Target);
		}

		graph.AddEdge(new Edge<string>(Source, Target));
	}

	public void RemoveNode(string nodeID) {
		graph.RemoveVertex(nodeID);
	}

	public void RemoveEdge(string Source, string Target) {
		graph.RemoveEdge(new Edge<string>(Source, Target));
	}

	public void ComputeComponents() {
		StartCoroutine("CoComputeComponents");
	}

	public IEnumerator CoComputeComponents() {
		WeaklyConnectedComponentsAlgorithm<string, Edge<string>> ConnectedComponent = new WeaklyConnectedComponentsAlgorithm<string, Edge<string>>(graph);
		ConnectedComponent.Compute();
		Node2Components = ConnectedComponent.Components;
		ComponentsCount = ConnectedComponent.ComponentCount;
		Components2Node = new List<List<string>>();
		for (int i = 0; i < ComponentsCount; i++) {
			Components2Node.Add(new List<string>());
			yield return null;
		}
		foreach(string key in Node2Components.Keys) {
			if (Node2Components[key] >= 0 && Node2Components[key] < Components2Node.Count) {
				Components2Node[Node2Components[key]].Add(key);
			}
			else if (Node2Components[key] >= Components2Node.Count) {
				Debug.Log("index is more than the Count !");
			}
			else {
				Debug.Log("index is less than 0 !");
			}
			yield return null;
		}
	}

	public List<string> FindChildren(string Root) {

		List<string> Children = new List<string>();
		if (!graph.ContainsVertex(Root)) {
			return Children;
		}

		DepthFirstSearchAlgorithm<string, Edge<string>> DFS = new DepthFirstSearchAlgorithm<string, Edge<string>>(graph);

		DFS.SetRootVertex(Root);
		DFS.DiscoverVertex += Children.Add;
		DFS.Compute();

		return Children;
	}


	public List<string> FindRoots(string root) {

		List<string> Roots = new List<string>();

		BidirectionalDepthFirstSearchAlgorithm<string, Edge<string>> DFS = new BidirectionalDepthFirstSearchAlgorithm<string, Edge<string>>(graph);


		DFS.SetRootVertex(root);
		DFS.DiscoverVertex += e => isRoot(e);
		// DFS.TreeEdge += e => isRoot(e);
		DFS.Compute();

		return Roots;
	}

	public void isRoot(string e) {
		// Debug.Log(e);
		// Debug.Log(graph.InDegree(e));
		if (graph.InDegree(e) == 0) {
			Roots.Add(e);
		}
	}

}
