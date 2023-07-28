using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour, IModel
{
    [SerializeField] private Transform headContainer;
    [SerializeField] private  Transform rightHandContainer;
    [SerializeField] private  Transform leftHandContainer;
    [SerializeField] private  Transform shieldContainer;
    [SerializeField] private  Transform headContainer;

    private Animator tempAnimator
    private GameObject headModel;
    private readonly List<GameObject> weaponModels = new List<GameObject>();

    public Animator TempAnimator { get { return tempAnimator; } }

    private void Start()
    {
        tempAnimator = GetComponent<Animator>();
    }

    public void SetHeadModel(GameObject model)
    {
        if (headModel != null)
            Destroy(headModel);
        headModel = AddModel(model, headContainer, null);
    }

    public void SetWeaponModel(GameObject rightHandModel, GameObject leftHandModel, GameObject shieldModel)
    {
        ClearGameObjects(weaponModels);
        AddModel(rightHandModel, rightHandContainer, weaponModels);
        AddModel(leftHandModel, leftHandContainer, weaponModels);
        AddModel(shieldModel, shieldContainer, weaponModels);
    }
}
