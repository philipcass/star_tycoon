using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class InGamePage : BPage {
 
    bool _haveAction = true;

    public int CollectedResources { get; set; }

    List<Planet> MyPlanets = new List<Planet>();
    FButton[,] AllPlanets;
    SpaceBackground background;
    float _planetCost;

    int PlanetCost {
        get {
            return (int)_planetCost;
        }
        set {
            _planetCost = value;
        }
    }

    int shipAmount = 0;
    FLabel resLabel;
    FLabel costLabel;
    FLabel shipLabel;

    public class floatContainer {
        public float val{ get; set; }
    }

    public class intContainer {
        public int val{ get; set; }
    }

    floatContainer timerLabelvalue;
    FLabel timerLabel;
    int _waitTime = 3;

    Player player1 = new Player(Color.red);
    Player player2 = new Player(Color.blue);

    override public void Start() {
        background = new SpaceBackground();
        background.alpha = 0;
        this.AddChild(background);
        Go.to(background, 3f, new TweenConfig().setEaseType(EaseType.CircOut).floatProp("alpha", 1));
     
		showGrid(768, 768);
     
		CreateGameMap(768);
        AddChild(AllPlanets[0,0]);
        ClaimPlanet(AllPlanets[0,0], player1);
        AddChild(AllPlanets[11,11]);
        ClaimPlanet(AllPlanets[11,11], player2);

        resLabel = new FLabel("Abstract", "Resources");
        resLabel.SetAnchor(0.5f, 0);
        resLabel.scale *= 2;
        resLabel.y = -Futile.screen.halfHeight;
        AddChild(resLabel);
        costLabel = new FLabel("Abstract", "Cost");
        costLabel.SetAnchor(0.5f, 0);
        costLabel.scale *= 2;
        costLabel.y = -Futile.screen.halfHeight + 32;
        AddChild(costLabel);
        shipLabel = new FLabel("Abstract", "Ship");
        shipLabel.SetAnchor(0.5f, 0);
        shipLabel.scale *= 2;
        shipLabel.y = -Futile.screen.halfHeight + 64;
        AddChild(shipLabel);
        
        timerLabelvalue = new floatContainer();
        timerLabelvalue.val = 3;
        timerLabel = new FLabel("Abstract", timerLabelvalue.val.ToString());
        timerLabel.scale *= 10;
        timerLabel.color = Color.gray;
        timerLabel.alpha = 0.75f;
        timerLabel.SetAnchor(0.5f, 0.5f);
        AddChild(timerLabel);
     
        PlanetCost = 50;
        CollectedResources = 1000000;
        BaseMain.Instance.StartCoroutine(collectResources());
    }

	void CreateGameMap (float width)	
	{
        AllPlanets = new FButton[(int)width / 64, (int)width / 64];
		float halfWidth = width / 2;
		int arrayLen = (int)width / 64;
		arrayLen--;
		Debug.Log (arrayLen);
		for (float y = -halfWidth; y < 0; y += 64) {
			for (float x = -halfWidth; x < halfWidth; x += 64) {
				Planet p1 = new Planet ();
				FButton butt = CreatePlanetButton (x, y, p1);
				Debug.Log (string.Format ("{0},{1}", (int)(y + halfWidth) / 64, (int)(x + halfWidth) / 64));
				AllPlanets [(int)(y + halfWidth) / 64, (int)(x + halfWidth) / 64] = butt;
				Planet p2 = new Planet ();
				//Make the resource count the same as for player1 side to keep game fair
				p2.ResourceCount = p1.ResourceCount;
				butt = CreatePlanetButton ((x * -1) - 64, (y * -1) - 64, p2);
				AllPlanets [arrayLen - ((int)(y + halfWidth) / 64), arrayLen - ((int)(x + halfWidth) / 64)] = butt;
			}
		}
	}

    FButton CreatePlanetButton(float x, float y, Planet p) {
        FButton butt = new FButton(Futile.whiteElement.name);
        butt.SetAnchor(0, 0);
        butt.scale = 3;
        butt.SetPosition(x, y);
        butt.SignalReleaseOutside += onClick;
        butt.data = p;
        butt.AddLabel("Abstract", p.ResourceCount.ToString(), Color.black);
        butt.label.scale *= .5f;
        return butt;
    }

    private void showGrid(float width, float height) {
        TweenChain chainY = new TweenChain();
        TweenChain chainX = new TweenChain();
     
        for(float y = height/2;y >= -height/2;y-=64) {
            FSliceSprite ss = new FSliceSprite(Futile.whiteElement, width, 1, 0, 0, 0, 0);
            ss.y = y;
            ss.x = -width;
            AddChild(ss);
            Tween t = new Tween(ss, 3 / (height / 64), new TweenConfig().floatProp("x", 0), null);
            chainY.append(t);
        }
        for(float x = -width/2;x<= width/2;x+=64) {
            FSliceSprite ss = new FSliceSprite(Futile.whiteElement, 1, height, 0, 0, 0, 0);
            ss.x = x;
            ss.y = height;
            AddChild(ss);
            Tween t = new Tween(ss, 3 / (width / 64), new TweenConfig().floatProp("y", 0), null);
            chainX.append(t);
        }
        chainY.play();
        chainX.play();
    }

    public void onClick(FButton button) {
        Player claimant = ((Planet)button.data).Owner;

        if(claimant == null)
            return;

        Vector2 direction = button.LocalToGlobal(button.GetLocalMousePosition()) - button.GetPosition();
        direction = direction / direction.magnitude;
        int buttony = -1;
        int buttonx = -1;
        GetPlanetArrayPosition(button, ref buttony, ref buttonx);
        button = AllPlanets[buttony+Mathf.RoundToInt(direction.y), buttonx + Mathf.RoundToInt(direction.x)];

        if(((Planet)button.data).Owner != null)
            return;

        if(_haveAction && CollectedResources >= PlanetCost) {
            ClaimPlanet(button, claimant);
        } else {
            this.resLabel.alpha = 0;
            Go.to(this.resLabel, 0.1f, new TweenConfig().floatProp("alpha", 1).setIterations(4));
        }
    }

    private void ClaimPlanet(FButton button, Player claimant) {
        CollectedResources -= PlanetCost;
        button.sprite.color = claimant.Color;
        ((Planet)button.data).Owner = claimant;
        MyPlanets.Add((Planet)button.data);
        _planetCost *= 1.10f;
        ActivateNeighbors(button);
    }

    private void ActivateNeighbors(FButton button) {
        int buttony = -1;
        int buttonx = -1;
        GetPlanetArrayPosition(button, ref buttony, ref buttonx);

        if(buttonx - 1 >= 0)
            AddChild(AllPlanets[buttony,buttonx-1]);
        if(buttonx + 1 <= AllPlanets.GetUpperBound(1))
            AddChild(AllPlanets[buttony,buttonx+1]);
        if(buttony - 1 >= 0)
            AddChild(AllPlanets[buttony-1,buttonx]);
        if(buttony + 1 <= AllPlanets.GetUpperBound(0))
            AddChild(AllPlanets[buttony+1,buttonx]);
    }

    void GetPlanetArrayPosition(FButton button, ref int buttony, ref int buttonx) {
        for(int y = 0;y <= AllPlanets.GetUpperBound(0);y++) {
            for(int x = 0;x <= AllPlanets.GetUpperBound(1);x++) {
                if(button == AllPlanets[y, x]) {
                    buttony = y;
                    buttonx = x;
                }
            }
        }
    }

    private void enableAction() {
        _haveAction = true;
    }

    private IEnumerator collectResources() {
        timerLabelvalue.val = 3;
        Go.to(this.timerLabelvalue, 3, new TweenConfig().floatProp("val", 0));
        yield return new WaitForSeconds(_waitTime);
     
        int newResource = MyPlanets.Sum(p => p.ResourceCount);
        CollectedResources += newResource;
        BaseMain.Instance.StartCoroutine(collectResources());
    }

    override public void HandleAddedToStage() {
        Futile.instance.SignalUpdate += HandleUpdate;
        base.HandleAddedToStage();  
    }

    override public void HandleRemovedFromStage() {
        Futile.instance.SignalUpdate -= HandleUpdate;
        base.HandleRemovedFromStage();  
    }

    void HandleUpdate() {
        resLabel.text = string.Format("Resources: {0}", CollectedResources);
        costLabel.text = string.Format("Cost: {0}", PlanetCost);
        shipLabel.text = string.Format("Ships: {0}", shipAmount);
        timerLabel.text = Mathf.CeilToInt(timerLabelvalue.val).ToString();
        background.Update();
    }
}