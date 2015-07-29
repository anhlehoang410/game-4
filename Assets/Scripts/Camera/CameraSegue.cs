using UnityEngine;
using System.Collections;

public class CameraSegue : MonoBehaviour {
	Transform tr;
	public RotacionaCamera rotacionaCamera;
	public Transform alvo;
	public Vector3 distancia;
	Vector3 distanciaRelativa;
	public float vel;
	Vector3 novaPos;

	void Awake()
	{
		tr = transform;
		DeterminaNovaPosicao();
		tr.position = novaPos;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		DeterminaNovaPosicao();
		tr.position = Vector3.Lerp(tr.position, novaPos, Time.deltaTime * vel);
	}

	void DeterminaNovaPosicao ()
	{
		switch (rotacionaCamera.rotacaoAtual)
		{
		case RotacionaCamera.RotacaoAtual.Frente:
			distanciaRelativa = distancia;
			break;
		case RotacionaCamera.RotacaoAtual.Direita:
			distanciaRelativa = new Vector3(distancia.z, distancia.y, distancia.x);
			break;
		case RotacionaCamera.RotacaoAtual.Fundo:
			distanciaRelativa = -distancia;
			distanciaRelativa.y = distancia.y;
			break;
		case RotacionaCamera.RotacaoAtual.Esquerda:
			distanciaRelativa = new Vector3(-distancia.z, distancia.y, -distancia.x);
			break;
		}
		novaPos = alvo.position + distanciaRelativa;
	}
}
