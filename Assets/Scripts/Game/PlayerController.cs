using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Range minMaxPosX;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float asteriodCollisionThreshold = 0.5f;
    [SerializeField] private ParticleSystem emissionEffect;

    [SerializeField] private GameObject explosion;

    private bool isDead = false;

    public static Action OnPlayerDeath;

    void Update()
    {
        if (GameController.IsGameOver) return;

        Movement();

        if (IsCollidingWithAsteriod()) // Death / Collision detection
        {
            OnDeath();
        }
    }

    private void Movement()
    {
        float inputX = Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed;
        float positionX = inputX + transform.position.x;
        transform.position = new Vector3(Mathf.Clamp(positionX, minMaxPosX.min, minMaxPosX.max), transform.position.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    private void OnDeath()
    {
        GameObject obj = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(obj, 2);
        isDead = true;
        OnPlayerDeath?.Invoke();
        emissionEffect.Stop();
    }

    private void Shoot()
    {
        BulletHandler.SpawnBullet((Vector2)transform.position, 0);
    }

    private bool IsCollidingWithAsteriod()
    {
        foreach (var asteriod in AsteriodSpawner.Asteriods)
        {
            float distance = Vector2.Distance(asteriod.Transform.position, transform.position) - (asteriod.Transform.localScale.x / 2f);

            if (distance < asteriodCollisionThreshold)
            {
                return true;
            }
        }
        return false;
    }
}