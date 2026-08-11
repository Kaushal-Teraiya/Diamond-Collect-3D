using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Heart[] hearts;
    [SerializeField] private TextMeshProUGUI collectedDiamondstxt;
    [SerializeField] private TextMeshProUGUI totalDiamondstxt;
    [SerializeField] private TextMeshProUGUI timerTxt;
    [SerializeField] private GameObject youWinUI;
    [SerializeField] private GameObject youLoseUI;
    [SerializeField] private GameObject fireBorderPrefab;
    [SerializeField] private GameObject gridParent;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private Image timerImage;
    private Tween timerBlinkTween;
    private Coroutine fireRoutine;

    void Start()
    {
        UpdateTimerUI(GameManager.Instance.CurrentTime);

        UpdateDiamondUI(
            GameManager.Instance.CollectedDiamonds,
            GameManager.Instance.TotalDiamonds
        );

        restartButton.SetActive(false);
        quitButton.SetActive(false);
        //subscribing to events to upgrade UI
        GameManager.Instance.OnLivesChanged += UpdateHeartsUI;
        GameManager.Instance.OnDiamondsChanged += UpdateDiamondUI;
        GameManager.Instance.OnTimerChanged += UpdateTimerUI;
        GameManager.Instance.OnStateChanged += HandleGameState;
        fireBorderPrefab.SetActive(false);
    }
    void OnDestroy()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnLivesChanged -= UpdateHeartsUI;
        GameManager.Instance.OnDiamondsChanged -= UpdateDiamondUI;
        GameManager.Instance.OnTimerChanged -= UpdateTimerUI;
        GameManager.Instance.OnStateChanged -= HandleGameState;
    }

    private void UpdateHeartsUI(int currentLives)
    {
        if (currentLives < 0 || currentLives >= hearts.Length)
        {
            Debug.LogError("Invalid heart index: " + currentLives);
            return;
        }

        hearts[currentLives].BlinkNDisable();
    }

    private void UpdateDiamondUI(int collectedDiamonds, int totalDiamonds)
    {
        //play +1 particle effect
        //update collected diamonds value
        collectedDiamondstxt.text = collectedDiamonds.ToString();
        totalDiamondstxt.text = totalDiamonds.ToString();
    }

    private void UpdateTimerUI(float currentTime)
    {
        timerTxt.text = Mathf.CeilToInt(currentTime).ToString();
    }

    private void HandleGameState(GameState currentState)
    {
        if (GameManager.Instance.IsInState(GameState.Running)) return;

        gridParent.SetActive(false);

        switch (currentState)
        {
            case GameState.Won:
                ShowWin();
                break;
            case GameState.Lost:
                ShowLose();
                break;
        }
    }

    public void ShowFireBorder()
    {
        if (fireRoutine != null) StopCoroutine(fireRoutine);

        fireRoutine = StartCoroutine(FireBorderRoutine());
    }

    private IEnumerator FireBorderRoutine()
    {
        fireBorderPrefab.SetActive(true);
        yield return new WaitForSeconds(2f);
        fireBorderPrefab.SetActive(false);
        fireRoutine = null;
    }

    public void StartTimerBlink()
    {
        Debug.Log("🔥 TIMER BLINK CALLED");

        if (timerBlinkTween != null && timerBlinkTween.IsActive())
            return;

        timerBlinkTween = timerImage
            .DOColor(Color.red, 0.4f)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopTimerBlink()
    {
        timerBlinkTween?.Kill();

        if (timerImage != null) timerImage.color = Color.white;
    }

    public void RestartGame()
    {
        AudioManager.Instance.PlayWrongClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    private void ShowWin()
    {
        youWinUI.SetActive(true);
        restartButton.SetActive(true);
        quitButton.SetActive(true);

        StopTimerBlink();

        AudioManager.Instance.PlayWin();

        RectTransform rect = youWinUI.GetComponent<RectTransform>();

        rect.localScale = Vector3.zero;
        rect.rotation = Quaternion.identity;

        Sequence seq = DOTween.Sequence();

        // Slowly grow while spinning for 2 seconds
        seq.Append(
            rect.DOScale(0.85f, 2f)
                .SetEase(Ease.OutSine));

        seq.Join(
            rect.DORotate(
                new Vector3(0, 0, 1440),   // 2 smooth rotations
                2f,
                RotateMode.FastBeyond360)
            .SetEase(Ease.Linear));

        // BANG onto the screen
        seq.Append(
            rect.DOScale(1.25f, 0.08f)
                .SetEase(Ease.OutExpo));

        // Settle back
        seq.Append(
            rect.DOScale(1f, 0.18f)
                .SetEase(Ease.OutBack));

        // Nice impact
        seq.Append(
            rect.DOPunchScale(
                Vector3.one * 0.12f,
                0.25f,
                10,
                0.8f));

        // Idle breathing
        seq.Append(
            rect.DOScale(1.03f, 0.8f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo));
    }
    private void ShowLose()
    {
        youLoseUI.SetActive(true);
        restartButton.SetActive(true);
        quitButton.SetActive(true);

        StopTimerBlink();

        AudioManager.Instance.PlayLose();

        RectTransform rect = youLoseUI.GetComponent<RectTransform>();

        Vector2 finalPos = rect.anchoredPosition;

        // Start way above
        rect.anchoredPosition = finalPos + Vector2.up * 15f;
        rect.localScale = Vector3.one;

        Sequence seq = DOTween.Sequence();

        // Long visible fall
        seq.Append(
            rect.DOAnchorPos(finalPos, 0.45f)
                .SetEase(Ease.InQuad));

        // First bounce
        seq.Append(
            rect.DOAnchorPos(finalPos + Vector2.up * 15f, 0.30f)
                .SetEase(Ease.OutQuad));

        seq.Append(
            rect.DOAnchorPos(finalPos, 0.5f)
                .SetEase(Ease.InQuad));

        // Tiny final bounce
        seq.Append(
            rect.DOAnchorPos(finalPos + Vector2.up * 3f, 0.3f)
                .SetEase(Ease.OutQuad));

        seq.Append(
            rect.DOAnchorPos(finalPos, 0.15f)
                .SetEase(Ease.InQuad));
    }
}