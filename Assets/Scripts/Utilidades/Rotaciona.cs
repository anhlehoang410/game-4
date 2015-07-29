using UnityEngine;
using System.Collections;

public class Rotaciona : MonoBehaviour {
	public bool randomico;
	public bool local;
	public Vector3 eixo;
	private Vector3 eixoOriginal;
	public float velocidade;
	private float r;
	public float delay = 0;
	// Use this for initialization
	public virtual void Start () 
	{
		eixoOriginal = eixo;
		if(!randomico)
			return;
		float r = Random.Range(0.0f, 1.0f);
		eixo = new Vector3(r*eixoOriginal.x, r*eixoOriginal.y, r*eixoOriginal.z);
	}
	
	// Update is called once per frame
    public virtual void Update() 
	{
		if(local)
			transform.RotateAroundLocal(eixo.x == 1 ? transform.right :
			                            eixo.y == 1 ? transform.up :
			                            eixo.z == 1 ? transform.forward :
			                            eixo.x == -1 ? -transform.right :
			                            eixo.y == -1 ? -transform.up :
			                            -transform.forward,
                    velocidade * Time.deltaTime);
	   	else
			transform.Rotate(eixo, velocidade * Time.deltaTime);
//		delay = Mathf.Clamp(delay - Time.deltaTime, 0, Mathf.Infinity);
//		if(delay != 0)
//			return;
//		delay = Random.Range(1, 5);
		if(randomico)
		{
			eixo = new Vector3(r*eixoOriginal.x, r*eixoOriginal.y, r*eixoOriginal.z);
		}
		
	}
}
