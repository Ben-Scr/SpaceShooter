using UnityEngine;
using System.Collections.Generic;

namespace SpaceShooter
{
    // Handles all bullet behavior in the game for better performance
    public class BulletHandler : MonoBehaviour
    {
        private DynamicObjectPool objectPool => PoolManager.BulletsPool;
        private List<BulletInstance> bulletInstances = new List<BulletInstance>();
        public static BulletHandler Instance { get; private set; }


        [SerializeField] private float distanceForDestruction = 25;


        private readonly HashSet<BulletInstance> bulletsToDestroy = new HashSet<BulletInstance>();

        public void Awake()
        {
            Instance = this;
        }

        public static void SpawnBullet(Bullet bullet, Vector2 at, Quaternion rotation)
        {
            Transform tr = Instance.objectPool.Get(bullet.GameObject, at, rotation).transform;
            Instance.bulletInstances.Add(new BulletInstance(bullet, tr));
        }

        public void Update()
        {
            MoveBullets();
        }

        public void MoveBullets()
        {
            foreach (var bulletInstance in bulletInstances)
            {
                if (bulletInstance.Bullet.IsHauning)
                    HauntingBulletMovement(bulletInstance);

                if (bulletInstance.Bullet.Bounceable)
                {
                    // Implemented in future
                }

                // Move bullet
                bulletInstance.Transform.Translate(bulletInstance.Transform.up * Time.deltaTime * bulletInstance.Bullet.Speed, Space.World);


                DestroyDetection(bulletInstance);
                HitAsteriodDetection(bulletInstance);
            }

            foreach (var persistentBullet in bulletsToDestroy)
            {
                DestroyBullet(persistentBullet);
            }

            bulletsToDestroy.Clear();
        }

        private void HauntingBulletMovement(BulletInstance bulletInstance)
        {
            if (bulletInstance.HasTarget())
            {
                UnityUtility.LookAt2D(bulletInstance.Transform, bulletInstance.Target.position, bulletInstance.Bullet.RotationSpeed);
            }
            else
            {
                bulletInstance.TryFindTarget();
            }
        }

        // Detects wether a bullet can be destroyed
        private void DestroyDetection(BulletInstance bulletInstance)
        {
            bool outOfRange = Vector2.Distance(bulletInstance.Transform.position, Camera.main?.transform.position ?? Vector2.zero) > distanceForDestruction;
            bool maxBounced = bulletInstance.Bounced > bulletInstance.Bullet.MaxBounces;


            if (outOfRange || maxBounced)
                bulletsToDestroy.Add(bulletInstance);
        }

        private void HitAsteriodDetection(BulletInstance bulletInstance)
        {
            AsteriodInstance collidedAsteriod = CollidesWith(bulletInstance.Transform, bulletInstance.Bullet.HitDistanceThreshold);

            if (collidedAsteriod != null)
            {
                bulletsToDestroy.Add(bulletInstance);
                collidedAsteriod.OnHit(bulletInstance.Bullet.Damage);
            }
        }

        private AsteriodInstance CollidesWith(Transform bulletTr, float overrideThreshold = -1)
        {
            float distanceThreshold = (overrideThreshold != -1) ? overrideThreshold : (bulletTr.localScale.x / 2f);
            AsteriodsHandler.IsCollidingWithAsteriod(distanceThreshold, bulletTr.position, out AsteriodInstance persistentAsteriod);
            return persistentAsteriod;
        }

        private void DestroyBullet(BulletInstance bulletInstance)
        {
            objectPool.Release(bulletInstance.Bullet.GameObject, bulletInstance.Transform.gameObject);
            bulletInstances.Remove(bulletInstance);
        }
    }
}

