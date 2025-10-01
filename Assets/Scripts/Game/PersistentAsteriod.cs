using System.Collections;
using UnityEngine;
public class PersistentAsteriod
{
    public Asteriod Asteriod;
    public Transform Transform;
    public SpriteRenderer SpriteRenderer;
    public float MovementSpeed;
    public int Health;
    public float LookSpeed;

    public PersistentAsteriod(Asteriod asteriod, Transform transform, float movementSpeed, float lookSpeed)
    {
        Asteriod = asteriod;
        Transform = transform;
        MovementSpeed = movementSpeed;
        LookSpeed = lookSpeed;
        Health = asteriod.Health;
        SpriteRenderer = Transform.GetComponent<SpriteRenderer>();
    }

    public void OnHit()
    {
        Health--;

        if (Health <= 0)
        {
            AsteriodsHandler.Instance.DestroyAsteriod(this);
            return;
        }

        if (SpriteRenderer.color != Color.red)
        {
            Color defaultColor = SpriteRenderer.color;
            SpriteRenderer.color = Color.red;
            AsteriodsHandler.Instance.StartCoroutine(SetColorDelayed(defaultColor, 0.05f));
        }
    }

    public IEnumerator SetColorDelayed(Color color, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpriteRenderer.color = color;
    }
}

