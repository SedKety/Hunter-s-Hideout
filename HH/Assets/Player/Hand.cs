using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class Hand : MonoBehaviour
{
    #region Hands static Instances Setup

    public static Hand Left;
    public static Hand Right;

    private void Awake()
    {
        if (handType == HandType.Left)
        {
            Left = this;
        }
        else
        {
            Right = this;
        }
    }

    public HandType handType;
    public enum HandType
    {
        Left,
        Right,
    };

    #endregion

    public InteractionController interactionController;

    private HapticImpulsePlayer hapticImpulsePlayer;


    private void Start()
    {
        hapticImpulsePlayer = GetComponent<HapticImpulsePlayer>();
    }




    [BurstCompile]
    public void SendVibration(float amplitude, float duration)
    {
        hapticImpulsePlayer.SendHapticImpulse(amplitude, duration);
    }

    [BurstCompile]
    public void SendVibration(VibrationParamaters vibrationParams)
    {
        if (vibrationParams.pulseCount > 1)
        {
            StartCoroutine(PulseVibration(vibrationParams.amplitude, vibrationParams.duration, vibrationParams.pulseCount, vibrationParams.pulseInterval));

            return;
        }

        hapticImpulsePlayer.SendHapticImpulse(vibrationParams.amplitude, vibrationParams.duration);
    }


    private IEnumerator PulseVibration(float amplitude, float duration, int pulseCount, float pulseInterval)
    {
        WaitForSeconds waitPulseInterval = new WaitForSeconds(duration + pulseInterval);

        for (int i = 0; i < pulseCount; i++)
        {
            hapticImpulsePlayer.SendHapticImpulse(amplitude, duration);

            yield return waitPulseInterval;
        }
    }
}


[System.Serializable]
public struct VibrationParamaters
{
    public VibrationParamaters(float _amplitude, float _duration, int _pulseCount = 1, float _pulseInterval = 0)
    {
        amplitude = _amplitude;
        duration = _duration;

        pulseCount = _pulseCount;
        pulseInterval = _pulseInterval;
    }

    [Header("Vibration Strength"), Range(0f, 1f)]
    public float amplitude;

    [Header("Duration off a vibration")]
    public float duration;

    [Header("Amount of times the controller vibrates"), Range(1, 16)]
    public int pulseCount;

    [Header("Delay between vibrations (after it finished)")]
    public float pulseInterval;
}
