using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ModelControler : MonoBehaviour {

	public enum EdgeRenderTypes {LocalCenter, LocalBackward, LocalForward, All}
	public EdgeRenderTypes EdgeRenderType = EdgeRenderTypes.LocalCenter;
	public EdgeRenderTypes LastEdgeRenderType = EdgeRenderTypes.LocalCenter;
	public int Frame = 0;
	public int LastFrame = -1;
	public int LastTrackDepth = 10;
	public int TrackDepth = 10;
	public Model Model;

	public Slider FrameSlider;

	// // Use this for initialization
	void Start () {
		FrameSlider.onValueChanged.AddListener(delegate {OnFrameChange(); });
	}
	//
	// // Update is called once per frame
	void Update () {
		FrameSlider.maxValue = Model.EdgeFrames.Count;
	}

	public void OnDepthChange(float v) {
		TrackDepth = (int)v;
	}

	public void OnFrameChange() {
		Frame = (int)FrameSlider.value;
	}

	public void OnEdgeTypeChange() {

	}

}
