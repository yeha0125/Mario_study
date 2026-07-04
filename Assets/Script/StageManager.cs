using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    public int playerLives = 3;
    public GameObject lifeDisplayPanel;
    public TextMeshProUGUI lifeText;
    public PlayerMove playerMove;

    void Awake()
    {
        if (instance == null) instance = this;
        
        if (playerMove == null)
            playerMove = FindFirstObjectByType<PlayerMove>();
    }

    void Start()
    {
        if (lifeDisplayPanel != null) lifeDisplayPanel.SetActive(false);
    }

    public void ReduceLife()
    {
        playerLives--;

        if (playerLives >= 0)
            StartCoroutine(ShowLifeScreenSequence());
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private System.Collections.IEnumerator ShowLifeScreenSequence()
    {
        // 1. 검은 화면 UI 활성화 및 목숨 개수 갱신
        if (lifeText != null) lifeText.text = "x  " + playerLives;
        if (lifeDisplayPanel != null) lifeDisplayPanel.SetActive(true);

        // 2. 플레이어 시작 지점으로 돌려놓음
        if (playerMove != null) playerMove.Respawn();

        // 3. 2초 대기
        yield return new WaitForSeconds(2f);

        // 4. 검은 화면 끄고 재시작
        if (lifeDisplayPanel != null) lifeDisplayPanel.SetActive(false);
    }
}