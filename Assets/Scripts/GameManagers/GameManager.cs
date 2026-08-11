using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<GameState> OnStateChanged;
    public event Action<int> OnLivesChanged;
    public event Action<int, int> OnDiamondsChanged;
    public event Action<float> OnTimerChanged;
    [SerializeField] private UIManager UIManager;


    [Header("Timer")]
    public float CurrentTime { get; private set; }
    [SerializeField] private float maxTime;

    [Header("Lives")]
    public int CurrentLives { get; private set; }
    [SerializeField] private int maxLives;
    private bool timeWarningStarted = false;

    [Header("Diamonds")]
    public int TotalDiamonds { get; private set; }
    public int CollectedDiamonds { get; private set; }
    public GameState CurrentState { get; private set; }

    void Start()
    {
        CurrentLives = maxLives;
        CurrentTime = maxTime;
        CurrentState = GameState.Running;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (!IsInState(GameState.Running)) return;

        CurrentTime = Mathf.Max(0f, CurrentTime - Time.deltaTime);
        OnTimerChanged?.Invoke(CurrentTime);

        if (CurrentTime <= 10f && !timeWarningStarted)
        {
            timeWarningStarted = true;
            AudioManager.Instance?.StartTimerWarning();
            UIManager.StartTimerBlink();
        }

        if (CurrentTime <= 0)
        {
            Debug.Log("Timer Ran out");
            SetState(GameState.Lost);
        }

    }

    public void SetTotalDiamonds(int amount)
    {
        TotalDiamonds = amount;
        OnDiamondsChanged?.Invoke(CollectedDiamonds, TotalDiamonds);
    }

    public void TakeDamage()
    {
        if (!IsInState(GameState.Running)) return;

        CurrentLives--;
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives <= 0)
        {
            Debug.Log("Player is Dead");
            SetState(GameState.Lost);
        }
    }

    public void CollectDiamond()
    {
        if (!IsInState(GameState.Running)) return;

        CollectedDiamonds++;
        OnDiamondsChanged?.Invoke(CollectedDiamonds, TotalDiamonds);

        if (CollectedDiamonds >= TotalDiamonds)
        {
            SetState(GameState.Won);
        }
    }

    public bool IsInState(GameState state)
    {
        return CurrentState == state;
    }

    public void SetState(GameState state)
    {
        if (CurrentState == state)
        {
            return;
        }

        CurrentState = state;

        OnStateChanged?.Invoke(CurrentState);
    }
}
