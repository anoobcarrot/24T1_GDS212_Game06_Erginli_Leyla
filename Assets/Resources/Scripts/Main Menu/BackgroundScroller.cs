using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public RectTransform image1; // Reference to the first image's RectTransform
    public RectTransform image2; // Reference to the second image's RectTransform
    public float speed = 50f; // Speed at which the images move

    private float imageWidth; // Width of the image

    void Start()
    {
        // Assuming the images have the same width, you can get the width of one image
        imageWidth = image1.rect.width;
    }

    void Update()
    {
        // Move the images to the left
        image1.anchoredPosition += Vector2.left * speed * Time.deltaTime;
        image2.anchoredPosition += Vector2.left * speed * Time.deltaTime;

        // Check if the first image has moved completely off the screen
        if (image1.anchoredPosition.x <= -imageWidth)
        {
            // Move the first image behind the second image
            image1.anchoredPosition += Vector2.right * 2 * imageWidth;
        }

        // Check if the second image has moved completely off the screen
        if (image2.anchoredPosition.x <= -imageWidth)
        {
            // Move the second image behind the first image
            image2.anchoredPosition += Vector2.right * 2 * imageWidth;
        }
    }
}

