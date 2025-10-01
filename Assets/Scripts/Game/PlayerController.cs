using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConntrolMode
{
    Mouse,
    Keyboard
}
public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private ConntrolMode controlMode;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float asteriodCollisionThreshold = 0.5f;
    [SerializeField] private float lookAtSpeed = 14;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] private float inputLerpSpeed = 10f;

    [Space(5)]
    [Header("Weapon")]
    [SerializeField] float spreadAngle = 60f;
    [SerializeField] private List<Weapon> initWeapons;
    private List<PersistentWeapon> persistentWeapons = new List<PersistentWeapon>();

    [Space(5)]
    [Header("Other")]
    [SerializeField] private ParticleSystem emissionEffect;
    [SerializeField] private GameObject deathExplosionPrefab;
    [SerializeField] private float infoPositionUpdateInterval = 0.5f;


    public static Action OnPlayerDeath;
    public static Vector2 Position;
    public static Vector2 AsteriodInfoPosition;

    private float updateInfoPositionCounter;
    private float lastInputValue = 1;

    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        foreach (var weapon in initWeapons)
        {
            if (weapon.Auto)
            {
                persistentWeapons.Add(new PersistentAutomaticWeapon(weapon));
            }
            else
            {
                persistentWeapons.Add(new PersistentWeapon(weapon));
            }
        }
    }

    void Update()
    {
        if (GameController.IsGameOver) return;

        Movement();

        if (Input.GetKeyDown(KeyCode.F2))
        {
            controlMode = controlMode == ConntrolMode.Keyboard ? ConntrolMode.Mouse : ConntrolMode.Keyboard;
        }

        if (AsteriodsHandler.IsCollidingWithAsteriod(asteriodCollisionThreshold, transform.position))
        {
            OnDeath();
        }

        updateInfoPositionCounter += Time.deltaTime;

        if (updateInfoPositionCounter >= infoPositionUpdateInterval)
        {
            updateInfoPositionCounter = 0;
            AsteriodInfoPosition = transform.position;
        }
    }

    private void Movement()
    {
        //lastInputValue = Mathf.Lerp(lastInputValue, Input.GetKey(KeyCode.LeftControl) ? 0 : 1, inputLerpSpeed * Time.deltaTime);
        float input = lastInputValue * Time.deltaTime * movementSpeed;

        if (controlMode == ConntrolMode.Keyboard)
        {
            float rotationInput = Input.GetAxisRaw("Horizontal");
            transform.Rotate(Vector3.forward, -rotationInput * rotationSpeed * Time.deltaTime * 100);
        }
        if (controlMode == ConntrolMode.Mouse)
        {

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float dst = Vector2.Distance(transform.position, mousePosition);
            UnityUtility.LookAt2D(transform, mousePosition, lookAtSpeed * Mathf.Clamp01(dst));
        }

        if (Input.GetKey(KeyCode.LeftShift)) input *= 2;

        transform.Translate(transform.up * input, Space.World);

        UpdateWeapons();

        Position = transform.position;
    }

    private void UpdateWeapons()
    {

        foreach (PersistentWeapon persistentWeapon in persistentWeapons)
        {
            Weapon weapon = persistentWeapon.Weapon;

            if (!weapon.Auto)
            {
                if (Input.GetKeyDown(weapon.CallKey))
                {
                    if (weapon is Bullet bullet)
                    {
                        BulletHandler.SpawnBullet(bullet, (Vector2)transform.position, transform.rotation);
                    }
                }
            }
            else if (persistentWeapon is PersistentAutomaticWeapon automaticWeapon)
            {
                if (weapon is Bullet bullet && automaticWeapon.CanCall())
                {
                    BulletHandler.SpawnBullet(bullet, (Vector2)transform.position, transform.rotation);
                }
            }
        }
    }

    private void OnDeath()
    {
        GameObject obj = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(obj, 2);
        emissionEffect.Stop();

        OnPlayerDeath?.Invoke();
    }
}

public class PersistentWeapon
{
    public Weapon Weapon;
    public PersistentWeapon(Weapon weapon)
    {
        Weapon = weapon;
    }
}

public class PersistentAutomaticWeapon : PersistentWeapon
{
    public PersistentAutomaticWeapon(Weapon weapon) : base(weapon)
    {
        LastCallTimeCounter = weapon.CallInterval;
    }

    public float LastCallTimeCounter;
    public bool CanCall()
    {
        LastCallTimeCounter += Time.deltaTime;
        bool canCall =  LastCallTimeCounter >= Weapon.CallInterval;
        if (canCall) LastCallTimeCounter = 0;
        return canCall;
    }
}


//// Circular shooting
//for (int i = 0; i < shootAmount; i++)
//{
//    float angle = i * (360f / shootAmount);
//    Quaternion rot = Quaternion.Euler(0, 0, angle);
//    BulletHandler.SpawnBullet((Vector2)transform.position, rot);
//}



//if (shootAmount == 1)
//{
//    BulletHandler.SpawnBullet(bullet, (Vector2)transform.position, transform.rotation);
//}
//else
//{
//    float startAngle = -spreadAngle / 2f;
//    float angleStep = spreadAngle / (shootAmount - 1);

//    for (int i = 0; i < shootAmount; i++)
//    {
//        float currentAngle = startAngle + angleStep * i;
//        Quaternion rot = transform.rotation * Quaternion.Euler(0, 0, currentAngle);
//        BulletHandler.SpawnBullet(bullet, (Vector2)transform.position, rot);
//    }
//}