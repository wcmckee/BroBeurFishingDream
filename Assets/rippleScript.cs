using UnityEngine;
using System.Collections;

public class rippleScript : MonoBehaviour {
	
	
	private float fade = 1f;
	private Vector3 scaleness = Vector3.one * 0.01f;
	
	public bool slow = false;
	public bool small = false;
	
	// Use this for initialization
	void Start () {
		if (small){
			fade = 0.5f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		float timeScale = 1f;
		if (slow) timeScale = 0.5f;
		
		scaleness.x += Time.deltaTime * timeScale;
		scaleness.y += Time.deltaTime * timeScale;
		scaleness.z += Time.deltaTime * timeScale;
		transform.localScale = scaleness;
		
		fade -= Time.deltaTime * timeScale;
		renderer.material.color = new Color(1f,1f,1f, fade);
		if (fade<0f){
			Destroy(gameObject);
		}
	}
}
