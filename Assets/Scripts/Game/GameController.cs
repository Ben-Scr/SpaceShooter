using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static bool IsGameOver = false;
    [SerializeField] private ParticleSystem backgroundEffect;
    private Vector2 bgEffetectOffset;
    [SerializeField] private int targetFrameRate = 60;

    private void Awake()
    {
        IsGameOver = false;
        bgEffetectOffset = backgroundEffect.transform.position;
        Application.targetFrameRate = targetFrameRate;
    }
    private void Update()
    {
        backgroundEffect.transform.position = (Vector2)PlayerController.Position + bgEffetectOffset;
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
