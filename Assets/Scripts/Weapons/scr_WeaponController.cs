using UnityEngine;
using static Models;

public class scr_WeaponController : MonoBehaviour
{
    private scr_CharacterController characterController;

    [Header("Settings")]
    public WeaponSettingsModel settings;

    bool isInitialised;

    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;

    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
    }

    public void Initialise(scr_CharacterController CharacterController)
    {
        characterController = CharacterController;
        isInitialised = true;
    }

    private void Update()
    {
        if(!isInitialised)
        {
            return;
        }

        newWeaponRotation.y += settings.SwayAmount * (settings.SwayXInverted ? -characterController.input_View.x : characterController.input_View.x) * Time.deltaTime;
        newWeaponRotation.x += settings.SwayAmount * (settings.SwayYInverted ? characterController.input_View.y : -characterController.input_View.y) * Time.deltaTime;
        //newWeaponRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampYmin, viewClampYmax);

        transform.localRotation = Quaternion.Euler(newWeaponRotation);


    }

}
