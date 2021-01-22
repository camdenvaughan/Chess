using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
public class PromotionPieceManager : MonoBehaviour
{
    public string pieceName;
    [SerializeField] private float rotationSpeed;
    public float initXValue;

    private MaterialSetter materialSetter;


    private void Awake()
    {
        materialSetter = GetComponent<MaterialSetter>();
        
    }

    private void OnSelect()
    {

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
