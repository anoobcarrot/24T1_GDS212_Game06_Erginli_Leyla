using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIImageSlider : MonoBehaviour
{
    [Header("Images")]
    public List<Image> images;

    [Header("Slider Settings")]
    public float slideSpeed = 100f;
    public float offScreenX = -1000f;

    private RectTransform[] imageTransforms;
    private float screenWidth;

    private void Start()
    {
        if (images == null || images.Count == 0)
        {
            Debug.LogWarning("UIImageSlider: No images assigned!");
            enabled = false;
            return;
        }

        imageTransforms = new RectTransform[images.Count];

        for (int i = 0; i < images.Count; i++)
        {
            if (images[i] != null)
            {
                imageTransforms[i] = images[i].rectTransform;
            }
        }

        screenWidth = Screen.width;
    }

    private void Update()
    {
        if (imageTransforms == null)
            return;

        foreach (RectTransform imageTransform in imageTransforms)
        {
            if (imageTransform == null)
                continue;

            // Move independently of the game's time scale.
            imageTransform.localPosition +=
                Vector3.left * slideSpeed * Time.unscaledDeltaTime;

            // Move the image to the end when it leaves the screen.
            if (imageTransform.localPosition.x < offScreenX)
            {
                MoveImageToBack(imageTransform);
            }
        }
    }

    private void MoveImageToBack(RectTransform imageTransform)
    {
        RectTransform rightmostImage = GetRightmostImage();

        if (rightmostImage == null)
            return;

        imageTransform.localPosition = new Vector3(
            rightmostImage.localPosition.x + screenWidth,
            imageTransform.localPosition.y,
            imageTransform.localPosition.z
        );
    }

    private RectTransform GetRightmostImage()
    {
        RectTransform rightmostImage = null;

        foreach (RectTransform imageTransform in imageTransforms)
        {
            if (imageTransform == null)
                continue;

            if (rightmostImage == null ||
                imageTransform.localPosition.x > rightmostImage.localPosition.x)
            {
                rightmostImage = imageTransform;
            }
        }

        return rightmostImage;
    }
}