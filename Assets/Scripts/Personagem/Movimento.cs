
using UnityEngine;
using System.Collections;

public class Movimento : MonoBehaviour {
	Transform tr;
	Rigidbody rb;
	Estados estados;
	Player objetoDeJogo;

	public float velocidade = 10;
	public float velAceleracao = 8;
	public float velAceleracaoNoAr = 7;
	public float velDesaceleracaoNoAr = 0.5f;
	public float velAceleracaoVertical = 30;
	public float maxVelVertical = -40;

	public float forcaPulo = 12;
	public float intervaloPuloMaisAlto = 0.6f;
	public float intervaloPuloMaisAltoOriginal = 0.6f;

	public float intervaloAtePuloDaParede = 0.25f;
	public Vector3 direcaoPuloDaParede = new Vector3(0.75f, 1, 0);
	public float forcaPuloDaParede = 8;
	public float velQuedaNaParede = 10;
	public float tempoMaximoAtePuloDirecionado = 0.5f;
	public float tempoMaximoAtePuloRapido = 0.05f;
	public float contTempoPraPularDaParede = 0;

	public float tempoAteCorrer = 0.5f;
	float tempoAndando = 0;

	float velAtual;
	float velFinal;
	Vector3 velAntesDeRotacionar;

	Vector3 dirMovimento;

	public Animator anim;
	public Transform malha;

	public enum DirDeMovimento
	{
		Idle,
		Direita,
		Esquerda
	};

	public bool log;

	void Start()
	{
		tr = transform;
		rb = GetComponent<Rigidbody>();
		estados = GetComponent<Estados>();
		objetoDeJogo = GetComponent<Player>();

		intervaloPuloMaisAltoOriginal = intervaloPuloMaisAlto;
	}

	void ChecaEstados ()
	{
		if(estados.pulando)
			Pula();

		if(estados.noAr)
		{
			anim.SetBool ("NoAr", true);
		}else
		{
			anim.SetBool("NoAr", false);
		}

		if(estados.pulandoMaisAlto)
		{
			if(intervaloPuloMaisAlto <= 0)
			{
				estados.DesativaPuloMaisAlto();	
			}else
			{
				rb.velocity += Vector3.up * forcaPulo * intervaloPuloMaisAlto * Time.deltaTime * 5;
				intervaloPuloMaisAlto -= Time.deltaTime;
			}
		}else
			intervaloPuloMaisAlto = intervaloPuloMaisAltoOriginal;

//		if(estados.naParede)
//		{
//			if(estados.direcaoDaParede.Equals(Definicoes.DIREITA) && estados.GetDirecao().Equals(Definicoes.DIREITA) ||
//			   estados.direcaoDaParede.Equals(Definicoes.ESQUERDA) && estados.GetDirecao().Equals(Definicoes.ESQUERDA))
//			{
//				contTempoPraPularDaParede = 0;
//			}else
//			{
//				contTempoPraPularDaParede += Time.deltaTime;
//				if(contTempoPraPularDaParede >= tempoMaximoAtePuloDirecionado)
//				{
//					estados.DesabilitaEstaNaParede();
////					tr.position -= Vector3.right * estados.direcaoDaParede * 0.25f;
//				}
//			}
//		}else if(!estados.noAr)
//		{
//			contTempoPraPularDaParede = 0;
//		}
	}

	void LateUpdate()
	{
		ChecaEstados();
		DefineNovoEixo();
		AtualizaOrientacao();
		AtualizaVelocidade();
		AtualizaAnimacao();
		ChecaVelocidadeDaQueda();
	}

	void AtualizaOrientacao ()
	{
		if(objetoDeJogo.rotacionaCamera.Rotacionando())
			return;

		if(estados.GetDirecao() == Definicoes.ESQUERDA)
			malha.localScale = new Vector3(-1, 1, 1);
		else if(estados.GetDirecao() == Definicoes.DIREITA)
			malha.localScale = Vector3.one;
		
	}

	void AtualizaAnimacao ()
	{
		if(estados.movendo)
			anim.SetFloat ("Velocidade", Mathf.Abs(velAtual/velocidade));
		else
			anim.SetFloat ("Velocidade", 0);
	}

	void AtualizaVelocidade ()
	{
//		if(estados.movendo)
//		{
//			if(tempoAndando >= tempoAteCorrer)
//			{
//				estados.correndo = true;
//			}else if(!estados.noAr)
//				tempoAndando += Time.deltaTime;
//		}else
//		{
//			tempoAndando = 0;
//		}

		if(estados.noAr)
		{
//			if(estados.correndo)
			if(estados.GetDirecao() != 0)
				velFinal = Mathf.Lerp(velFinal, (estados.GetDirecao() * velocidade) -(estados.GetDirecao() * velDesaceleracaoNoAr),
				                      Time.deltaTime * velAceleracaoNoAr);
			else
				velFinal = Mathf.Lerp(velFinal, 0, Time.deltaTime * velAceleracaoNoAr);

			if(estados.naParede)
			{
				if(estados.direcaoDaParede == estados.GetDirecao())
					velFinal = 0;
			}
//			}
//			else
//			{
//				velFinal = Mathf.Clamp(Mathf.Lerp(velFinal, (estados.GetDirecao() * velocidade) -(estados.GetDirecao() * velDesaceleracaoNoAr * 2),
//				                      	Time.deltaTime * velDesaceleracaoNoAr),
//				                       -velocidade * (0.5f + tempoAndando),
//				                       velocidade * (0.5f + tempoAndando));
//			}

			if((velAtual > 0 && velFinal < 0) || (velAtual < 0 && velFinal > 0))
				velAtual = 0;
			else
				velAtual = Mathf.Lerp(velAtual, velFinal, Time.deltaTime * velAceleracaoNoAr + Mathf.Abs(velAtual * 1.25f));
		}else
		{
			if(!estados.naParede)
			{
//				if(estados.correndo)
				if(estados.direcaoDaParede == estados.GetDirecao())
					velFinal = 0;
				else
					velFinal = estados.GetDirecao() * velocidade;
//				else
//					velFinal = estados.GetDirecao() * (velocidade/2);
				velAtual = Mathf.Lerp(velAtual, velFinal, Time.deltaTime * velAceleracao);
			}else
			{
				ZeraEixo(Vector3.right);
			}
		}
		
		if(rb.velocity.y > maxVelVertical)
		{
			if(estados.noAr)
				rb.velocity += Vector3.ClampMagnitude(Vector3.down * Mathf.Abs(rb.velocity.y) * 0.01f, 0.1f) + (Vector3.down * velAceleracaoVertical) * Time.deltaTime;
//			else if(estados.naParede && rb.velocity.x < 0)
//			{
//				float novoY = Mathf.Clamp(rb.velocity.y - (velQuedaNaParede * Time.deltaTime), -velQuedaNaParede, 0);
//				rb.velocity = new Vector3(rb.velocity.x, novoY, rb.velocity.z);
//			}
		}
//		else
//			rb.velocity += Vector3.down * velAceleracaoVertical * 0.05f * Time.deltaTime;
		
		
		if(estados.rotacionando)
		{
			rb.velocity = Vector3.zero;
			anim.speed = 0;
		}else if(estados.movendo || estados.noAr)
		{
			anim.speed = 1;
			rb.MovePosition(rb.position + (new Vector3(dirMovimento.x * velAtual, 0, dirMovimento.z * velAtual) * Time.deltaTime));
		}
//		else
//		{
//			rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, velDesaceleracaoNoAr);
//		}
	}

	void ChecaVelocidadeDaQueda ()
	{
		if(rb.velocity.y < maxVelVertical)
		{
			tr.position = objetoDeJogo.ultimaPos;
			if(objetoDeJogo.ultimaPlataformaColidida != null && !objetoDeJogo.HaChaoNoPonto(tr.position))
				tr.position += (objetoDeJogo.ultimaPlataformaColidida.position - tr.position).normalized * 2;
			objetoDeJogo.ResetaRotacaoDeCamera();
			estados.Morre();
			rb.velocity = Vector3.zero;
		}
	}

	void Pula()
	{
		if(estados.naParede)
		{
//			if(estados.direcaoDaParede.Equals(Definicoes.DIREITA))
//			{
//				direcaoPuloDaParede.x = -Mathf.Abs(direcaoPuloDaParede.x);
//				estados.dirDeMovimento = Estados.DirDeMovimento.Esquerda;
//			}else
//			{
//				direcaoPuloDaParede.x = Mathf.Abs(direcaoPuloDaParede.x);
//				estados.dirDeMovimento = Estados.DirDeMovimento.Direita;
//			}
//
//			rb.position += direcaoPuloDaParede * 0.25f;
//			Vector3 direcaoPuloDaPredeFinal = direcaoPuloDaParede;
//			if(!estados.direcaoDaParede.Equals(estados.direcaoDaParede))
//				direcaoPuloDaPredeFinal.x *= 1.5f;
//			rb.velocity = direcaoPuloDaParede * forcaPuloDaParede;
//			estados.DesabilitaEstaNaParede();
//			if(log)
//				Debug.Log("Pulando da parede");
//			contTempoPraPularDaParede = 0;
		}else
		{
			rb.position += Vector3.up * 0.1f;
			rb.AddForce(Vector3.up * forcaPulo, ForceMode.Impulse);
		}
		estados.pulando = false;
		anim.SetBool ("NoAr", true);
	}

	public void DefineNovoEixo ()
	{
		switch (objetoDeJogo.rotacionaCamera.rotacaoAtual)
		{
		case RotacionaCamera.RotacaoAtual.Frente:
			dirMovimento = Vector3.one;
			dirMovimento.z = 0;
			break;
		case RotacionaCamera.RotacaoAtual.Direita:
			dirMovimento = new Vector3(0, objetoDeJogo.eixo.y, objetoDeJogo.eixo.x);
			break;
		case RotacionaCamera.RotacaoAtual.Fundo:
			dirMovimento = -Vector3.one;
			dirMovimento.y = 1;
			dirMovimento.z = 0;
			break;
		case RotacionaCamera.RotacaoAtual.Esquerda:
			dirMovimento = new Vector3(0, objetoDeJogo.eixo.y, -objetoDeJogo.eixo.x);
			break;
		}
	}
	
	public void ZeraEixo(Vector3 eixoParaZerar)
	{
		eixoParaZerar = eixoParaZerar.normalized;
		switch (objetoDeJogo.rotacionaCamera.rotacaoAtual)
		{
		case RotacionaCamera.RotacaoAtual.Frente:
			if(eixoParaZerar.x > 0)
				dirMovimento.x = 0;
			if(eixoParaZerar.y > 0)
				dirMovimento.y = 0;
			if(eixoParaZerar.z > 0)
				dirMovimento.z = 0;
			break;
		case RotacionaCamera.RotacaoAtual.Direita:
			if(eixoParaZerar.x > 0)
				dirMovimento.z = 0;
			if(eixoParaZerar.y > 0)
				dirMovimento.y = 0;
			if(eixoParaZerar.z > 0)
				dirMovimento.x = 0;
			break;
		case RotacionaCamera.RotacaoAtual.Fundo:
			if(eixoParaZerar.x > 0)
				dirMovimento.x = 0;
			if(eixoParaZerar.y > 0)
				dirMovimento.y = 0;
			if(eixoParaZerar.z > 0)
				dirMovimento.z = 0;
			break;
		case RotacionaCamera.RotacaoAtual.Esquerda:
			if(eixoParaZerar.x > 0)
				dirMovimento.z = 0;
			if(eixoParaZerar.y > 0)
				dirMovimento.y = 0;
			if(eixoParaZerar.z > 0)
				dirMovimento.x = 0;
			break;
		}
	}

}
