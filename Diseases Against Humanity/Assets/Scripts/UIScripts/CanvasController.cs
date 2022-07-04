using UnityEngine;
using static CanvasTypes;


public interface ICanvasController
{
    CanvasType CanvasType { get; }

    GameObject gameObject { get; }
}

/// <summary>
/// This class must be added to all children of the main canvas to specify a canvas type.
/// </summary>
public class CanvasController : MonoBehaviour, ICanvasController
{
    [SerializeField]
    private CanvasType _CanvasType;

    public CanvasType CanvasType => _CanvasType;
}
