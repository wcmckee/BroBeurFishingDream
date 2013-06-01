using UnityEngine;
using System.Collections;

public class TextDisplayer : MonoBehaviour {
	
	
	private float fadey = 0f;
	
	public bool fading = true;

	private TextMesh textness;
	
	public string displayString = "";
	
	private int charaIndex = 0;
	private float nextCharaTime = 0f;
	
	public bool instant = false;
	
	// Use this for initialization
	void Start () {
		textness = GetComponent<TextMesh>();
		
	}
	
	public void Reset(){
		fadey = 0f;
		displayString = "";
		textness.text = "";
		charaIndex = 0;
		nextCharaTime = 0f;
		instant = false;
	}
	
	// Update is called once per frame
	void Update () {
		renderer.material.color = new Color(0f,0f,0f, fadey);
		
		
		if (charaIndex<displayString.Length && Time.time>nextCharaTime && !instant){
			
			string nextchara = displayString.Substring(charaIndex,1);//"";
			//if (charaIndex<displayString.Length-1) displayString.Substring(charaIndex+1,1);
			
			textness.text += displayString.Substring(charaIndex,1);
			charaIndex++;
			nextCharaTime = Time.time + 0f;
			if (nextchara == " ") nextCharaTime = Time.time + 0.2f;
			if (nextchara == "\n") nextCharaTime = Time.time + 0.8f;
		}
		
		
		if (fading){
			fadey -= Time.deltaTime * 0.5f;
			if (fadey < 0f){
				fadey = 0f;
				displayString = "";
				textness.text = "";
				charaIndex = 0;
				nextCharaTime = 0f;
				instant = false;
			}
			
		}else{
			fadey = 1f;
			if (instant) textness.text = displayString;
		}
	}
}
