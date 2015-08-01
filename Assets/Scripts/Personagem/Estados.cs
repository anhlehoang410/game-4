using UnityEngine;
using System.Collections;

public class Estados : MonoBehaviour {
	public bool podePular;
	public bool pulando;
	public bool pulandoMaisAlto;
	public bool noAr;
	public bool movendo;
	public bool correndo;
	public bool rotacionando;
	public bool naParede;
	public int direcaoDaParede;

	public enum DirDeMovimento
	{
		Idle,
		Direita,
		Esquerda
	};

	public DirDeMovimento dirDeMovimento;

	public bool log;

	public void Direita()
	{
		dirDeMovimento = DirDeMovimento.Direita;
	}

	public void Esquerda()
	{
		dirDeMovimento = DirDeMovimento.Esquerda;
	}

	public void Idle()
	{
		dirDeMovimento = DirDeMovimento.Idle;
		correndo = false;
		movendo = false;
	}

	public void Move()
	{
		movendo = true;
	}

	public void Para()
	{
		movendo = false;
		correndo = false;
	}

	public void Pula()
	{
		if(!podePular)
			return;
		
		pulando = true;
		podePular = false;
		if(!naParede)
			pulandoMaisAlto = true;
		correndo = false;
		Invoke("AtivaEstaNoAr", 0.05f);
	}

	public void HabilitaPulo()
	{
		noAr = false;
		podePular = true;
		DesativaPuloMaisAlto();
		pulando = false;
	}

	public void AtivaEstaNoAr()
	{
		noAr = true;
		podePular = false;
	}

	public void AtivaEstaNaParede()
	{
//		noAr = false;
		naParede = true;
//		DesativaPuloMaisAlto();
		pulando = false;
//		podePular = true;
		movendo = false;
	}

	public void DesabilitaEstaNaParede()
	{
		naParede = false;
	}

	public void DesativaPuloMaisAlto()
	{
//		if(log)
//			Debug.Log("Desativando pulo mais alto");
		pulandoMaisAlto = false;
		pulando = false;
	}

	public void Morre ()
	{
		noAr = false;
		podePular = true;
		naParede = false;
		pulando	= false;
		movendo = false;
		pulandoMaisAlto = false;
		correndo = false;
		rotacionando = false;
	}

	public bool ApertandoParaParede()
	{
		return GetDirecao().Equals(direcaoDaParede);
	}

	public bool ApertandoParaOsLados()
	{
		return (GetDirecao().Equals(Definicoes.DIREITA) || GetDirecao().Equals(Definicoes.ESQUERDA));
	}

	public int GetDirecao()
	{
		switch(dirDeMovimento)
		{
		case DirDeMovimento.Direita:
			return Definicoes.DIREITA;
		case DirDeMovimento.Esquerda:
			return Definicoes.ESQUERDA;
		default:
			return Definicoes.IDLE;
		}
	}
}
