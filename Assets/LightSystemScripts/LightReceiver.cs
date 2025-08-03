using UnityEngine;

public class LightReceiver : MonoBehaviour
{
    [Header("Light Accumulation Settings")]
    public float accumulatedLight = 0f;          // Current light level
    public float accumulatedBurstLight = 0f;          // Current light level
    public float maxLight = 5f;                  // Max cap for accumulated light
    public float decayRate = 1f;                 // How much light decays per second

    public float clampLight = 1f;

    public SpriteRenderer spriteRenderer;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

        FixedUpdateSpriteLuminosity();
        FixedUpdateDecayLight();
        
    }


    public void ReceiveLight(float intensity)
    {
        accumulatedLight += intensity;
        accumulatedLight = Mathf.Clamp(accumulatedLight, 0f, maxLight);
    }

    public void ReceiveLightburst(float intensity)
    {
        accumulatedBurstLight += intensity;
        //accumulatedLight = Mathf.Clamp(accumulatedLight, 0f, maxLight);
    }


    public void FixedUpdateDecayLight()
    {
        if (accumulatedLight > 0)
            accumulatedLight -= decayRate;
        if (accumulatedBurstLight > 0)   
            accumulatedBurstLight -= decayRate;
    }


    private void FixedUpdateSpriteLuminosity()
    {
        float currentLight = Mathf.Max(accumulatedLight, accumulatedBurstLight);
        currentLight = Mathf.Clamp(currentLight, 0f, maxLight);
        if (currentLight < clampLight)
            currentLight = 0f;
        float alpha = currentLight / maxLight;
        Color color = spriteRenderer.color;
        color.a = alpha; // adjust transparency if needed
        spriteRenderer.color = color;
    }        


}
