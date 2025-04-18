using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;

    [Header("UI Elements")]
    public Text coinsText;
    public Text timerText;
    public Text winText;
    public Text gameOverText;
    public float winTextDuration = 3f;

    [Header("Game Settings")]
    public int totalCoins = 10;

    [Header("NPC")]
    public Animator npcAnimator; // Assign in inspector

    private int collectedCoins;
    private float startTime;
    private bool isRacing = true;
    private bool isGameOver = false;

    void Awake()
    {
        instance = this;
        startTime = Time.time;
        UpdateUI();
        winText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isRacing)
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        float elapsed = Time.time - startTime;
        timerText.text = $"Time: {elapsed.ToString("0.00")}s";
    }

    public void CollectCoin()
    {
        if (isGameOver) return;

        collectedCoins++;
        UpdateUI();

        if (collectedCoins >= totalCoins)
        {
            StartCoroutine(ShowWinText());
        }
    }

    void UpdateUI()
    {
        coinsText.text = $"Coins: {collectedCoins}/{totalCoins}";
    }

    IEnumerator ShowWinText()
    {
        isRacing = false;
        float finalTime = Time.time - startTime;

        winText.text = $"YOU WIN!\nFinal Time: {finalTime.ToString("0.00")}s";
        winText.gameObject.SetActive(true);

        // NPC does victory dance
        if (npcAnimator != null)
            npcAnimator.SetTrigger("Victory");

        yield return new WaitForSeconds(winTextDuration);

        winText.gameObject.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isRacing = false;
        isGameOver = true;

        gameOverText.text = "GAME OVER!";
        gameOverText.gameObject.SetActive(true);

        // NPC does cry animation
        if (npcAnimator != null)
            npcAnimator.SetTrigger("Cry");

        // Optionally pause the game
        // Time.timeScale = 0f;
    }
}
