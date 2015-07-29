using UnityEngine;
using System.Collections;

public class RandomizaVelDeAnimacao : MonoBehaviour {
	public Vector2 range = new Vector2(0, 1);
	public bool randomizaTempo;
	
	void Awake () 
	{
		if(GetComponent<Animation>())
		{
			foreach(AnimationState a in GetComponent<Animation>())
			{
				if(randomizaTempo)
					a.time = Random.Range(0, a.length);
				a.speed = Random.Range(a.speed * range.x, a.speed * range.y);	
			}
		}else
		{
			Animator anim = GetComponent<Animator>();
			if(anim)
			{
				anim.speed = Random.Range(anim.speed * range.x, anim.speed * range.y);
			}
		}
		Destroy(this, Random.Range(0, 2));
	}
}
