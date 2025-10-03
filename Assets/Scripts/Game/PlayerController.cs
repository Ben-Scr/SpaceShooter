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
        [Header("Other")]
        [SerializeField] private ParticleSystem emissionEffect;
        [SerializeField] private GameObject deathExplosionPrefab;
        [SerializeField] private float infoPositionUpdateInterval = 0.5f;
        [SerializeField] private int health = 5;


        public static Action OnPlayerDeath;
        public static Vector2 Position;
        public static Vector2 AsteriodInfoPosition;

        private SpriteRenderer spriteRenderer;
        private float updateInfoPositionCounter;
        private float lastInputValue = 1;

        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (GameController.IsGameOver) return;

            Movement();


            if (Input.GetKeyDown(KeyCode.F2))
            {
                controlMode = controlMode == ConntrolMode.Keyboard ? ConntrolMode.Mouse : ConntrolMode.Keyboard;
            }

            if (AsteriodsHandler.IsCollidingWithAsteriod(asteriodCollisionThreshold, transform.position, out AsteriodInstance asteriodInstance))
            {
                health -= asteriodInstance.Health;

                if(health <= 0)
                {
                    OnDeath();
                }
                else
                {
                    AsteriodsHandler.Instance.DestroyAsteriod(asteriodInstance);
                    Color defaultColor = spriteRenderer.color;
                    spriteRenderer.color = Color.red;
                    AsteriodsHandler.Instance.StartCoroutine(UnityUtility.SetColorDelayed(spriteRenderer, defaultColor, 0.05f));
                }
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
            Position = transform.position;
        }


        private void OnDeath()
        {
            GameObject obj = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(obj, 2);
            emissionEffect.Stop();
            OnPlayerDeath?.Invoke();
            gameObject.SetActive(false);
        }
    }
}