using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessNotationManager : MonoBehaviour
{
    private const string letters = "abcdefgh";

    private Board board;
    private ChessGameController chessController;

    private string newChessCoord;
    private string typeNotation;
    private string take;
    private string oldFile;
    private string oldRank;
    private string promotionSymbol;
    private string castleNotation;
    private string checkOrMate;

    private string printNotation;

    private Piece currentPiece;


    public void NotateSquareCoord(Vector2Int coords, Piece piece)
    {
        newChessCoord = (XCoordToLetter(coords.x) + (coords.y + 1).ToString());
        currentPiece = piece;
        typeNotation = AddTypeNotation(piece);
    }

    public void AddCaptureNotation()
    {
        take = "x";
    }
    public void AddPassantNotation()
    {
        oldFile = XCoordToLetter(currentPiece.occupiedSquare.x);
    }

    public string AddTypeNotation(Piece piece)
    {

        switch (piece.GetType().ToString())
        {
            case "King":
                return "K";
            case "Bishop":
                return "B";
            case "Knight":
                return "N";
            case "Rook":
                return "R";
            case "Queen":
                return "Q";
            default:
                return null;
        }
    }

    public void AddPromotionNotation(Type type)
    {
        switch (type.ToString())
        {
            case "Bishop":
                promotionSymbol = "B";
                break;
            case "Knight":
                promotionSymbol = "N";
                break;
            case "Rook":
                promotionSymbol = "R";
                    break;
            case "Queen":
                promotionSymbol = "Q";
                break;
            default:
                break;
        }
    }
    public void AddDisambiguousNotation()
    {

    }


    public void AddCheckOrMate(string symbol)
    {
        checkOrMate = symbol;
    }

    public void Castle(string castleSide)
    {
        castleNotation = castleSide;
    }

    public void CombineNotation()
    {
        if (castleNotation == null)
        {
            Debug.Log(printNotation = typeNotation + oldFile + oldRank + take + newChessCoord + promotionSymbol + checkOrMate);
        }
        else
            Debug.Log(castleNotation);
        ClearStringsAndPiece();
    }

    private void ClearStringsAndPiece()
    {
        newChessCoord = null;
        typeNotation = null;
        take = null;
        oldFile = null;
        oldRank = null;
        promotionSymbol = null;
        castleNotation = null;
        checkOrMate = null;
        currentPiece = null;
}


    //public string CheckDisambiguatingMoves(Piece piece, Vector2Int oldCoords)
    //{
    //    string notation = "";

    //    List<Piece> activePieces = new List<Piece>();
    //    List<Piece> disambiguousPieces = new List<Piece>();

    //    foreach (var p in activePlayer.activePieces)
    //    {
    //        if (p.CompareTag(piece.tag))
    //            activePieces.Add(p);
    //    }

    //    foreach (var p in activePieces)
    //    {

    //        foreach (var move in p.avaliableMoves.ToList())
    //        {

    //            if (move == piece.occupiedSquare)
    //            {
    //                disambiguousPieces.Add(p);
    //            }
    //        }
    //    }
    //    if (disambiguousPieces.Count > 1)
    //    {
    //        foreach (var p in disambiguousPieces.ToList())
    //        {
    //            if (p.occupiedSquare.y == oldCoords.y)
    //            {
    //                notation += (XCoordToLetter(oldCoords.x));
    //            }

    //            if (p.occupiedSquare.x == oldCoords.x)
    //            {
    //                Debug.Log(oldCoords.y);
    //                notation += (oldCoords.y + 1).ToString();
    //            }
    //        }
    //    }
    //    return notation;
    //}
    public string XCoordToLetter(int xCoord)
    {
        var coordXletter = "";
        return coordXletter += letters[xCoord % letters.Length];
    }
}
