using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
[RequireComponent(typeof(IObjectTweener))]
public abstract class Piece : MonoBehaviour
{
    private IObjectTweener tweener;
    private MaterialSetter materialSetter;
    public Board board { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    public TeamColor team { get; set; }
    public bool hasMoved { get; private set; }

    public List<Vector2Int> avaliableMoves;


    public abstract List<Vector2Int> SelectAvaliableSquares();

    private void Awake()
    {
        SetDependencies();
        avaliableMoves = new List<Vector2Int>();
        hasMoved = false;
    }
    private void SetDependencies()
    {
        tweener = GetComponent<IObjectTweener>();
        materialSetter = GetComponent<MaterialSetter>();
    }    
    public void SetMaterial(Material material)
    {
        if (materialSetter == null)
            materialSetter = GetComponent<MaterialSetter>();
        materialSetter.SetSingleMaterial(material);
    }

    public bool IsAttackingPieceOfType<T>() where T : Piece
    {
        foreach (var square in avaliableMoves)
        {
            if (board.GetPieceOnSquare(square) is T)
                return true;
        }
        return false;
    }

    public bool IsFromSameTeam(Piece piece)
    {
        return team == piece.team;
    }

    public bool CanMoveTo(Vector2Int coords)
    {
        return avaliableMoves.Contains(coords);
    }

    public virtual void MovePiece(Vector2Int coords)
    {
        Vector3 targetPosition = board.CalculatePositionFromCoords(coords);
        occupiedSquare = coords;
        hasMoved = true;
        tweener.MoveTo(transform, targetPosition);
    }
    
    protected void TryToAddMove(Vector2Int coords)
    {
        avaliableMoves.Add(coords);
    }

    public void SetData(Vector2Int coords, TeamColor team, Board board)
    {
        this.team = team;
        occupiedSquare = coords;
        this.board = board;
        transform.position = board.CalculatePositionFromCoords(coords);
    }
    public Piece GetPieceInDirection<T>(TeamColor team, Vector2Int direction) where T : Piece
    {
        for (int i = 1; i <= Board.BOARD_SIZE; i++)
        {
            Vector2Int nextcoords = occupiedSquare + direction * i;
            Piece piece = board.GetPieceOnSquare(nextcoords);
            if (!board.CheckIfCoordsAreOnBoard(nextcoords))
                return null;
            if (piece != null)
            {
                if (piece.team != team || !(piece is T))
                    return null;
                else if (piece.team == team && (piece is T))
                    return piece;
            }
        }
        return null;
    }
    protected Piece GetPieceBeside<T>(TeamColor team, Vector2Int direction) where T : Piece
    {
        Vector2Int nextcoords = occupiedSquare + direction;
        Piece piece = board.GetPieceOnSquare(nextcoords);
        if (!board.CheckIfCoordsAreOnBoard(nextcoords))
            return null;
        if (piece != null)
        {
            if (piece.team != team || !(piece is T))
                return null;
            else if (piece.team == team && (piece is T))
                return piece;
        }
        return null;
    }

}
