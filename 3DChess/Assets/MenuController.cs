using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private PieceCreator pieceCreator;
    [Header("Board Creation")]
    [SerializeField] Board board;
    [SerializeField] private BoardLayout startingBoardLayout;
    [Header("Menu UI")]
    [SerializeField] GameObject menuUI;
    [SerializeField] GameObject Canvas;



    private void Awake()
    {
        SetDependencies();
    }
    private void SetDependencies()
    {
        pieceCreator = GetComponent<PieceCreator>();
    }
    private void Start()
    {
        CreatePiecesFromLayout(startingBoardLayout);
    }
    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoods = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoods, team, type);
        }
    }
    private void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

    }

    public void ChangeUIScreen(GameObject nextUI)
    {
        menuUI.SetActive(false);
        nextUI.SetActive(true);
    }
    public void Back(GameObject thisUI)
    {
        thisUI.SetActive(false);
        menuUI.SetActive(true);
    }
}
