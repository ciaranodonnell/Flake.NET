using System;
using System.Collections.Generic;
using System.Text;

namespace COD.FlakeDN.Client
{
    public enum FlakeDNErrors : Int64
    {
        InvalidAuthCode = -1,
        TimeMovingBackwards = -2,
        SequenceExhausted = -3
    }
}
