using System.Collections;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
// using SD.Tools.Algorithmia.Graphs;
// using SD.Tools.Algorithmia.Graphs.Algorithms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ConnectedComponents;
using QuickGraph.Algorithms.Search;

public class ttest : MonoBehaviour {

	private System.Random _random;
	private int _seed;
	public Graph G;
	public string path;
	public string toRoot;
	public List<string> Roots;

	void Start()
	{
		// Only take the int part because that's what the Random constructor needs.
		_seed = (int)DateTime.Now.Ticks;
		_random = new System.Random(_seed);
		Roots = new List<string>();
		// testLoad();
	}

	public void testing4() {
		BidirectionalGraph<string, Edge<string>> graph = new BidirectionalGraph<string, Edge<string>>(true);

		graph.AddVertex("A");
		graph.AddVertex("B");
		graph.AddVertex("C");
		graph.AddVertex("D");
		graph.AddVertex("E");
		graph.AddVertex("F");
		graph.AddVertex("G");
		graph.AddVertex("H");
		graph.AddVertex("I");
		graph.AddVertex("J");

		graph.AddEdge(new Edge<string>("A", "B"));
		graph.AddEdge(new Edge<string>("B", "C"));
		graph.AddEdge(new Edge<string>("C", "D"));
		graph.AddEdge(new Edge<string>("C", "E"));
		graph.AddEdge(new Edge<string>("E", "F"));
		graph.AddEdge(new Edge<string>("G", "H"));
		graph.AddEdge(new Edge<string>("H", "I"));
		graph.AddEdge(new Edge<string>("H", "J"));

		WeaklyConnectedComponentsAlgorithm<string, Edge<string>> ConnectedComponent = new WeaklyConnectedComponentsAlgorithm<string, Edge<string>>(graph);
		ConnectedComponent.Compute();
		// Debug.Log(ConnectedComponent.ComponentCount);
		//
		// Debug.Log(ConnectedComponent.Components);
		// foreach (string key in ConnectedComponent.Components.Keys) {
		// 	Debug.Log(key);
		// 	Debug.Log(ConnectedComponent.Components[key]);
		// }

		List<string> vertexfound = new List<string>();
		List<string> startvertexfound = new List<string>();

		// ReversedBidirectionalGraph<string, Edge<string>> Rgraph = new ReversedBidirectionalGraph<string, Edge<string>>(graph);

		BidirectionalDepthFirstSearchAlgorithm<string, Edge<string>> DFS = new  BidirectionalDepthFirstSearchAlgorithm<string, Edge<string>>(graph);
		DFS.SetRootVertex("C");
		// DFS.TreeEdge += vertexfound.Add;
		DFS.TreeEdge += e => isRoot(graph, e);
		// DFS.FinishVertex += startvertexfound.Add;
		DFS.Compute();
		//
		Debug.Log("Found:");
		foreach(string V in Roots) {
			Debug.Log(V);
		}
		//
		// Debug.Log("START VERTEX Found:");
		// foreach(string V in vertexfound) {
		// 	Debug.Log(V);
		// }
		// Debug.Log(DFS.rootVertex);

	}

	public void isRoot(BidirectionalGraph<string, Edge<string>> graph, Edge<string> e) {
		if (graph.InDegree(e.Source) == 0) {
			Roots.Add(e.Source);
		}
	}


	public void testing() {
		myC T = new myC("ID1", "name1", 4.0f, 5.0f, 6.0f);
		myC2 TT = new myC2("ID33", T);
		T.ID = "newID";
		Debug.Log(T.ID);
		Debug.Log(TT.testref.ID);
	}

	public void testingC() {
		Debug.Log("Computing Components...");
		G.ComputeComponents();
		StartCoroutine("coTestingC");
		Debug.Log("Finding roots");
		List<string> Roots = G.FindRoots(toRoot);
		foreach(string V in G.Roots) {
			Debug.Log(V);
		}

		// Debug.Log(G.Node2Components);
		// foreach (string key in G.Node2Components.Keys) {
		// 	Debug.Log(key);
		// 	Debug.Log(G.Node2Components[key]);
		// }
		// for(int i = 0; i < G.Components2Node.Count(); i++) {
		// 	Debug.Log(string.Format("Component {0} has {1} nodes", i, G.Components2Node[i].Count));
		// }
	}

	public IEnumerator coTestingC() {
		while (G.ComponentsCount == 0) {
			yield return null;
		}
		Debug.Log("Component Found:");
		Debug.Log(G.ComponentsCount);
	}

		public void testLoad() {
			// path = "/home/lblondel/Documents/Harvard/ExtavourLab/projects/Project_Parhyale/Microscopy/JaneliaLightSheet/Bro1/Mamut/Bro1-fused2-mamut.xml";
			path = "/home/lblondel/Documents/Harvard/ExtavourLab/projects/Project_Parhyale/Microscopy/JaneliaLightSheet/Bro1/Mamut/UnityTest.xml";
			// G = new Graph();
			StartLoading();
		}

		public void testLoadFull() {
			path = "/home/lblondel/Documents/Harvard/ExtavourLab/projects/Project_Parhyale/Microscopy/JaneliaLightSheet/Bro1/Mamut/Bro1-fused2-mamut.xml";
			// path = "/home/lblondel/Documents/Harvard/ExtavourLab/projects/Project_Parhyale/Microscopy/JaneliaLightSheet/Bro1/Mamut/UnityTest.xml";
			// G = new Graph();
			StartLoading();
		}

		public void StartLoading() {
			StartCoroutine("Load");
		}

		public IEnumerator Load () {
			int spots = 0;
			int edge = 0;
			int nspots = 0;
			XDocument xdoc = XDocument.Load(path);
			List<string> TrackEdgeList = new List<string>();
			foreach (XElement element in xdoc.Descendants())
			{
					if (element.Name == "AllSpots") {
						nspots = (int)element.Attribute("nspots");
					}
					else if (element.Name == "Spot") {
					string ID = (string)element.Attribute("ID");
					G.AddNode(ID);
					toRoot = ID;
					if (spots % 2000 == 0) {
						Debug.Log(string.Format("{0}/{1} Spots Loaded ...", spots, nspots));
					}
					if (spots % 100 == 0) {
						yield return null;
					}
					spots += 1;
				}
				else if (element.Name == "Edge") {
					string SourceID = (string)element.Attribute("SPOT_SOURCE_ID");
					string TargetID = (string)element.Attribute("SPOT_TARGET_ID");

					G.AddEdge(SourceID, TargetID);

					if (edge % 2000 == 0) {
						Debug.Log(string.Format("{0} Edges Loaded ...", edge));
					}
					if (edge % 100 == 0) {
						yield return null;
					}
					edge += 1;
				}
				else {
					yield return null;
				}
			}
		}


	// public void testting2() {
	// 	int maxSize = 50;
	// 	int size = _random.Next(maxSize);
	//
	// 	DirectedGraph<int, DirectedEdge<int>> graph = new DirectedGraph<int, DirectedEdge<int>>((a, b) => new DirectedEdge<int>(a, b));
	// 		for(int i = 0; i < size;)
	// 		{
	// 			int startNode = _random.Next(maxSize);
	// 			int endNode = _random.Next(maxSize);
	// 			if(!graph.ContainsEdge(startNode, endNode))
	// 			{
	// 				graph.Add(startNode);
	// 				graph.Add(endNode);
	// 				graph.Add(new DirectedEdge<int>(startNode, endNode));
	// 				i++;
	// 			}
	// 		}
	// 	foreach(DirectedEdge<int> edge in graph.Edges) {
	// 		Debug.Log(edge.ToString());
	// 	}
	// 	foreach(int vertice in graph.Vertices) {
	// 		Debug.Log(vertice.ToString());
	// 	}
	//
	// }
	//
	// public void testting3() {
	//
	// 	NonDirectedGraph<string, NonDirectedEdge<string>> graph = new NonDirectedGraph<string, NonDirectedEdge<string>>(false);
	// 	graph.Add("node1");
	// 	graph.Add("node2");
	// 	graph.Add("node3");
	// 	graph.Add("node4");
	// 	graph.Add("node5");
	// 	graph.Add("node6");
	// 	graph.Add("node7");
	// 	graph.Add("node8");
	// 	graph.Add("node9");
	// 	graph.Add("node10");
	// 	graph.Add("node11");
	// 	graph.Add("node12");
	//
	// 	graph.Add(new NonDirectedEdge<string>("node1", "node2"));
	// 	graph.Add(new NonDirectedEdge<string>("node2", "node3"));
	// 	graph.Add(new NonDirectedEdge<string>("node3", "node4"));
	// 	graph.Add(new NonDirectedEdge<string>("node3", "node5"));
	// 	graph.Add(new NonDirectedEdge<string>("node5", "node6"));
	// 	graph.Add(new NonDirectedEdge<string>("node7", "node8"));
	// 	graph.Add(new NonDirectedEdge<string>("node8", "node9"));
	// 	graph.Add(new NonDirectedEdge<string>("node9", "node10"));
	// 	graph.Add(new NonDirectedEdge<string>("node10", "node11"));
	// 	graph.Add(new NonDirectedEdge<string>("node10", "node12"));
	//
	// 	foreach(NonDirectedEdge<string> edge in graph.Edges) {
	// 		Debug.Log(string.Format("{0} -> {1}", edge.StartVertex, edge.EndVertex));
	// 	}
	// 	foreach(string vertice in graph.Vertices) {
	//
	// 		Debug.Log(vertice.ToString());
	// 	}
	//
	// 	Debug.Log(graph.IsConnected());
	// 	Debug.Log(graph.RemoveOrphanedVerticesOnEdgeRemoval);
	//
	// 	DisconnectedGraphsFinder<string, NonDirectedEdge<string>> finder = new DisconnectedGraphsFinder<string, NonDirectedEdge<string>>(
	// 						() => new SubGraphView<string, NonDirectedEdge<string>>(graph), graph);
	//
	// 	finder.FindDisconnectedGraphs();
	//
	// 	Debug.Log(string.Format("Found {0} number of disconnected graphs", finder.FoundDisconnectedGraphs.Count));
	//
	// 	for (int i = 0; i < finder.FoundDisconnectedGraphs.Count; i++) {
	// 		Debug.Log(string.Format("Graph: {0}", i));
	// 		foreach(string vertice in finder.FoundDisconnectedGraphs[i].Vertices) {
	// 			Debug.Log(vertice.ToString());
	// 		}
	// 		foreach(NonDirectedEdge<string> edge in finder.FoundDisconnectedGraphs[i].Edges) {
	// 			Debug.Log(string.Format("{0} -> {1}", edge.StartVertex, edge.EndVertex));
	// 		}
	// 	}
	//
	// }


}
