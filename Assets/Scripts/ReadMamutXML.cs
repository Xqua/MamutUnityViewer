using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class ReadMamutXML : MonoBehaviour {

	public List<string> hdf5path;
	public List<string> viewsID;
	public List<Vector3> viewsSize;
	public List<int> viewsChannel;
	public List<int> ChannelsId;
	public List<string> ChannelsName;
	public int TimepointsFirst;
	public int TimepointsLast;
	private bool setups = false;
	private bool channels = false;
	private bool tpoints = false;
	public Model Model;
	public string path;



	public void testClick() {
		path = "/home/lblondel/Documents/Harvard/ExtavourLab/projects/Project_Parhyale/Microscopy/JaneliaLightSheet/Bro1/Mamut/Bro1-fused2-mamut.xml";
		StartLoading();
	}

	public void StartLoading() {
		StartCoroutine("Load");
	}

	public IEnumerator Load () {
		long spots = 0;
		long edge = 0;
		int nspots = 1;
		XDocument xdoc = XDocument.Load(path);
		List<string> TrackEdgeList = new List<string>();
		foreach (XElement element in xdoc.Descendants())
		{
			if (element.Name == "AllSpots") {
				nspots = (int)element.Attribute("nspots");
			}
			else if (element.Name == "Spot") {
				string ID = (string)element.Attribute("ID");
				string Name = (string)element.Attribute("name");
				float X = (float)element.Attribute("POSITION_X");
				float Y = (float)element.Attribute("POSITION_Y");
				float Z = (float)element.Attribute("POSITION_Z");
				int Frame = (int)element.Attribute("FRAME");
				float Radius = (float)element.Attribute("RADIUS");
				Model.AddNode(ID, Name, X, Y, Z, Frame, Radius);
				if (spots % 1000 == 0) {
					Debug.Log(string.Format("{0}/{1} Spots Loaded ...", spots, nspots));
				}
				if (spots % 100 == 0) {
					yield return null;
				}
				spots += 1;
			}
			else if (element.Name == "Track") {
				string ID = (string)element.Attribute("TRACK_ID");
				string Name = (string)element.Attribute("name");
				Model.AddTrack(ID, Name);
			}
			else if (element.Name == "Edge") {
				string SourceID = (string)element.Attribute("SPOT_SOURCE_ID");
				string TargetID = (string)element.Attribute("SPOT_TARGET_ID");

				Model.AddEdge(SourceID, TargetID);

				if (edge % 1000 == 0) {
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
		Model.MakeTracks();
		Model.Ready = true;
		Debug.Log("DONE !");
	}

}
