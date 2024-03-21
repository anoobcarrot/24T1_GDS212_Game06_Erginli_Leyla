using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIImageSlider : MonoBehaviour
{
    public List<Image> images; // List of UI images to slide
    public float slideSpeed = 100f; // Speed at which the images slide
    public float offScreenX = -1000f; // X position when image is off the screen

    private RectTransform[] imageTransforms; // Array to store the RectTransform components of the images
    private float screenWidth; // Width of the screen

    private void Start()
    {
        // Initialize the array of RectTransform components
        imageTransforms = new RectTransform[images.Count];
        for (int i = 0; i < images.Count; i++)
        {
            imageTransforms[i] = images[i].rectTransform;
        }

        // Get the width of the screen
        screenWidth = Screen.width;
    }

    private void Update()
    {
        // Slide each image to the left
        foreach (RectTransform imageTransform in imageTransforms)
        {
            // Move the image to the left
            imageTransform.localPosition += Vector3.left * slideSpeed * Time.deltaTime;

            // If the image is off the screen, move it behind all other images
            if (imageTransform.localPosition.x < offScreenX)
            {
                MoveImageToBack(imageTransform);
            }
        }
    }

    // Function to move an image to the back (behind all other images)
    private void MoveImageToBack(RectTransform imageTransform)
    {
        // Find the rightmost image
        RectTransform rightmostImage = GetRightmostImage();

        // Set the X position of the image to be behind the rightmost image
        imageTransform.localPosition = new Vector3(rightmostImage.localPosition.x + screenWidth, imageTransform.localPosition.y, imageTransform.localPosition.z);
    }

    // Function to find the rightmost image
    private RectTransform GetRightmostImage()
    {
        RectTransform rightmostImage = imageTransforms[0];
        foreach (RectTransform imageTransform in imageTransforms)
        {
            if (imageTransform.localPosition.x > rightmostImage.localPosition.x)
            {
                rightmostImage = imageTransform;
            }
        }
        return rightmostImage;
    }
}

