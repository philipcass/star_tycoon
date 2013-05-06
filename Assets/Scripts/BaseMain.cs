using UnityEngine;
using System.Collections;

public enum BPageType
{
    None,
    InGamePage,
}

public class BaseMain : MonoBehaviour{
    public static BaseMain Instance;
    private BPageType _currentPageType = BPageType.None;
    private BPage _currentPage = null;

	private FStage _stage;
    // Use this for initialization
    void Start()
    {
        Instance = this;
        FutileParams fparams = new FutileParams(false, false, true, false);

        fparams.AddResolutionLevel(1024.0f, 1.0f, 1.0f, ""); 
        fparams.origin = new Vector2(0.5f, 0.5f);

        Futile.instance.Init(fparams);

        Futile.atlasManager.LoadImage("Parallax100");
        Futile.atlasManager.LoadImage("Atlases/abstract_0");
        Futile.atlasManager.LoadFont("Abstract", "Atlases/abstract_0", "Atlases/abstract",0,0);
        
        _stage = Futile.stage;
		
		this.GoToPage(BPageType.InGamePage);
    }
	
    public void GoToPage(BPageType pageType)
    {
        if(_currentPageType == pageType)
            return; //we're already on the same page, so don't bother doing anything

        BPage pageToCreate = null;

        if(pageType == BPageType.InGamePage){
            pageToCreate = new InGamePage();
        }

        if(pageToCreate != null){ //destroy the old page and create a new one
            _currentPageType = pageType;    

            if(_currentPage != null){
                _stage.RemoveChild(_currentPage);
            }

            _currentPage = pageToCreate;
            _stage.AddChild(_currentPage);
            _currentPage.Start();
        }
    }
}
