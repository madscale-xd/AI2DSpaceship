using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    public float countdownTime = 30f; // Total time in seconds
    private float currentTime;
    private bool timerRunning = false;

    public TextMeshProUGUI timerText; // Drag the TMP UI element here in Inspector

    void Start()
    {
        currentTime = countdownTime;
        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                timerRunning = false;
                OnTimerFinished();
            }

            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerText.text = seconds.ToString("00"); // e.g., "09"
        }
    }

    void OnTimerFinished()
    {
        timerText.text = "00";
        Debug.Log("Timer finished! Do the thing here.");
        SceneManager.LoadScene("WinScene");
    }
}
