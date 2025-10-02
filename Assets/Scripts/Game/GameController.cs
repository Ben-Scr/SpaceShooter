using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem backgroundEffect;

        [Space(5)]
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI nextWaveTxt;
        [SerializeField] private TextMeshProUGUI curScoreTxt;
        [SerializeField] private Animator backgroundAnimator;
        [SerializeField] private HorizontalLayoutGroup UtilLayoutGroup;

        public static bool IsGameOver = false;

        private Vector2 bgEffectOffset;
        private int curScore;

        private void Awake()
        {
            LoadSceneManager.CheckForPersistentScene();

            IsGameOver = false;
            bgEffectOffset = backgroundEffect.transform.position;
            StartCoroutine(UpdateLayout());
        }

        private IEnumerator UpdateLayout() // Force layout update
        {
            yield return null;
            UtilLayoutGroup.spacing += 1;
        }

        private void Update()
        {
            backgroundEffect.transform.position = PlayerController.Position + bgEffectOffset;
        }

        private void OnEnable()
        {
            WaveSpawner.OnNextWave += OnNextWave;
            AsteriodsHandler.OnDestroyAsteriod += OnDestroyAsteriod;
            PlayerController.OnPlayerDeath += OnPlayerDeath;
        }
        private void OnDisable()
        {
            WaveSpawner.OnNextWave -= OnNextWave;
            AsteriodsHandler.OnDestroyAsteriod -= OnDestroyAsteriod;
            PlayerController.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnNextWave(int curWave)
        {
            nextWaveTxt.text = $"Wave {curWave}";
            nextWaveTxt.GetComponent<Animator>().SetTrigger("Wave");
            backgroundAnimator.SetBool("Active", true);
            StartCoroutine(WaitForBackgroundAnimEnd());
        }

        private IEnumerator WaitForBackgroundAnimEnd()
        {
            yield return new WaitForSeconds(1f);
            backgroundAnimator.SetBool("Active", false);
        }

        private void OnDestroyAsteriod(Asteriod asteriod)
        {
            curScore += asteriod.ScoreValue;
            curScoreTxt.text = curScore.ToString();
            curScoreTxt.transform.parent.GetComponent<Animator>().SetTrigger("Score");
        }

        private void OnPlayerDeath()
        {
            IsGameOver = true;

            CameraController.Instance.camMode = CameraController.CamMode.Follow;
            CameraController.Instance.followOrthoSize = 5;
        }
    }
}