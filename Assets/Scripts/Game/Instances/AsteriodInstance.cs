using System.Collections;
using UnityEngine;

namespace SpaceShooter
{
    public class AsteriodInstance
    {
        public Asteriod Asteriod;
        public Transform Transform;
        public SpriteRenderer SpriteRenderer;
        public float MovementSpeed;
        public int Health;
        public float LookSpeed;

        public AsteriodInstance(Asteriod asteriod, Transform transform, float movementSpeed, float lookSpeed)
        {
            Asteriod = asteriod;
            Transform = transform;
            MovementSpeed = movementSpeed;
            LookSpeed = lookSpeed;
            Health = asteriod.Health;
            SpriteRenderer = Transform.GetComponent<SpriteRenderer>();
        }

        public void OnHit(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                AsteriodsHandler.Instance.DestroyAsteriod(this);
                return;
            }

            if (SpriteRenderer.color != Color.red)
            {
                Color defaultColor = SpriteRenderer.color;
                SpriteRenderer.color = Color.red;
                AsteriodsHandler.Instance.StartCoroutine(UnityUtility.SetColorDelayed(SpriteRenderer, defaultColor, 0.05f));
            }
        }
    }
}


