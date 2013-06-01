using UnityEngine;
using System.Collections;

public class FishScript : MonoBehaviour {
	
	public GameObject ripplePrefab;
	public Renderer fishRenderer;
	private Material[] mats;
	private Color[] cols;
	
	private Vector3 startPoint;
	
	public MusicScript music;
	
	// Use this for initialization
	void Start () {
		
		lureScript = lure.GetComponent<LureScript>();
		
		mats = fishRenderer.materials;
		cols = new Color[mats.Length];
		for (int i=0; i<mats.Length; i++){
			cols[i] = mats[i].color;
		}
		
		startPoint = transform.position;
		
		timeOffset = Random.Range(0f,10f);
	}
	
	private float rippleTime = 0f;
	
	private bool below = true;
	
	private string aistate = "idle";
	
	public Transform lure;
	
	private LureScript lureScript;
	
	private float timeOffset = 0f;
	
	public GameObject splashSoundPrefab;
	
	// Update is called once per frame
	void Update () {
		
		Vector3 fromPos = transform.position;
		
		Vector3 targetPos = startPoint;
		
		if (aistate != "caught"){
		
			if (aistate == "idle"){
				targetPos = startPoint;
				targetPos.x += Mathf.Sin((Time.time+timeOffset) * 2f);
				targetPos.z += Mathf.Cos((Time.time+timeOffset) * 2f);
				transform.position = targetPos;
				
				if (Vector3.Distance(lure.position, transform.position) < 3f && lure.position.y < -0.5f){
					aistate = "chase";
				}
				
			}
			if (aistate == "chase"){
				
				targetPos = lure.position;
				transform.position += (targetPos - transform.position).normalized * Time.deltaTime * 3f;
				
				if (Vector3.Distance(lure.position, transform.position) > 5f || lure.position.y > -0.5f || lureScript.bittenFish != null){
					aistate = "return";
				}else{
					if (Vector3.Distance(transform.position, lure.position) < 0.4f){
						//bite now
						aistate = "bitten";
						lureScript.bittenFish = this;
					}
				}
				
			}
			if (aistate == "return"){
				
				targetPos = startPoint;
				transform.position += (targetPos - transform.position).normalized * Time.deltaTime * 6f;
				
				
				if (Vector3.Distance(startPoint, transform.position) < 1f){
					aistate = "idle";
				}
			}
			
			if (aistate == "bitten"){
				
				if (Input.GetKeyDown("space")){
					//delete me!
					aistate = "return";
					lureScript.bittenFish = null;
				}
				
				targetPos = lure.position;
				transform.position = targetPos;
			}
			
			if (aistate == "flyReturn"){
				flycaminfluence -= Time.deltaTime * 0.25f;
				if (flycaminfluence<0f) flycaminfluence = 0f;
				
				flyspeed += Time.deltaTime;
				if (flyspeed > 4f) flyspeed = 4f;
				
				float flyHeight = 5f;
				
				Vector3 camDir = camParent.forward;
				Vector3 originDir = new Vector3(startPoint.x, flyHeight, startPoint.z) - transform.position;
				
				Vector3 targetDir = camDir * flycaminfluence;
				targetDir += originDir.normalized * (1f-flycaminfluence);
				
				//Debug.Log(targetDir.magnitude);
				
				transform.position += targetDir * Time.deltaTime * flyspeed;
				targetPos = transform.position + targetDir;
				
				if (Vector3.Distance(transform.position, new Vector3(startPoint.x, flyHeight, startPoint.z))<1f){
					aistate = "return";
					animObj.animation.Play("swim");
					wings.enabled = false;
					
					
				}
			}
			
			Quaternion fromRot = transform.rotation;
			transform.LookAt(transform.position + (targetPos-fromPos), Vector3.up);
			
			if (aistate == "flyReturn"){
				transform.rotation = Quaternion.Slerp(fromRot, transform.rotation, Time.deltaTime * 0.9f);
			}
			
		}else{
			//caught
			caughtTime += Time.deltaTime;
			
			transform.localPosition = Vector3.Lerp(transform.localPosition, fishHoverSpot.localPosition, Time.deltaTime * 0.7f);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, fishHoverSpot.localRotation, Time.deltaTime * 0.7f);
			
			if (caughtTime > 4f && caughtStage == 0){
				caughtStage = 1;
				animObj.CrossFade("bobtalk");
				texty.Reset();
				texty.fading = false;
				texty.instant = false;
				texty.displayString = wisdom[wisdomIndex];
				wisdomIndex++;
				if (wisdomIndex >= wisdom.Length) wisdomIndex = 0;
			}
			if (caughtTime > 10f && caughtStage == 1){
				caughtStage = 2;
				animObj.Play("unfurl");
				wings.enabled = true;
			}
			if (caughtStage == 2 && animObj.animation["unfurl"].normalizedTime>=1f){
				caughtStage = 3;
				animObj.Play("fly");
			}
			if (caughtStage == 3 && caughtTime > 12f){
				caughtStage = 4;
				aistate = "flyReturn";
				transform.parent = null;
				lureScript.allowCatch = true;
				texty.fading = true;
				
				music.playMusic = false;
			}
		}
		
	
		if (transform.position.y >0f || aistate == "caught"){
			for (int i=0; i<mats.Length; i++){
				mats[i].color = cols[i];
			}
			if (below){
				below = false;
				GameObject splashRipple = (GameObject)GameObject.Instantiate(ripplePrefab);
				splashRipple.transform.position = new Vector3(transform.position.x,0.01f,transform.position.z);
				GameObject splashRipple2 = (GameObject)GameObject.Instantiate(ripplePrefab);
				splashRipple2.transform.position = new Vector3(transform.position.x,0.01f,transform.position.z);
				splashRipple2.GetComponent<rippleScript>().slow = true;
				
				GameObject splashSound = (GameObject)GameObject.Instantiate(splashSoundPrefab);
				splashSound.transform.position = new Vector3(lure.position.x,0.01f,lure.position.z);
				splashSound.GetComponent<SplashSoundScript>().splashin = false;
			}
		}else{
			float whiteness = 0.5f - (transform.position.y*0.035f);
			for (int i=0; i<mats.Length; i++){
				mats[i].color = new Color(whiteness,whiteness,whiteness,1f);//Color.red;
			}
			
			if (Time.time>rippleTime){
				GameObject fishRipple = (GameObject)GameObject.Instantiate(ripplePrefab);
				fishRipple.transform.position = new Vector3(transform.position.x,0.01f,transform.position.z);
				fishRipple.GetComponent<rippleScript>().slow = true;
				fishRipple.GetComponent<rippleScript>().small = true;
				
				rippleTime = Time.time + Random.Range(0.3f,0.6f);
			}
			
			if (!below){
				below = true;
				GameObject splashRipple = (GameObject)GameObject.Instantiate(ripplePrefab);
				splashRipple.transform.position = new Vector3(transform.position.x,0.01f,transform.position.z);
				GameObject splashRipple2 = (GameObject)GameObject.Instantiate(ripplePrefab);
				splashRipple2.transform.position = new Vector3(transform.position.x,0.01f,transform.position.z);
				splashRipple2.GetComponent<rippleScript>().slow = true;
				
				GameObject splashSound = (GameObject)GameObject.Instantiate(splashSoundPrefab);
				splashSound.transform.position = new Vector3(lure.position.x,0.01f,lure.position.z);
				splashSound.GetComponent<SplashSoundScript>().splashin = true;
			}
		}
	}
	
	public string[] wisdom;
	private int wisdomIndex = 0;
	
	public Renderer wings;
	
	public Transform camParent;
	public Animation animObj;
	public Transform fishHoverSpot;
	private float caughtTime = 0f;
	private int caughtStage = 0;
	private float flycaminfluence = 1f;
	private float flyspeed = 0f;
	
	public TextDisplayer texty;
	
	public void Catch(){
		if (lureScript.allowCatch){
			lureScript.allowCatch = false;
			
			aistate = "caught";
			transform.parent = camParent;
			animObj.animation.Play("bob");
			lureScript.bittenFish = null;
			caughtTime = 0f;
			caughtStage = 0;
			flycaminfluence = 1f;
			flyspeed = 1f;
			
			music.playMusic = true;
		}
	}
}
