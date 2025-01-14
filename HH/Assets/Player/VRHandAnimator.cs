using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRHandAnimator : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private float controllerButtonPressPercent;
    [SerializeField] private float _cButtonPressPercent;
    public float valueUpdateSpeed;

    private Vector3 localPos;
    [HideInInspector]
    public Quaternion localRot;


    private void Start()
    {
        anim = GetComponent<Animator>();

        localPos = transform.localPosition;
        localRot = transform.localRotation;

    }
    public void Update()
    {
        _cButtonPressPercent = Mathf.MoveTowards(_cButtonPressPercent, controllerButtonPressPercent, valueUpdateSpeed * Time.deltaTime);
        anim.SetFloat("GrabStrength", _cButtonPressPercent);
    }

    public void OnBigTriggerStateChange(InputAction.CallbackContext ctx)
    {
        controllerButtonPressPercent = ctx.ReadValue<float>();
    }



    public void UpdateHandTransform(Vector3 pos, Quaternion rot, bool flipHand)
    {
        Quaternion targetRot = rot;
        if (flipHand)
        {
            targetRot *= Quaternion.Euler(0, 0, 180);
        }

        transform.SetPositionAndRotation(pos, targetRot);
    }

    public void ResetHandTransform()
    {
        transform.SetLocalPositionAndRotation(localPos, localRot);
    }
}
