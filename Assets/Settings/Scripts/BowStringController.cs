using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BowStringController : MonoBehaviour
{
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

    private void PrepareBowString(SelectEnterEventArgs args)
    {
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

        OnBowReleased?.Invoke(strength);
        strength = 0f;

        // stop session tracking
        SessionTracker.Instance.OnBowRelease();

        currentHaptics = null;
        interactor = null;

        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;

        bowStringRenderer.CreateString(null);
    }

    private void Update()
    {
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
            else
                midPointVisualObject.position = midPointParent.TransformPoint(limitLocal);
        }

        TutorialController.Instance?.OnBowPulled(strength);
        bowStringRenderer.CreateString(midPointVisualObject.position);
    }
}
