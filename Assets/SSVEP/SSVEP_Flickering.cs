using UnityEngine;
using UnityEngine.UI;

public class SSVEP_Flickering : MonoBehaviour
{
    public Color startColor = Color.white;
    public Color endColor = Color.black;
    [Range(1, 40)]
    public int frequencyImg = 1; // Frequency in Hz

    //Image Blinking Paramemter 
    public Image blinkImg;
    private float elapsedTime = 0f;
    private bool isStartColor = true;

    //For interval calculations
    private float interval;

    private void Awake() {
        blinkImg = GetComponent<Image>();   
    }

    void Start()
    {
        interval = 1f / frequencyImg;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime; // Increment elapsed time

        if (elapsedTime >= interval / 2f) // Check if it's time to switch colors
        {

            blinkImg.color = isStartColor ? endColor : startColor;// Color changing
            isStartColor = !isStartColor; // Toggle the color flag
            elapsedTime = 0f; // Reset the elapsed time
        }
    }
}