using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxAmountX, parallaxAmountY;
    private float length;
    private Vector2 startPos;

    void Start()
    {
        startPos = transform.position;
        length = this.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float tempX = cam.transform.position.x * (1 - parallaxAmountX);
        float distX = cam.transform.position.x * parallaxAmountX;
        float distY = cam.transform.position.y * parallaxAmountY;

        transform.position = new Vector3(startPos.x + distX, startPos.y + distY, transform.position.z);

        if (tempX > startPos.x + length) startPos.x += length;
        else if (tempX < startPos.x - length) startPos.x -= length;
    }
}
