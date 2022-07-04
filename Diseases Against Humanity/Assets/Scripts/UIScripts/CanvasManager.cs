using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CanvasTypes;

/// <summary>
/// This class can only be created once for the main canvas.
/// The transitions between the canvases are controlled here.
/// </summary>
public class CanvasManager : Singleton<CanvasManager>
{
    [SerializeField]
    CanvasType InitialCanvas;

    private readonly List<ICanvasController> CanvasControllerList = new List<ICanvasController>();
    private readonly List<ICanvasController> ActiveCanvases = new List<ICanvasController>();

    /// <summary>
    /// This method gets called once on startup.
    /// All UI elements tagged with an canvas controller are loaded here.
    /// In addition, the all canvases to be displayed when the app starts must be selected here.
    /// </summary>
    protected override void Awake()
    {
        base.Awake(); // this has to be called first!

        // load all elements tagged with an canvas controller
        CanvasControllerList.Clear();
        CanvasControllerList.AddRange(GetComponentsInChildren<ICanvasController>());
        CanvasControllerList.ForEach(x => x.gameObject.SetActive(false));

        // select canvases to be displayed when the app starts
        SwitchCanvas(this.InitialCanvas);

        // Output a warning if a canvas is marked with the canvas type 'None'.
        if (CanvasControllerList.Any(x => x.CanvasType == CanvasType.None))
        {
            Debug.LogWarning(string.Format("Class {0}, Method {1}: There is a canvas controller wich uses the canvas type '{2}'! Check if warnings with unused canvas types appear below.", nameof(CanvasManager), nameof(Awake), Enum.GetName(typeof(CanvasType), CanvasType.None)));
        }

        // Output a warning if not all canvas types are used.
        foreach (var canvasType in Enum.GetValues(typeof(CanvasType)).Cast<CanvasType>())
        {
            if (canvasType != CanvasType.None && !CanvasControllerList.Any(x => x.CanvasType == canvasType))
            {
                Debug.LogWarning(string.Format("Class {0}, Method {1}: No canvas controller uses the canvas type '{2}'!", nameof(CanvasManager), nameof(Awake), Enum.GetName(typeof(CanvasType), canvasType)));
            }
        }
    }

    /// <summary>
    /// Use this method for switching to one or multiple canvases corresponding to the given canvas type(s).
    /// </summary>
    /// <param name="canvasFlagType">The type(s) of the canvases to be switched to.</param>
    /// <param name="forceReactivation">Determines whether the canvases should be reactivated if they are already active. Default ist false.</param>
    public void SwitchCanvas(CanvasType canvasFlagType, bool forceReactivation = false)
    {
        var canvasTypeList = new List<CanvasType>(Enum.GetValues(typeof(CanvasType)).Cast<Enum>().Where(canvasFlagType.HasFlag).Cast<CanvasType>());
        if (forceReactivation)
        {
            ActiveCanvases.ForEach(x => DeactivateCanvas(x));
            canvasTypeList.ForEach(x => ActivateCanvas(GetCanvasController(x)));
        }
        else
        {
            ActiveCanvases.FindAll(x => !canvasTypeList.Contains(x.CanvasType)).ForEach(x => DeactivateCanvas(x));
            canvasTypeList.FindAll(x => !ActiveCanvases.Any(y => y.CanvasType == x)).ForEach(x => ActivateCanvas(GetCanvasController(x)));
        }
    }

    /// <summary>
    /// Gets an enumerable of the types of all active canvases.
    /// </summary>
    /// <returns>An enumerable of canvas types.</returns>
    public IEnumerable<CanvasType> GetActiveCanvasTypes()
    {
        return ActiveCanvases.ConvertAll(new Converter<ICanvasController, CanvasType>(x => x.CanvasType));
    }

    /// <summary>
    /// Gets the types of all active canvases in a single value.
    /// </summary>
    /// <returns>An enumerable of canvas types.</returns>
    public CanvasType GetActiveCanvasAsSingleType()
    {
        var canvasTypes = GetActiveCanvasTypes();
        CanvasType canvasType = CanvasType.None;
        foreach (var ct in canvasTypes)
        {
            canvasType |= ct;
        }

        return canvasType & ~CanvasType.None;
    }

    /// <summary>
    /// Determines whether the settings menu is active or not.
    /// </summary>
    /// <returns>True, if the settings menu is displayed right now, otherwise false.</returns>
    public bool IsSettingsMenuActive()
    {
        return GetActiveCanvasTypes().Contains(CanvasType.SettingsMenu);
    }

    /// <summary>
    /// This method returns the controller belonging to the given canvas type.
    /// This method does not support flag enums. Only use one specific type at a time.
    /// </summary>
    /// <param name="canvasType"></param>
    /// <returns>The canvas controller corresponding to the given canvas type.</returns>
    private ICanvasController GetCanvasController(CanvasType canvasType)
    {
        var desiredCanvas = CanvasControllerList.Find(x => x.CanvasType == canvasType);
        if (desiredCanvas == null)
        {
            Debug.LogWarning(string.Format("Class {0}, Method {1}: The canvas '{2}' was not found!", nameof(CanvasManager), nameof(GetCanvasController), Enum.GetName(typeof(CanvasType), canvasType)));
        }

        return desiredCanvas;
    }

    /// <summary>
    /// This method should be called instead of "gameObject.SetActive(true)".
    /// </summary>
    /// <param name="canvasController">The controller of the canvas to be activated.</param>
    private void ActivateCanvas(ICanvasController canvasController)
    {
        if (canvasController == null) { return; }

        canvasController.gameObject.SetActive(true);
        ActiveCanvases.Add(canvasController);
    }

    /// <summary>
    /// This method should be called instead of "gameObject.SetActive(false)".
    /// </summary>
    /// <param name="canvasController">The controller of the canvas to be deactivated.</param>
    private void DeactivateCanvas(ICanvasController canvasController)
    {
        if (canvasController == null) { return; }

        canvasController.gameObject.SetActive(false);
        ActiveCanvases.Remove(canvasController);
    }
}
