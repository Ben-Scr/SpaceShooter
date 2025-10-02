using UnityEngine;
using System.Collections.Generic;

// Handles all bullet behavior in the game for better performance
public class BulletHandler : MonoBehaviour
{
    private DynamicObjectPool objectPool = new DynamicObjectPool();
    private List<PersistentBullet> persistentBullets = new List<PersistentBullet>();
    public static BulletHandler Instance;

    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float distanceForDeath = 25;

    public void Awake()
    {
        Instance = this;
    }

    public static void SpawnBullet(Bullet bullet, Vector2 at, Quaternion rotation)
    {
        Transform tr = Instance.objectPool.Get(bullet.GameObject, at, rotation).transform;
        Instance.persistentBullets.Add(new PersistentBullet(bullet, tr));
    }

    public void Update()
    {
        MoveBullets();
    }

    HashSet<PersistentBullet> bulletsToDestroy = new HashSet<PersistentBullet>();

    public void MoveBullets()
    {
        foreach (var persistentBullet in persistentBullets)
        {
            if (persistentBullet.Bullet.IsHauning)
            {
                HauntingBulletMovement(persistentBullet);
            }

            if (persistentBullet.Bullet.Bounceable)
            {
                // Implemented in future
            }

            persistentBullet.Transform.Translate(persistentBullet.Transform.up * Time.deltaTime * persistentBullet.Bullet.Speed, Space.World);



            DeathDetection(persistentBullet);

            AsteriodHitDetection(persistentBullet);
        }

        foreach (var persistentBullet in bulletsToDestroy)
        {
            DestroyBullet(persistentBullet);
        }

        bulletsToDestroy.Clear();
    }

    private void HauntingBulletMovement(PersistentBullet persistentBullet)
    {
        if (persistentBullet.HasTarget())
        {
            UnityUtility.LookAt2D(persistentBullet.Transform, persistentBullet.Target.position, persistentBullet.Bullet.RotationSpeed);
        }
        else
        {
            persistentBullet.TryFindTarget();
        }
    }

    private void DeathDetection(PersistentBullet persistentBullet)
    {
        bool outOfRange = Vector2.Distance(persistentBullet.Transform.position, PlayerController.Position) > distanceForDeath;

        if (outOfRange)
        {
            bulletsToDestroy.Add(persistentBullet);
        }
    }

    private void AsteriodHitDetection(PersistentBullet persistentBullet)
    {
        PersistentAsteriod collidedAsteriod = CollidesWith(persistentBullet.Transform);

        if (collidedAsteriod != null)
        {
            bulletsToDestroy.Add(persistentBullet);
            collidedAsteriod.OnHit();
        }
    }

    private PersistentAsteriod CollidesWith(Transform bulletTr)
    {
        AsteriodsHandler.IsCollidingWithAsteriod(bulletTr.localScale.x / 2f, bulletTr.position, out PersistentAsteriod persistentAsteriod);
        return persistentAsteriod;
    }

    private void DestroyBullet(PersistentBullet persistentBullet)
    {
        objectPool.Release(persistentBullet.Bullet.GameObject, persistentBullet.Transform.gameObject);
        persistentBullets.Remove(persistentBullet);
    }
}

public class PersistentBullet
{
    public Transform Transform;
    public Bullet Bullet;
    public Transform Target;
    public int Bounced = 0;
    public PersistentBullet(Bullet bullet, Transform transform)
    {
        Transform = transform;
        Bullet = bullet;

        if (bullet.IsHauning)
        {
            TryFindTarget();
        }
    }

    public void TryFindTarget()
    {
        float minDistance = float.MaxValue;

        foreach (var persistentAsteriod in AsteriodsHandler.PersistentAsteriods)
        {
            float dst = Vector2.Distance(Transform.position, persistentAsteriod.Transform.position);

            if (dst < minDistance)
            {
                minDistance = dst;
                Target = persistentAsteriod.Transform;
            }
        }
    }

    public bool HasTarget()
    {
        return Target?.gameObject.activeInHierarchy ?? false;
    }
}