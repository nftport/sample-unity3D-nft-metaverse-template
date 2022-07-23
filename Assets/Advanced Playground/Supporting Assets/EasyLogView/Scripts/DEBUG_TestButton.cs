using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nmxi.easylogview{
	public class DEBUG_TestButton : MonoBehaviour{
		public void LogTest(){
			Debug.Log("This is test log : " + Random.Range(0, 999));
		}

		public void WarningLogTest(){
			Debug.LogWarning("This is test log : " + Random.Range(0, 999));
		}

		public void ErrorLogTest(){
			Debug.LogError("This is test log : " + Random.Range(0, 999));
		}
	}
}
