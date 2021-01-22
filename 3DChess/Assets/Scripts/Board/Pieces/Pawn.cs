using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public Piece passantPawn;
    public override List<Vector2Int> SelectAvaliableSquares()
    {
        avaliableMoves.Clear();
        Vector2Int direction = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
        float range = hasMoved ? 1 : 2;
        for (int i = 1; i <= range; i++)
        {
            Vector2Int nextCoords = occupiedSquare + direction * i;
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordsAreOnBoard(nextCoords))
                break;
            if (piece == null)
                TryToAddMove(nextCoords);
            else if (piece.IsFromSameTeam(this))
                break;
        }
        Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y) };
        for (int i = 0; i < takeDirections.Length; i++)
        {
            Vector2Int nextCoords = occupiedSquare + takeDirections [i];
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordsAreOnBoard(nextCoords))
                continue;
            if (piece != null && !piece.IsFromSameTeam(this))
                TryToAddMove(nextCoords);
        }
        CheckIfEnPassant();
        return avaliableMoves;
    }

    public override void MovePiece(Vector2Int coords)
    {
        base.MovePiece(coords);
        CheckPromotion();
    }

    private void CheckPromotion()
    {
        int endOfBoardYCoord = team == TeamColor.White ? Board.BOARD_SIZE - 1 : 0;
        if (occupiedSquare.y == endOfBoardYCoord)
        {
            board.CreatePromoteScreen(this);
        }
            //board.PromotePiece(this);
    }

    private void CheckIfEnPassant()
    {
        int enPassantYCoord = team == TeamColor.White ? Board.BOARD_SIZE - 4 : 3;
        int enPassantTakeYCoord = team == TeamColor.White ? Board.BOARD_SIZE - 3 : 2;
        TeamColor opponentColor = team == TeamColor.White ? TeamColor.Black : TeamColor.White;

        if (occupiedSquare == new Vector2Int(occupiedSquare.x, enPassantYCoord))
        {
            Piece rightPawn = GetPieceBeside<Pawn>(opponentColor, Vector2Int.right);
            Piece leftPawn = GetPieceBeside<Pawn>(opponentColor, Vector2Int.left);
            if (rightPawn == board.lastMovedPiece)
            {
                board.possibleEnPassant = new Vector2Int(rightPawn.occupiedSquare.x, enPassantTakeYCoord);
                board.passantPawn = rightPawn;
                TryToAddMove(board.possibleEnPassant);
            }
            else if (leftPawn == board.lastMovedPiece)
            {
                board.possibleEnPassant = new Vector2Int(leftPawn.occupiedSquare.x, enPassantTakeYCoord);
                board.passantPawn = leftPawn;
                TryToAddMove(board.possibleEnPassant);
            }
        }

    
    }
}
