using UnityEngine;

namespace SpaceShooter
{
    public class BulletInstance
    {
        public Transform Transform;
        public Bullet Bullet;
        public Transform Target;
        public int Bounced = 0;
        public BulletInstance(Bullet bullet, Transform transform)
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

            foreach (var persistentAsteriod in AsteriodsHandler.AsteriodInstances)
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
}
