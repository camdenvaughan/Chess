using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState { Init, Play, Promotion, Pause, Finished }

    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;
    [SerializeField] private ChessUIManager uIManager;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject pauseButton;


    private PieceCreator pieceCreator;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer activePlayer;
    private GameState state;

    private int possibleMoves;
    private string displayNotation;


    private void Awake()
    {
        SetDependencies();
        CreatePlayers();
    }

    private void SetDependencies()
    {
        pieceCreator = GetComponent<PieceCreator>();
    }

    private void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    void Start()
    {
        StartNewGame();
    }

    private void StartNewGame()
    {
        uIManager.HideUI();
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        SetGameState(GameState.Play);
        
    }

    public void RestartGame()
    {
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        StartNewGame();
    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }

    private void SetGameState(GameState state)
    {
        this.state = state;
    }

    public bool IsGameInProgress()
    {
        return state == GameState.Play;
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

    public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    public bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

    public void EndTurn(string notation)
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished(notation))
            EndGameCheckmate();
        else
        {
            ChangeActiveTeam();
            if (CheckIfStalemate())
                EndGameStalemate();
            Debug.Log(displayNotation);
            cameraController.MoveCamera();
        }
            
    }
    private bool CheckIfStalemate()
    {
        possibleMoves = 0;
        activePlayer.GenerateAllPossibleMoves();
        foreach (var piece in activePlayer.activePieces)
        {
            RemoveMovesEnablingAttackOnPieceOfType<King>(piece);
            possibleMoves += piece.avaliableMoves.Count;
        }
        if (possibleMoves == 0)
            return true;
        return false;
    }

    private bool CheckIfGameIsFinished(string notation)
    {
        displayNotation = notation;
        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
        if(kingAttackingPieces.Length > 0 )
        {
            ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackOnPiece<King>(activePlayer, attackedKing);

            int avalaibleKingMoves = attackedKing.avaliableMoves.Count;
            if(avalaibleKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!canCoverKing)
                {
                    displayNotation = notation + "#";
                    return true;
                }
                    
            }
            displayNotation = notation + "+";
        }
        return false;
    }

    public bool CheckPromotion()
    {
        foreach (var piece in activePlayer.activePieces)
        {
            if (piece.GetType() == typeof (Pawn))
            {
                int endOfBoardYCoord = piece.team == TeamColor.White ? Board.BOARD_SIZE - 1 : 0;
                if (piece.occupiedSquare.y == endOfBoardYCoord)
                    return true;
            }
        }
        return false;
    }

    public string CheckDisambiguatingMoves(Piece piece, Vector2Int oldCoords)
    {       
        string notation = "";
        
        List<Piece> activePieces = new List<Piece>();
        List<Piece> disambiguousPieces = new List<Piece>();

        foreach (var p in activePlayer.activePieces)
        {
           if (p.CompareTag(piece.tag))
                activePieces.Add(p);
        }

         foreach (var p in activePieces)
         {
  
            foreach (var move in p.avaliableMoves.ToList())
             {
                
                 if (move == piece.occupiedSquare)
                 {
                    disambiguousPieces.Add(p);
                 }
             } 
         }
        if (disambiguousPieces.Count > 1)
        {
            foreach (var p in disambiguousPieces.ToList())
            {
                if (p.occupiedSquare.y == oldCoords.y)
                {
                    notation += (board.XCoordToLetter(oldCoords.x));
                }

                if (p.occupiedSquare.x == oldCoords.x)
                {
                    Debug.Log(oldCoords.y);
                    notation += (oldCoords.y + 1).ToString();
                }
            }
        } 
        return notation;
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }


    private void EndGameCheckmate()
    {
        uIManager.OnGameFinished(activePlayer.team.ToString());
        SetGameState(GameState.Finished);
    }

    private void EndGameStalemate()
    {
        uIManager.Stalemate();
        SetGameState(GameState.Finished);
    }

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(GetOpponentToPlayer(activePlayer), piece);
    }
    public bool IsActiveTeamWhite()
    {
        if (activePlayer == whitePlayer)
            return true;
        else
            return false;
    }



    public void ShowPauseScreen()
    {
        pauseUI.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void HidePauseScreen()
    {
        pauseUI.SetActive(false);
        pauseButton.SetActive(true);
    }
    public void PauseGame()
    {
        SetGameState(GameState.Pause);
    }

    public void SetToPromotionState()
    {
        SetGameState(GameState.Promotion);
    }
    public void ResumeGame()
    {
        SetGameState(GameState.Play);
    }
}

