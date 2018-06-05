using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ScreenHandler : MonoBehaviour
{
    private Text[] _textArray;

    public void Start()
    {
        _textArray = GetComponentsInChildren<Text>().ToArray();
    }

    public void ShowScreen(bool win, bool timeAttack, int score, int chain)
    {
        if (win)
            _textArray[0].text = "Winner!";
        else
        {
            _textArray[0].text = timeAttack ? "Game Over" : "Too bad!";
        }

        _textArray[1].text = string.Format("Score:\n{0}", score);
        _textArray[2].text = string.Format("Longest chain:\n{0}", chain);

        gameObject.SetActive(true);
    }
}