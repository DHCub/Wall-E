using System;
using GSharp.Objects;

namespace GSharp.Exceptions;

internal class Return : Exception
{
  internal GSObject Value { get; }

  internal Return(GSObject value)
  {
    this.Value = value;
  }
}