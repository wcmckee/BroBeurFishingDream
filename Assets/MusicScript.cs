using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {
	
	public bool playMusic = true;
	
	private float musicScale = 1f;
	
	
	// Use this for initialization
	void Start () {
		if (playMusic) musicScale = 1f;
		if (!playMusic) musicScale = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (playMusic){
			musicScale += Time.deltaTime * 0.2f;
		}else{
			musicScale -= Time.deltaTime * 0.12f;
		}
		if (musicScale <0f) musicScale = 0f;
		if (musicScale >1f) musicScale = 1f;
		
		audio.volume = musicScale * 0.2f;
		
	}
}
