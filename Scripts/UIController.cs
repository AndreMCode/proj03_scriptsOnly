using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] TMP_Text healthCounter;
    [SerializeField] TMP_Text scoreCounter;
    private int score;

    void Start()
    {
        healthCounter.text = "50";
        scoreCounter.text = "0";
        score = 0;
    }

    void OnEnable()
    {
        Messenger<float>.AddListener(GameEvent.PLAYER_HEALTH_UPDATE, PlayerHealthUpdate);
        Messenger<int>.AddListener(GameEvent.PLAYER_SCORE_UPDATE, PlayerScoreUpdate);
    }

    void OnDisable()
    {
        Messenger<float>.RemoveListener(GameEvent.PLAYER_HEALTH_UPDATE, PlayerHealthUpdate);
        Messenger<int>.RemoveListener(GameEvent.PLAYER_SCORE_UPDATE, PlayerScoreUpdate);
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
}
