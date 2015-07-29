using UnityEngine;
using System.Collections;

public class Controles : MonoBehaviour {
	Estados estados;
	Player player;
	Movimento movimento;
	public RotacionaCamera cam;

	float inputHorizontal;


	public bool log;
	// Use this for initialization
	void Start () 
	{
		estados = GetComponent<Estados>();
		player = GetComponent<Player>();
		movimento = GetComponent<Movimento>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		inputHorizontal = Input.GetAxis("Horizontal");

		if(inputHorizontal > 0)
			estados.Direita();
		else if(inputHorizontal < 0)
			estados.Esquerda();
		else
			estados.Idle();
		
		if(Input.GetButtonDown("Jump") && estados.podePular)
		{
			estados.Pula();
		}else if(Input.GetButtonUp("Jump"))
		{
			if(log)
				Debug.Log("Soltou botao");
			estados.DesativaPuloMaisAlto();
		}
		
		if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
		{
			movimento.DefineNovoEixo();
			player.LimpaUltimaPlataformaColidida();
		}
		
		MudaVelocidadeDoJogo();
		ChecaCamera();
	}


	void MudaVelocidadeDoJogo ()
	{
		if(Input.GetKey(KeyCode.Tab))
			Time.timeScale = 0.1f;
		else if(Input.GetKey(KeyCode.Q))
			Time.timeScale = 3f;
		else
			Time.timeScale = 1f;
	}

	void ChecaCamera ()
	{
		if(Input.GetButton("MostraOutraCamera") ||
		   Input.GetAxis("MostraOutraCamera") != 0)
		{
			cam.MostraOutraCamera();
		}else
		{
			cam.EscondeOutraCamera();
		}

		estados.rotacionando = cam.Rotacionando();
	}
}
