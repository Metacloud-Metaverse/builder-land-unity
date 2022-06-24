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
  },
  
  GetUrlParam: function(param)
  {
    //No funciona. Por alguna razón, el parámetro que recibe, lo interpreta como número.
    const parsedParam = UTF8ToString(param);
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    const paramValue = urlParams.get(param);

    return paramValue;
  },
  
  GetUser: function()
  {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    let paramValue = urlParams.get('user');
    if(paramValue == null)
        paramValue = -1
    return paramValue;
  },
  
  GetPass: function()
  {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    const paramValue = urlParams.get('pass');
    
    if(paramValue != null)
    {
        var bufferSize = lengthBytesUTF8(paramValue) + 1;
        var buffer = _malloc(bufferSize);                
        stringToUTF8(paramValue, buffer, bufferSize);    
        return buffer;                                   
    }
    return null;
  }
});