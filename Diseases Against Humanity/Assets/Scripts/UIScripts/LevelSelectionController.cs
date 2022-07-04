using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionController : Singleton<LevelSelectionController>
{
    [SerializeField]
    private GameObject[] _Panes;

    [SerializeField]
    private int _SelectedPaneIndex;

    protected override void Awake()
    {
        base.Awake();
        this.UpdatePanes();
    }

    public void Next()
    {
        this._SelectedPaneIndex = Math.Min(this._Panes.Length - 1, this._SelectedPaneIndex + 1);
        this.UpdatePanes();
    }

    public void Previous()
    {
        this._SelectedPaneIndex = Math.Max(0, this._SelectedPaneIndex - 1);
        this.UpdatePanes();
    }

    private void UpdatePanes()
    {
        for (int i = 0; i < this._Panes.Length; i++)
        {
            this._Panes[i].SetActive(i == this._SelectedPaneIndex);
        }
    }
}
