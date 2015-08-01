using UnityEngine;
using System.Collections;

public class ObjetoDeJogo : MonoBehaviour {
	
	protected Transform tr;

	public Vector3 ultimaPos;
	public Transform ultimaPlataformaColidida;
	
	public RotacionaCamera rotacionaCamera;
	public RotacionaCamera.RotacaoAtual ultimaRotacao;
	public Vector3 eixo = Vector3.one;

	public float delayParaChecar = 0.15f;
	public float profundidadeDeChecagem = 100;
	
	public Transform trBaseM;
	public Transform trBaseD;
	public Transform trBaseE;
	public Transform trMeio;
	public Transform trCentroD;
	public Transform trCentroE;
	public Transform trTopoM;
	public Transform trTopoD;
	public Transform trTopoE;
	
	public LayerMask layerPlataforma;
	public LayerMask layerParede;

	protected virtual void Start()
	{
		tr = transform;

		if(!rotacionaCamera)
		{
			Debug.LogWarning("Nenhuma referencia para RotacionaCamera Encontrada", tr);
			enabled = false;
		}
		StartCoroutine(AtualizaUltimaPosicao());
	}

	protected virtual void Update()
	{
//		ultimaRotacao = rotacionaCamera.rotacaoAtual;
		rotacionaCamera.AtualizaRotacaoAlvo();
	}

	protected virtual IEnumerator AtualizaUltimaPosicao ()
	{
		while(true)
		{
			yield return new WaitForEndOfFrame();
			if(VerificaChao() && !rotacionaCamera.Rotacionando())
			{
				ultimaPos = tr.position;
				ultimaRotacao = rotacionaCamera.rotacaoAtual;
			}
			yield return new WaitForSeconds(delayParaChecar);
		}
	}

	public virtual bool VerificaChao()
	{
		Debug.DrawLine(trBaseM.position, trBaseM.position - trBaseM.up * 0.25f);
		Debug.DrawLine(trBaseD.position, trBaseD.position - trBaseD.up * 0.25f);
		Debug.DrawLine(trBaseE.position, trBaseE.position - trBaseE.up * 0.25f);
		
		if (Physics.Linecast (trBaseM.position, trBaseM.position - trBaseM.up * 0.25f, layerPlataforma) &&

		    (Physics.Linecast (trBaseD.position, trBaseD.position - trBaseD.up * 0.25f, layerPlataforma) ||	
		 	Physics.Linecast (trBaseE.position, trBaseE.position - trBaseE.up * 0.25f, layerPlataforma))) 
		{
			return true;
		}else
		{
			return false;
		}		
	}

	public void LimpaUltimaPlataformaColidida()
	{
		ultimaPlataformaColidida = null;
	}

	public void ResetaRotacaoDeCamera()
	{
		rotacionaCamera.rotacaoAtual = ultimaRotacao;
		rotacionaCamera.AtualizaRotacaoAlvo();
	}
}
