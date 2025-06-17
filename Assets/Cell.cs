using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    private int x;
    private int y;
    private CaroGameManager gameManager;
    private Image image;
    
    public void Initialize(int xPos, int yPos, CaroGameManager manager)
    {
        x = xPos;
        y = yPos;
        gameManager = manager;
        image = GetComponent<Image>();
        // Thêm sự kiện click
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    
    private void OnClick()
    {
        gameManager.OnCellClicked(x, y);
    }
    
    public void UpdateDisplay(Sprite sprite)
    {
        image.sprite = sprite;
        image.color = Color.white;
    }
}
