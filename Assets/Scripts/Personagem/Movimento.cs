
using UnityEngine;
using System.Collections;

public class Movimento : MonoBehaviour {
	Transform tr;
	Rigidbody rb;
	Estados estados;
	Player objetoDeJogo;

	public float velocidade = 8;
	public float velAceleracao = 8;
	public float velAceleracaoNoAr = 3;
	public float velDesaceleracaoNoAr = 3;
	public float velAceleracaoVertical = 20;
	public float maxVelVertical = -40;

	public float forcaPulo = 9;
	public float intervaloPuloMaisAlto = 0.6f;
	public float intervaloPuloMaisAltoOriginal = 0.6f;

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
			anim.SetBool ("NoAr", true);
		else
		{
			intervaloPuloMaisAlto = intervaloPuloMaisAltoOriginal;
			anim.SetBool("NoAr", false);
		}

		if(estados.pulandoMaisAlto)
		{
			if(intervaloPuloMaisAlto <= 0)
			{
				estados.DesativaPuloMaisAlto();	
			}else
			{
//				if(log)
//					Debug.Log("Pulando mais alto");
				rb.velocity += Vector3.up * forcaPulo * intervaloPuloMaisAlto * Time.deltaTime * 5;
				intervaloPuloMaisAlto -= Time.deltaTime;
			}
		}else
			intervaloPuloMaisAlto = intervaloPuloMaisAltoOriginal;
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
		if(estados.movendo)
		{
			if(tempoAndando >= tempoAteCorrer)
			{
				estados.correndo = true;
			}else if(!estados.noAr)
				tempoAndando += Time.deltaTime;
		}else
		{
			tempoAndando = 0;
		}

		if(estados.noAr)
		{
			if(estados.correndo)
			{
				velFinal = Mathf.Lerp(velFinal, (estados.GetDirecao() * velocidade) -(estados.GetDirecao() * velDesaceleracaoNoAr),
				                      Time.deltaTime * velAceleracaoNoAr);
			}else
			{
				velFinal = Mathf.Clamp(Mathf.Lerp(velFinal, (estados.GetDirecao() * velocidade) -(estados.GetDirecao() * velDesaceleracaoNoAr * 2),
				                      	Time.deltaTime * velDesaceleracaoNoAr),
				                       -velocidade * (0.5f + tempoAndando),
				                       velocidade * (0.5f + tempoAndando));
			}


			velAtual = Mathf.Lerp(velAtual, velFinal, Time.deltaTime * velAceleracaoNoAr + Mathf.Abs(velAtual * 1.25f));
		}else
		{
			velFinal = estados.GetDirecao() * velocidade;
			velAtual = Mathf.Lerp(velAtual, velFinal, Time.deltaTime * velAceleracao);
		}
		
		if(estados.noAr && rb.velocity.y > maxVelVertical)
			rb.velocity += Vector3.ClampMagnitude(Vector3.down * Mathf.Abs(rb.velocity.y) * 0.01f, 0.1f) + (Vector3.down * velAceleracaoVertical) * Time.deltaTime;
		else
			rb.velocity += Vector3.down * velAceleracaoVertical * 0.05f * Time.deltaTime;
		
		
		if(estados.rotacionando)
		{
			rb.velocity = Vector3.zero;
			anim.speed = 0;
		}else if(estados.movendo || estados.noAr)
		{
			anim.speed = 1;
			rb.MovePosition(rb.position + (new Vector3(dirMovimento.x * velAtual, 0, dirMovimento.z * velAtual) * Time.deltaTime));
		}
	}

	void ChecaVelocidadeDaQueda ()
	{
		if(rb.velocity.y < maxVelVertical)
		{
			tr.position = objetoDeJogo.ultimaPos;
			if(objetoDeJogo.ultimaPlataformaColidida != null && !objetoDeJogo.HaChaoNoPonto(tr.position))
				tr.position += (objetoDeJogo.ultimaPlataformaColidida.position - tr.position).normalized * 2;
			objetoDeJogo.ResetaRotacaoDeCamera();

			rb.velocity = Vector3.zero;
		}
	}

	void Pula()
	{
//		if(log)
//			Debug.Log("Pulando");
		estados.pulando = false;
		rb.position += Vector3.up * 0.1f;
		rb.velocity = Vector3.up * forcaPulo;
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
