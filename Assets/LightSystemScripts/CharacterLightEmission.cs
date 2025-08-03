using UnityEngine;

public class CharacterLightEmission : MonoBehaviour
{
    [Header("Raycast Settings")]
    public int rayCount = 360;             // Number of rays to emit around the character
    public float angleStep = 1f;           // Degrees between each ray
    public float rayLength = 10f;          // Max distance for each ray
    public LayerMask obstacleMask;         // What layers block the ray

    [Header("Raycast update Settings")]
    public int raysPerUpdate = 10;         // Number of rays to handle per FixedUpdate

    [Header("Light Emission Settings")]
    public float maxLightIntensity = 1f; // Max light intensity at the origin


    [Header("Burst Emission Settings")]
    public float dotDirScaleMin = 1f;
    public float dotDirScaleMax = 10f;
    public float dotExpansionRate = 1.0f;

    public float burstLightIntensity = 20f;

    private bool isDotMode = false;
    private float currentDirScale;
    private Vector2[] rayDirections;       // Pre-calculated unit vectors for each ray
    private int startingRayIndex = 0;      // Index of the next ray to handle

    private Vector2 burstOrigin;



    void Start()
    {
        PrecalculateRayDirections();
        currentDirScale = dotDirScaleMin;
    }

    // Update is called once per frame
    void Update()
    {
        //EmitRays();
        if (!isDotMode && Input.GetMouseButtonDown(1))
        {
            isDotMode = true;
            currentDirScale = dotDirScaleMin;
            burstOrigin = transform.position;
        }
    }

    void FixedUpdate()
    {
        if (isDotMode)
        {
            EmitDots();
        }
        else
        {
            EmitRays();
        }
       
    }

    void PrecalculateRayDirections()
    {
        rayDirections = new Vector2[rayCount];
        float angleOffset = 0f; // Can be changed to rotate the full spread

        for (int i = 0; i < rayCount; i++)
        {
            float angleDeg = i * angleStep + angleOffset;
            float angleRad = angleDeg * Mathf.Deg2Rad;
            rayDirections[i] = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
        }
    }

    private void EmitSingleRay(int index)
    {
        Vector2 origin = transform.position;
        Vector2 dir = rayDirections[index];
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayLength, obstacleMask);

        if (hit.collider != null)
        {
            Debug.DrawLine(origin, hit.point, Color.red, 0.5f);

            LightReceiver receiver = hit.collider.GetComponent<LightReceiver>();
            if (receiver != null)
            {
                float distance = Vector2.Distance(origin, hit.point);
                float lightIntensity = Mathf.Lerp(maxLightIntensity, 0f, distance / rayLength);
                lightIntensity *= lightIntensity;
                receiver.ReceiveLightburst(lightIntensity);
            }

        }
        else
        {
            Debug.DrawLine(origin, origin + dir * rayLength, Color.yellow, 0.5f);
        }
    }

    public void EmitRays()
    {
        for (int i = 0; i < raysPerUpdate; i++)
        {
            EmitSingleRay(startingRayIndex);
            startingRayIndex++;

            if (startingRayIndex >= rayDirections.Length)
            {
                startingRayIndex = 0;
            }
        }
    }


    private void EmitSingleDot(int index, float dirScale)
    {
        Vector2 origin = burstOrigin;
        Vector2 dir = rayDirections[index].normalized * dirScale;
        Vector2 targetPos = origin + dir;
        Collider2D hitCollider = Physics2D.OverlapPoint(targetPos, obstacleMask);
        if (hitCollider != null)
        {
            Debug.DrawLine(origin, targetPos, Color.green, 0.5f);
            LightReceiver receiver = hitCollider.GetComponent<LightReceiver>();
            if (receiver != null)
            {
                float distance = Vector2.Distance(origin, targetPos);
                float lightIntensity = burstLightIntensity;
                receiver.ReceiveLight(lightIntensity);
            }

        }
        else
        {
            Debug.DrawLine(origin, targetPos, Color.cyan, 0.5f);
        }

    }

    public void EmitDotsForScale(float dirScale)
    {
        for (int i = 0; i < raysPerUpdate; i++)
        {
            EmitSingleDot(startingRayIndex, dirScale);
            startingRayIndex++;

            if (startingRayIndex >= rayDirections.Length)
            {
                startingRayIndex = 0;
            }
        }
    }

    public void EmitDots()
    {
        EmitDotsForScale(currentDirScale);
        currentDirScale = Mathf.MoveTowards(currentDirScale, dotDirScaleMax, dotExpansionRate * Time.fixedDeltaTime);
        if (currentDirScale >= dotDirScaleMax)
        {
            isDotMode = false;
            //currentDirScale = dotDirScaleMin;            
        }

    }


}
