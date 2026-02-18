using System;
<<<<<<< HEAD
using Unity.VisualScripting;
=======
using System.Collections;
using System.Collections.Generic;
>>>>>>> parent of 19e44bc (Project setup again)
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BowStringController : MonoBehaviour
{
<<<<<<< HEAD
    [Header("References")]
    [SerializeField] private BowString bowStringRenderer;
    [SerializeField] private Transform midPointGrabObject;
    [SerializeField] private Transform midPointVisualObject;
    [SerializeField] private Transform midPointParent;

    [Header("Settings")]
    [SerializeField] private float bowStringStretchLimit = 0.6f;

    [Header("Haptics")]
    [SerializeField] private HapticSender hapticsFallback;
    private HapticSender currentHaptics;
    private float lastPulseTime;

    private XRGrabInteractable interactable;
=======
    [SerializeField]
    private BowString bowStringRenderer;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable;

    [SerializeField]
    private Transform midPointGrabObject, midPointVisualObject, midPointParent;

    [SerializeField]
    private float bowStringStretchLimit = 0.3f;

>>>>>>> parent of 19e44bc (Project setup again)
    private Transform interactor;

    private float strength;

    public UnityEvent OnBowPulled;
    public UnityEvent<float> OnBowReleased;

    private void Awake()
    {
        interactable = midPointGrabObject.GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        interactable.selectEntered.AddListener(PrepareBowString);
        interactable.selectExited.AddListener(ResetBowString);
    }

    private void ResetBowString(SelectExitEventArgs arg0)
    {
<<<<<<< HEAD
        interactor = args.interactorObject.transform;

        currentHaptics =
            args.interactorObject.transform.GetComponentInChildren<HapticSender>()
            ?? hapticsFallback;

        OnBowPulled?.Invoke();

        // Session tracking + tutorial
        SessionTracker.Instance.OnBowGrab();
        TutorialController.Instance?.OnBowGrab();
    }

    private void ResetBowString(SelectExitEventArgs args)
    {
        // Strong release pulse
        currentHaptics?.SendHapticImpulse(1f, 0.12f);

=======
>>>>>>> parent of 19e44bc (Project setup again)
        OnBowReleased?.Invoke(strength);
        strength = 0;


<<<<<<< HEAD
        // stop session tracking
        SessionTracker.Instance.OnBowRelease();

        currentHaptics = null;
=======
>>>>>>> parent of 19e44bc (Project setup again)
        interactor = null;

        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;

<<<<<<< HEAD
        bowStringRenderer.CreateString(null);
=======
    }

    private void PrepareBowString(SelectEnterEventArgs arg0)
    {
        interactor = arg0.interactorObject.transform;
        OnBowPulled?.Invoke();
>>>>>>> parent of 19e44bc (Project setup again)
    }

    private void Update()
    {
<<<<<<< HEAD
        if (interactor == null) return;

        Vector3 grabLocal =
            midPointParent.InverseTransformPoint(midPointGrabObject.position);

        float localZ = Mathf.Clamp(grabLocal.z, -bowStringStretchLimit, 0f);
        float pullAbs = Mathf.Abs(localZ);

        // quadratic strength curve (better feel)
        if (localZ < 0f && pullAbs > 0f)
        {
            float normalized = Mathf.Clamp01(pullAbs / bowStringStretchLimit);
            strength = normalized * normalized;
        }
        else
            strength = 0f;

        // continuous haptics while pulling
        if (currentHaptics != null && Time.time - lastPulseTime > 0.05f)
        {
            currentHaptics.SendHapticImpulse(Mathf.Clamp01(strength * 0.5f), 0.02f);
            lastPulseTime = Time.time;
        }

        Vector3 targetLocal = new Vector3(0f, 0f, localZ);

        if (midPointVisualObject.parent == midPointParent)
            midPointVisualObject.localPosition = targetLocal;
        else
            midPointVisualObject.position = midPointParent.TransformPoint(targetLocal);

        if (pullAbs >= bowStringStretchLimit)
        {
            strength = 1f;
            Vector3 limitLocal = new Vector3(0f, 0f, -bowStringStretchLimit);

            if (midPointVisualObject.parent == midPointParent)
                midPointVisualObject.localPosition = limitLocal;
=======
        if (interactor != null)
        {
            // get grab object's position in midPointParent local space
            Vector3 grabLocal = midPointParent.InverseTransformPoint(midPointGrabObject.position);

            // allow only backward pulls: clamp local Z to [-bowStringStretchLimit, 0]
            float localZ = Mathf.Clamp(grabLocal.z, -bowStringStretchLimit, 0f);
            float pullAbs = Mathf.Abs(localZ);

            // compute strength
            if (localZ < 0f && pullAbs > 0f)
            {
                strength = Mathf.Clamp01(Remap(pullAbs, 0f, bowStringStretchLimit, 0f, 1f));
            }
>>>>>>> parent of 19e44bc (Project setup again)
            else
            {
                strength = 0f;
            }

            // target in midPointParent local space (force X/Y = 0 so no lateral movement)
            Vector3 targetLocal = new Vector3(0f, 0f, localZ);

            // if visual midpoint is a child of the parent, set localPosition; otherwise set world position via TransformPoint
            if (midPointVisualObject.parent == midPointParent)
            {
                midPointVisualObject.localPosition = targetLocal;
            }
            else
            {
                midPointVisualObject.position = midPointParent.TransformPoint(targetLocal);
            }

            // enforce absolute limit defensively
            if (pullAbs >= bowStringStretchLimit)
            {
                strength = 1f;
                Vector3 limitLocal = new Vector3(0f, 0f, -bowStringStretchLimit);
                if (midPointVisualObject.parent == midPointParent)
                    midPointVisualObject.localPosition = limitLocal;
                else
                    midPointVisualObject.position = midPointParent.TransformPoint(limitLocal);
            }

            bowStringRenderer.CreateString(midPointVisualObject.position);
        }
    }
<<<<<<< HEAD
=======

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (Mathf.Approximately(fromMax, fromMin)) return toMin;
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
>>>>>>> parent of 19e44bc (Project setup again)
}
