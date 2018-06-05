using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class Board : MonoBehaviour
{
    public int Width;
    public int Height;
    public GameObject Tile;

    [SerializeField] private BoardUIHandler _boardUiHandler;

    private const float DefaultTileSpacing = .72f;
    private const int DefaultNumTilesX = 7;

    private float _currentBoardScalar;
    private float _currentTileSpacing;


    public ScreenHandler TimeAttackEndScreen;
    public ScreenHandler ObjectivesEndScreen;

    [SerializeField] private GameObject _startScreen;

    private float _selectTilesPitch = 0.5f;


    [SerializeField] private UIObjective[] _objectives;

    [NonSerialized] public int MissionRed = 0;
    [NonSerialized] public int MissionPurple = 0;
    [NonSerialized] public int MissionYellow = 0;
    [NonSerialized] public int MissionGreen = 0;
    [NonSerialized] public int MissionBlue = 0;

    private int _stateRed = 0;
    private int _statePurple = 0;
    private int _stateYellow = 0;
    private int _stateGreen = 0;
    private int _stateBlue = 0;


    [NonSerialized] public int ObjectiveScore;
    [NonSerialized] public int CurrentLevelIndex = 0;

    public float TimeAttackTime = 60f;
    private bool _timeAttack = false;

    [SerializeField] private AudioSource _boardClearSound;
    private LineRenderer _lineRenderer;
    private int _score = 0;
    private Tile[,] _tiles;
    private readonly List<Tile> _chain = new List<Tile>();
    private int _longestChain = 0;
    private bool _gameStopped = false;


    public TileDefinition[] TileDefinitions;


    private readonly ProgressHandler _progressHandler = new ProgressHandler();

    private void Start()
    {
        _currentBoardScalar = (float) DefaultNumTilesX / Width;
        _currentTileSpacing = DefaultTileSpacing * _currentBoardScalar;
        transform.position = new Vector2(-(_currentTileSpacing * Width * .5f - (_currentTileSpacing * .5f)),
            transform.position.y + (_currentTileSpacing * .5f));

        _objectives[0].ObjectiveCount = MissionRed;
        _objectives[1].ObjectiveCount = MissionPurple;
        _objectives[2].ObjectiveCount = MissionGreen;
        _objectives[3].ObjectiveCount = MissionBlue;
        _objectives[4].ObjectiveCount = MissionYellow;

        _stateRed = MissionRed;
        _statePurple = MissionPurple;
        _stateYellow = MissionYellow;
        _stateGreen = MissionGreen;
        _stateBlue = MissionBlue;

        _progressHandler.Load();


        if (TimeAttackTime > 0)
        {
            _timeAttack = true;
            Time.timeScale = 0f;
            _gameStopped = true;

            _startScreen.SetActive(true);
        }


        TimeAttackEndScreen.Start();
        ObjectivesEndScreen.Start();
        _boardUiHandler.Setup(
            _timeAttack,
            (_timeAttack) ? _progressHandler.GetTimeAttackScore() : ObjectiveScore,
            (_timeAttack) ? _progressHandler.GetTimeAttackChain() : 0);


        var distance = 0;
        var definitionIndex = 0;
        foreach (var objective in _objectives)
        {
            objective.Setup(distance, TileDefinitions[definitionIndex]);

            if (objective.ObjectiveCount > 0)
            {
                distance++;
            }

            definitionIndex++;
        }


        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;

        _tiles = new Tile[Width, Height];

        FillBoard(!_timeAttack);
        UpdateScore();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        _gameStopped = false;
        _startScreen.SetActive(false);
    }

    public void LoadLevelMap()
    {
        SceneManager.LoadScene("LevelMap");
    }

    private void UpdateScore()
    {
        if (_gameStopped && !_timeAttack)
        {
            Time.timeScale = 0f;
            ObjectivesEndScreen.ShowScreen(false, _timeAttack, _score, _longestChain);
        }
        
        if (ObjectiveScore > 0)
        {
            var objectivesLeft = 0;
            foreach (var objective in _objectives)
                objectivesLeft += objective.ObjectiveCount;

            if (_score >= ObjectiveScore && objectivesLeft == 0)
            {
                ObjectivesEndScreen.ShowScreen(true, _timeAttack, _score, _longestChain);

                ProgressHandler.SaveLevelProgress(_progressHandler.GetScore() + _score,
                    (CurrentLevelIndex == _progressHandler.GetLevelProgress())
                        ? (CurrentLevelIndex + 1)
                        : _progressHandler.GetLevelProgress());

                Time.timeScale = 0f;
                _gameStopped = true;
            }

        
        }

        _boardUiHandler.UpdatePanel(_timeAttack, _score, _longestChain);
    }

    public void RestartTimeAttack()
    {
        SceneManager.LoadScene("GameBoard");
    }

    public void RestartLevel()
    {
        SceneManager.LoadSceneAsync("GameBoard").completed += operation =>
        {
            FindObjectOfType<Board>().Width = Width;
            FindObjectOfType<Board>().Height = Height;
            FindObjectOfType<Board>().TimeAttackTime = 0;
            FindObjectOfType<Board>().MissionRed = _stateRed;
            FindObjectOfType<Board>().MissionPurple = _statePurple;
            FindObjectOfType<Board>().MissionYellow = _stateYellow;
            FindObjectOfType<Board>().MissionGreen = _stateGreen;
            FindObjectOfType<Board>().MissionBlue = _stateBlue;
            FindObjectOfType<Board>().ObjectiveScore = ObjectiveScore;
            FindObjectOfType<Board>().CurrentLevelIndex = CurrentLevelIndex;

            Time.timeScale = 1f;
        };
    }

    private void Update()
    {
        if (_timeAttack && !_gameStopped)
        {
            TimeAttackTime -= Time.deltaTime;

            _boardUiHandler.UpdateTime(TimeAttackTime);

            if (TimeAttackTime < 0)
            {
                _gameStopped = true;
                Time.timeScale = 0f;
                TimeAttackEndScreen.ShowScreen(false, _timeAttack, _score, _longestChain);
                _progressHandler.SaveTimeAttackScore(_score, _longestChain);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            
            if (!_gameStopped)
                BreakChain();
        }
    }

    public void AddToChain(Tile tile)
    {
        var go = false;

        // Accept "on the fly" tile changes except if it's the same type.
        if (_chain.Count > 0 &&
            _chain[0].GetTileType() != tile.GetTileType())
        {
            if (Vector2.Distance(_chain[_chain.Count - 1].transform.position, (Vector2) tile.transform.position) >
                1.1f)
            {
                foreach (var tTile in _chain)
                    tTile.SetSelected(false);

                _chain.Clear();
                go = true;
            }
        }
        else
        {
            go = true;
        }

        // If the user is going back the same way, remove the tile from the chain.
        if (_chain.Count > 1 && tile.transform.position == _chain[_chain.Count - 2].transform.position)
        {
            _chain[_chain.Count-1].SetSelected(false);
            _chain.RemoveAt(_chain.Count - 1);
            CheckChain();
            _selectTilesPitch -= 0.1f;

            return;
        }

        // If the user tries to select a tile thats already inside the chain.
        foreach (var tTile in _chain)
        {
            if (tile.transform.position == tTile.transform.position)
                go = false;
        }

        // If there's only one tile and the chain, and the user selects another one. Empty the chain and add the new tile.
        if (_chain.Count == 1 &&
            _chain[0].GetTileType() != tile.GetTileType())
        {
            foreach (var tTile in _chain)
                tTile.GetComponent<Tile>().SetSelected(false);

            _chain.Clear();
            _selectTilesPitch = 0.5f;
            go = true;
        }


        if (go && !_gameStopped)
        {
            tile.SetSelected(true);
            _chain.Add(tile);

            CheckChain();


            tile.PlaySound(_selectTilesPitch);
            _selectTilesPitch += 0.1f;
        }
    }

    private bool CheckChain()
    {
        if (_chain.Count > 0)
        {
            var tempType = _chain[0].GetTileType();
            var lineRendererIndex = 0;
            Vector2 tempPos = _chain[0].transform.position;
            var broken = false;
            foreach (var tile in _chain)
            {
                _lineRenderer.positionCount = (lineRendererIndex + 1);
                _lineRenderer.SetPosition(lineRendererIndex, tile.transform.position);
                lineRendererIndex++;

                var tileType = tile.GetTileType();
                if (tempType != tileType)
                {
                    broken = true;
                }


                if (Vector2.Distance(tempPos, (Vector2) tile.transform.position) > (_currentBoardScalar * 1.1f))
                    broken = true;

                if (broken)
                {
                    foreach (var tTile in _chain)
                        tTile.GetComponent<Tile>().SetSelected(false);

                    _chain.Clear();


                    _lineRenderer.positionCount = 0;
                    _selectTilesPitch = 0.5f;
                    return false;
                }

                tempPos = tile.transform.position;
                tempType = tileType;
            }


            return true;
        }

        return false;
    }


    private void BreakChain()
    {
        if (CheckChain())
        {
            if (_chain.Count > 2)
            {
                _lineRenderer.positionCount = 0;
                _boardClearSound.Play();
    
                foreach (var tile in _chain)
                    tile.Kill();


                var tileType = _chain[0].GetTileType();

                foreach (var objective in _objectives)
                {
                    if (tileType == objective.GetTypeId())
                    {
                        objective.UpdateObjective(_chain.Count);
                    }
                }

                _score += 100 * (int) (_chain.Count * 1.5f);

                if (_longestChain < _chain.Count)
                    _longestChain = _chain.Count;

                UpdateScore();


                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        if (_tiles[x, y] != null && _tiles[x, y].IsKilled())
                        {
                            _tiles[x, y].Destroy(); // Destory when the "selected"-sound has ended.
                            _tiles[x, y] = null;
                        }
                    }
                }

                _chain.Clear();
                _selectTilesPitch = 0.5f;
                


                Tile[,] tempTiles = new Tile[Width, Height];
                var tempY = 0;
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        if (y == 0)
                            tempY = 0;

                        if (_tiles[x, y] != null)
                        {
                            tempTiles[x, tempY] = _tiles[x, y];
                            tempY++;
                        }
                    }
                }

                _tiles = tempTiles;

                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        if (_tiles[x, y] != null)
                        {
                            _tiles[x, y]
                                .AnimateTo((Vector2) transform.position +
                                           new Vector2(x * _currentTileSpacing, y * _currentTileSpacing));
                        }
                    }
                }

                
                FillBoard(true);
            }
        }

        foreach (var tTile in _chain)
            tTile.GetComponent<Tile>().SetSelected(false);


        _lineRenderer.positionCount = 0;
        _selectTilesPitch = 0.5f;
        _chain.Clear();
    }

    private void FillBoard(bool animate)
    {
        var i = 0;
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                if (_tiles[x, y] == null)
                {
                    Vector2 pos;
                    if (!animate)
                    {
                        pos = (Vector2) transform.position +
                              new Vector2((_currentTileSpacing * x), _currentTileSpacing * y);
                    }
                    else
                    {
                        pos = new Vector2(transform.position.x + (_currentTileSpacing * x),
                            ((i * 0.5f) + (y * _currentTileSpacing)));
                    }

                    _tiles[x, y] = Instantiate(Tile.gameObject).GetComponent<Tile>();
                    _tiles[x, y].transform.parent = gameObject.transform;
                    _tiles[x, y].GetComponent<Tile>()
                        .SetupTile(TileDefinitions[Random.Range(0, TileDefinitions.Length)]);

                    _tiles[x, y].transform.localScale = new Vector3(_currentBoardScalar, _currentBoardScalar, 0f);
                    _tiles[x, y].transform.position = pos;

                    if (animate)
                    {
                        _tiles[x, y].GetComponent<Tile>()
                            .AnimateTo((Vector2) transform.position +
                                       new Vector2(_currentTileSpacing * x, _currentTileSpacing * y));
                    }

                    i++;
                }
            }
        }

        if (!ValidateBoard())
        {
            if (_timeAttack)
            {
                foreach (var tile in _tiles)
                    Destroy(tile.gameObject);

                _tiles = new Tile[Width, Height];

                FillBoard(false);
            }
            else
            {
                _gameStopped = true;
                UpdateScore();
            }
        }
    }

    private bool ValidateBoard()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var type = _tiles[x, y].GetTileType();
                var sameCount = 0;

                if ((y - 1) > -1 && type == _tiles[x, y - 1].GetTileType())
                    sameCount++;
                if ((y + 1) < Height && type == _tiles[x, y + 1].GetTileType())
                    sameCount++;
                if ((x - 1) > -1 && type == _tiles[x - 1, y].GetTileType())
                    sameCount++;
                if ((x - 1) > -1 && (y - 1) > -1 && type == _tiles[x - 1, y - 1].GetTileType())
                    sameCount++;
                if ((x - 1) > -1 && (y + 1) < Height && type == _tiles[x - 1, y + 1].GetTileType())
                    sameCount++;
                if ((x + 1) < Width && type == _tiles[x + 1, y].GetTileType())
                    sameCount++;
                if ((x + 1) < Width && (y - 1) > -1 && type == _tiles[x + 1, y - 1].GetTileType())
                    sameCount++;
                if ((x + 1) < Width && (y + 1) < Height && type == _tiles[x + 1, y + 1].GetTileType())
                    sameCount++;

                if (sameCount > 1)
                    return true;
            }
        }

        return false;
    }
}