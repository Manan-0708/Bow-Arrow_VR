using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class BowStringController : MonoBehaviour
{
    [SerializeField] private BowString bowStringRenderer;
    [SerializeField] private Transform midPointGrabObject;
    [SerializeField] private Transform midPointVisualObject;
    [SerializeField] private Transform midPointParent;
    [SerializeField] private float bowStringStretchLimit = 0.3f;

    // Haptics
    [SerializeField] private HapticSender hapticsFallback;
    private HapticSender currentHaptics;
    private float lastPulseTime;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable;
<<<<<<< HEAD

    [SerializeField]
    private Transform midPointGrabObject, midPointVisualObject, midPointParent;

    [SerializeField]
    private float bowStringStretchLimit = 0.3f;

=======
>>>>>>> ac3cd72d3309f67a6c8613bbd632811f9ed8f144
    private Transform interactor;
    private float strength;

    public UnityEvent OnBowPulled;
    public UnityEvent<float> OnBowReleased;

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

    private void PrepareBowString(SelectEnterEventArgs args)
    {
<<<<<<< HEAD
        OnBowReleased?.Invoke(strength);
        strength = 0;


=======
        interactor = args.interactorObject.transform;
        // prefer HapticSender on the interactor/controller, otherwise use fallback
        currentHaptics = args.interactorObject.transform.GetComponentInChildren<HapticSender>() ?? hapticsFallback;
        OnBowPulled?.Invoke();
    }

    private void ResetBowString(SelectExitEventArgs args)
    {
        // strong haptic pulse on release
        currentHaptics?.SendHapticImpulse(1f, 0.12f);

        OnBowReleased?.Invoke(strength);
        strength = 0f;

        currentHaptics = null;
>>>>>>> ac3cd72d3309f67a6c8613bbd632811f9ed8f144
        interactor = null;
        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;
        bowStringRenderer.CreateString(null);
<<<<<<< HEAD

    }

    private void PrepareBowString(SelectEnterEventArgs arg0)
    {
        interactor = arg0.interactorObject.transform;
        OnBowPulled?.Invoke();
=======
>>>>>>> ac3cd72d3309f67a6c8613bbd632811f9ed8f144
    }

    private void Update()
    {
        if (interactor == null) return;

        // get grab object's position in midPointParent local space
        Vector3 grabLocal = midPointParent.InverseTransformPoint(midPointGrabObject.position);

        // allow only backward pulls: clamp local Z to [-bowStringStretchLimit, 0]
        float localZ = Mathf.Clamp(grabLocal.z, -bowStringStretchLimit, 0f);
        float pullAbs = Mathf.Abs(localZ);

        // compute strength [0..1]
        if (localZ < 0f && pullAbs > 0f)
            strength = Mathf.Clamp01(Remap(pullAbs, 0f, bowStringStretchLimit, 0f, 1f));
        else
            strength = 0f;

        // small throttled haptic while holding (scale amplitude by strength)
        if (currentHaptics != null && Time.time - lastPulseTime > 0.05f)
        {
<<<<<<< HEAD
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
=======
            currentHaptics.SendHapticImpulse(Mathf.Clamp01(strength * 0.5f), 0.02f);
            lastPulseTime = Time.time;
>>>>>>> ac3cd72d3309f67a6c8613bbd632811f9ed8f144
        }

        // target in midPointParent local space (force X/Y = 0 so no lateral movement)
        Vector3 targetLocal = new Vector3(0f, 0f, localZ);

        if (midPointVisualObject.parent == midPointParent)
            midPointVisualObject.localPosition = targetLocal;
        else
            midPointVisualObject.position = midPointParent.TransformPoint(targetLocal);

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

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (Mathf.Approximately(fromMax, fromMin)) return toMin;
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
