using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState { Init, Play, Promotion, Pause, Finished }

    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;
    [SerializeField] private CameraController cameraController;
    private ChessNotationManager chessNotator;



    private PieceCreator pieceCreator;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer activePlayer;

    private Scene sceneUI;
    private UINavigator navigatorUI;

    


    private GameState state;

    private int possibleMoves;


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
        sceneUI = SceneManager.GetSceneByName("UI");
        SetDependenciesInUI();
        StartNewGame();
    }

    private void SetDependenciesInUI()
    {
        navigatorUI = GameObject.Find("Canvas").GetComponent<UINavigator>();
        chessNotator = GameObject.Find("Canvas").GetComponent<ChessNotationManager>();
        board.SetUIDependencies(chessNotator);
        navigatorUI.PauseStateChanged += OnPauseStateChanged;
    }

    private void StartNewGame()
    {
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        cameraController.SetCameraToSpin(false);
        SetGameState(GameState.Play);
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

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished())
            EndGameCheckmate();
        else
        {
            ChangeActiveTeam();
            if (CheckIfStalemate())
                EndGameStalemate();

            chessNotator.CombineNotation();
            if (PlayerPrefs.GetInt("isCameraFlipOn") == 0)
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

    private bool CheckIfGameIsFinished()
    {
        
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
                    chessNotator.AddCheckOrMate("#");
                    chessNotator.CombineNotation();
                    return true;
                }
            }
            chessNotator.AddCheckOrMate("+");
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



    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }


    private void EndGameCheckmate()
    {
        navigatorUI.OnGameFinished(activePlayer.team.ToString());
        cameraController.SetCameraToSpin(true);
        SetGameState(GameState.Finished);
    }

    private void EndGameStalemate()
    {
        navigatorUI.Stalemate();
        cameraController.SetCameraToSpin(true);
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
    public void SetToPromotionState()
    {
        SetGameState(GameState.Promotion);
    }
    public void OnPauseStateChanged(object source, EventArgs e)
    {
        GameState state = (this.state ==  GameState.Pause) ? GameState.Play : GameState.Pause;
        SetGameState(state);
    }
    public void ResumeGame()
    {
        SetGameState(GameState.Play);
    }
}

