using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public class Board : MonoBehaviour
{
    public const int BOARD_SIZE = 8;
    private const string letters = "abcdefgh";

    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;
    [SerializeField] private PromotionPieceManager promotionManager;

    private Piece[,] grid;
    private Piece selectedPiece;
    private ChessGameController chessController;
    private SquareSelectorCreator squareSelector;
    [HideInInspector] public Piece lastMovedPiece;

    [HideInInspector] public Vector2Int possibleEnPassant;
    [HideInInspector] public Piece passantPawn;

    private void Awake()
    {
        squareSelector = GetComponent<SquareSelectorCreator>();
        CreateGrid();
    }

    public void SetDependencies(ChessGameController chessController)
    {
        this.chessController = chessController;
    }

    private void CreateGrid()
    {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + BOARD_SIZE / 2;
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + BOARD_SIZE / 2;
        return new Vector2Int(x, y);
    }

    public void OnGameRestarted()
    {
        selectedPiece = null;
        CreateGrid();
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        if (!chessController.IsGameInProgress())
            return;
        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);

        if (selectedPiece)
        {
            if (piece != null && selectedPiece == piece)
                DeselectPiece();
            else if (piece != null && selectedPiece != piece && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(piece);
            else if (selectedPiece.CanMoveTo(coords))
                OnSelectedPieceMoved(coords, selectedPiece);
        }
        else
        {
            if (piece != null && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(piece);
        }
    }

    public void CreatePromoteScreen(Piece piece)
    {
        chessController.SetToPromotionState();
        promotionManager.InitAndPlacePieces(piece);
    }



    public void PromotePiece(Piece promotionPiece, Type type)
    {
        TakePiece(promotionPiece);
        chessController.CreatePieceAndInitialize(promotionPiece.occupiedSquare, promotionPiece.team, type);
    }

    private void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
        List<Vector2Int> selection = selectedPiece.avaliableMoves;
        ShowSelectionSquares(selection);
    }
    private void ShowSelectionSquares(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < selection.Count; i++)
        {
            Vector3 position = CalculatePositionFromCoords(selection[i]);
            bool isSquareFree = GetPieceOnSquare(selection[i]) == null;
            squaresData.Add(position, isSquareFree);
        }
        squareSelector.ShowSelection(squaresData);
    }

    private void DeselectPiece()
    {
        selectedPiece = null;
        squareSelector.ClearSelection();
    }

    private void OnSelectedPieceMoved(Vector2Int coords, Piece piece)
    {
        Vector2Int oldCoords = piece.occupiedSquare;
        bool wasPieceTaken = TryToTakeOppositePiece(coords);
        UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
        selectedPiece.MovePiece(coords);
        lastMovedPiece = selectedPiece;
        DeselectPiece();
        if (!chessController.CheckPromotion())
            chessController.EndTurn(CreateChessNotation(coords, oldCoords, wasPieceTaken, piece));
        
    }

    private string CreateChessNotation(Vector2Int newCoords,Vector2Int oldCoords, bool wasPieceTaken, Piece piece)
    {
        // Convert Grid coords to Chess Coords
        int coordYValue = newCoords.y + 1;
        string notation = (XCoordToLetter(newCoords.x) + coordYValue.ToString());

        // Add x if a piece was taken
        if (wasPieceTaken)
            notation = ("x" + notation);

        // Check for Disambiguating Moves
        

        switch (piece.GetType().ToString())
        {
            case "Pawn":
                if (possibleEnPassant != null && newCoords == possibleEnPassant)
                {
                    notation = (XCoordToLetter(oldCoords.x) + notation);
                }

                break;
            case "King":
                Vector2Int kingSideCoords;
                Vector2Int queenSideCoords;
                if (piece.team == TeamColor.White)
                {
                    kingSideCoords = oldCoords + Vector2Int.right * 2;
                    queenSideCoords = oldCoords + Vector2Int.left * 2;
                } else
                {
                    kingSideCoords = oldCoords + Vector2Int.left * 2;
                    queenSideCoords = oldCoords + Vector2Int.right * 2;
                }
                if (newCoords == kingSideCoords)
                {
                    notation = ("0-0");
                    break;
                }
                else if (newCoords == queenSideCoords)
                {
                    notation = ("0-0-0");
                    break;
                } else
                {
                    notation = ("K" + notation);
                    break;
                }
            case "Bishop":
                notation = ("B" + notation);
                break;
            case "Knight":
                notation = ("N" + notation);
                break;
            case "Rook":
                notation = ("R" + notation);
                break;
            case "Queen":
                notation = ("Q" + notation);
                break;
            default:
                break;
        }
        return notation;
    }
    public string XCoordToLetter(int xCoord)
    {
        var coordXletter = "";
        return coordXletter += letters[xCoord % letters.Length];
    }

    private bool TryToTakeOppositePiece(Vector2Int coords)
    {

        Piece piece = GetPieceOnSquare(coords);
        if (piece != null && !selectedPiece.IsFromSameTeam(piece))
        {
            TakePiece(piece);
            return true;
        }
        else if (possibleEnPassant != null && coords == possibleEnPassant)
        {
            TakePiece(passantPawn);
            return true;
        } else
        return false;
    }

    private void TakePiece(Piece piece)
    {
        if (piece)
        {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessController.OnPieceRemoved(piece);
        }
    }


    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[newCoords.x, newCoords.y] = newPiece;
    }


    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if (CheckIfCoordsAreOnBoard(coords))
            return grid[coords.x, coords.y];
        return null;
    }

    public bool CheckIfCoordsAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE)
            return false;
        return true;
    }

    public bool HasPiece(Piece piece)
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if (grid[i, j] == piece)
                    return true;
            }
        }
        return false;
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece piece)
    {
        if (CheckIfCoordsAreOnBoard(coords))
            grid[coords.x, coords.y] = piece;
    }
}

