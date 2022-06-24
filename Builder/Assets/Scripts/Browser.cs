using System.Runtime.InteropServices;

public class Browser
{
    [DllImport("__Internal")]
    public static extern int GetSceneId();
    
    [DllImport("__Internal")]
    public static extern int GetUser();
    
    [DllImport("__Internal")]
    public static extern string GetPass();

    [DllImport("__Internal")]
    public static extern string GetUrlParam(string param); //Not working

}
