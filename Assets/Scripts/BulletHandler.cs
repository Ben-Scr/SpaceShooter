using UnityEngine;
using System.Collections.Generic;

// Handles bullet behavior in the game for better performance
public class BulletHandler : MonoBehaviour
{
    private List<Transform> bullets = new List<Transform>();
    public static BulletHandler Instance;

    [SerializeField] private float bulletSpeed = 10f;

    public void Awake()
    {
        Instance = this;
    }

    public static void Sumbit(GameObject prefab, Vector2 at, float rotationZ)
    {
        Transform tr = Instantiate(prefab, at, Quaternion.Euler(0, 0, rotationZ)).transform;
        Instance.bullets.Add(tr);
    }

    public void Update()
    {
        MoveBullets();
    }

    public void MoveBullets()
    {
        foreach(var transform in bullets)
        {
            transform.Translate(transform.up * Time.deltaTime * bulletSpeed, Space.World);
        }
    }
}
