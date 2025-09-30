using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool IsGameOver = false;
    [SerializeField] private ParticleSystem backgroundEffect;
    [SerializeField] private int targetFrameRate = 60;

    private Vector2 bgEffetectOffset;

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
        PlayerController.OnPlayerDeath += OnPlayerDeath;
    }
    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        backgroundEffect.Pause();
        IsGameOver = true;
    }
}
