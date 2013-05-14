using System;
using UnityEngine;

public class PlanetButton : FButton {
    public delegate void PlanetButtonSignalDelegate(PlanetButton button);

    public new event PlanetButtonSignalDelegate SignalReleaseOutside;

    public PlanetButton(float x, float y, Planet p, PlanetButtonSignalDelegate onClick) : base(Futile.whiteElement.name){
        this.SetAnchor(0, 0);
        this.scale = 3;
        this.SetPosition(x, y);
        base.SignalReleaseOutside += baseClick;
        this.SignalReleaseOutside += onClick;
        this.data = p;
        this.AddLabel("Abstract", p.ResourceCount.ToString(), Color.black);
        this.label.scale *= .5f;
    }

    void baseClick(FButton button) {
        this.SignalReleaseOutside(this);
    }

}

