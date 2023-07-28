using UnityEngine;

public interface IModel
{
    Animator TempAnimator { get; }

    void SetHeadModel(GameObject model);
    void SetWeaponModel(GameObject rightHandModel, GameObject leftHandModel, GameObject shieldModel);
}
