using System;
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
    [SerializeField] private float bowStringStretchLimit = 0.3f;

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

        // Try to get haptics from controller, otherwise use fallback
        currentHaptics =
            args.interactorObject.transform.GetComponentInChildren<HapticSender>()
            ?? hapticsFallback;

        OnBowPulled?.Invoke();
    }

    private void ResetBowString(SelectExitEventArgs args)
    {
        // Strong haptic pulse on release
        currentHaptics?.SendHapticImpulse(1f, 0.12f);

        OnBowReleased?.Invoke(strength);
        strength = 0f;

        currentHaptics = null;
        interactor = null;

        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;

        bowStringRenderer.CreateString(null);
    }

    private void Update()
    {
        if (interactor == null) return;

        // Convert grab position into local space of the bow
        Vector3 grabLocal =
            midPointParent.InverseTransformPoint(midPointGrabObject.position);

        // Only allow backward pull on Z axis
        float localZ = Mathf.Clamp(grabLocal.z, -bowStringStretchLimit, 0f);
        float pullAbs = Mathf.Abs(localZ);

        // Compute strength (0..1)
        if (localZ < 0f && pullAbs > 0f)
            strength = Mathf.Clamp01(
                Remap(pullAbs, 0f, bowStringStretchLimit, 0f, 1f)
            );
        else
            strength = 0f;

        // Light continuous haptics while pulling
        if (currentHaptics != null && Time.time - lastPulseTime > 0.05f)
        {
            currentHaptics.SendHapticImpulse(
                Mathf.Clamp01(strength * 0.5f), 0.02f
            );
            lastPulseTime = Time.time;
        }

        // Move visual midpoint only along Z
        Vector3 targetLocal = new Vector3(0f, 0f, localZ);

        if (midPointVisualObject.parent == midPointParent)
            midPointVisualObject.localPosition = targetLocal;
        else
            midPointVisualObject.position =
                midPointParent.TransformPoint(targetLocal);

        // Enforce hard limit
        if (pullAbs >= bowStringStretchLimit)
        {
            strength = 1f;
            Vector3 limitLocal = new Vector3(0f, 0f, -bowStringStretchLimit);

            if (midPointVisualObject.parent == midPointParent)
                midPointVisualObject.localPosition = limitLocal;
            else
                midPointVisualObject.position =
                    midPointParent.TransformPoint(limitLocal);
        }

        // Update string renderer
        bowStringRenderer.CreateString(midPointVisualObject.position);
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (Mathf.Approximately(fromMax, fromMin)) return toMin;
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
