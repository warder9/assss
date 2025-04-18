using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PrometeoCarController))]
public class AICarController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform playerCar;
    public float followDistance = 15f;
    public float maxSpeed = 80f;
    public float minSpeed = 20f;

    [Header("Steering Behavior")]
    public float steeringSharpness = 2f;
    public float avoidanceDistance = 8f;
    public LayerMask obstacleLayers;
    public float predictionTime = 1f;

    [Header("Advanced Settings")]
    public float accelerationSensitivity = 0.5f;
    public float brakeSensitivity = 1f;
    public float corneringSpeedFactor = 0.7f;

    // Private variables
    private PrometeoCarController carController;
    private Rigidbody rb;
    private float currentThrottle;
    private float currentSteering;
    private Vector3 targetPosition;
    private Vector3 avoidanceVector;

    void Start()
    {
        carController = GetComponent<PrometeoCarController>();
        rb = GetComponent<Rigidbody>();

        // Disable player input for this car
        carController.useTouchControls = false;

        if (playerCar == null)
        {
            playerCar = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void FixedUpdate()
    {
        if (playerCar == null) return;

        CalculateTargetPosition();
        CalculateAvoidance();
        ApplyDrivingLogic();
        UpdateWheelVisuals();
    }

    void CalculateTargetPosition()
    {
        // Predict player's future position
        Rigidbody playerRB = playerCar.GetComponent<Rigidbody>();
        Vector3 predictedPosition = playerCar.position + (playerRB.linearVelocity * predictionTime);

        // Calculate ideal follow position (behind player)
        targetPosition = predictedPosition - (playerCar.forward * followDistance);

        // Add some lateral offset to prevent perfect alignment
        targetPosition += playerCar.right * Mathf.Sin(Time.time * 0.5f) * 2f;
    }

    void CalculateAvoidance()
    {
        avoidanceVector = Vector3.zero;

        // Front collision check
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit,
                          avoidanceDistance, obstacleLayers))
        {
            avoidanceVector += hit.normal * 5f;
        }

        // Side collision checks
        Vector3[] rayDirections = {
            transform.right * 0.5f,
            -transform.right * 0.5f,
            (transform.right + transform.forward * 0.5f).normalized,
            (-transform.right + transform.forward * 0.5f).normalized
        };

        foreach (Vector3 dir in rayDirections)
        {
            if (Physics.Raycast(transform.position, dir, out hit,
                              avoidanceDistance * 0.7f, obstacleLayers))
            {
                avoidanceVector -= dir.normalized * 3f;
            }
        }
    }

    void ApplyDrivingLogic()
    {
        Vector3 localTarget = transform.InverseTransformPoint(targetPosition + avoidanceVector);
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float currentSpeed = rb.linearVelocity.magnitude;

        // Calculate throttle (speed control)
        float speedFactor = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
        float targetThrottle = 0f;

        if (distanceToTarget > followDistance * 0.8f)
        {
            // Accelerate if too far
            targetThrottle = 1f - (speedFactor * accelerationSensitivity);
        }
        else if (distanceToTarget < followDistance * 0.5f)
        {
            // Brake if too close
            targetThrottle = -brakeSensitivity;
        }

        currentThrottle = Mathf.Lerp(currentThrottle, targetThrottle, Time.fixedDeltaTime * 3f);

        // Calculate steering (direction control)
        float targetSteering = (localTarget.x / Mathf.Max(1f, localTarget.z)) * steeringSharpness;
        targetSteering = Mathf.Clamp(targetSteering, -1f, 1f);

        // Reduce steering at high speeds for stability
        float speedAdjustedSteering = targetSteering * (1f - (currentSpeed / maxSpeed * corneringSpeedFactor));
        currentSteering = Mathf.Lerp(currentSteering, speedAdjustedSteering, Time.fixedDeltaTime * 5f);

        // Apply physics controls
        ApplyMotorTorque();
        ApplySteering();
        ApplyBrakes();
    }

    void ApplyMotorTorque()
    {
        float torque = carController.accelerationMultiplier * 50f * currentThrottle;

        carController.frontLeftCollider.motorTorque = torque;
        carController.frontRightCollider.motorTorque = torque;
        carController.rearLeftCollider.motorTorque = torque;
        carController.rearRightCollider.motorTorque = torque;
    }

    void ApplySteering()
    {
        float steeringAngle = currentSteering * carController.maxSteeringAngle;

        carController.frontLeftCollider.steerAngle = Mathf.Lerp(
            carController.frontLeftCollider.steerAngle,
            steeringAngle,
            carController.steeringSpeed);

        carController.frontRightCollider.steerAngle = Mathf.Lerp(
            carController.frontRightCollider.steerAngle,
            steeringAngle,
            carController.steeringSpeed);
    }

    void ApplyBrakes()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerCar.position);
        bool shouldBrake = distanceToPlayer < followDistance * 0.6f && rb.linearVelocity.magnitude > 10f;

        float brakeTorque = shouldBrake ? carController.brakeForce : 0f;

        carController.frontLeftCollider.brakeTorque = brakeTorque;
        carController.frontRightCollider.brakeTorque = brakeTorque;
        carController.rearLeftCollider.brakeTorque = brakeTorque;
        carController.rearRightCollider.brakeTorque = brakeTorque;
    }

    void UpdateWheelVisuals()
    {
        // Update wheel visuals to match colliders
        Quaternion FLWRotation;
        Vector3 FLWPosition;
        carController.frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
        carController.frontLeftMesh.transform.position = FLWPosition;
        carController.frontLeftMesh.transform.rotation = FLWRotation;

        Quaternion FRWRotation;
        Vector3 FRWPosition;
        carController.frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
        carController.frontRightMesh.transform.position = FRWPosition;
        carController.frontRightMesh.transform.rotation = FRWRotation;

        Quaternion RLWRotation;
        Vector3 RLWPosition;
        carController.rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
        carController.rearLeftMesh.transform.position = RLWPosition;
        carController.rearLeftMesh.transform.rotation = RLWRotation;

        Quaternion RRWRotation;
        Vector3 RRWPosition;
        carController.rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
        carController.rearRightMesh.transform.position = RRWPosition;
        carController.rearRightMesh.transform.rotation = RRWRotation;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Draw target position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.5f);
        Gizmos.DrawLine(transform.position, targetPosition);

        // Draw avoidance vectors
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * avoidanceDistance);
        Gizmos.DrawRay(transform.position, transform.right * avoidanceDistance * 0.5f);
        Gizmos.DrawRay(transform.position, -transform.right * avoidanceDistance * 0.5f);

        // Draw current velocity
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, rb.linearVelocity.normalized * 2f);
    }
}