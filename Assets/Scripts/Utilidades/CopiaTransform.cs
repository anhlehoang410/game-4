using UnityEngine;
using System.Collections;

public class CopiaTransform : MonoBehaviour {
	
	public Transform alvo;
	private Transform tr;
	private Rigidbody rb;
	public bool	posicao;
	public bool pX,pY,pZ;
	public bool	rotacao;
	public bool rX,rY,rZ;
	public bool	tamanho;
	public bool tX,tY,tZ;
	
	public bool usaRigidbody;
	
	private Vector3 novaPos;
	private Vector3 euleRot;
	private Quaternion novaRot;
	private Vector3 novoTam;
	
	public bool lerp;
	public float velLerp = 1;
	
	void Start()
	{
		if(usaRigidbody)
			rb = GetComponent<Rigidbody>();
		tr = transform;
	}
	
	void Update () 
	{
		if(!alvo)
			return;
		if(posicao)
		{
			novaPos = tr.position;
			if(pX)
				novaPos.x = alvo.position.x;
			if(pY)
				novaPos.y = alvo.position.y;
			if(pZ)
				novaPos.z = alvo.position.z;				
		}
		if(rotacao)
		{
			euleRot = tr.rotation.eulerAngles;
			if(rX)
				euleRot.x = alvo.rotation.eulerAngles.x;
			if(rY)
				euleRot.y = alvo.rotation.eulerAngles.y;
			if(rZ)
				euleRot.z = alvo.rotation.eulerAngles.z;
            novaRot = Quaternion.Euler(euleRot);
		}
		if(tamanho)
		{
			novoTam = tr.localScale;
			if(tX)
				novoTam.x = alvo.localScale.x;
			if(tY)
				novoTam.y = alvo.localScale.y;
			if(tZ)
				novoTam.z = alvo.localScale.z;
		}
	}
	
	void LateUpdate()
	{
		if(!alvo)
			return;
		if(!lerp)
		{
			if(!usaRigidbody && posicao)
				tr.position = novaPos;
			if(!usaRigidbody && rotacao)
				tr.rotation = novaRot;
			if(tamanho)
				tr.localScale = novoTam;
		}else
		{
			if(!usaRigidbody && posicao)
				tr.position = Vector3.Lerp(tr.position, novaPos, Time.deltaTime * velLerp);
			if(!usaRigidbody && rotacao)
				tr.rotation = novaRot;
			if(tamanho)
				tr.localScale = novoTam;
		}
	}
	
	void FixedUpdate()
	{
		if(!usaRigidbody || !alvo)
			return;
		if(posicao)
			rb.MovePosition(novaPos);
		if(rotacao)
			rb.MoveRotation(novaRot);	
	}
}
