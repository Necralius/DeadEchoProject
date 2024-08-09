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
		V1,
		V2,
		V3
	}

	public enum TopType
	{
		V1,
		V2,
		V3,
		V4

	}

	public enum BottomType
	{
		V1,
		V2,
		V3
	}

    public enum EyesGlow
    {
        No,
        Yes
    }

    public BodyType bodyType;
	public TopType topType;
	public BottomType bottomType;
    public EyesGlow eyesGlow;

    // Use this for initialization
    void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
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

		bodyTyp = (int)bodyType;
		topTyp = (int)topType;
		bottomTyp = (int)bottomType;
        eyesTyp = (int)eyesGlow;

        charCustomize (bodyTyp, topTyp, bottomTyp, eyesTyp);

	}
}
