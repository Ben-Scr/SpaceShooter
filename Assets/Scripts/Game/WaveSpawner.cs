using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private List<WaveEntity<Asteriod>> asteriods;
        private List<Asteriod> asteriodsToSpawn = new List<Asteriod>();

        [SerializeField] private Difficulty difficulty;
        [SerializeField] private int[] waveValueMultipliers = new int[3] { 5, 10, 20 };
        [SerializeField] private float spawnInterval = 0.1f;
        [SerializeField] private float spawnDistance = 10f;
        [SerializeField] private Range spawnRangeY = new Range(-5f, 5f);
        [SerializeField] private float wavePauseTime = 2f;
        [SerializeField] private Slider waveSlider;
        private int waveValueMultiplier => waveValueMultipliers[(int)difficulty];
        private int currentWave;
        private int currentWaveAmount;
        private int cheapestWaveCost = int.MaxValue;

        private bool isSpawning;
        private float spawnTimer;

        public static Action<int> OnNextWave;

        private void Awake()
        {
            foreach (var entity in asteriods)
            {
                if (entity.WaveCost < cheapestWaveCost)
                {
                    cheapestWaveCost = entity.WaveCost;
                }
            }
        }


        private void OnEnable()
        {
            AsteriodsHandler.OnDestroyAsteriod += OnDestroyAsteriod;
        }

        private void OnDisable()
        {
            AsteriodsHandler.OnDestroyAsteriod -= OnDestroyAsteriod;
        }

        private void OnDestroyAsteriod(Asteriod asteriod)
        {
            waveSlider.value--;
        }

        private void Update()
        {
            if (isSpawning)
            {
                spawnTimer += Time.deltaTime;

                if (spawnTimer >= spawnInterval)
                {
                    spawnTimer = 0f;
                    if (asteriodsToSpawn.Count > 0)
                    {
                        var asteriod = asteriodsToSpawn[0];
                        asteriodsToSpawn.RemoveAt(0);

                        Vector2 spawnPos = GetRandomWaveEntityPos();

                        while (AsteriodsHandler.IsCollidingWithAsteriod(0, spawnPos))
                        {
                            spawnPos = GetRandomWaveEntityPos();
                        }

                        AsteriodsHandler.SpawnAsteriod(asteriod, spawnPos);
                    }
                    else
                    {
                        isSpawning = false;
                    }
                }
            }

            if (AsteriodsHandler.AsteriodInstances.Count == 0 && asteriodsToSpawn.Count == 0)
            {
                NextWave();
                OnNextWave?.Invoke(currentWave);
            }
        }

        public void NextWave()
        {
            currentWave++;
            currentWaveAmount = currentWave * waveValueMultiplier;
            GenerateWave();
            StartCoroutine(StartWaveDelayed());
        }

        private System.Collections.IEnumerator StartWaveDelayed()
        {
            yield return new WaitForSeconds(wavePauseTime);
            isSpawning = true;
            waveSlider.maxValue = asteriodsToSpawn.Count;
            waveSlider.value = waveSlider.maxValue;
        }

        public void GenerateWave()
        {
            int iters = 0;

            while (currentWaveAmount >= cheapestWaveCost)
            {
                var waveEntity = asteriods[UnityEngine.Random.Range(0, asteriods.Count)];
                if (waveEntity.WaveCost <= currentWaveAmount)
                {
                    asteriodsToSpawn.Add(waveEntity.Entity);
                    currentWaveAmount -= waveEntity.WaveCost;
                }
                iters++;
                if (iters > 1000)
                    break;
            }
        }

        public Vector2 GetRandomWaveEntityPos()
        {
            bool isLeft = UnityEngine.Random.Range(0, 2) == 0;
            return new Vector2(isLeft ? -spawnDistance : spawnDistance, UnityEngine.Random.Range(spawnRangeY.Min, spawnRangeY.Max)) + Camera.main?.transform.position ?? Vector2.zero;
        }
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [Serializable]
    public class WaveEntity<T>
    {
        public T Entity;
        public int WaveCost;
    }
}