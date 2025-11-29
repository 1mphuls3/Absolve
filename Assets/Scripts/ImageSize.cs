using UnityEngine;
using UnityEngine.UI;

public class ImageSize : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private int pixelsPerUnit;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (image != null && image.material != null)
        {
            var imageRect = image.rectTransform.rect;
            var widthHeight = new Vector2(x: imageRect.width / pixelsPerUnit, y: imageRect.height / pixelsPerUnit);
            image.material.SetVector(name: "_Size", widthHeight);
        }
    }
}
