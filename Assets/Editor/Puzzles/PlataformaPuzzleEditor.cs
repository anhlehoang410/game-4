using UnityEngine;
using System.Collections;
using UnityEditor;


public class PlataformaPuzzleEditor : EditorWindow {

	public GameObject spawner;

	TipoDesignPlataformasEnum tipoDesignPlataforma;

	//Controle de itens do menu
	bool grupoHabilitaSpawn;
	//Local onde sera criada a nova plataforma
	public Vector3 spawnPoint;
	//Indica se a plataforma e movel ou nao
	public bool podeMover;
	//Velocidade de movimento
	public float velocidade;
	//Destino do deslocamento
	public Vector3 destino;
	//Prefab a ser criado
	private GameObject prefab;

	//Adiciona uma janela de menu com o nome "ConstrutorPlataforma" no Window Menu
	[MenuItem("Window/Platform Builder")]
	public static void ShowWindow(){
		//Mostra a instancia atual da janela. Caso ainda nao exista, cria uma
		EditorWindow.GetWindow(typeof(PlataformaPuzzleEditor), false, "Platforms");
	}

	void OnGUI(){

		//Cria novo objeto vazio na hierarquia para funcao de posicionar as plataformas
		if (spawner == null) {
			spawner = new GameObject("Platform Creator");
		}

		EditorGUILayout.LabelField("Type of Platform", EditorStyles.boldLabel);
		tipoDesignPlataforma = (TipoDesignPlataformasEnum)EditorGUILayout.EnumPopup ("Select:", tipoDesignPlataforma);
		EditorGUILayout.HelpBox ("Use Platform Creator object in hierarchy to set the position of the platform. " +
			"When you are ready, click in the button Create.", MessageType.Info);
		EditorGUILayout.Separator ();

		if (GUILayout.Button ("Create")) {
			string nomePrefab = null;

			switch(tipoDesignPlataforma)
			{
				case TipoDesignPlataformasEnum.PLATAFORMA_QUADRADA_1:
				nomePrefab = "PlataformaQuadrada_1";
				break;

				default:
				nomePrefab = null;
				break;
			}

			if(nomePrefab != null){
				GameObject prefabResource = (GameObject)Resources.Load("Prefabs/Plataformas/" + nomePrefab, typeof(GameObject));
				if(prefabResource != null){
					prefab = new GameObject();
					prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabResource);
					prefab.transform.position = spawner.transform.position;
				}
			}
		}
	}

	void OnDestroy(){
		//Destroi o objeto de posicionamento das plataformas
		DestroyImmediate (spawner);
	}

}
