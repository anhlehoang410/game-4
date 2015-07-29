using UnityEngine;
using System.Collections;

public class RotacionaCamera : MonoBehaviour {
	public bool player1;
	Transform tr;
	public enum RotacaoAtual
	{
		Frente,
		Direita,
		Fundo,
		Esquerda
	};
	public RotacaoAtual rotacaoAtual;
	Quaternion rotacaoAlvo = Quaternion.Euler(0,0,0);
	public float velRotacao = 5;
	bool rotacionando;
	bool idle;
	float cont = 0;

	public Camera camP1;
	public Camera camP2;
	bool mostrandoOutraCamera;
	float velMostraCamera = 5;

	// Use this for initialization
	void Awake () 
	{
		tr = transform;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(player1)
		{
			if(Input.GetButtonDown("RotacionaCameraEsq"))
				Rotaciona(false);
			else if(Input.GetButtonDown("RotacionaCameraDir"))
				Rotaciona(true);
		}else
		{
			if(Input.GetKeyDown(KeyCode.LeftControl))
				Rotaciona(false);
			else if(Input.GetKeyDown(KeyCode.RightControl))
				Rotaciona(true);
		}

		VerificaSeEstaIdle();
		AtualizaRotacaoAlvo();
		if(player1)
			AtualizaViewportRect();
	}

	void LateUpdate () 
	{
		if(idle)
			tr.Rotate(0, velRotacao * Time.deltaTime, 0);
		else
			tr.rotation = Quaternion.Lerp(tr.rotation, rotacaoAlvo, Time.deltaTime* velRotacao);
	}

	void Rotaciona(bool paraDireita)
	{
		switch(rotacaoAtual)
		{
		case RotacaoAtual.Frente:
			if(paraDireita)
				rotacaoAtual = RotacaoAtual.Direita;
			else
				rotacaoAtual = RotacaoAtual.Esquerda;
			break;
		case RotacaoAtual.Direita:
			if(paraDireita)
				rotacaoAtual = RotacaoAtual.Fundo;
			else
				rotacaoAtual = RotacaoAtual.Frente;
			break;
		case RotacaoAtual.Fundo:
			if(paraDireita)
				rotacaoAtual = RotacaoAtual.Esquerda;
			else
				rotacaoAtual = RotacaoAtual.Direita;
			break;
		case RotacaoAtual.Esquerda:
			if(paraDireita)
				rotacaoAtual = RotacaoAtual.Frente;
			else
				rotacaoAtual = RotacaoAtual.Fundo;
			break;
		default:
			rotacaoAtual = RotacaoAtual.Frente;
			break;
		}
		AtualizaRotacaoAlvo();
	}

	public void AtualizaRotacaoAlvo()
	{
		switch(rotacaoAtual)
		{
		case RotacaoAtual.Frente:
			rotacaoAlvo = Quaternion.Euler(0,0,0);
			break;
		case RotacaoAtual.Direita:
			rotacaoAlvo = Quaternion.Euler(0,270,0);
			break;
		case RotacaoAtual.Fundo:
			rotacaoAlvo = Quaternion.Euler(0,180,0);
			break;
		case RotacaoAtual.Esquerda:
			rotacaoAlvo = Quaternion.Euler(0,90,0);
			break;
		}
	}

	public void MostraOutraCamera()
	{
		mostrandoOutraCamera = true;
	}

	public void EscondeOutraCamera()
	{
		mostrandoOutraCamera = false;
	}

	void AtualizaViewportRect ()
	{
		Rect novoRectP1 = new Rect();
		Rect novoRectP2 = new Rect();
		novoRectP1 = camP1.rect;
		novoRectP2 = camP2.rect;

		if(mostrandoOutraCamera)
		{
			novoRectP1.x = Mathf.Lerp(camP1.rect.x, -0.5f, Time.deltaTime * velMostraCamera);
			novoRectP2.x = Mathf.Lerp(camP2.rect.x, 0.5f, Time.deltaTime * velMostraCamera);
		}else
        {
			novoRectP1.x = Mathf.Lerp(camP1.rect.x, 0, Time.deltaTime * velMostraCamera);
			novoRectP2.x = Mathf.Lerp(camP2.rect.x, 1, Time.deltaTime * velMostraCamera);
		}

		camP1.rect = novoRectP1;

		if(camP2)
			camP2.rect = novoRectP2;
	}

	public bool Rotacionando()
	{
		float dist = Mathf.Abs(tr.eulerAngles.y - rotacaoAlvo.eulerAngles.y);
		if(dist > 1f)
			return true;
		return false;
	}

	void VerificaSeEstaIdle ()
	{
		if(Input.anyKey)
		{
			cont = 0;
			idle = false;
		}else if(cont >= 20)
			idle = true;
		else
			cont+= Time.deltaTime;
	}
}
