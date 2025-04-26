using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] GamePopup gamePopup;
    [SerializeField] TMP_Text healthCounter;
    [SerializeField] TMP_Text scoreCounter;
    [SerializeField] TMP_Text levelCompleteTXT;
    [SerializeField] TMP_Text getReadyTXT;
    [SerializeField] TMP_Text gameOverTXT;
    private int score;

    void Start()
    {
        gamePopup.Close();
        levelCompleteTXT.enabled = false;
        getReadyTXT.enabled = false;
        gameOverTXT.enabled = false;

        healthCounter.text = "50";
        scoreCounter.text = PlayerPrefs.GetInt("runningScore", 0).ToString();
        score = PlayerPrefs.GetInt("runningScore", 0);
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.TOGGLE_GAME_POPUP, ToggleGamePopup);
        Messenger<float>.AddListener(GameEvent.PLAYER_HEALTH_UPDATE, PlayerHealthUpdate);
        Messenger<int>.AddListener(GameEvent.PLAYER_SCORE_UPDATE, PlayerScoreUpdate);
        Messenger.AddListener(GameEvent.CARRY_OVER_SCORE, PlayerScoreLevelCarryover);
        Messenger.AddListener(GameEvent.LEVEL_COMPLETE_TXT, LevelCompleteTXT);
        Messenger.AddListener(GameEvent.GET_READY_TXT, GetReadyTXT);
        Messenger.AddListener(GameEvent.GAME_OVER_TXT, GameOverTXT);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.TOGGLE_GAME_POPUP, ToggleGamePopup);
        Messenger<float>.RemoveListener(GameEvent.PLAYER_HEALTH_UPDATE, PlayerHealthUpdate);
        Messenger<int>.RemoveListener(GameEvent.PLAYER_SCORE_UPDATE, PlayerScoreUpdate);
        Messenger.RemoveListener(GameEvent.CARRY_OVER_SCORE, PlayerScoreLevelCarryover);
        Messenger.RemoveListener(GameEvent.LEVEL_COMPLETE_TXT, LevelCompleteTXT);
        Messenger.RemoveListener(GameEvent.GET_READY_TXT, GetReadyTXT);
        Messenger.RemoveListener(GameEvent.GAME_OVER_TXT, GameOverTXT);
    }

    void PlayerHealthUpdate(float value)
    {
        healthCounter.text = value.ToString();
    }

    void PlayerScoreUpdate(int value)
    {
        score += value;
        scoreCounter.text = score.ToString();
    }

    public void ToggleGamePopup()
    {
        if (gamePopup.isActiveAndEnabled)
        {
            gamePopup.Close();
        } else {
            gamePopup.Open();
        }
    }

    public void PlayerScoreLevelCarryover()
    {
        PlayerPrefs.SetInt("runningScore", score);

        if (score > PlayerPrefs.GetInt("highScore"))
        {
            PlayerPrefs.SetInt("highScore", score);
        }
    }

    private void LevelCompleteTXT()
    {
        if (levelCompleteTXT.enabled)
        {
            levelCompleteTXT.enabled = false;
        }
        else
        {
            levelCompleteTXT.enabled = true;
        }
    }

    private void GetReadyTXT()
    {
        if (getReadyTXT.enabled)
        {
            getReadyTXT.enabled = false;
        }
        else
        {
            getReadyTXT.enabled = true;
        }
    }

    private void GameOverTXT()
    {
        if (gameOverTXT.enabled)
        {
            gameOverTXT.enabled = false;
        }
        else
        {
            gameOverTXT.enabled = true;
        }
    }
}
