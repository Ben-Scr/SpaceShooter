using UnityEngine;
using System.Collections.Generic;

public class AsteriodSpawner : MonoBehaviour
{
    private DynamicObjectPool objectPool = new DynamicObjectPool();
    public static List<PersistentAsteriod> Asteriods = new List<PersistentAsteriod>();
    [SerializeField] private GameObject asteriodPrefab;

    [SerializeField] private Range movementSpeed;
    [SerializeField] private Range scaleRange;
    [SerializeField] private Range lookspeed;

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
            Vector2 spawnPosition = new Vector2(Random.Range(spawnPosX.Min, spawnPosX.Max), transform.position.y);
            Vector2 moveDirection = (PlayerController.Position - spawnPosition).normalized;
            SpawnAsteriod(spawnPosition, moveDirection);
        }

        MoveAsteriods();
    }

    private void MoveAsteriods()
    {
        List<PersistentAsteriod> destroyedAsteriods = new List<PersistentAsteriod>();

        foreach (var asteriod in Asteriods)
        {
            UnityUtility.LookAt2D(asteriod.Transform, (Vector3)PlayerController.Position, asteriod.LookSpeed);
            asteriod.Transform.Translate(asteriod.Transform.up * Time.deltaTime * asteriod.MovementSpeed, Space.World);

            if(IsCollidingWithAsteriod(asteriod.Transform.localScale.x / 2f, asteriod.Transform.position))
            {
                destroyedAsteriods.Add(asteriod);
            }
        }

        foreach (var asteriod in destroyedAsteriods)
        {
            DestroyAsteriod(asteriod, true);
        }

        destroyedAsteriods.Clear();
    }

    public static bool IsCollidingWithAsteriod(float distanceThreshold, Vector3 at)
    {
        foreach (var asteriod in AsteriodSpawner.Asteriods)
        {
            if (asteriod.Transform.position == at) continue;

            float distance = Vector2.Distance(asteriod.Transform.position, at) - (asteriod.Transform.localScale.x / 2f);

            if (distance < distanceThreshold)
            {
                return true;
            }
        }
        return false;
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
        tr.localScale = Vector3.one * Random.Range(scaleRange.Min, scaleRange.Max);

        float movementSpeedValue = Random.Range(movementSpeed.Min, movementSpeed.Max);
        float lookSpeedValue = Random.Range(lookspeed.Min, lookspeed.Max);
        Asteriods.Add(new PersistentAsteriod(tr, movementSpeedValue, lookSpeedValue));
    }
}

public class PersistentAsteriod
{
    public Transform Transform;
    public float MovementSpeed;
    public int Health = 1;
    public float LookSpeed;

    public PersistentAsteriod(Transform transform, float movementSpeed, float lookSpeed)
    {
        Transform = transform;
        MovementSpeed = movementSpeed;
        LookSpeed = lookSpeed;
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
