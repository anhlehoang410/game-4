
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
		base.Update();
		tr.rotation = rotacionaCamera.transform.rotation;
	}

//	void LateUpdate()
//	{
//		if(!estados.naParede)
//			VerificaChao();
//	}

	void FixedUpdate()
	{
		if(!estados.noAr)
		{
			trCentroD.localPosition = new Vector3(0.6f, trCentroD.localPosition.y, trCentroD.localPosition.z);
			trCentroE.localPosition = new Vector3(-0.4f, trCentroE.localPosition.y, trCentroE.localPosition.z);
		}else
		{
			trCentroD.localPosition = new Vector3(0.5f, trCentroD.localPosition.y, trCentroD.localPosition.z);
			trCentroE.localPosition = new Vector3(-0.3f, trCentroE.localPosition.y, trCentroE.localPosition.z);
		}

		ChecaProfundidade();
//		if(EstaCaindo() && !estados.naParede)
			VerificaChao();

		intensidadeVelVertical = Mathf.Clamp(rb.velocity.y*0.5f, 1, 5);
		vetorIntensidadeVertical = Vector3.down * 0.25f * (-0.5f + intensidadeVelVertical);
	}
	
	////---CHECAGENS DE COLISAO---////
	void ChecaProfundidade ()
	{
		estados.direcaoDaParede = 0;
		if (rotacionaCamera.Rotacionando ())
			return;

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
				estados.HabilitaPulo();
			}
		}

		if(ColidiuComAlgoNaDireita() || ColidiuComAlgoNaEsquerda())
		{
			Transform trObjeto = colisaoEmProfundidade.transform;
//			Debug.Log(rb.velocity.x);
			if(trObjeto.CompareTag("Parede"))
			{
				estados.direcaoDaParede = RetornaLadoEmRelacaoAoPlayer(trObjeto.position);
				if(estados.noAr)
				{
					novaPos = colisaoEmProfundidade.point + tr.forward * 0.5f;
					AtualizaProfundidade(trObjeto);

					if(estados.ApertandoParaOsLados() || movimento.contTempoPraPularDaParede <= movimento.tempoMaximoAtePuloRapido)
						estados.AtivaEstaNaParede();

					estados.Para();
					return;
				}else
				{
					if(ApertandoParaOLadoDaParede())
					{
						estados.Para();
						return;
					}
				}
			}else
			{
				estados.DesabilitaEstaNaParede();
			}
		}

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

		if(estados.naParede)
		{
			return;
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
							{
								AtualizaProfundidade(trObjetoDireita);
								if(trObjetoDireita.CompareTag("Parede"))
								{
									estados.AtivaEstaNaParede();
									return;
								}
							}else
							{
								AtualizaProfundidade(colisaoEmProfundidade.transform);
								if(colisaoEmProfundidade.transform.CompareTag("Parede"))
								{
									estados.AtivaEstaNaParede();
									return;
								}
							}
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
				estados.DesabilitaEstaNaParede();
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
							{
								AtualizaProfundidade(trObjetoEsquerda);
								if(trObjetoEsquerda.CompareTag("Parede"))
								{
									estados.AtivaEstaNaParede();
									return;
								}
							}else
							{
								AtualizaProfundidade(colisaoEmProfundidade.transform);
								if(colisaoEmProfundidade.transform.CompareTag("Parede"))
								{
									estados.AtivaEstaNaParede();
									return;
								}
							}
							estados.Para();
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
				estados.DesabilitaEstaNaParede();
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
		if(log)
			Debug.Log("Verificando chao");
		if(HaPlataformaEmProfundidade(trBaseM.position) ||
		   (HaPlataformaEmProfundidade(trBaseD.position) || HaPlataformaEmProfundidade(trBaseE.position)))

		{
			if(log)
				Debug.Log("Chao encontrado");
			if(estados.noAr && EstaCaindo())
				Invoke("AtivaEstaNoChao", 0.01f);
//			else if(!estados.noAr && !estados.podePular)
//				AtivaEstaNoChao();

			CancelInvoke("AtivaEstaNoAr");
			estados.DesabilitaEstaNaParede();
			return true;
		}else
		{
			if(log)
				Debug.Log("Nenhum chao encontrado");
			if(estados.naParede)
				return true;

			Invoke("AtivaEstaNoAr", 0.2f);
			CancelInvoke("AtivaEstaNoChao");
//			estados.podePular = false;
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
		estados.HabilitaPulo();
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
		case RotacionaCamera.RotacaoAtual.Direita:
			if(ponto1.z > ponto2.z)
				return ponto2;
			return ponto1;
		default:
			if(ponto1.x < ponto2.x)
				return ponto1;
			return ponto2;
		}
	}

	int RetornaLadoEmRelacaoAoPlayer (Vector3 ponto)
	{
		switch(rotacionaCamera.rotacaoAtual)
		{
		case RotacionaCamera.RotacaoAtual.Frente:
			if(ponto.x < tr.position.x)
				return Definicoes.ESQUERDA;
			return Definicoes.DIREITA;
		case RotacionaCamera.RotacaoAtual.Esquerda:
			if(ponto.z < tr.position.z)
				return Definicoes.DIREITA;
			return Definicoes.ESQUERDA;
		case RotacionaCamera.RotacaoAtual.Fundo:
			if(ponto.x < tr.position.x)
				return Definicoes.DIREITA;
			return Definicoes.ESQUERDA;
		case RotacionaCamera.RotacaoAtual.Direita:
			if(ponto.z < tr.position.z)
				return Definicoes.ESQUERDA;
			return Definicoes.DIREITA;
		default:
			if(ponto.x < tr.position.x)
				return Definicoes.ESQUERDA;
			return Definicoes.DIREITA;
		}
	}
	
	bool AtrasDeAlgo()
	{
		Debug.DrawLine(trMeio.position - trMeio.forward * profundidadeDeChecagem, trMeio.position, Color.gray);
		return (Physics.Linecast(trMeio.position - trMeio.forward * profundidadeDeChecagem, trMeio.position, layerPlataforma + layerParede));
	}
	////---FIM CHECAGENS DE COLISAO---////
	
	bool ApertandoParaOLadoDaParede ()
	{
		return ((estados.direcaoDaParede.Equals(Definicoes.DIREITA) && estados.GetDirecao().Equals(Definicoes.DIREITA)) ||
			(estados.direcaoDaParede.Equals(Definicoes.ESQUERDA) && estados.GetDirecao().Equals(Definicoes.ESQUERDA)));
	}

	protected override IEnumerator AtualizaUltimaPosicao ()
	{
		while(true)
		{
			yield return new WaitForEndOfFrame();
			if(!estados.noAr && !rotacionaCamera.Rotacionando() && !estados.naParede && HaChaoNoPonto(tr.position))
			{
				ultimaPos = tr.position;
				ultimaRotacao = rotacionaCamera.rotacaoAtual;
			}
			yield return new WaitForSeconds(delayParaChecar);
		}
	}
}
