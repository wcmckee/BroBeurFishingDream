using UnityEngine;
using System.Collections;

public class WaveUVShifter : MonoBehaviour {
	
	private Mesh meshy;
	private Vector2[] uvs;
	private bool[] shifting;
	
	
	
	// Use this for initialization
	void Start () {
	
		
		meshy = GetComponent<MeshFilter>().mesh;
		
		uvs = new Vector2[meshy.uv.Length];
		shifting = new bool[uvs.Length];
		
		for (int i=0; i<uvs.Length; i++){
			uvs[i] = meshy.uv[i];
			if (uvs[i].y < 0.5f) shifting[i] = true;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<uvs.Length; i++){
			if (shifting[i]){
				uvs[i].y = Mathf.Sin(Time.time * 2f) * 0.4f;
			}
		}
		meshy.uv = uvs;
	}
}
