
using UnityEngine;
using System.Collections;

public class Player : ObjetoDeJogo {
	Rigidbody rb;
	float intensidadeVelVertical;
	Vector3 vetorIntensidadeVertical;
	protected Estados estados;
	protected Movimento movimento;
	protected Vector3 novaPos;
	
	protected RaycastHit colisaoEmProfundidade;
	
	public bool log;
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		rb = GetComponent<Rigidbody>();
		estados = GetComponent<Estados>();
		movimento = GetComponent<Movimento>();
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		ChecaProfundidade();
		if(EstaCaindo())
			VerificaChao();

		tr.rotation = rotacionaCamera.transform.rotation;
		base.Update();
	}

	void FixedUpdate()
	{
		intensidadeVelVertical = Mathf.Clamp(rb.velocity.y*0.5f, 1, 5);
		vetorIntensidadeVertical = Vector3.down * 0.25f * (-0.5f + intensidadeVelVertical);
	}
	
	////---CHECAGENS DE COLISAO---////
	void ChecaProfundidade ()
	{
		if (rotacionaCamera.Rotacionando ())
			return;

		if(CabecaBateuEmAlgo())
		{
			Transform trColisor = colisaoEmProfundidade.transform;
			Vector3 pontoDeColisao = colisaoEmProfundidade.point;

			if(RetornaPontoMaisProximo(pontoDeColisao, tr.position).Equals(pontoDeColisao))
			{
				if(log)
					Debug.Log("Topo colidiu com "+colisaoEmProfundidade.collider.name, colisaoEmProfundidade.transform);
				novaPos = pontoDeColisao - tr.forward;
				if(!ColisaoOcorreuComObjetoAoFundo())
					AtualizaProfundidade(trColisor);
			}
		}
		if(AtrasDeAlgo())
		{
//			if(log)
//				Debug.Log("Atras de algum objeto");
			return;
		}
		
		Debug.DrawLine(trBaseM.position - (trBaseM.forward * profundidadeDeChecagem) + vetorIntensidadeVertical, 
		               trBaseM.position + (trBaseM.forward * profundidadeDeChecagem) + vetorIntensidadeVertical, 
		               Color.green, 0.01f);
		Debug.DrawLine(trMeio.position - trMeio.forward * profundidadeDeChecagem, 
		               trMeio.position + trMeio.forward * profundidadeDeChecagem, 
		               Color.green, 0.01f);
		Debug.DrawLine(trTopoM.position - trTopoM.forward * profundidadeDeChecagem, 
		               trTopoM.position + trTopoM.forward * profundidadeDeChecagem, 
		               Color.green, 0.01f);

		if(EstaCaindo() &&
		   (HaPlataformaEmProfundidade(trBaseM.position + vetorIntensidadeVertical) || 
			 HaPlataformaEmProfundidade(trBaseD.position + vetorIntensidadeVertical) || 
			 HaPlataformaEmProfundidade(trBaseE.position + vetorIntensidadeVertical)))
		{
			Transform trColisor = colisaoEmProfundidade.transform;
			Vector3 pontoDeColisao = colisaoEmProfundidade.point;
			if(log)
				Debug.Log("Base colidiu com "+trColisor.name, trColisor);
			
//			if(RetornaPontoMaisProximo(pontoDeColisao, tr.position).Equals(pontoDeColisao))
			{
//				novaPos = pontoDeColisao + tr.forward * 0.5f;
//				AtualizaProfundidade(colisaoEmProfundidade.transform);
//			}else
//			{
				novaPos = pontoDeColisao + tr.forward * 0.5f;
				AtualizaProfundidade(trColisor);
			}
		}
		if(estados.GetDirecao() == Definicoes.DIREITA) //movendo para a direita
		{
			Debug.DrawLine(trCentroD.position - trCentroD.forward * profundidadeDeChecagem, 
			               trCentroD.position + trCentroD.forward * profundidadeDeChecagem, 
			               Color.blue, 0.01f);
			if(ColidiuComAlgoNaDireita())
			{
				Transform trObjetoDireita = colisaoEmProfundidade.transform;
				if(!ColisaoOcorreuComObjetoAoFundo())
				{
					if(log)
						Debug.Log("Colisao nao ocorreu ao fundo ", trObjetoDireita);
					if(ColidiuComAlgoNaEsquerda())
					{
						if(!trObjetoDireita.Equals(colisaoEmProfundidade.transform))
						{
							if(log)
								Debug.Log("Bloqueio a frente ", trObjetoDireita);
							Vector3 colisorMaisProximoDaCamera = RetornaPontoMaisProximo(trObjetoDireita.position, colisaoEmProfundidade.transform.position);
							novaPos = colisorMaisProximoDaCamera - tr.forward * 1.5f;
							if(colisorMaisProximoDaCamera == trObjetoDireita.position)
								AtualizaProfundidade(trObjetoDireita);
							else
								AtualizaProfundidade(colisaoEmProfundidade.transform);
							estados.Para();
						}else
						{
							estados.Move();
						}
					}else
					{
						if(log)
							Debug.Log("Bloqueio a direita ", trObjetoDireita);
						if(HaChaoNoPonto(colisaoEmProfundidade.point))
						{
							novaPos = trObjetoDireita.position - tr.forward * 1.5f;
							AtualizaProfundidade(trObjetoDireita);
							estados.Move();
							movimento.DefineNovoEixo();
						}else
						{
							estados.Para();
							movimento.ZeraEixo(Vector3.right);
							rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
						}
					}
				}else
				{
					estados.Move();
				}
			}else
			{
				estados.Move();
				movimento.DefineNovoEixo();
			}
		}else if(estados.GetDirecao() == Definicoes.ESQUERDA) //movendo para a esquerda
		{
			Debug.DrawLine(trCentroE.position - trCentroE.forward * profundidadeDeChecagem, 
			               trCentroE.position + trCentroE.forward * profundidadeDeChecagem, 
			               Color.blue, 0.01f);
			if(ColidiuComAlgoNaEsquerda())
			{
				Transform trObjetoEsquerda = colisaoEmProfundidade.transform;
				if(!ColisaoOcorreuComObjetoAoFundo())
				{
					if(log)
						Debug.Log("Colisao nao ocorreu ao fundo ", trObjetoEsquerda);
					if(ColidiuComAlgoNaDireita())
					{
						if(!trObjetoEsquerda.Equals(colisaoEmProfundidade.transform))
						{
							if(log)
								Debug.Log("Bloqueio a frente ", trObjetoEsquerda);
							Vector3 colisorMaisProximoDaCamera = RetornaPontoMaisProximo(trObjetoEsquerda.position, colisaoEmProfundidade.transform.position);
							novaPos = colisorMaisProximoDaCamera - tr.forward * 1.5f;
							if(colisorMaisProximoDaCamera == trObjetoEsquerda.position)
								AtualizaProfundidade(trObjetoEsquerda);
							else
								AtualizaProfundidade(colisaoEmProfundidade.transform);
						}else
						{
							estados.Move();
						}
					}else
					{
						if(log)
							Debug.Log("Bloqueio a esquerda ", trObjetoEsquerda);
						if(HaChaoNoPonto(colisaoEmProfundidade.point))
						{
							novaPos = trObjetoEsquerda.position - tr.forward * 1.5f;
							AtualizaProfundidade(trObjetoEsquerda);
							estados.Move();
							movimento.DefineNovoEixo();

						}else
						{
							estados.Para();
							movimento.ZeraEixo(Vector3.right);
							rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
						}
					}
				}else
				{
					estados.Move();
				}
			}else
			{
				estados.Move();
				movimento.DefineNovoEixo();
			}
		}
	}
	
	bool CabecaBateuEmAlgo ()
	{
		if(Physics.Linecast(trTopoM.position - trTopoM.forward * profundidadeDeChecagem, trTopoM.position + trTopoM.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede) ||
		   Physics.Linecast(trTopoD.position - trTopoD.forward * profundidadeDeChecagem, trTopoD.position + trTopoD.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede) ||
		   Physics.Linecast(trTopoE.position - trTopoE.forward * profundidadeDeChecagem, trTopoE.position + trTopoE.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede))
		{
			Debug.DrawLine(trTopoM.position - trTopoM.forward * profundidadeDeChecagem, trTopoM.position + trTopoM.forward * profundidadeDeChecagem, Color.red);
			Debug.DrawLine(trTopoD.position - trTopoD.forward * profundidadeDeChecagem, trTopoD.position + trTopoD.forward * profundidadeDeChecagem, Color.red);
			Debug.DrawLine(trTopoE.position - trTopoE.forward * profundidadeDeChecagem, trTopoE.position + trTopoE.forward * profundidadeDeChecagem, Color.red);
			return true;
		}else
		{
			Debug.DrawLine(trTopoM.position - trTopoM.forward * profundidadeDeChecagem, trTopoM.position + trTopoM.forward * profundidadeDeChecagem, Color.green);
			Debug.DrawLine(trTopoD.position - trTopoD.forward * profundidadeDeChecagem, trTopoD.position + trTopoD.forward * profundidadeDeChecagem, Color.green);
			Debug.DrawLine(trTopoE.position - trTopoE.forward * profundidadeDeChecagem, trTopoE.position + trTopoE.forward * profundidadeDeChecagem, Color.green);
			return false;
		}
	}
	
	public override bool VerificaChao()
	{
		Debug.DrawLine(trBaseM.position, trBaseM.position - trBaseM.up * 0.25f * intensidadeVelVertical);
		Debug.DrawLine(trBaseD.position, trBaseD.position - trBaseD.up * 0.25f * intensidadeVelVertical);
		Debug.DrawLine(trBaseE.position, trBaseE.position - trBaseE.up * 0.25f * intensidadeVelVertical);
		
//		if (Physics.Linecast (trBaseM.position, trBaseM.position - trBaseM.up * 0.25f * intensidadeVelVertical, layerPlataforma) &&
//		    
//		    (Physics.Linecast (trBaseD.position, trBaseD.position - trBaseD.up * 0.25f * intensidadeVelVertical, layerPlataforma) ||	
//		 	Physics.Linecast (trBaseE.position, trBaseE.position - trBaseE.up * 0.25f * intensidadeVelVertical, layerPlataforma))) 
		if(HaPlataformaEmProfundidade(trBaseM.position) &&
		   (HaPlataformaEmProfundidade(trBaseD.position) || HaPlataformaEmProfundidade(trBaseE.position)))

		{
			if(estados.noAr && EstaCaindo())
				Invoke("AtivaEstaNoChao", 0.01f);
			CancelInvoke("AtivaEstaNoAr");
			return true;
		}else
		{
			Invoke("AtivaEstaNoAr", 0.2f);
			CancelInvoke("AtivaEstaNoChao");
			estados.podePular = false;
			return false;
		}
	}
	
	bool EstaCaindo ()
	{
		return (rb.velocity.y < -0.1f);
	}

	void AtivaEstaNoAr()
	{
		estados.AtivaEstaNoAr();
	}

	void AtivaEstaNoChao()
	{
		Debug.Log("Chao");
		estados.AtivaEstaNoChao();
	}
	
	bool HaPlataformaEmProfundidade(Vector3 pontoParaChecar)
	{
		if(Physics.Linecast(pontoParaChecar + (Vector3.up * 0.5f) - (tr.forward * profundidadeDeChecagem),
		                    pontoParaChecar + (Vector3.down * 0.5f) + (tr.forward * profundidadeDeChecagem),
		                    out colisaoEmProfundidade, 
		                    layerPlataforma + layerParede))
		{
			LayerMask layerObjetoColidido = colisaoEmProfundidade.transform.gameObject.layer;
//			if(log)
//				Debug.Log("Layer "+(1<<layerObjetoColidido)+" eh igual a "+layerPlataforma.value+"? "+ (1<<layerObjetoColidido).Equals(layerPlataforma.value));
			if((1<<layerObjetoColidido).Equals(layerPlataforma.value))
				return HaChaoNoPonto(colisaoEmProfundidade.point + tr.up * 0.5f + tr.forward * 0.5f);
		}
		return false;
	}
	
	bool ColidiuComAlgoNaDireita ()
	{
		if(Physics.Linecast(trCentroD.position - trCentroD.forward * profundidadeDeChecagem, trCentroD.position + trCentroD.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede) ||
//		   Physics.Linecast(trBaseD.position - trBaseD.forward * profundidadeDeChecagem, trBaseD.position + trBaseD.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede) ||
		   Physics.Linecast(trTopoD.position - trTopoD.forward * profundidadeDeChecagem, trTopoD.position + trTopoD.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede))
			return true;
		return false;

		
	}
	
	bool ColidiuComAlgoNaEsquerda ()
	{
		if(Physics.Linecast(trCentroE.position - trCentroE.forward * profundidadeDeChecagem, trCentroE.position + trCentroE.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede) ||
//		   Physics.Linecast(trBaseE.position - trBaseE.forward * profundidadeDeChecagem, trBaseE.position + trBaseE.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede) ||
		   Physics.Linecast(trTopoE.position - trTopoE.forward * profundidadeDeChecagem, trTopoE.position + trTopoE.forward * profundidadeDeChecagem, out colisaoEmProfundidade, layerPlataforma + layerParede))
			return true;
		return false;
	}
	
	public bool HaChaoNoPonto (Vector3 pontoDeColisao)
	{
		
		if(Physics.Linecast(pontoDeColisao, pontoDeColisao + (Vector3.down * (trMeio.position.y - trBaseM.position.y) * 1.25f), out colisaoEmProfundidade, layerPlataforma))
		{
			Debug.DrawLine(pontoDeColisao, pontoDeColisao + (Vector3.down * (trMeio.position.y - trBaseM.position.y) * 1.25f), Color.red, 1f);
			return true;
		}else
		{
			Debug.DrawLine(pontoDeColisao, pontoDeColisao + (Vector3.down * (trMeio.position.y - trBaseM.position.y) * 1.25f), Color.black, 1f);
			return false;
		}
		
	}
	
	bool ColisaoOcorreuComObjetoAoFundo ()
	{
		switch(rotacionaCamera.rotacaoAtual)
		{
		case RotacionaCamera.RotacaoAtual.Frente:
			if(colisaoEmProfundidade.point.z > tr.position.z)
				return true;
			return false;
		case RotacionaCamera.RotacaoAtual.Esquerda:
			if(colisaoEmProfundidade.point.x > tr.position.x)
				return true;
			return false;
		case RotacionaCamera.RotacaoAtual.Fundo:
			if(colisaoEmProfundidade.point.z < tr.position.z)
				return true;
			return false;
		case RotacionaCamera.RotacaoAtual.Direita:
			if(colisaoEmProfundidade.point.x < tr.position.x)
				return true;
			return false;
		default:
			return false;
		}
	}
	
	void AtualizaProfundidade(Transform trColisor)
	{
		switch(rotacionaCamera.rotacaoAtual)
		{
		case RotacionaCamera.RotacaoAtual.Frente:
			novaPos.x = tr.position.x;
			break;
		case RotacionaCamera.RotacaoAtual.Esquerda:
			novaPos.z = tr.position.z;
			break;
		case RotacionaCamera.RotacaoAtual.Fundo:
			novaPos.x = tr.position.x;
			break;
		case RotacionaCamera.RotacaoAtual.Direita:
			novaPos.z = tr.position.z;
			break;
		}
		if(log)
			Debug.Log("Personagem movido para"+novaPos);
		novaPos.y = tr.position.y;
		tr.position = novaPos;
		ultimaPlataformaColidida = trColisor;
	}
	
	Vector3 RetornaPontoMaisProximo (Vector3 ponto1, Vector3 ponto2)
	{
		switch(rotacionaCamera.rotacaoAtual)
		{
		case RotacionaCamera.RotacaoAtual.Frente:
			if(ponto1.z < ponto2.z)
				return ponto1;
			return ponto2;
		case RotacionaCamera.RotacaoAtual.Esquerda:
			if(ponto1.x < ponto2.x)
				return ponto1;
			return ponto2;
		case RotacionaCamera.RotacaoAtual.Fundo:
			if(ponto1.z > ponto2.z)
				return ponto1;
			return ponto2;
		default:
			if(ponto1.x < ponto2.x)
				return ponto1;
			return ponto2;
		}
	}
	
	bool AtrasDeAlgo()
	{
		Debug.DrawLine(trMeio.position - trMeio.forward * profundidadeDeChecagem, trMeio.position, Color.gray);
		return (Physics.Linecast(trMeio.position - trMeio.forward * profundidadeDeChecagem, trMeio.position, layerPlataforma + layerParede));
	}
	////---FIM CHECAGENS DE COLISAO---////
}
