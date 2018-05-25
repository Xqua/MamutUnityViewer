using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myC2 : MonoBehaviour {

	public string ID;
	public myC testref;

	public myC2(string ID, myC Test) {
		this.ID = ID;
		this.testref = Test;
	}

}
