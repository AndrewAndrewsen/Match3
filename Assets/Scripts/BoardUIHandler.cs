using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardUIHandler : MonoBehaviour
{
    private Text[] _boardTexts;
    private int _goalScore;
    private int _goalChain;

    public void Setup(bool timeAttack, int goalScore, int goalChain)
    {
        _boardTexts = GetComponentsInChildren<Text>().ToArray();

        _goalScore = goalScore;
        _goalChain = goalChain;

        if (!timeAttack)
            _boardTexts[2].enabled = false;
    }

    public void UpdatePanel(bool timeAttack, int score, int chain)
    {
        _boardTexts[0].text = string.Format("Score: {0} / {1}", score, _goalScore);

        if (timeAttack)
            _boardTexts[1].text = string.Format("Chain: {0} / {1}", chain, _goalChain);
        else
            _boardTexts[1].text = string.Format("Chain: {0}", chain);
    }

    public void UpdateTime(float time)
    {
        _boardTexts[2].text = string.Format("Time: {0}", Mathf.Round(time));
    }
}