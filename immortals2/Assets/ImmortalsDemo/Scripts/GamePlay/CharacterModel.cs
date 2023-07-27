using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    [SerializeField] private Transform headContainer;

    private Animator tempAnimator
    private GameObject headModel;
    private readonly List<GameObject> weaponModels = new List<GameObject>();

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
}
