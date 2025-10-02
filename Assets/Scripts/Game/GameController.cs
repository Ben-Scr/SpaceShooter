using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static bool IsGameOver = false;
    [SerializeField] private ParticleSystem backgroundEffect;
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private TextMeshProUGUI nextWaveTxt;
    [SerializeField] private TextMeshProUGUI curScoreTxt;
    [SerializeField] private Animator backgroundAnimator;

    private Vector2 bgEffetectOffset;
    

    private int curScore;

    private void Awake()
    {
        IsGameOver = false;
        bgEffetectOffset = backgroundEffect.transform.position;

        Application.targetFrameRate = targetFrameRate;
    }
    private void Update()
    {
        backgroundEffect.transform.position = PlayerController.Position + bgEffetectOffset;
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
       StartCoroutine( WaitForBackgroundAnimEnd());
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
        backgroundEffect.Pause();
        IsGameOver = true;
    }
}
