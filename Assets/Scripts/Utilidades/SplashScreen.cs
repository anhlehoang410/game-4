using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	public float tempoFadeInProxFase = 1;
	public float tempoExibindo = 3;
	public float tempoFadeOut = 0.5f;
	public Color corFade;
	
	public string proxFase;
	
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(Executa());
	}
	
	public IEnumerator Executa ()
	{
		yield return new WaitForSeconds(tempoExibindo);
		AutoFade.LoadLevel(proxFase, tempoFadeOut, tempoFadeInProxFase, corFade);
	}
}
