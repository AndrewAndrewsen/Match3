using UnityEngine;
using UnityEngine.UI;


public class UIObjective : MonoBehaviour
{
    [SerializeField] private TileDefinition _tileDefinition;
    private Text _text;
    public int ObjectiveCount;

    public void Setup(int distance, TileDefinition tileDefinition)
    {
        _tileDefinition = tileDefinition;
        _text = GetComponentInChildren<Text>();

        _text.text = ObjectiveCount.ToString();

        if (ObjectiveCount > 0)
            transform.gameObject.SetActive(true);
    }

    public void UpdateObjective(int number)
    {
        ObjectiveCount -= number;

        if (ObjectiveCount < 0)
            ObjectiveCount = 0;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _text.text = ObjectiveCount.ToString();
    }

    public int GetTypeId()
    {
        return _tileDefinition.GetTypeId();
    }
}