using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Rendering;
using System;
using UnityEngine.InputSystem.Android;
using System.Linq;

public class BoggleDirector : MonoBehaviour
{
    int maxCol = 4;
    int maxRow = 4;
    [SerializeField] BoggleCellView cellPrefab;
    [SerializeField] RectTransform boggleGridRect;
 
    char[,] data = 
    { 
        {'Q', 'V', 'S', 'M'},
        {'S', 'S', 'K', 'Z'},
        {'E', 'R', 'T', 'O'},
        {'B', 'U', 'N', 'A'}
    };
    string[] answer =
    {
        "unrest", "best", "ant", "nurse", "true"
    };

    string ans = "";
    
    List<BoggleCellView> path = new List<BoggleCellView>();
    bool isDrag = false;

    private void Start()
    {
        SetBoard(data);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) || !isDrag)
        {
            LeavePath();
        }
    }

    private void SetBoard(char[,] data)
    {
        for (int col = 0; col < maxCol; col++)
        {
            for (int row  = 0; row < maxRow; row++)
            {
                BoggleCellView tmp = Instantiate(cellPrefab, boggleGridRect);
                tmp.SetCell(col, row, data[col, row]);
                tmp.CellPointerEnter += AddPath;
                tmp.CellPointerDown += EnterPath;
                tmp.CellPointerDown += AddPath;
            }
        } 
    }

    private void EnterPath(BoggleCellView cell)
    {
        isDrag = true;
    }

    private void AddPath(BoggleCellView cell)
    {
        if (isDrag && !cell.IsSelected) // 드래그중 + 이미 선택된게 아니면
        {
            if (path.Count > 0) //이전에 path에 들어온 게 있으면 이전것과 비교
            {
                BoggleCellView peek = path[path.Count - 1];
                int x = peek.Col, y = peek.Row, new_x = cell.Col, new_y = cell.Row;
                if (Math.Abs(x - new_x) <= 1 && Math.Abs(y - new_y) <= 1) //이전에 path에 들어온 애랑 인접한지 아닌지
                {
                    path.Add(cell);
                    cell.SetSelected();
                    ans += data[cell.Col, cell.Row];
                }
            }
            else // 이전에 path에 들어온 게 없으면 바로 추가
            {
                path.Add(cell);
                cell.SetSelected();
                ans += data[cell.Col, cell.Row];
            }
        }
    }

    private void LeavePath()
    {
        isDrag = false;
        if (answer.Contains(ans))
        {

        }
        for (int i = 0; i < path.Count; i++)
        {
            path[i].UnSelected();
        }
    }

}
