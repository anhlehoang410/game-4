using UnityEngine;
using System.Collections;

public class Fundo : MonoBehaviour {
	public MeshRenderer fundo;
	public Vector2 vel;
	public bool inicioRandomico;

	void Start()
	{
		if(inicioRandomico)
			fundo.material.mainTextureOffset = vel * Random.Range(0f, 1f);
	}

	void Update () 
	{
		fundo.material.mainTextureOffset += vel * Time.deltaTime;
	}
}
