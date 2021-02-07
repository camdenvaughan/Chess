using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ChessPlayer
{
    public TeamColor team { get; set; }
    public Board board { get; set; }
    public List<Piece> activePieces { get; private set; }
    public ChessPlayer(TeamColor team, Board board)
    {
        this.board = board;
        this.team = team;
        activePieces = new List<Piece>();
    }

    public void AddPiece(Piece piece)
    {
        if (!activePieces.Contains(piece))
            activePieces.Add(piece);
    }
    public void RemovePiece(Piece piece)
    {
        if (activePieces.Contains(piece))
            activePieces.Remove(piece);
    }

    public void GenerateAllPossibleMoves()
    {
        foreach (var piece in activePieces)
        {
            if (board.HasPiece(piece))
                piece.SelectAvaliableSquares();
        }
    }

    public Piece[] GetPiecesAttackingOppositePieceOfType<T>() where T : Piece
    {
        return activePieces.Where(p => p.IsAttackingPieceOfType<T>()).ToArray();
    }

    public Piece[] GetPiecesOfType<T>() where T : Piece
    {
        return activePieces.Where(p => p is T).ToArray();
    }

    public void RemoveMovesEnablingAttackOnPiece<T>(ChessPlayer opponent, Piece selectedPiece) where T : Piece
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>();
        foreach (var coords in selectedPiece.avaliableMoves)
        {
            Piece pieceOnSquare = board.GetPieceOnSquare(coords);
            board.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);
            opponent.GenerateAllPossibleMoves();
            if (opponent.CheckIfIsAttackingPiece<T>())
                coordsToRemove.Add(coords);
            board.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, coords, selectedPiece, pieceOnSquare);
        }
        foreach (var coords in coordsToRemove)
        {
            selectedPiece.avaliableMoves.Remove(coords);
        }
    }
    public void RemoveCastlingMoves(Piece attackedKing)
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>();
        Vector2Int leftCastlingMove = new Vector2Int(-1, -1);
        Vector2Int rightCastlingMove = new Vector2Int(-1, -1);
        Piece leftRook;
        Piece rightRook;
        foreach (var coords in attackedKing.avaliableMoves)
        {
            leftRook = attackedKing.GetPieceInDirection<Rook>(team, Vector2Int.left);
            if (leftRook && !leftRook.hasMoved)
            {
                leftCastlingMove = attackedKing.occupiedSquare + Vector2Int.left * 2;
                coordsToRemove.Add(leftCastlingMove);
            }
            rightRook = attackedKing.GetPieceInDirection<Rook>(team, Vector2Int.right);
            if (rightRook && !rightRook.hasMoved)
            {
                rightCastlingMove = attackedKing.occupiedSquare + Vector2Int.right * 2;
                coordsToRemove.Add(rightCastlingMove);
            }
        }
        foreach (var coords in coordsToRemove)
        {
            attackedKing.avaliableMoves.Remove(coords);
        }
    }

    private bool CheckIfIsAttackingPiece<T>() where T : Piece
    {
        foreach (var piece in activePieces)
        {
            if (board.HasPiece(piece) && piece.IsAttackingPieceOfType<T>())
                return true;
        }
        return false;
    }

    public bool CanHidePieceFromAttack<T>(ChessPlayer opponentPlayer) where T : Piece
    {
        foreach (var piece in activePieces)
        {
            foreach (var coords in piece.avaliableMoves)
            {
                Piece pieceOnCoords = board.GetPieceOnSquare(coords);
                board.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
                opponentPlayer.GenerateAllPossibleMoves();
                if (!opponentPlayer.CheckIfIsAttackingPiece<T>())
                {
                    board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
                    return true;
                }
                board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
            }
        }
        return false;
    }

}
