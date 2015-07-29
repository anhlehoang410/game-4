using UnityEngine;
using System.Collections;

public class Rotacao : MonoBehaviour {
	
	public Vector3 rotacionar;
	public float velocidade;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.Rotate(rotacionar*velocidade);
		
	}
}
