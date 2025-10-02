using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
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
        private List<WeaponInstance> persistentWeapons = new List<WeaponInstance>();
        [SerializeField] private Slider[] weaponSliders;
        [SerializeField] private Color weaponAvailableColor, weaponUnavailableColor;


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

            int i = 0;
            foreach (var weaponSlider in weaponSliders)
            {
                weaponSlider.maxValue = initWeapons[i++].CallInterval;
                weaponSlider.value = weaponSlider.maxValue;
            }

            foreach (var weapon in initWeapons)
            {
                    persistentWeapons.Add(new WeaponInstance(weapon));
            }
        }

        void Update()
        {
            if (GameController.IsGameOver) return;

            Movement();

            int i = 0;
            foreach (var weaponSlider in weaponSliders)
            {
                weaponSlider.value = persistentWeapons[i++].LastCallTimeCounter;
                bool isMax = weaponSlider.value >= weaponSlider.maxValue;
                weaponSlider.fillRect.GetComponent<Image>().color = isMax ? weaponAvailableColor : weaponUnavailableColor;
            }

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

            if (!GameAssetsManager.Instance.MapBounds.Contains(transform.position))
            {
                transform.rotation = Quaternion.Euler(0, 0, transform.localEulerAngles.z + 180f);
            }

            if (Input.GetKey(KeyCode.LeftShift)) input *= 2;

            transform.Translate(transform.up * input, Space.World);

            UpdateWeapons();

            Position = transform.position;
        }

        private void UpdateWeapons()
        {

            foreach (WeaponInstance persistentWeapon in persistentWeapons)
            {
                Weapon weapon = persistentWeapon.Weapon;

                if (persistentWeapon.CanCall() && (Input.GetKeyDown(weapon.CallKey) || (weapon.Auto)))
                {
                    persistentWeapon.LastCallTimeCounter = 0;

                    if (weapon is Bullet bullet)
                    {
                        if (bullet.EmitCount == 1)
                        {
                            BulletHandler.SpawnBullet(bullet, (Vector2)transform.position, transform.rotation);
                        }
                        else
                        {
                            if (bullet.SpreadType == SpreadType.Frustum)
                            {
                                float startAngle = -spreadAngle / 2f;
                                float angleStep = spreadAngle / (bullet.EmitCount - 1);

                                for (int i = 0; i < bullet.EmitCount; i++)
                                {
                                    float currentAngle = startAngle + angleStep * i;
                                    Quaternion rot = transform.rotation * Quaternion.Euler(0, 0, currentAngle);
                                    BulletHandler.SpawnBullet(bullet, (Vector2)transform.position, rot);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < bullet.EmitCount; i++)
                                {
                                    float angle = i * (360f / bullet.EmitCount);
                                    Quaternion rot = Quaternion.Euler(0, 0, angle);
                                    BulletHandler.SpawnBullet(bullet, (Vector2)transform.position, rot);
                                }
                            }
                        }
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
}