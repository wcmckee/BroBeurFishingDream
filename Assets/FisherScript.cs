using UnityEngine;
using System.Collections;

public class FisherScript : MonoBehaviour {
	
	
	public GameObject camHolder;
	
	private Vector3 camDir;
	private Vector3 camOffset;
	
	private bool free = true;
	
	
	private CharacterController cc;
	
	public GameObject spindle;
	
	public Transform rodpos_idle;
	public Transform rodpos_pull;
	public Transform rodpos_cast;
	
	public Transform rod;
	public Transform rodHolder;
	
	public Transform lure;
	
	public AudioClip[] steps;
	public AudioSource stepper;
	
	// Use this for initialization
	void Start () {
		camDir = camHolder.transform.localEulerAngles;
		camOffset = camHolder.transform.localPosition;
		
		cc = GetComponent<CharacterController>();
	}
	
	private int skippedFirstFrames = 0;
	private float helpfadetimer = 0f;
	private float stepdir = -1f;
	private bool steppingDown = false;
	
	public TextDisplayer texty;
	
	public GameObject splashSoundPrefab;
	
	private bool delme = false;
	// Update is called once per frame
	void Update () {
		
		if (delme) return;
		
		if (skippedFirstFrames<5){
			skippedFirstFrames++;
			return;
		}
		
		if (Input.GetKeyDown("mouse 0") && Screen.lockCursor) allowCastNow = true;
		
		if (Input.GetKeyDown("mouse 0") || Input.GetKeyDown("mouse 1")) Screen.lockCursor = true;
		
		bool walking = false;
		
		if (Input.anyKeyDown && !Input.GetKeyDown("mouse 0") && !Input.GetKeyDown("mouse 1") && !Input.GetKeyDown("mouse 2") && !Input.GetKeyDown("escape")){
			if (texty.fading){
				texty.fading = false;
				texty.instant = true;
				texty.displayString = "Right click - Walk\nLeft Click - Cast\nMouse Wheel - Reel";
				helpfadetimer = 2f;
			}
		}
		
		if (helpfadetimer>0f){
			helpfadetimer -= Time.deltaTime;
			if (helpfadetimer<=0f){
				texty.fading = true;
			}
		}
		
		if (free){
			
			
			
			
			
			/////////////look
			camDir.x -= Input.GetAxis("Mouse Y") * 100f * Time.deltaTime;
			camDir.y += Input.GetAxis("Mouse X") * 100f * Time.deltaTime;
			if (camDir.x > 25f ) camDir.x = 25f;
			if (camDir.x < -30f ) camDir.x = -30f;
			
			camHolder.transform.eulerAngles = camDir;
		
			//////////////walk
			if (Input.GetKey("mouse 1")){
				Vector3 walkVector = camHolder.transform.forward;
				walkVector.y = 0f;
				walkVector.Normalize();
				
				cc.Move(walkVector * Time.deltaTime * 6f);
				cc.Move(-Vector3.up * 4f);
				
				camHolder.transform.localPosition = camOffset + (Vector3.up*Mathf.Sin(Time.time*16f) * 0.1f);
				rodHolder.position = rodHolder.parent.transform.position + (Vector3.up*Mathf.Sin((Time.time-0.5f)*16f) * 0.05f);
				
				
				float lastStep = stepdir;
				stepdir = Mathf.Sin(Time.time*16f);
				
				if (stepdir-lastStep<0f){
					steppingDown = true;
				}else{
					if (steppingDown){
						
						//stepper.clip = steps[Random.Range(0,steps.Length)];
						stepper.PlayOneShot(steps[Random.Range(0,steps.Length)]);
					}
					steppingDown = false;
				}
				
				
				walking = true;
			}else{
				camHolder.transform.localPosition = camOffset;
				rodHolder.localPosition = Vector3.zero;
			}
		
		}
		
		
		///////////// spindle control
		float spindleRot = 0f;
		spindleRot = -Input.GetAxis("Mouse ScrollWheel");
		
		//stop out of control spins
		if (spindleSpin<0f && spindleRot > 0f) spindleSpin = 0f;
		if (spindleSpin>0f && spindleRot < 0f) spindleSpin = 0f;
		
		
		spindleSpin += spindleRot * Time.deltaTime * 300f;
		
		//start at faster than super slow
		float minSpin = 4f;
		if ((spindleSpin<minSpin && spindleSpin>-minSpin) && spindleRot != 0f){
			if (spindleRot>0f) spindleSpin = minSpin;
			if (spindleRot<0f) spindleSpin = -minSpin;
		}
		
		if (spindleSpin> 0.3f) {
			spindleSpin -= Time.deltaTime * 15f;
		}else if (spindleSpin < -0.3f){
			spindleSpin += Time.deltaTime * 15f;
		}else{
			if (spindleSpin<0.1f && spindleSpin>-0.1f){
				spindleSpin -= (spindleSpin) * Time.deltaTime * 5f;
			}else{
				spindleSpin -= (spindleSpin) * Time.deltaTime * 1f;
			}
		}
		if (spindleSpin>15f) spindleSpin = 15f;
		if (spindleSpin<-15f) spindleSpin = -15f;
		
		
		//Debug.Log(spindleSpin);
		
		if (Input.GetKey("mouse 2")){
			spindleSpin -= (spindleSpin) * Time.deltaTime * 5f;
		}
		spindle.transform.Rotate(spindleSpin * Time.deltaTime * 100f, 0f,0f);
		
		/////////////rod position
		if (walking){
			rod.localPosition = Vector3.Lerp(rod.localPosition, rodpos_idle.localPosition, Time.deltaTime * 2f);
			rod.localRotation = Quaternion.Slerp(rod.localRotation, rodpos_idle.localRotation, Time.deltaTime * 2f);
		}else{
			if (Input.GetKey("mouse 0")){
				rod.localPosition = Vector3.Lerp(rod.localPosition, rodpos_pull.localPosition, Time.deltaTime * 5f);
				rod.localRotation = Quaternion.Slerp(rod.localRotation, rodpos_pull.localRotation, Time.deltaTime * 5f);
			}else{
				rod.localPosition = Vector3.Lerp(rod.localPosition, rodpos_cast.localPosition, Time.deltaTime * 10f);
				rod.localRotation = Quaternion.Slerp(rod.localRotation, rodpos_cast.localRotation, Time.deltaTime * 10f);
			}
		}
		
		reel.lineLength -= spindleSpin * 0.025f;
		if (reel.lineLength<1f) reel.lineLength = 1f;
		
		
		///////////// lure stuff
		
		if (reel.casting){
			if (Input.GetKey("mouse 2")){
				reel.featherFactor -= Time.deltaTime;
				if (reel.featherFactor<0f) reel.featherFactor = 0f;
			}else{
				//reel.featherFactor += Time.deltaTime;
				//if (reel.featherFactor>1f) reel.featherFactor = 1f;
				reel.featherFactor = 1f;
			}
		}else{
			reel.featherFactor = 0f;
		}
		
		if (spindleRot != 0f || Input.GetKey("mouse 0")){
			//also have thi
			reel.casting = false;
		}
		
		if (lure.position.y < -2f){
			/////////////////////////////////////////////////////////////////////////////////////////DELETE ME
			//reel.casting = false;
			//lure.transform.position = new Vector3(lure.position.x,-2f, lure.position.z);
			
		}
		if (reel.lineLength>50f){
			reel.lineLength = 50f;
			reel.casting = false;
		}
		
		//if (!reel.casting) reel.lineLength -= Time.deltaTime;
		
		if (lure.transform.position.y >0f){
			lureForces.y -= Time.deltaTime * 10f;
			
			if (Input.GetKey("mouse 2")){
				//lureForces.x -= lureForces.x * Time.deltaTime * 2f;
				//lureForces.z -= lureForces.z * Time.deltaTime * 2f;
			}
			if (!lureAbove){
				lureAbove = true;
				
				GameObject newRipple = (GameObject)GameObject.Instantiate(ripplePrefab);
				newRipple.transform.position = new Vector3(lure.position.x,0.01f,lure.position.z);
				GameObject newRipple2 = (GameObject)GameObject.Instantiate(ripplePrefab);
				newRipple2.transform.position = new Vector3(lure.position.x,0.01f,lure.position.z);
				newRipple2.GetComponent<rippleScript>().slow = true;
				
				GameObject splashSound = (GameObject)GameObject.Instantiate(splashSoundPrefab);
				splashSound.transform.position = new Vector3(lure.position.x,0.01f,lure.position.z);
				splashSound.GetComponent<SplashSoundScript>().splashin = false;
			}
			//lureAbove = true;
		}else{
			lureForces.y -= Time.deltaTime * 4f;
			
			if (lureForces.y < -2f) lureForces.y = -2f;
			
			lureForces.x -= lureForces.x * Time.deltaTime * 8f;
			lureForces.z -= lureForces.z * Time.deltaTime * 8f;
			if (lureAbove){
				lureAbove = false;
				
				GameObject newRipple = (GameObject)GameObject.Instantiate(ripplePrefab);
				newRipple.transform.position = new Vector3(lure.position.x,0.01f,lure.position.z);
				GameObject newRipple2 = (GameObject)GameObject.Instantiate(ripplePrefab);
				newRipple2.transform.position = new Vector3(lure.position.x,0.01f,lure.position.z);
				newRipple2.GetComponent<rippleScript>().slow = true;
				
				GameObject splashSound = (GameObject)GameObject.Instantiate(splashSoundPrefab);
				splashSound.transform.position = new Vector3(lure.position.x,0.01f,lure.position.z);
				splashSound.GetComponent<SplashSoundScript>().splashin = true;
			}
		}
		Vector3 fromPos = lure.position;
		//lure.transform.position += lureForces * Time.deltaTime;
		MoveLureTo(lure.position + (lureForces * Time.deltaTime), 5);
		
		if (Vector3.Distance(rodEnd.position, lure.position) > reel.lineLength){
			//lure hit end of the available line, let the line keep spilling out or drag on the lure
			float difference = Vector3.Distance(rodEnd.position, lure.position) - reel.lineLength;
			float lineSpill = difference * reel.featherFactor;
			reel.lineLength += lineSpill;
			
			//Debug.Log(reel.featherFactor);
			
			spindle.transform.Rotate(lineSpill * -100f, 0f,0f);
			
			//slow down lure appropriate amount
			if (reel.casting){
				lureForces.x *= reel.featherFactor;
				lureForces.z *= reel.featherFactor;
			}
			
			Vector3 dirToRod = rodEnd.transform.position - lure.position;
			Vector3 targetLurePos = rodEnd.transform.position - (dirToRod.normalized * reel.lineLength);
			//lure.position = rodEnd.transform.position - (dirToRod.normalized * reel.lineLength);
			
			lureForces += (targetLurePos - lure.position) * 3f;
			
			//lure.position = targetLurePos;//+= (targetLurePos - lure.position) * 1.5f;
			MoveLureTo(targetLurePos, 5);
			//delme = true;
			
			//lureForces += (rodEnd.position - lure.position) * 1f * (1f-reel.featherFactor);
		}
		
		if (!reel.casting){
			lureForces -= (fromPos - lure.position) * Time.deltaTime * 10f;
		}
		
		
		
		if (Input.GetKeyUp("mouse 0") && reel.lineLength < 2f && allowCastNow){
			allowCastNow = false;
			
			lure.position = castpoint.transform.position;// + camHolder.transform.forward;
			lureForces = camHolder.transform.forward * 20f;
			lureAbove = true;
			reel.lineLength = 1f;
			reel.casting = true;
			reel.featherFactor = 1f;
		}
		
		/////////// draw line
		float slack =  reel.lineLength - Vector3.Distance(rodEnd.position, lure.position);
		if (slack<0f) slack = 0f;
		
		Vector3 lineDir = lure.position - rodEnd.position;
		
		Vector3[] linePoints = new Vector3[9];
		
		linePoints[0] = rodEnd.position;
		
		linePoints[1] = pointAbove( rodEnd.position + (lineDir * 0.125f ) - (Vector3.up * slack * 0.2f) );
		linePoints[2] = pointAbove( rodEnd.position + (lineDir * 0.25f ) - (Vector3.up * slack * 0.35f) );
		linePoints[3] = pointAbove( rodEnd.position + (lineDir * 0.375f ) - (Vector3.up * slack * 0.45f) );
		linePoints[4] = pointAbove( rodEnd.position + (lineDir * 0.5f ) - (Vector3.up * slack * 0.5f) );
		linePoints[5] = pointAbove( rodEnd.position + (lineDir * 0.625f ) - (Vector3.up * slack * 0.45f) );
		linePoints[6] = pointAbove( rodEnd.position + (lineDir * 0.75f ) - (Vector3.up * slack * 0.35f) );
		linePoints[7] = pointAbove( rodEnd.position + (lineDir * 0.875f ) - (Vector3.up * slack * 0.2f) );
		linePoints[8] = lure.position;
		
		
		lineLine.SetVertexCount(9);
		lineLine.SetPosition(0, linePoints[0]);//rodEnd.position);
		
		lineLine.SetPosition(1, linePoints[1]);//pointAbove( rodEnd.position + (lineDir * 0.125f ) - (Vector3.up * slack * 0.2f) ));
		lineLine.SetPosition(2, linePoints[2]);//pointAbove( rodEnd.position + (lineDir * 0.25f ) - (Vector3.up * slack * 0.35f) ));
		lineLine.SetPosition(3, linePoints[3]);//pointAbove( rodEnd.position + (lineDir * 0.375f ) - (Vector3.up * slack * 0.45f) ));
		lineLine.SetPosition(4, linePoints[4]);//pointAbove( rodEnd.position + (lineDir * 0.5f ) - (Vector3.up * slack * 0.5f) ));
		
		
		lineLine.SetPosition(5, linePoints[5]);//pointAbove( rodEnd.position + (lineDir * 0.625f ) - (Vector3.up * slack * 0.45f) ));
		
		lineLine.SetPosition(6, linePoints[6]);//pointAbove( rodEnd.position + (lineDir * 0.75f ) - (Vector3.up * slack * 0.35f) ));
		
		
		
		lineLine.SetPosition(7, linePoints[7]);//pointAbove( rodEnd.position + (lineDir * 0.875f ) - (Vector3.up * slack * 0.2f) ));
		
		
		lineLine.SetPosition(8, linePoints[8]);//lure.position);
		/*
		lineLine.SetVertexCount(2);
		lineLine.SetPosition(0, rodEnd.position);
		lineLine.SetPosition(1, lure.position);
		*/
		
		//lure shadow
		Vector3 shadowPos = lure.position;
		//if (shadowPos.y <0f){
		//	shadowPos.y = 0f;
		//	lureShadow.transform.position = shadowPos;
		//	lureShadow.transform.LookAt(lureShadow.transform.position + Vector3.up);
		//}else{
			Ray ray = new Ray(shadowPos, -Vector3.up);
			RaycastHit hit = new RaycastHit();
			int layera = 1<<0;
			if (Physics.Raycast(ray, out hit, 99f, layera)){
				shadowPos = hit.point + hit.normal * 0.02f;
				lureShadow.transform.LookAt(lureShadow.transform.position + hit.normal);
			
				lureShadow2.transform.position = shadowPos;
				lureShadow2.transform.LookAt(lureShadow.transform.position + hit.normal);
			}else{
				
				shadowPos.y -= 9999f;
				lureShadow2.transform.position = shadowPos;
			}
			
			if (shadowPos.y<0f && lure.position.y>=0f){
				shadowPos.y = 0f;
				lureShadow.transform.position = shadowPos;
				lureShadow.transform.LookAt(lureShadow.transform.position + Vector3.up);
			}
			
			lureShadow.transform.position = shadowPos;
		//}
		if (lureShadow.transform.position.y<0f){
			shadow1Renderer.enabled = false;
		}else{
			shadow1Renderer.enabled = true;
		}
		if (lureShadow2.transform.position.y>0f){
			shadow2Renderer.enabled = false;
		}else{
			shadow2Renderer.enabled = true;
		}
		
		//ripples where the line passes through the water
		bool lineAbove = false;
		if (linePoints[0].y>0f) lineAbove = true;
		for (int i=1; i<linePoints.Length; i++){
			if ((lineAbove && linePoints[i].y <0f) || (!lineAbove && linePoints[i].y>0f)){
				//line has dipped into or risen out of the water
				
					
				Vector3 ripplePoint = new Vector3(linePoints[i].x,0.01f,linePoints[i].z);
				
				
				Ray rippleRay = new Ray(linePoints[i-1], linePoints[i]-linePoints[i-1]);
				RaycastHit ripplehit = new RaycastHit();
				int rippleLayer = 1<<10;
				if (Physics.Raycast(rippleRay, out ripplehit, Vector3.Distance(linePoints[i],linePoints[i-1]), rippleLayer)){
					ripplePoint = ripplehit.point + (Vector3.up*0.01f);
				}
				
				//if (Random.Range(0,100)<5){
				//if (Time.time>rippleTimess[i]){
				if (Vector3.Distance(ripplePoint, ripplePoints[i])>0.6f || Time.time>rippleTimess[i]){
					
					GameObject lineRipple = (GameObject)GameObject.Instantiate(ripplePrefab);
					lineRipple.transform.position = ripplePoint;//new Vector3(linePoints[i].x,0.01f,linePoints[i].z);
					lineRipple.GetComponent<rippleScript>().slow = true;
					lineRipple.GetComponent<rippleScript>().small = true;
					
					ripplePoints[i] = ripplePoint;
					rippleTimess[i] = Time.time + Random.Range(0.3f,0.6f);
				}
				
			}
			
			lineAbove = false;
			if (linePoints[i].y>0f) lineAbove = true;
			
			//catch time
			if (lureScript.bittenFish != null && reel.lineLength <=2f){
				lureScript.bittenFish.Catch();
			}
		}
	}
	
	public LureScript lureScript;
	
	public GameObject lureShadow;
	public Renderer shadow1Renderer;
	public GameObject lureShadow2;
	public Renderer shadow2Renderer;
	
	private Vector3[] ripplePoints = new Vector3[9];
	private float[] rippleTimess = new float[9];
	
	
	private float lureRadius = 0.1f;
	void MoveLureTo(Vector3 lureToPos, int resolveSteps){
		
		//if (resolveSteps<0) return; // don't go resolving for infinity
		
		Vector3 lureFrom = lure.position;
		
		
		Ray ray = new Ray(lureFrom, lureToPos - lureFrom);
		RaycastHit hit = new RaycastHit();
		int layera = (1<<0);
		if (Physics.SphereCast(lureFrom, lureRadius, (lureToPos-lureFrom).normalized, out hit, Vector3.Distance(lureToPos, lureFrom), layera)){
			lure.position = hit.point + (hit.normal * lureRadius * 1.01f);
			
			//float distDiff = Vector3.Distance(lure.position, lureFrom);
			lureForces = Vector3.Reflect(lureForces, hit.normal);
			lureForces *= 0.4f;
			
		}else{
			lure.position = lureToPos;
		}
	}
	
	Vector3 pointAbove(Vector3 pointIn){
		
		
		Ray ray = new Ray(pointIn + (Vector3.up*99f), -Vector3.up);
		RaycastHit hit = new RaycastHit();
		int layera = 1<<0;
		if (Physics.Raycast(ray, out hit, 99f, layera)){
			return hit.point + hit.normal * 0.05f;
		}else{
			return pointIn;
		}
		
	}
	
	public LineRenderer lineLine;
	
	private bool allowCastNow = false;
	
	
	public GameObject ripplePrefab;
	private Vector3 lureForces = Vector3.zero;
	private bool lureAbove = true;
	public GameObject castpoint;
	
	public Transform rodEnd;
	
	private Reel reel = new Reel();
	
	private float spindleSpin = 0f;
}
