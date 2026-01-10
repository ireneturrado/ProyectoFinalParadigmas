using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState
{
    Menu,
    Playing,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private EnemyFactory enemyFactory;

    [Header("Spawn")]
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Player")]
    public MonoBehaviour playerController;

    [Header("UI")]
    public GameObject playButton;
    public GameObject getReady;
    public GameObject gameOverImage;
    public GameObject winImage;
    public GameObject difficultyDropdown;
    public GameObject restartButton;

    [Header("Game State")]
    public GameState State { get; private set; } = GameState.Menu;
    public bool HasBeenDetected { get; private set; } = false;

    [Header("Global Alert (Guardian)")]
    public bool GlobalAlertActive { get; private set; } = false;
    [SerializeField] private float globalAlertDuration = 8f;
    private float globalAlertTimer = 0f;

    [Header("Coins")]
    public int totalCoins;
    private int collectedCoins = 0;
    public int CollectedCoins => collectedCoins;
    public int TotalCoins => totalCoins;

    private bool ended = false;

    [Header("SFX")]
    public GameObject winSfxPrefab;
    public GameObject loseSfxPrefab;

    void Awake()
    {
        // SINGELTON
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        StartCoroutine(InitUI());
    }

    IEnumerator InitUI()
    {
        // Espera 1 frame para que UIManager y TMP estén listos
        yield return null;

        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

        UIManager.Instance?.SetCoins(0, totalCoins);

        ShowReady();
    }

    void Update()
    {
        // Gestiona el temporizador de la alerta global
        if (GlobalAlertActive)
        {
            globalAlertTimer -= Time.unscaledDeltaTime;
            if (globalAlertTimer <= 0f)
            {
                GlobalAlertActive = false;
                Debug.Log("[ALERTA GLOBAL] FINALIZADA");
            }
        }
    }

    public void ShowReady()
    {
        ended = false;
        State = GameState.Menu;
        UIManager.Instance?.HideCoins();

        if (playButton) playButton.SetActive(true);
        if (getReady) getReady.SetActive(true);
        if (gameOverImage) gameOverImage.SetActive(false);
        if (winImage) winImage.SetActive(false);
        if (difficultyDropdown) difficultyDropdown.SetActive(true);
        if (restartButton) restartButton.SetActive(false);


        Time.timeScale = 0f;
        if (playerController) playerController.enabled = false;
    }

    public void Play()
    {
        Debug.Log("PLAY LLAMADO");

        ended = false;
        State = GameState.Playing;
        enemyFactory?.SpawnEnemies();
        HasBeenDetected = false;
        GlobalAlertActive = false;
        collectedCoins = 0;

        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        UIManager.Instance?.SetCoins(collectedCoins, totalCoins);

        if (playButton) playButton.SetActive(false);
        if (getReady) getReady.SetActive(false);
        if (gameOverImage) gameOverImage.SetActive(false);
        if (winImage) winImage.SetActive(false);
        if (difficultyDropdown) difficultyDropdown.SetActive(false);
        if (restartButton) restartButton.SetActive(false);


        Time.timeScale = 1f;
        if (playerController) playerController.enabled = true;
    }

    public void Win()
    {
        if (ended) return;
        if (winSfxPrefab != null)
        {
            Instantiate(winSfxPrefab, Vector3.zero, Quaternion.identity);
        }

        ended = true;
        State = GameState.Win;

        Time.timeScale = 0f;
        if (playerController) playerController.enabled = false;


        if (winImage) winImage.SetActive(true);
        if (restartButton) restartButton.SetActive(true);

    }

    public void Lose()
    {
        if (ended) return;
        ended = true;
        State = GameState.Lose;

        if (loseSfxPrefab != null)
        {
            Instantiate(loseSfxPrefab, Vector3.zero, Quaternion.identity);
        }

        Time.timeScale = 0f;

        if (playerController) playerController.enabled = false;
        if (gameOverImage) gameOverImage.SetActive(true);
        if (restartButton) restartButton.SetActive(true);

    }

    public void RestartGame()
    {
        // 1. Reset estado
        ended = false;
        State = GameState.Menu;
        HasBeenDetected = false;
        GlobalAlertActive = false;

        // 2. Reset monedas
        collectedCoins = 0;
        // UIManager.Instance?.SetCoins(collectedCoins, totalCoins);


        // 3. Reset jugador
        if (playerController != null)
        {
            playerController.enabled = false;

            if (playerSpawnPoint != null)
            {
                playerController.transform.position = playerSpawnPoint.position;
            }
        }

        // 5. Eliminar monedas
        foreach (var coin in GameObject.FindGameObjectsWithTag("Coin"))
        {
            Destroy(coin);
        }

        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        UIManager.Instance?.SetCoins(0, totalCoins);

        // 6. Volver a menú
        ShowReady();
    }

    public void ReportPlayerDetected()
    {
        if (State != GameState.Playing) return;
        HasBeenDetected = true;
    }

    public void TriggerGlobalAlert()
    {
        if (State != GameState.Playing) return;

        GlobalAlertActive = true;
        globalAlertTimer = globalAlertDuration;

        Debug.Log($"[ALERTA GLOBAL] ACTIVADA durante {globalAlertDuration} segundos");
    }

    public void CollectCoin()
    {
        collectedCoins++;

        // Si es la primera moneda, mostrar UI
        if (collectedCoins == 1)
        {
            UIManager.Instance?.ShowCoins();
        }

        UIManager.Instance?.SetCoins(collectedCoins, totalCoins);
        Debug.Log($"Monedas recogidas: {collectedCoins}/{totalCoins}");
    }

    public bool AllCoinsCollected()
    {
        return collectedCoins == totalCoins;
    }


}
