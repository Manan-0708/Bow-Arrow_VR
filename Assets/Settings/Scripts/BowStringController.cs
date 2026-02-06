using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class BowStringController : MonoBehaviour
{
    [SerializeField]
    private BowString bowStringRenderer;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable;

    [SerializeField]
    private Transform midPointGrabObject, midPointVisualObject, midPointParent;

    [SerializeField]
    private float bowStringStretchLimit = 0.3f;

    private Transform interactor;

    private float strength;

    public UnityEvent OnBowPulled;
    public UnityEvent<float> OnBowReleased;

    private void Awake()
    {
        interactable = midPointGrabObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    private void Start()
    {
        interactable.selectEntered.AddListener(PrepareBowString);
        interactable.selectExited.AddListener(ResetBowString);
    }

    private void ResetBowString(SelectExitEventArgs arg0)
    {
        OnBowReleased?.Invoke(strength);
        strength = 0;


        interactor = null;
        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;
        bowStringRenderer.CreateString(null);

    }

    private void PrepareBowString(SelectEnterEventArgs arg0)
    {
        interactor = arg0.interactorObject.transform;
        OnBowPulled?.Invoke();
    }

    private void Update()
    {
        if (interactor != null)
        {
            // compute pull along the midPointParent's forward axis (forward/back)
            float pull = Vector3.Dot(midPointGrabObject.position - midPointParent.position, midPointParent.forward);
            float pullAbs = Mathf.Abs(pull);

            HandleStringPushedBackToStart(pull);

            HandleStringPulledBackTolimit(pullAbs, pull);

            HandlePullingString(pullAbs, pull);

            // set visual midpoint in the local space of midPointParent along its forward axis
            midPointVisualObject.localPosition = new Vector3(0f, 0f, pull);
            bowStringRenderer.CreateString(midPointVisualObject.position);
        }
    }

    private void HandlePullingString(float midPointLocalZAbs, float pull)
    {
        // what happens when we are between point 0 and the string pull limit (pull < 0 means pulled backward)
        if (pull < 0f && midPointLocalZAbs < bowStringStretchLimit)
        {
            strength = Remap(midPointLocalZAbs, 0f, bowStringStretchLimit, 0f, 1f);
            midPointVisualObject.localPosition = new Vector3(0f, 0f, pull);
        }
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    private void HandleStringPulledBackTolimit(float midPointLocalZAbs, float pull)
    {
        // We specify max pulling limit for the string. We don't allow the string to go any farther than "bowStringStretchLimit"
        if (pull < 0f && midPointLocalZAbs >= bowStringStretchLimit)
        {
            strength = 1;
            midPointVisualObject.localPosition = new Vector3(0f, 0f, -bowStringStretchLimit);
        }
    }

    private void HandleStringPushedBackToStart(float pull)
    {
        if (pull >= 0f)
        {
            strength = 0;
            midPointVisualObject.localPosition = Vector3.zero;
        }
    }
}
