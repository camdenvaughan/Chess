using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessNotationManager : MonoBehaviour
{
    private int moveNumber;
    private int lastMoveNumber;

    [SerializeField] Text notationText;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] float scrollUpdateAmount;

    private const string letters = "abcdefgh";

    private string newChessCoord;
    private string typeNotation;
    private string take;
    private string oldFile;
    private string oldRank;
    private string promotionSymbol;
    private string castleNotation;
    private string checkOrMate;

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

    public void AddCheckOrMate(string symbol)
    {
        checkOrMate = symbol;
    }

    public void Castle(string castleSide)
    {
        castleNotation = castleSide;
    }
    public void UpdateMoveNumber()
    {
        moveNumber++;

    }
    public void CombineNotation()
    {
        string printNotation;
        string displayMove;
        if (lastMoveNumber == moveNumber)
            displayMove = "";
        else
            displayMove = "      " + moveNumber.ToString() + ".  ";

        if (castleNotation == null )
        {
            printNotation = displayMove + typeNotation + oldFile + oldRank + take + newChessCoord + promotionSymbol + checkOrMate;
        }
        else
            printNotation = displayMove + castleNotation;

        lastMoveNumber = moveNumber;

        PrintNotation(printNotation);
        ScrollTextRight();
        ClearStringsAndPiece();
    }

    private void PrintNotation(string printNotation)
    {
        notationText.text += printNotation + "    ";
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

    private void ScrollTextRight()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.horizontalNormalizedPosition = scrollUpdateAmount;
    }

    public string XCoordToLetter(int xCoord)
    {
        var coordXletter = "";
        return coordXletter += letters[xCoord % letters.Length];
    }
}
