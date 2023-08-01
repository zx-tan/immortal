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

    private Animator tempAnimator;
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

    public void SetEquipmentModel(GameObject rightHandModel, GameObject leftHandModel, GameObject shieldModel)
    {
        ClearGameObjects(weaponModels);
        AddModel(rightHandModel, rightHandContainer, weaponModels);
        AddModel(leftHandModel, leftHandContainer, weaponModels);
        AddModel(shieldModel, shieldContainer, weaponModels);
    }

    private void ClearGameObjects(List<GameObject> list)
    {
        foreach (var entry in list)
            Destroy(entry);
        list.Clear();
    }

    private GameObject AddModel(GameObject model, Transform transform, List<GameObject> list)
    {
        if (model == null)
            return null;
        var newModel = InstantiateModel(model, transform);
        if (list != null)
            list.Add(newModel);
        return newModel;
    }

    private GameObject InstantiateModel(GameObject model, Transform transform)
    {
        var newModel = Instantiate(model);
        newModel.transform.parent = transform;
        newModel.transform.localPosition = Vector3.zero;
        newModel.transform.localEulerAngles = Vector3.zero;
        newModel.transform.localScale = Vector3.one;
        newModel.gameObject.SetActive(true);

        return newModel;
    }
}
