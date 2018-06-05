using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour

{
    private LevelMarker[] _levels;

    [SerializeField] private GameObject _levelHolder;

    [SerializeField] private Sprite _levelClosed;

    [SerializeField] private Sprite _levelOpen;

    [SerializeField] private Sprite _levelBeaten;

    [SerializeField] private LineRenderer _lineRenderer;


    [SerializeField] private Text _scoreText;

    private readonly ProgressHandler _progressHandler = new ProgressHandler();


    private void Start()
    {
        Time.timeScale = 1f;

        _levels = _levelHolder.GetComponentsInChildren<LevelMarker>().OrderBy(marker => marker.LevelIndex).ToArray();

        _progressHandler.Load();
        _scoreText.text = string.Format("Score: {0}", _progressHandler.GetScore());

        var levelProgress = _progressHandler.GetLevelProgress();

        var i = 0;
        _lineRenderer.positionCount = (levelProgress < _levels.Length) ? levelProgress + 1 : _levels.Length;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;

        foreach (var level in _levels)
        {
            if (i < levelProgress)
            {
                level.GetComponentInChildren<Image>().sprite =
                    _levelBeaten;
                _lineRenderer.SetPosition(i, level.transform.position);
            }

            if (i == levelProgress)
            {
                level.GetComponentInChildren<Image>().sprite = _levelOpen;
                _lineRenderer.SetPosition(i, level.transform.position);
            }

            if (i > levelProgress)
                level.GetComponentInChildren<Image>().sprite = _levelClosed;

            i++;
        }
    }

    public void StartTimeAttack()
    {
        SceneManager.LoadScene("GameBoard");
    }
}