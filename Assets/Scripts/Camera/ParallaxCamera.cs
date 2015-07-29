using UnityEngine;
using System.Collections;

public class ParallaxCamera : MonoBehaviour {
	Transform tr;
	public RotacionaCamera rotacionaCamera;
	public Transform alvo;
	Vector3 posOriginalAlvo;
	Vector3 posOriginal;
	Vector3 ultimaPos;
	Vector3 deltaPos; //variacao de posicao do objeto alvo
	[Range(-5f, 5f)]	public float intensidadeX = 0.5f;
	[Range(-5f, 5f)]	public float intensidadeY = 0.5f;
	Vector3 movimentoFinal;

	void Start()
	{
		if(!alvo)
			enabled = false;
		tr = transform;
		ultimaPos = alvo.position;
		posOriginalAlvo = alvo.position;
		posOriginal = tr.position;
	}

	// Update is called once per frame
	void LateUpdate () 
	{
		if(rotacionaCamera.Rotacionando())
		{
			ultimaPos = alvo.position;
			return;
		}
		deltaPos = posOriginalAlvo - ultimaPos;
//		posFinal = Vector3.zero;

		switch(rotacionaCamera.rotacaoAtual)
		{
		case RotacionaCamera.RotacaoAtual.Frente:
		case RotacionaCamera.RotacaoAtual.Fundo:
			movimentoFinal.x = deltaPos.x * intensidadeX;
			break;
		case RotacionaCamera.RotacaoAtual.Direita:
		case RotacionaCamera.RotacaoAtual.Esquerda:
			movimentoFinal.z = deltaPos.z * intensidadeX;
			break;
		}

		movimentoFinal.y = deltaPos.y * intensidadeY;
		tr.position =  posOriginal + movimentoFinal;
		ultimaPos = alvo.position;
	}
}
