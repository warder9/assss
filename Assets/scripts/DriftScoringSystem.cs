using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class DriftScoringSystem : MonoBehaviour
{
    [Header("Drift Settings")]
    public float minDriftAngle = 15f;
    public float maxDriftAngle = 60f;
    public float minSpeed = 10f;
    public float pointsPerSecond = 50f;
    public float multiplierGrowth = 0.2f;
    public float maxMultiplier = 5f;

    [Header("UI References")]
    public Text driftPointsText;
    public Text driftMultiplierText;
    public GameObject driftIndicator;

    // Private variables
    private Rigidbody carRigidbody;
    private PrometeoCarController carController;
    private float currentDriftTime;
    private float currentDriftPoints;
    private float totalDriftPoints;
    private float currentMultiplier = 1f;
    private bool isDrifting;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<PrometeoCarController>();

        // Initialize UI
        UpdateDriftUI();
        if (driftIndicator != null)
            driftIndicator.SetActive(false);
    }

    void Update()
    {
        if (carController == null) return;

        // Calculate drift angle (between velocity and car's forward direction)
        Vector3 localVelocity = transform.InverseTransformDirection(carRigidbody.linearVelocity);
        float driftAngle = Mathf.Abs(Mathf.Atan2(localVelocity.x, localVelocity.z) * Mathf.Rad2Deg);

        // Check if drifting conditions are met
        bool isDriftingNow = driftAngle > minDriftAngle &&
                            carRigidbody.linearVelocity.magnitude > minSpeed &&
                            carController.isDrifting; // Use the original script's drift detection

        // Handle drift scoring
        if (isDriftingNow)
        {
            currentDriftTime += Time.deltaTime;
            CalculateDriftPoints(driftAngle);
            if (!isDrifting) StartDrift();
        }
        else if (isDrifting)
        {
            EndDrift();
        }
    }

    void CalculateDriftPoints(float driftAngle)
    {
        // Increase multiplier over time
        currentMultiplier = Mathf.Min(currentMultiplier + (multiplierGrowth * Time.deltaTime), maxMultiplier);

        // Points based on angle and speed
        float angleFactor = Mathf.Clamp01((driftAngle - minDriftAngle) / (maxDriftAngle - minDriftAngle));
        currentDriftPoints = currentDriftTime * pointsPerSecond * currentMultiplier * (1f + angleFactor);

        UpdateDriftUI();
    }

    void StartDrift()
    {
        isDrifting = true;
        if (driftIndicator != null)
            driftIndicator.SetActive(true);
    }

    void EndDrift()
    {
        isDrifting = false;
        totalDriftPoints += currentDriftPoints;
        currentDriftPoints = 0f;
        currentDriftTime = 0f;
        currentMultiplier = 1f;
        UpdateDriftUI();
        if (driftIndicator != null)
            driftIndicator.SetActive(false);
    }

    void UpdateDriftUI()
    {
        if (driftPointsText != null)
            driftPointsText.text = "Drift: " + Mathf.RoundToInt(totalDriftPoints + currentDriftPoints);

        if (driftMultiplierText != null)
            driftMultiplierText.text = "x" + currentMultiplier.ToString("F1");
    }

    
}