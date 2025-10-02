using UnityEngine;
using System.Collections.Generic;
using System;

namespace SpaceShooter
{
    public class AsteriodsHandler : MonoBehaviour
    {
        [SerializeField] private GameObject asteriodExplosion;

        public static List<AsteriodInstance> AsteriodInstances = new List<AsteriodInstance>();
        public static Action<Asteriod> OnDestroyAsteriod;
        public static AsteriodsHandler Instance { get; private set; }

        private static DynamicObjectPool objectPool => PoolManager.AsteriodsPool;
        private readonly HashSet<AsteriodInstance> destroyedAsteriods = new HashSet<AsteriodInstance>();

        private void Awake()
        {
            Instance = this;
            AsteriodInstances = new List<AsteriodInstance>();
        }

        void Update()
        {
            if (GameController.IsGameOver) return;

            MoveAsteriods();
        }

        private void MoveAsteriods()
        {
            foreach (var asteriodInstance in AsteriodInstances)
            {
                UnityUtility.LookAt2D(asteriodInstance.Transform, (Vector3)PlayerController.AsteriodInfoPosition, asteriodInstance.LookSpeed);
                asteriodInstance.Transform.Translate(asteriodInstance.Transform.up * Time.deltaTime * asteriodInstance.MovementSpeed, Space.World);

                if (IsCollidingWithAsteriod(asteriodInstance.Transform.localScale.x / 2f, asteriodInstance.Transform.position))
                {
                    destroyedAsteriods.Add(asteriodInstance);
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
            foreach (var asteriodInstance in AsteriodInstances)
            {
                if (asteriodInstance.Transform.position == at) continue;

                float distance = Vector2.Distance(asteriodInstance.Transform.position, at) - (asteriodInstance.Transform.localScale.x / 2f);

                if (distance < distanceThreshold)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsCollidingWithAsteriod(float distanceThreshold, Vector3 at, out AsteriodInstance asteriodInstance)
        {
            asteriodInstance = null;

            foreach (var asteriod in AsteriodInstances)
            {
                if (asteriod.Transform.position == at) continue;

                float distance = Vector2.Distance(asteriod.Transform.position, at) - (asteriod.Transform.localScale.x / 2f);

                if (distance < distanceThreshold)
                {
                    asteriodInstance = asteriod;
                    return true;
                }
            }
            return false;
        }

        public void DestroyAsteriod(AsteriodInstance asteriodInstance, bool fromLaser = true)
        {
            if (fromLaser)
            {
                GameObject explosionEffect = Instantiate(asteriodExplosion, asteriodInstance.Transform.position, Quaternion.Euler(0, 0, 0));
                Destroy(explosionEffect, 1f);
            }

            OnDestroyAsteriod?.Invoke(asteriodInstance.Asteriod);
            objectPool.Release(asteriodInstance.Asteriod.GameObject, asteriodInstance.Transform.gameObject);
            AsteriodInstances.Remove(asteriodInstance);
        }

        public static void SpawnAsteriod(Asteriod asteriod, Vector2 at)
        {
            Transform tr = objectPool.Get(asteriod.GameObject, at, Quaternion.Euler(0, 0, 0)).transform;
            tr.localScale = Vector3.one * asteriod.ScaleMultiplier;
            AsteriodInstances.Add(new AsteriodInstance(asteriod, tr, asteriod.MovementSpeed, asteriod.RotationSpeed));
        }
    }
}

