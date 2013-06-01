using UnityEngine;
using System.Collections;

public class SplashSoundScript : MonoBehaviour {
	
	
	public AudioClip[] splashsin;
	public AudioClip[] splashsout;
	public bool splashin;
	
	private float destroyTime = 3f;
	
	// Use this for initialization
	void Start () {
		if (splashin){
			audio.PlayOneShot(splashsin[Random.Range(0,splashsin.Length)]);
		}else{
			audio.PlayOneShot(splashsout[Random.Range(0,splashsout.Length)]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		destroyTime -= Time.deltaTime;
		if (destroyTime<0f) Destroy(gameObject);
	}
}
