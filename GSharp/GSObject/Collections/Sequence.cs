namespace GSharp.GSObject.Collections;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using GSharp.GSObject.Figures;
using System;

public abstract class Sequence : GSObject
{    
    public abstract Sequence GetRemainder(int start);
    public abstract GSObject this[int i] {get;}
}