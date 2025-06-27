using UnityEngine;

public class CharacterSelectVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _material;

    private void Awake()
    {
        _material = new Material(_meshRenderer.material);

        _meshRenderer.material = _material;
    }

    public void SetPlayerColor(Color color)
    {
        _material.color = color;
    }
}
