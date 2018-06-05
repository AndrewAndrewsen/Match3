using UnityEngine;

public class ProgressHandler
{
    private int _campaignScore;
    private int _timeAttackScore;
    private int _timeAttackChain;
    private int _levelProgress;


    public void Load()
    {
        _campaignScore = PlayerPrefs.GetInt("Score", 0);
        _timeAttackScore = PlayerPrefs.GetInt("TimeAttackScore", 0);
        _timeAttackChain = PlayerPrefs.GetInt("TimeAttackChain", 0);
        _levelProgress = PlayerPrefs.GetInt("Level", 0);
    }

    public void Save(int score, int level, int timeAttackScore, int timeAttackChain)
    {
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("TimeAttackScore", timeAttackScore);
        PlayerPrefs.SetInt("TimeAttackChain", timeAttackChain);

        _campaignScore = score;
        _timeAttackChain = timeAttackChain;
        _timeAttackScore = timeAttackScore;
        _levelProgress = level;
    }

    public int GetScore()
    {
        return _campaignScore;
    }

    public int GetLevelProgress()
    {
        return _levelProgress;
    }

    public int GetTimeAttackScore()
    {
        return _timeAttackScore;
    }

    public int GetTimeAttackChain()
    {
        return _timeAttackChain;
    }


    public static void SaveLevelProgress(int score, int levelProgress)
    {
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("Level", levelProgress);
    }

    public void SaveTimeAttackScore(int timeAttackScore, int timeAttackChain)
    {
        if (_timeAttackScore < timeAttackScore)
            PlayerPrefs.SetInt("TimeAttackScore", timeAttackScore);

        if (_timeAttackChain < timeAttackChain)
            PlayerPrefs.SetInt("TimeAttackChain", timeAttackChain);
    }
}