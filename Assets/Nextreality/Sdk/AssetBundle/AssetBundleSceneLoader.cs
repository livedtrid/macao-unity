using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Nextreality.Sdk.AssetBundle
{
    public class AssetBundleSceneLoader : MonoBehaviour
    {
        public string url;
        public int downloaded;
        UnityEngine.AssetBundle _bundle;
        public Slider progressbar;

        public float progress;

        UnityWebRequest _www;
        void Update()
        {
            progress = _www.downloadProgress;
            progressbar.value = progress;
        }

        IEnumerator Start()
        {
            ClearCacheExample();
            if (downloaded == 0)
            {
                using (_www = UnityWebRequestAssetBundle.GetAssetBundle(url))
                {
                    yield return _www;
                    if (_www.error != null)
                        throw new Exception("WWW download had an error:" + _www.error);
                    if (_www.error == null)
                    {
                        _bundle = DownloadHandlerAssetBundle.GetContent(_www);
                    }
                }
                if (Caching.ready)
                {
                    downloaded = 1;
                    var scenePath = _bundle.GetAllScenePaths();
                    Debug.Log(scenePath[0]);
                    SceneManager.LoadScene(scenePath[0]);
                }
            }
        }

        void ClearCacheExample()
        {
            Directory.CreateDirectory("Cache1");
            Directory.CreateDirectory("Cache2");
            Directory.CreateDirectory("Cache3");

            Caching.AddCache("Cache1");
            Caching.AddCache("Cache2");
            Caching.AddCache("Cache3");

            bool success = Caching.ClearCache();

            if (!success)
            {
                Debug.Log("Unable to clear cache");
            }
        }
    }
}