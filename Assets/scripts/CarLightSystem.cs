using UnityEngine;

[RequireComponent(typeof(Light))]
public class CarLightSystem : MonoBehaviour
{
    public enum LightType { Headlight, Brakelight }
    public LightType lightType = LightType.Headlight;

    [Header("Light Settings")]
    public float baseIntensity = 3f;
    public float maxIntensity = 5f;
    public Color lightColor = Color.white;
    public float range = 30f;
    public float spotAngle = 60f;

    [Header("Behavior")]
    public bool alwaysOn = true;
    public KeyCode toggleKey = KeyCode.L;
    public bool enableBrakeEffect = true;

    private Light lightComponent;
    private Renderer lightMesh; // Optional: for light bulb visualization
    private Material originalMaterial;
    private bool isOn = true;

    void Awake()
    {
        // Get components
        lightComponent = GetComponent<Light>();
        lightMesh = GetComponentInChildren<Renderer>();

        // Store original material if exists
        if (lightMesh != null)
        {
            originalMaterial = lightMesh.sharedMaterial;
        }

        // Initialize light
        InitializeLight();
    }

    void InitializeLight()
    {
        // Basic light setup
        lightComponent.type = UnityEngine.LightType.Spot;
        lightComponent.range = range;
        lightComponent.spotAngle = spotAngle;
        lightComponent.color = lightColor;
        lightComponent.intensity = isOn ? baseIntensity : 0f;

        // Additional settings based on light type
        switch (lightType)
        {
            case LightType.Headlight:
                lightComponent.shadows = LightShadows.Soft;
                lightComponent.renderMode = LightRenderMode.ForcePixel;
                break;

            case LightType.Brakelight:
                lightComponent.shadows = LightShadows.None;
                lightComponent.renderMode = LightRenderMode.Auto;
                break;
        }

        UpdateLightVisualization();
    }

    void Update()
    {
        HandleInput();
        UpdateLightStatus();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleLights();
        }
    }

    public void ToggleLights()
    {
        if (!alwaysOn)
        {
            isOn = !isOn;
            UpdateLightStatus();
        }
    }

    void UpdateLightStatus()
    {
        // For brakelights - react to input
        if (lightType == LightType.Brakelight && enableBrakeEffect)
        {
            bool isBraking = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.DownArrow);
            lightComponent.intensity = isBraking ? maxIntensity : baseIntensity;
        }
        else // For headlights
        {
            lightComponent.intensity = isOn ? baseIntensity : 0f;
        }

        UpdateLightVisualization();
    }

    void UpdateLightVisualization()
    {
        // Optional: Change material emission when light is on
        if (lightMesh != null)
        {
            if (lightComponent.intensity > 0)
            {
                lightMesh.material.EnableKeyword("_EMISSION");
                lightMesh.material.SetColor("_EmissionColor", lightColor * 2f);
            }
            else
            {
                lightMesh.material.DisableKeyword("_EMISSION");
            }
        }
    }

    void OnValidate()
    {
        // Update in editor when values change
        if (lightComponent == null)
            lightComponent = GetComponent<Light>();

        if (lightComponent != null)
        {
            lightComponent.range = range;
            lightComponent.spotAngle = spotAngle;
            lightComponent.color = lightColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize light cone in editor
        if (lightComponent != null && lightComponent.type == UnityEngine.LightType.Spot)
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = transform.position;
            Vector3 forward = transform.forward;
            float coneDistance = lightComponent.range;
            float coneRadius = Mathf.Tan(lightComponent.spotAngle * 0.5f * Mathf.Deg2Rad) * coneDistance;

            Gizmos.DrawLine(pos, pos + forward * coneDistance);
            Gizmos.DrawWireSphere(pos + forward * coneDistance, coneRadius);
        }
    }
}