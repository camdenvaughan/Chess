using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    // 8 movement directions of a king
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1),
    };

    // Cordinated for Castling
    private Vector2Int leftCastlingMove;
    private Vector2Int rightCastlingMove;

    private Piece leftRook;
    private Piece rightRook;

    // Assign Regular moves, then castling moves if available
    public override List<Vector2Int> SelectAvaliableSquares()
    {
        avaliableMoves.Clear();
        AssignStandardMoves();
        AssignCastlingMoves();
        return avaliableMoves;
    }

    private void AssignCastlingMoves()
    {
        leftCastlingMove = new Vector2Int(-1, -1);
        rightCastlingMove = new Vector2Int(-1, -1);
        if (!hasMoved)
        {
            leftRook = GetPieceInDirection<Rook>(team, Vector2Int.left);
            if (leftRook && !leftRook.hasMoved)
            {
                leftCastlingMove = occupiedSquare + Vector2Int.left * 2;
                avaliableMoves.Add(leftCastlingMove);
            }
            rightRook = GetPieceInDirection<Rook>(team, Vector2Int.right);
            if (rightRook && !rightRook.hasMoved)
            {
                rightCastlingMove = occupiedSquare + Vector2Int.right * 2;
                avaliableMoves.Add(rightCastlingMove);
            }
        }
    }



    private void AssignStandardMoves()
    {
        float range = 1;
        foreach (var direction in directions)
        {
            for (int i = 1; i <= range; i++)
            {
                Vector2Int nextCoords = occupiedSquare + direction * i;
                Piece piece = board.GetPieceOnSquare(nextCoords);
                if (!board.CheckIfCoordsAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddMove(nextCoords);
                else if (!piece.IsFromSameTeam(this))
                {
                    TryToAddMove(nextCoords);
                    break;
                }
                else if (piece.IsFromSameTeam(this))
                    break;
            }
        }
    }
    public override void MovePiece(Vector2Int coords)
    {
        base.MovePiece(coords);
        if (coords == leftCastlingMove)
        {
            board.UpdateBoardOnPieceMove(coords + Vector2Int.right, leftRook.occupiedSquare, leftRook, null);
            leftRook.MovePiece(coords + Vector2Int.right);

            // Send false to indicate that it is Queenside
            board.CreateCastlingNotation(false);
        }
        if (coords == rightCastlingMove)
        {
            board.UpdateBoardOnPieceMove(coords + Vector2Int.left, rightRook.occupiedSquare, rightRook, null);
            rightRook.MovePiece(coords + Vector2Int.left);

            // Send true to indicate that it is a kingside
            board.CreateCastlingNotation(true);
        }
    }

 }
