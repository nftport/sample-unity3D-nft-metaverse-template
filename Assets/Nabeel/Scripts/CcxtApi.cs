using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Nabeel.Scripts
{
    public class CcxtApi : MonoBehaviour
    {
        // Datafiniti Search API
        public void OnClickConvertApi(InputField from = null, InputField to = null)
        {
            if (from == null || to == null)
            {
                Debug.Log("OnClickConvertApi: Parameters are null!");
                return;
            }
            
            StartCoroutine(ConvertApi((flag,value) =>
            {
                if (!flag)
                {
                    Debug.Log("Converted: Failed!");
                    return;
                }
                Debug.Log("Converted: " + from.text + " to " + to.text + " = " + value);
            }, from.text, to.text));
        }
        private IEnumerator ConvertApi(Action<bool, float> callback, string from, string to)
        {
            // Check Android Permissions
            CheckAndroidPermissions();
            
            // url where we want to send request
            var url = "https://api-ccxt/currency/convert";

            // set post data for body
            var body = JsonUtility.ToJson(new ConvertApiRequestBody
            {
                convertFrom = from,
                convertTo = to
            });

            // sending request
            using var request = new UnityWebRequest(url, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log("Error : " + request.error);
                callback(false, 0f);
            }
            else
            {
                var result = request.downloadHandler.text;
                var value = JsonConvert.DeserializeObject<float>(result);
                Debug.Log("Success : " + value);
                callback(true, value);
            }
        }
        
        private void CheckAndroidPermissions()
        {
            // handling internet permissions on android platform
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission("android.permission.INTERNET"))
            {
                Debug.Log("No Internet permission");
                Permission.RequestUserPermission("android.permission.INTERNET");
            }

            Debug.Log("Has internet permission");
#endif
        }
    }

    // To send Post Data for Convert Api
    [Serializable]
    public class ConvertApiRequestBody
    {
        public string convertFrom;
        public string convertTo;
    }
    
    // To send Post Data for Trade Api
    [Serializable]
    public class TradeApiRequestBody
    {
        public string from;
        public string to;
        public float value; // TODO: Confirm datatype
    }
}
