using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaroGameManager : MonoBehaviour
{
    // Enum định nghĩa trạng thái của ô cờ
    public enum CellState
    {
        Empty = 0,
        X = 1,
        O = 2
    }

    public int boardWidth = 10;
    public int boardHeight = 10;

    // Các prefab và components
    public GameObject cellPrefab;
    public Transform boardTransform;
    public Button restartButton;

    // Mảng lưu trạng thái ô cờ dùng enum thay vì số nguyên
    private CellState[,] boardState;

    // Các sprites cho X và O
    public Sprite xSprite;
    public Sprite oSprite;

    // Người chơi hiện tại: true = X, false = O
    private bool isPlayerX = true;

    // Lưu trữ các ô cờ đã tạo
    private Cell[,] cells;

    // Biến để kiểm tra trò chơi đã kết thúc hay chưa
    private bool gameOver = false;

    public TextMeshProUGUI gameStatusText;

    void Start()
    {
        InitializeGame();
        restartButton.onClick.AddListener(RestartGame);
    }

    void InitializeGame()
    {
        // Khởi tạo mảng lưu trạng thái và các ô cờ
        boardState = new CellState[boardWidth, boardHeight];
        cells = new Cell[boardWidth, boardHeight];

        // Xóa các ô cờ cũ (nếu có)
        foreach (Transform child in boardTransform)
        {
            Destroy(child.gameObject);
        }

        // Tạo các ô cờ mới
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                // Tạo ô cờ và thiết lập vị trí
                GameObject cellObject = Instantiate(cellPrefab, boardTransform);
                RectTransform rectTransform = cellObject.GetComponent<RectTransform>();

          

                // Lưu thông tin và thêm sự kiện click
                Cell cell = cellObject.GetComponent<Cell>();
                cell.Initialize(x, y, this);
                cells[x, y] = cell;

                // Đặt giá trị ban đầu cho ô là trống
                boardState[x, y] = CellState.Empty;
            }
        }

        // Thiết lập lại các giá trị
        isPlayerX = true;
        gameOver = false;
        UpdateGameStatus();
    }

    public void OnCellClicked(int x, int y)
    {
        // Kiểm tra xem trò chơi đã kết thúc chưa hoặc ô đã được đánh chưa
        if (gameOver || boardState[x, y] != CellState.Empty)
            return;

        // Cập nhật trạng thái ô theo người chơi hiện tại
        boardState[x, y] = isPlayerX ? CellState.X : CellState.O;

        // Cập nhật hiển thị
        cells[x, y].UpdateDisplay(isPlayerX ? xSprite : oSprite);

        // Kiểm tra thắng/thua
        if (CheckWin(x, y))
        {
            gameStatusText.text = "Người chơi " + (isPlayerX ? "X" : "O") + " thắng!";
            gameOver = true;
            return;
        }

        // Kiểm tra hòa
        if (CheckDraw())
        {
            gameStatusText.text = "Hòa!";
            gameOver = true;
            return;
        }

        // Chuyển lượt người chơi
        isPlayerX = !isPlayerX;
        UpdateGameStatus();
    }

    private void UpdateGameStatus()
    {
        gameStatusText.text = "Lượt người chơi: " + (isPlayerX ? "X" : "O");
    }

    private bool CheckDraw()
    {
        // Kiểm tra xem tất cả các ô đều đã được đánh chưa
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (boardState[x, y] == CellState.Empty)
                    return false; // Còn ô trống, chưa hòa
            }
        }

        return true; // Không còn ô trống, hòa
    }
    private static readonly Vector2Int[] directions = new Vector2Int[]
{
    new (1, 0),   // Ngang →
    new (0, 1),   // Dọc ↓
    new (1, 1),   // Chéo xuống ↘
    new (1, -1)   // Chéo lên ↗
};
    private bool CheckWin(int x, int y)
    {
        CellState cellState = boardState[x, y];

        foreach (Vector2Int dir in directions)
        {
            int count = 1;

            // Hướng thuận
            count += CountSameInDirection(x, y, dir, cellState);

            // Hướng ngược
            count += CountSameInDirection(x, y, -dir, cellState);

            // Nếu đủ 5 quân, kiểm tra bị chặn
            if (count >= 5)
            {
                bool isForwardBlocked = IsBlockedAtEnd(x, y, dir, cellState);
                bool isBackwardBlocked = IsBlockedAtEnd(x, y, -dir, cellState);

                if (!(isForwardBlocked && isBackwardBlocked))
                    return true; // Thắng nếu không bị chặn cả hai đầu
            }
        }

        return false;
    }

    private int CountSameInDirection(int startX, int startY, Vector2Int dir, CellState cellState)
    {
        int count = 0;
        int x = startX + dir.x;
        int y = startY + dir.y;

        while (IsValidPosition(x, y) && boardState[x, y] == cellState)
        {
            count++;
            x += dir.x;
            y += dir.y;
        }

        return count;
    }

    private bool IsBlockedAtEnd(int startX, int startY, Vector2Int dir, CellState player)
    {
        int x = startX + dir.x;
        int y = startY + dir.y;

        while (IsValidPosition(x, y) && boardState[x, y] == player)
        {
            x += dir.x;
            y += dir.y;
        }

        // Nếu ra khỏi biên => coi như bị chặn
        if (!IsValidPosition(x, y))
            return true;

        // Nếu ô kế tiếp là quân địch => bị chặn
        CellState cell = boardState[x, y];
        return cell != CellState.Empty && cell != player;
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
    }

    public void RestartGame()
    {
        InitializeGame();
    }
}