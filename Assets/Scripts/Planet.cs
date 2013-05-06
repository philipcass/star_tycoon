using System;
using UnityEngine;

public class Planet{
	enum PlanetNames{
		Omcron,
		Persei,
		Zebe,
		Delta,
		Rigel,
		Niga,
		Pentak,
		Welbe,
		Quanta,
		Keter,
	}
	
	
	public string Name {get; set;}
	public int ResourceCount {get; set;}
	public Planet (){
		Name = string.Format("{0}-{1}-{2}",((PlanetNames) RXRandom.Int(10)).ToString(),
								((PlanetNames) RXRandom.Int(10)).ToString(),
								RXRandom.Int(100));
		ResourceCount = RXRandom.Int(10);
	}
}
