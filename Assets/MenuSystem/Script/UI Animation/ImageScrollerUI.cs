using UnityEngine;
using UnityEngine.UI;

public class ImageScrollerUI : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Vector2 scrollSpeed;

    private void Update()
    {
        image.uvRect = new Rect(image.uvRect.position + scrollSpeed * Time.deltaTime, image.uvRect.size);
        if (image.uvRect.position.x >= 1f || image.uvRect.position.y >= 1)
        {
            image.uvRect = new Rect(Vector2.zero, image.uvRect.size);
        }
    }
}
