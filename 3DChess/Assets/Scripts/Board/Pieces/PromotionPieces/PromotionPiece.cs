using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
public class PromotionPiece : MonoBehaviour
{
    public string pieceName;
    private float rotationSpeed = .4f;
    public float initXValue;
    public Piece piece;

    private MaterialSetter materialSetter;


    private void Awake()
    {
        materialSetter = GetComponent<MaterialSetter>();
        
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f, rotationSpeed, 0f));
    }
    public void SetMaterial(Material material)
    {
        if (!materialSetter)
            materialSetter = GetComponent<MaterialSetter>();
        materialSetter.SetSingleMaterial(material);
    }
}
