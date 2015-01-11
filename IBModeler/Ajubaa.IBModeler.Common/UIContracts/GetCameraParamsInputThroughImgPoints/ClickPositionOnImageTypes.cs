using System;

namespace Ajubaa.IBModeler.Common
{
    [Serializable]
    public enum ClickPositionOnImageTypes
    {
        None,
        LeftEndOfRotatingDisc,
        RightEndOfRotatingDisc,
        BottomMostPartOfModel,
        MarkerLeftFromCenter,
        MarkerRightFromCenter
    }
}
