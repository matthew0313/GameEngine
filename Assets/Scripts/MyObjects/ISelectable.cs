using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    public void OnSelect();
    public void OnDeselect();
}