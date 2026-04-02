using System.Collections.Generic;
using UnityEngine;
using static Sango.Window;

using Sango.Core; namespace Sango.UI
{
    public enum ContextMenuType : int
    {
        Common = 0,
        Command,
        Object,
        System,
    }
}
