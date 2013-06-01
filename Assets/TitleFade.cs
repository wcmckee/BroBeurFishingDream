using UnityEngine;
using System.Collections;

public class TitleFade : MonoBehaviour {
	
	
	private float alphaness = 2f;
	
	private bool fadenow = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (fadenow){
			 alphaness -= Time.deltaTime * 0.25f;
			
			float theFade = alphaness;
			if (theFade>1f) theFade = 1f;
			if (theFade<0f) Destroy(gameObject);
			
			renderer.material.color = new Color(0f,0f,0f,theFade);
		}else{
			if (Screen.lockCursor) fadenow = true;
		}
		
	}
}
