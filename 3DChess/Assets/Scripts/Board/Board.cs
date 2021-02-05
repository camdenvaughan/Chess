using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(SquareSelectorCreator))]
public class Board : MonoBehaviour
{
    public const int BOARD_SIZE = 8;


    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;
    [SerializeField] private PromotionPieceManager promotionManager;
    [SerializeField] private GameObject audioManagerPrefab;
    private ChessNotationManager chessNotator;


    private Piece[,] grid;
    private Piece selectedPiece;
    private ChessGameController chessController;
    private SquareSelectorCreator squareSelector;
    private AudioManager audioManager;


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
        if (FindObjectOfType<AudioManager>() == null)
            Instantiate(audioManagerPrefab);
        audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
    }
    public void SetDependencies()
    {
        if (FindObjectOfType<AudioManager>() == null)
            Instantiate(audioManagerPrefab);
        audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
    }

    public void SetUIDependencies(ChessNotationManager chessNotationManager)
    {
        chessNotator = chessNotationManager;
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
        chessNotator.AddPromotionNotation(type);
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
        chessNotator.NotateSquareCoord(coords, piece);
        TryToTakeOppositePiece(coords);
        UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
        selectedPiece.MovePiece(coords);
        lastMovedPiece = selectedPiece;
        DeselectPiece();
        if (chessController.IsTeamTurnActive(TeamColor.White))
            chessNotator.UpdateMoveNumber();
        if (!chessController.CheckPromotion())
            chessController.EndTurn();
        PlayPieceSound();
    }

    private void PlayPieceSound()
    {
        audioManager.Play("move");
    }

    private void TryToTakeOppositePiece(Vector2Int coords)
    {

        Piece piece = GetPieceOnSquare(coords);
        if (piece != null && !selectedPiece.IsFromSameTeam(piece))
        {
            TakePiece(piece);
            
        }
        else if (possibleEnPassant != null && coords == possibleEnPassant)
        {
            TakePiece(passantPawn);
            chessNotator.AddPassantNotation();
            
        }
        
    }

    private void TakePiece(Piece piece)
    {
        if (piece)
        {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessController.OnPieceRemoved(piece);
            chessNotator.AddCaptureNotation();
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
    public void CreateCastlingNotation(bool isCastledKingSide)
    {
        if (isCastledKingSide)
        {
            // Send KingSide Notation
            chessNotator.Castle("0-0");
        }
        else if (!isCastledKingSide)
        {
            // Send QueenSide Notation
            chessNotator.Castle("0-0-0");
        }

    }
    public void SetPieceOnBoard(Vector2Int coords, Piece piece)
    {
        if (CheckIfCoordsAreOnBoard(coords))
            grid[coords.x, coords.y] = piece;
    }

}

