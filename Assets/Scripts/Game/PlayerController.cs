using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Range minMaxPosX;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float asteriodCollisionThreshold = 0.5f;
    [SerializeField] private ParticleSystem emissionEffect;
    [SerializeField] private float lookAtSpeed = 14;
    [SerializeField] private bool spray;
    [SerializeField] private GameObject explosion;
    public static Action OnPlayerDeath;

    public static Vector2 Position;

    void Update()
    {
        if (GameController.IsGameOver) return;

        Movement();

        if (AsteriodSpawner.IsCollidingWithAsteriod(asteriodCollisionThreshold, transform.position)) // Death / Collision detection
        {
            OnDeath();
        }
    }

    private void Movement()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(Vector2.Distance(transform.position, mousePosition) > 0.3f)
        {
            UnityUtility.LookAt2D(transform, mousePosition, lookAtSpeed);
        }



        float input = 1 * Time.deltaTime * movementSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            input *= 2;
        }

        transform.Translate(transform.up * input, Space.World);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
        if (Input.GetKey(KeyCode.Space) && spray)
        {
            Shoot();
        }

        Position = transform.position;
    }

    private void OnDeath()
    {
        GameObject obj = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(obj, 2);
        OnPlayerDeath?.Invoke();
        emissionEffect.Stop();
    }

    private void Shoot()
    {
        BulletHandler.SpawnBullet((Vector2)transform.position, transform.rotation);
    }
}