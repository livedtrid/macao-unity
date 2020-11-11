using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationActions : MonoBehaviour
{
    public void Unload()
    {
        Application.Unload();
    }

    public void Quit()
    {
#if UNITY_IOS && !UNITY_EDITOR
                                 Application.Unload();
#else

        Application.Quit();
#endif
        AssetBundle.UnloadAllAssetBundles(true);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}