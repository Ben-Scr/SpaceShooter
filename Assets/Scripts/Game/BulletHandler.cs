using UnityEngine;
using System.Collections.Generic;

// Handles bullet behavior in the game for better performance
public class BulletHandler : MonoBehaviour
{
    private DynamicObjectPool objectPool = new DynamicObjectPool();
    private List<Transform> bullets = new List<Transform>();
    public static BulletHandler Instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;

    public void Awake()
    {
        Instance = this;
    }

    public static void SpawnBullet(Vector2 at, Quaternion rotation)
    {
        Transform tr = Instance.objectPool.Get(Instance.bulletPrefab, at, rotation).transform;
        Instance.bullets.Add(tr);
    }

    public void Update()
    {
        MoveBullets();
    }

    public void MoveBullets()
    {
        List<Transform> destroyedBullets = new List<Transform>();

        foreach (var transform in bullets)
        {
            transform.Translate(transform.up * Time.deltaTime * bulletSpeed, Space.World);

            if (Vector2.Distance(transform.position, PlayerController.Position) > 12)
            {
                destroyedBullets.Add(transform);
            }

            PersistentAsteriod collidedAsteriod = CollidesWith(transform);

            if(collidedAsteriod != null)
            {
                destroyedBullets.Add(transform);
                collidedAsteriod.OnHit();
            }
        }

        foreach (var bullet in destroyedBullets)
        {
            DestroyBullet(bullet);
        }

        destroyedBullets.Clear();
    }

    private PersistentAsteriod CollidesWith(Transform bulletTr)
    {
        foreach(var asteriod in AsteriodSpawner.Asteriods)
        {
            float distance = Vector2.Distance(asteriod.Transform.position, bulletTr.position);
            if (distance - (asteriod.Transform.localScale.x / 2f) <  0)
            {
                return asteriod;
            }
        }

        return null;
    }

    private void DestroyBullet(Transform bullet)
    {
        objectPool.Release(bulletPrefab, bullet.gameObject);
        bullets.Remove(bullet);
    }
}
