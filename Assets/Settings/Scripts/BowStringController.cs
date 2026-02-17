using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class BowStringController : MonoBehaviour
{
    [SerializeField] private BowString bowStringRenderer;
    [SerializeField] private Transform midPointGrabObject;
    [SerializeField] private Transform midPointVisualObject;
    [SerializeField] private Transform midPointParent;
    [SerializeField] private float bowStringStretchLimit = 0.6f;

    // Haptics
    [SerializeField] private HapticSender hapticsFallback;
    private HapticSender currentHaptics;
    private float lastPulseTime;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable;
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

    private void PrepareBowString(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject.transform;

        currentHaptics = args.interactorObject.transform.GetComponentInChildren<HapticSender>() ?? hapticsFallback;

        OnBowPulled?.Invoke();

        // ✅ NEW: Start tracking bow hold time
        SessionTracker.Instance.OnBowGrab();
        TutorialController.Instance?.OnBowGrab();

    }

    private void ResetBowString(SelectExitEventArgs args)
    {


        currentHaptics?.SendHapticImpulse(1f, 0.12f);

        OnBowReleased?.Invoke(strength);
        strength = 0f;

        currentHaptics = null;
        interactor = null;
        midPointGrabObject.localPosition = Vector3.zero;
        midPointVisualObject.localPosition = Vector3.zero;
        bowStringRenderer.CreateString(null);

        // ✅ NEW: Stop tracking bow hold time
        SessionTracker.Instance.OnBowRelease();
    }

    private void Update()
    {
        if (interactor == null) return;

        Vector3 grabLocal = midPointParent.InverseTransformPoint(midPointGrabObject.position);

        float localZ = Mathf.Clamp(grabLocal.z, -bowStringStretchLimit, 0f);
        float pullAbs = Mathf.Abs(localZ);

        if (localZ < 0f && pullAbs > 0f)
        {
            float normalized = Mathf.Clamp01(pullAbs / bowStringStretchLimit);
            strength = normalized * normalized; // quadratic curve

        }


        else
            strength = 0f;

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

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (Mathf.Approximately(fromMax, fromMin)) return toMin;
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }


}
