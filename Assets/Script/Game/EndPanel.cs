using System.Linq;
using TMPro;
using UnityEngine;

public class EndPanel : MonoBehaviour
{
    public static EndPanel Instance;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI teamWinText;
    [SerializeField] private TextMeshProUGUI mvpNameText;
    [SerializeField] private TextMeshProUGUI mvpPointNumberText;
    [SerializeField] private Canvas canvas;

    private Vector3 hiddenPosition;
    private Vector3 centerPosition;

    private bool isEndMenuActive = false;

    private void Start()
    {
        Instance = this;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform panelRect = endPanel.GetComponent<RectTransform>();

        float offScreenY = canvasRect.rect.height / 2 + panelRect.rect.height / 2;
        hiddenPosition = new Vector3(0, offScreenY, 0);
        centerPosition = Vector3.zero;

        endPanel.GetComponent<RectTransform>().anchoredPosition = hiddenPosition;
    }

    public void ShowWin(BasketTeam team)
    {
        if (isEndMenuActive == true) return;
        isEndMenuActive = true;
        teamWinText.text = team.teamName.ToString().ToUpper() + " TEAM";
        var mvpPokemon = team.pokeTeam.OrderByDescending(player => player.pointScored).First();
        mvpNameText.text = mvpPokemon.actualPokemon.pokemonName;
        mvpPointNumberText.text = mvpPokemon.pointScored.ToString() + " Points"; 
        GameManager.Instance.CameraManager.SetNewLookAtTransform(mvpPokemon.transform, new Vector3(0, 5, -8), new Vector3(0, 0.1f));
        LeanTween.move(endPanel.GetComponent<RectTransform>(), centerPosition, 1.2f).setEaseOutSine();
        LeanTween.delayedCall(10f, () =>
        {
            SceneTransitor.Instance.LoadScene(1);
        });
    }
}