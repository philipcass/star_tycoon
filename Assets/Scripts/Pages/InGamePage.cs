using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class InGamePage : BPage{
	
	bool _haveAction = true;
	public int CollectedResources {get;set;}
	List<Planet> MyPlanets = new List<Planet>();
	SpaceBackground background;
	float _planetCost;
	int PlanetCost{
		get{
			return (int)_planetCost;
		}
		set{
			_planetCost = value;
		}
	}
	int shipAmount = 0;
		
	override public void Start(){
		background = new SpaceBackground();
		background.alpha = 0;
		this.AddChild(background);
		Go.to(background,3f, new TweenConfig().setEaseType(EaseType.CircOut).floatProp("alpha", 1));
		
		showGrid(Futile.screen.width,Futile.screen.width);
		
		for(float y = -Futile.screen.halfWidth; y < Futile.screen.halfWidth; y+=64){
			for(float x = -Futile.screen.halfWidth; x< Futile.screen.halfWidth; x+=64){
				FButton butt = new FButton(Futile.whiteElement.name);
				butt.SetAnchor(0,0);
				butt.scale = 3;
				butt.SetPosition(x, y);
				butt.SignalRelease += onClick;
				butt.data = new Planet();
				AddChild(butt);
			}
		}		
		resLabel = new FLabel("Abstract","Resources");
		resLabel.SetAnchor(0.5f,1);
		resLabel.scale *=2;
		resLabel.y = Futile.screen.halfHeight;
		AddChild(resLabel);
		costLabel = new FLabel("Abstract","Cost");
		costLabel.SetAnchor(0.5f,1);
		costLabel.scale *=2;
		costLabel.y = Futile.screen.halfHeight-32;
		AddChild(costLabel);
		shipLabel = new FLabel("Abstract","Ship");
		shipLabel.SetAnchor(0.5f,1);
		shipLabel.scale *=2;
		shipLabel.y = Futile.screen.halfHeight-64;
		AddChild(shipLabel);
		
		PlanetCost = 50;
		CollectedResources = PlanetCost;
		BaseMain.Instance.StartCoroutine(collectResources());
	}
	FLabel resLabel;
	FLabel costLabel;
	FLabel shipLabel;
	private void showGrid(float width, float height){
		TweenChain chainY = new TweenChain();
		TweenChain chainX = new TweenChain();
		
		for(float y = height/2; y >= -height/2;y-=64){
			FSliceSprite ss = new FSliceSprite(Futile.whiteElement,width,1,0,0,0,0);
			ss.y = y;
			ss.x = -width;
			AddChild(ss);
			Tween t =  new Tween(ss, 3/(height/64), new TweenConfig().floatProp("x", 0));
			chainY.append(t);
		}
		for(float x = -width/2; x<= width/2;x+=64){
			FSliceSprite ss = new FSliceSprite(Futile.whiteElement,1,height	,0,0,0,0);
			ss.x = x;
			ss.y = height;
			AddChild(ss);
			Tween t =  new Tween(ss, 3/(width/64), new TweenConfig().floatProp("y", 0));
			chainX.append(t);
		}
		chainY.play();
		chainX.play();
	}
	
	public void onClick (FButton button){
		if(_haveAction && CollectedResources>=PlanetCost){
			CollectedResources -= PlanetCost;
			Debug.Log(((Planet)button.data).Name);
			button.sprite.color = Color.red;
			MyPlanets.Add((Planet)button.data);
			_planetCost*=1.10f;
		}
	}
	
	private void enableAction(){
		_haveAction = true;
	}
	
	private IEnumerator collectResources(){
		yield return new WaitForSeconds(5);
		
		int newResource = MyPlanets.Sum(p => p.ResourceCount);
		CollectedResources += newResource;
		BaseMain.Instance.StartCoroutine(collectResources());
	}
	
	override public void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += HandleUpdate;
        base.HandleAddedToStage();  
    }

    override public void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= HandleUpdate;
        base.HandleRemovedFromStage();  
    }
    
    void HandleUpdate(){
		resLabel.text = string.Format("Resources: {0}", CollectedResources);
		costLabel.text = string.Format("Cost: {0}", PlanetCost);
		shipLabel.text = string.Format("Ships: {0}", shipAmount);
		background.Update();
	}


}