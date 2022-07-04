using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCheats : MonoBehaviour
{
    private List<KeyCode> KeyCodes = new List<KeyCode>
    {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A,
    };

    private int KeyIndex = 0;

    // Update is called once per frame
    void Update()
    {
        HandleKey();
    }

    private void HandleKey()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(this.KeyCodes[this.KeyIndex]))
            {
                this.KeyIndex++;
                if (this.KeyIndex == this.KeyCodes.Count)
                {
                    this.KeyIndex = 0;
                    ActivateCheats();
                }
            }
            else
            {
                this.KeyIndex = 0;
            }
        }
    }

    private void ActivateCheats()
    {
        GameManager.GetInstance().AllowDebugCheats = true;
    }
}
