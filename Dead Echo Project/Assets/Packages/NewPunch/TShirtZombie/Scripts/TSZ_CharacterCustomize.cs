using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSZ_CharacterCustomize : MonoBehaviour
{
	private int bodyTyp;
	private int topTyp;
	private int bottomTyp;
    private int eyesTyp;

    private TSZ_AssetsList materialsList;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	public enum BodyType
	{
		V1 = 0,
		V2 = 1,
		V3 = 2
	}

	public enum TopType
	{
		V1 = 0,
		V2 = 1,
		V3 = 2,
		V4 = 3

	}

	public enum BottomType
	{
		V1 = 0,
		V2 = 1,
		V3 = 2
	}

    public enum EyesGlow
    {
        No = 0,
        Yes = 1
    }

    public BodyType bodyType;
	public TopType topType;
	public BottomType bottomType;
    public EyesGlow eyesGlow;

    void Start ()
	{
        RandomizeZombie();
    }
	
	void Update ()
	{
		
	}

	private void RandomizeZombie()
	{
        bodyType	= GetRandomEnumValue<BodyType>();
        topType		= GetRandomEnumValue<TopType>();
        bottomType	= GetRandomEnumValue<BottomType>();
        eyesGlow	= GetRandomEnumValue<EyesGlow>();

        charCustomize(bodyTyp, topTyp, bottomTyp, eyesTyp);
    }

    private T GetRandomEnumValue<T>()
    {
        // Obter todos os valores do enumerador
        Array enumValues = Enum.GetValues(typeof(T));
        // Selecionar um índice aleatório
        int randomIndex = UnityEngine.Random.Range(0, enumValues.Length);
        // Retornar o valor correspondente ao índice aleatório
        return (T)enumValues.GetValue(randomIndex);
    }

    public void charCustomize (int body, int top, int bottom, int eyes)
	{
		materialsList = gameObject.GetComponent<TSZ_AssetsList> ();
		// Set Body Type
//		
		if (body == 2) {
			materialsList.HairObject.SetActive (false);

		} else {
			materialsList.HairObject.SetActive (true);
		}
		foreach (Transform child in materialsList.BodyObject.transform) {

			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.BodyMaterials [body];
		}

		// Set Top Type
		foreach (Transform child in materialsList.TopObject.transform) {
			//print ("Foreach loop: " + child);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.TopMaterials [top];
		}
		// Set Bottom Type
		foreach (Transform child in materialsList.BottomObject.transform) {
			//print ("Foreach loop: " + child);
			Renderer skinRend = child.gameObject.GetComponent<Renderer> ();
			skinRend.material = materialsList.BottomMaterials [bottom];
		}

        if (eyes == 0)
        {
            materialsList.BodyMaterials[body].DisableKeyword("_EMISSION");
            materialsList.BodyMaterials[body].SetFloat("_EmissiveExposureWeight", 1);
        }
        else
        {
            materialsList.BodyMaterials[body].EnableKeyword("_EMISSION");
            materialsList.BodyMaterials[body].SetFloat("_EmissiveExposureWeight", 0);
        }
    }

	void OnValidate ()
	{
		//code for In Editor customize

		bodyTyp		= (int)bodyType;
		topTyp		= (int)topType;
		bottomTyp	= (int)bottomType;
        eyesTyp		= (int)eyesGlow;

        charCustomize (bodyTyp, topTyp, bottomTyp, eyesTyp);
	}

    private void OnEnable()
    {
        RandomizeZombie();
    }
}
