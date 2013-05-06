using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceBackground : FContainer{
	
	FRepeatSprite background100;
	FRepeatSprite background80;
	FRepeatSprite background60;
	public SpaceBackground(){
		background100 = new FRepeatSprite("Parallax100", Futile.screen.width, Futile.screen.height);
		background80 = new FRepeatSprite("Parallax100", Futile.screen.width, Futile.screen.height, 0, 80);
		background60 = new FRepeatSprite("Parallax100", Futile.screen.width, Futile.screen.height, 0, 60);
		background80.alpha = 0.8f;
		background60.alpha = 0.6f;
		
		this.AddChild(background100);		
		this.AddChild(background80);
		this.AddChild(background60);
	}
	
    public void Update(){
		background100.scrollX +=0.01f;
		background80.scrollX +=0.008f;
		background60.scrollX +=0.006f;
    }


}