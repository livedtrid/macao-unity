using System;
using System.Collections;
using System.Linq;
using Nextreality.Sdk.Enum;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Nextreality.Sdk.AssetBundle
{
    /**
 * Created by Zincre on 2019-04-23
 * Reviewed by firetrap on 2019-04-23
 */
    public class LoadAssetBundle : MonoBehaviour
    {
        private string _bundlePath;
        private AssetBundleSourceLocation _assetBundleSourceLocation = AssetBundleSourceLocation.StreamingAssets;
        public Slider progressbar;
        public static bool isValidUrl(string source) => Uri.TryCreate(source, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;


        // Start is called before the first frame update
        void Start()
        {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) ||
                !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.Camera);
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
#endif
        }

        public void startExperience(string bundlePath)
        {
            this._assetBundleSourceLocation = isValidUrl(bundlePath) ? AssetBundleSourceLocation.Web : AssetBundleSourceLocation.StreamingAssets;
            this._bundlePath = bundlePath;

            Debug.Log(_assetBundleSourceLocation);
            if (_bundlePath?.Any() != true)
            {
                throw new Exception("Bundle path cannot null or empty");
            }

            if (_bundlePath?.Any() != true) return;
            switch (_assetBundleSourceLocation)
            {
                case AssetBundleSourceLocation.Web:
                    StartCoroutine(GetAssetBundleFromURL(_bundlePath));
                    break;

                case AssetBundleSourceLocation.StreamingAssets:
                    StartCoroutine(GetAssetBundleFromLocalStorage(_bundlePath));
                    break;

                default:
                    throw new Exception("AssetBundle path isn't Web or StreamingAssets");
            }
        }

        IEnumerator GetAssetBundleFromURL(string url)
        {
            Debug.Log("Loading from web");

            var www = UnityWebRequestAssetBundle.GetAssetBundle(url);
            www.SendWebRequest();

            progressbar.gameObject.SetActive(true);
            var progress = www.downloadProgress;

            while (!www.isDone)
            {
                Debug.Log(progress);
                progressbar.value = progress;
                yield return null;
            }

            progressbar.gameObject.SetActive(false);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var assetBundle = DownloadHandlerAssetBundle.GetContent(www);

                var asset = assetBundle.LoadAllAssets()[0]; //get first asset, we will only support one gameObject per assetBundle
                var go = Instantiate(asset) as GameObject;
            }
        }

        IEnumerator GetAssetBundleFromLocalStorage(string url)
        {
            var request = UnityEngine.AssetBundle.LoadFromFileAsync(url);

            progressbar.gameObject.SetActive(true);
            var progress = request.progress;
            while (!request.isDone)
            {
                Debug.Log(progress);
                progressbar.value = progress;
                yield return null;
            }

            progressbar.gameObject.SetActive(false);

            var assetBundle = request.assetBundle;
            var asset = assetBundle.LoadAllAssets()[0]; //get first asset, we will only support one gameObject per assetBundle
            var go = Instantiate(asset) as GameObject;
        }
    }
}