using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectCodeBlock : PropertyCodeBlock
{
    public override PropertyType propertyType => PropertyType.Object;
}
