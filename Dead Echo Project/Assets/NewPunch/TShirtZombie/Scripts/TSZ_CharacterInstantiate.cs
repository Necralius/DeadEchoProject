using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSZ_CharacterInstantiate : MonoBehaviour
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

    public Transform prefabObject;
	public BodyType bodyType;
	public TopType topType;
	public BottomType bottomType;
    public EyesGlow eyesGlow;

    // Use this for initialization
    void Start ()
	{
		Transform pref = Instantiate (prefabObject, gameObject.transform.position, gameObject.transform.rotation);
		bodyTyp = (int)bodyType;
		topTyp = (int)topType;
		bottomTyp = (int)bottomType;
        eyesTyp = (int)eyesGlow;

        pref.gameObject.GetComponent<TSZ_CharacterCustomize> ().charCustomize (bodyTyp, topTyp, bottomTyp, eyesTyp);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


}
