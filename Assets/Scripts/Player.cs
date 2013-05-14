using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Player {
    int _waitTime = 3;
    bool _haveAction = true;

    public Color Color{ 
        get; 
        set;
    }

    public int CollectedResources {
        get;
        set;
    }

    public List<Planet> MyPlanets {
        get;
        set;
    }

    public Player(Color playerColor) {
        this.Color = playerColor;
        this.CollectedResources = 0;
        this.MyPlanets = new List<Planet>();
        BaseMain.Instance.StartCoroutine(collectResources());
    }

    private IEnumerator collectResources() {
        yield return new WaitForSeconds(_waitTime);

        int newResource = MyPlanets.Sum(p => p.ResourceCount);
        CollectedResources += newResource;
        BaseMain.Instance.StartCoroutine(collectResources());
    }

    private void enableAction() {
        _haveAction = true;
    }
}
