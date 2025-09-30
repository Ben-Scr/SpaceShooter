using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Range minMaxPosX;
    public float movementSpeed = 5;
    public GameObject bulletPrefab;

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;
        float positionX = inputX + transform.position.x;
        transform.position = new Vector3(Mathf.Clamp(positionX, minMaxPosX.min, minMaxPosX.max), transform.position.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        BulletHandler.Sumbit(bulletPrefab, (Vector2)transform.position, 0);
    }
}