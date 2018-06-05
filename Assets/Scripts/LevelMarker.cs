using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelMarker : MonoBehaviour
{
    [SerializeField] [Range(3, 20)] private int _boardWidth = 4;

    [SerializeField] [Range(3, 10)] private int _boardHeight = 4;

    [SerializeField] [Range(0, 120)] private int _timeAttackTime;

    [SerializeField] private int _redObjective = 0;
    [SerializeField] private int _purpleObjective = 0;
    [SerializeField] private int _yellowObjective = 0;
    [SerializeField] private int _greenObjective = 0;
    [SerializeField] private int _blueObjective = 0;

    [SerializeField] private int _objectiveScore = 0;


    public int LevelIndex = 0;

    private readonly ProgressHandler _progressHandler = new ProgressHandler();

    private void Awake()
    {
        _progressHandler.Load();


        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            if (LevelIndex <= _progressHandler.GetLevelProgress())
            {
                SceneManager.LoadSceneAsync("GameBoard").completed += operation =>
                {
                    FindObjectOfType<Board>().Width = _boardWidth;
                    FindObjectOfType<Board>().Height = _boardHeight;
                    FindObjectOfType<Board>().TimeAttackTime = _timeAttackTime;

                    if (_redObjective > 0)
                        FindObjectOfType<Board>().MissionRed = _redObjective;
                    if (_purpleObjective > 0)
                        FindObjectOfType<Board>().MissionPurple = _purpleObjective;
                    if (_yellowObjective > 0)
                        FindObjectOfType<Board>().MissionYellow = _yellowObjective;
                    if (_greenObjective > 0)
                        FindObjectOfType<Board>().MissionGreen = _greenObjective;
                    if (_blueObjective > 0)
                        FindObjectOfType<Board>().MissionBlue = _blueObjective;

                    FindObjectOfType<Board>().ObjectiveScore = _objectiveScore;

                    FindObjectOfType<Board>().CurrentLevelIndex = LevelIndex;
                };
            }
        });
    }
}