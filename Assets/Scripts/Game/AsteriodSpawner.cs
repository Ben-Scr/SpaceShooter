using UnityEngine;
using System.Collections.Generic;

public class AsteriodSpawner : MonoBehaviour
{
    private DynamicObjectPool objectPool = new DynamicObjectPool();
    public static List<PersistentAsteriod> Asteriods = new List<PersistentAsteriod>();
    [SerializeField] private GameObject asteriodPrefab;

    [SerializeField] private Range rotationSpeed;
    [SerializeField] private Range movementSpeed;
    [SerializeField] private Range scaleRange;

    [SerializeField] private Range spawnPosX;
    [SerializeField] private float spawnInterval = 0.5f;

    public static AsteriodSpawner Instance;

    [SerializeField] private GameObject asteriodExplosion;

    private float spawnCounter = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Asteriods = new List<PersistentAsteriod>();
    }

    void Update()
    {
        if (GameController.IsGameOver) return;

        spawnCounter += Time.deltaTime;
        if (spawnCounter >= spawnInterval)
        {
            spawnCounter = 0;
            Vector2 spawnPosition = new Vector2(Random.Range(spawnPosX.min, spawnPosX.max), transform.position.y);
            Vector2 moveDirection = new Vector2(Random.Range(-0.5f, 0.5f), -1).normalized;
            SpawnAsteriod(spawnPosition, moveDirection);
        }

        MoveAsteriods();
    }

    private void MoveAsteriods()
    {
        List<PersistentAsteriod> destroyedAsteriods = new List<PersistentAsteriod>();

        foreach (var asteriod in Asteriods)
        {
            asteriod.Transform.Rotate(0, 0, asteriod.RotationSpeed * Time.deltaTime);
            asteriod.Transform.Translate(asteriod.MoveDirection * Time.deltaTime * asteriod.MovementSpeed, Space.World);

            if (asteriod.Transform.position.y < -10)
            {
                destroyedAsteriods.Add(asteriod);
            }
        }

        foreach (var asteriod in destroyedAsteriods)
        {
            DestroyAsteriod(asteriod, false);
        }

        destroyedAsteriods.Clear();
    }

    public void DestroyAsteriod(PersistentAsteriod asteriod, bool fromLaser = true)
    {
        if (fromLaser)
        {
            GameObject effect = Instantiate(asteriodExplosion, asteriod.Transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(effect, 1f);
        }

        objectPool.Release(asteriodPrefab, asteriod.Transform.gameObject);
        Asteriods.Remove(asteriod);
    }

    private void SpawnAsteriod(Vector2 at, Vector2 moveDirection)
    {
        Transform tr = objectPool.Get(asteriodPrefab, at, Quaternion.Euler(0, 0, 0)).transform;
        tr.localScale = Vector3.one * Random.Range(scaleRange.min, scaleRange.max);

        float rotationSpeedValue = Random.Range(rotationSpeed.min, rotationSpeed.max);
        float movementSpeedValue = Random.Range(movementSpeed.min, movementSpeed.max);
        Asteriods.Add(new PersistentAsteriod(tr, moveDirection, rotationSpeedValue, movementSpeedValue));
    }
}

public class PersistentAsteriod
{
    public Transform Transform;
    public float RotationSpeed;
    public float MovementSpeed;
    public Vector2 MoveDirection;
    public int Health = 1;

    public PersistentAsteriod(Transform transform, Vector2 moveDirection, float rotationSpeed, float movementSpeed)
    {
        MoveDirection = moveDirection;
        Transform = transform;
        RotationSpeed = rotationSpeed;
        MovementSpeed = movementSpeed;
    }

    public void OnHit()
    {
        Health--;

        if (Health <= 0)
        {
            AsteriodSpawner.Instance.DestroyAsteriod(this);
        }
    }
}
