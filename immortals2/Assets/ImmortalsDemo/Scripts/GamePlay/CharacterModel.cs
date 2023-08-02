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

    private Animator _animator;
    private GameObject _headModel;
    private readonly List<GameObject> _weaponModels = new List<GameObject>();

    public Animator Animator { get { return _animator; } }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetHeadModel(GameObject model)
    {
        if (_headModel != null)
            Destroy(_headModel);
        _headModel = AddModel(model, headContainer, null);
    }

    public void SetEquipmentModel(GameObject rightHandModel, GameObject leftHandModel, GameObject shieldModel)
    {
        ClearGameObjects(_weaponModels);
        AddModel(rightHandModel, rightHandContainer, _weaponModels);
        AddModel(leftHandModel, leftHandContainer, _weaponModels);
        AddModel(shieldModel, shieldContainer, _weaponModels);
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
