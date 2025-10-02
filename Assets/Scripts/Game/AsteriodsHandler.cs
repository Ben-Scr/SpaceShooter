using UnityEngine;
using System.Collections.Generic;
using System;

public class AsteriodsHandler : MonoBehaviour
{
    private DynamicObjectPool objectPool = new DynamicObjectPool();
    public static List<PersistentAsteriod> PersistentAsteriods = new List<PersistentAsteriod>();

    public static AsteriodsHandler Instance;
    [SerializeField] private GameObject asteriodExplosion;

    public static Action<Asteriod> OnDestroyAsteriod;

    private void Awake()
    {
        Instance = this;
        PersistentAsteriods = new List<PersistentAsteriod>();
    }

    void Update()
    {
        if (GameController.IsGameOver) return;

        MoveAsteriods();
    }

    private void MoveAsteriods()
    {
        List<PersistentAsteriod> destroyedAsteriods = new List<PersistentAsteriod>();

        foreach (var asteriod in PersistentAsteriods)
        {
            UnityUtility.LookAt2D(asteriod.Transform, (Vector3)PlayerController.AsteriodInfoPosition, asteriod.LookSpeed);
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
        foreach (var asteriod in AsteriodsHandler.PersistentAsteriods)
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
    public static bool IsCollidingWithAsteriod(float distanceThreshold, Vector3 at, out PersistentAsteriod persistentAsteriod)
    {
        persistentAsteriod = null;

        foreach (var asteriod in AsteriodsHandler.PersistentAsteriods)
        {
            if (asteriod.Transform.position == at) continue;

            float distance = Vector2.Distance(asteriod.Transform.position, at) - (asteriod.Transform.localScale.x / 2f);

            if (distance < distanceThreshold)
            {
                persistentAsteriod = asteriod;
                return true;
            }
        }
        return false;
    }

    public void DestroyAsteriod(PersistentAsteriod persistentAsteriod, bool fromLaser = true)
    {
        if (fromLaser)
        {
            GameObject effect = Instantiate(asteriodExplosion, persistentAsteriod.Transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(effect, 1f);
        }

        OnDestroyAsteriod?.Invoke(persistentAsteriod.Asteriod);
        objectPool.Release(persistentAsteriod.Asteriod.GameObject, persistentAsteriod.Transform.gameObject);
        PersistentAsteriods.Remove(persistentAsteriod);
    }

    public static void SpawnAsteriod(Asteriod asteriod,Vector2 at)
    {
        Transform tr = Instance.objectPool.Get(asteriod.GameObject, at, Quaternion.Euler(0, 0, 0)).transform;
        tr.localScale = Vector3.one * asteriod.ScaleMultiplier;
        PersistentAsteriods.Add(new PersistentAsteriod(asteriod,tr, asteriod.MovementSpeed, asteriod.RotationSpeed));
    }
}
