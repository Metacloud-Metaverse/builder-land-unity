mergeInto(LibraryManager.library, 
{
  GetSceneId: function() 
  {
    const queryString = window.location.search;

    const urlParams = new URLSearchParams(queryString);
    
    let id = urlParams.get('id');
    
    if(id == null)
    {
        id = -1;
    }
    
    return id;
  }
});