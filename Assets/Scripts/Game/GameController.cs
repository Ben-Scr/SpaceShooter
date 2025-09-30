using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool IsGameOver = false;
    [SerializeField] private ParticleSystem backgroundEffect;

    private void Awake()
    {
        IsGameOver = false;
        Application.targetFrameRate = 60;
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
