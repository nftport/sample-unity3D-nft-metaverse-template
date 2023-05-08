using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Nabeel.Scripts
{
    public class CcxtApi : MonoBehaviour
    {
        [Tooltip("JWT Token")]
        [SerializeField] public string jwtToken = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJfaWQiOiI2NDU4YjUyYjhiNzkyM2IyY2FmMGZlZjkiLCJlbWFpbCI6ImExQGVtYWlsLmNvbSIsImlhdCI6MTY4MzUzNTM4MiwiZXhwIjoxNjgzNTM3MTgyfQ.WBeVLGxMUILQ-5XCKGZeOhS_CQP_ZB1P6FRo_Uf_ZN8";
        
        [Tooltip("SignUp API")]
        [SerializeField] public SignUpApiRequestBody signUpApiRequestBody = new SignUpApiRequestBody
        {
            firstName = "mr",
            lastName = "a1",
            email = "a2@gmail.com",
            password = "a123456"
        };
        
        [Tooltip("Login API")]
        [SerializeField] public LoginApiRequestBody loginApiRequestBody = new LoginApiRequestBody
        {
            email = "a2@gmail.com",
            password = "a123456"
        };
        
        [Tooltip("PostKeys API")]
        [SerializeField] public PostKeysApiRequestBody postKeysApiRequestBody = new PostKeysApiRequestBody
        {
            marketId = "binance",
            apiKey = "d9JKW3LKi8gn6xbl3LEfcFVcYDV9hY4ju1wLZ6xrvi2s9XktzlOTEmpTzMEb5gHkq7Mncgz+DmGcvGCfgNc/DDt6VaIEjewmOHedgYEriwzVYmFJw3j40cNJg7OgSTjSFnUMubev4aXc3rdPpwrzA2crvWQsE6OLINaQE/NXZezKN7B+xAcwyZ5/fcYysJWkF0c2+o6gC4Y4yUSdlbtQbHMDCQAqCFG67csDOFWZBlJ7Ua5PP4V2bKNxvTuCQziEA77ZHLlEL0cF9GvKDHUevdWpQ+tTlhwGEH4WXc6APoVV91G5YbVz8RihV9VQiAxZXMrbM5u0RoBPasA93eLtqOnb7xPwfgiPpHSIQWXSkageUwvrw+VzpjwTANOp4i5iWiXRzs2jlzQ/+4H2umcjJujPUNmLREHp/S0zsWMuuXin+NO05vNKI0901uOvNVl23kV5dbfh+yMRg3KNSlRmxs+p/+kHkbxsA7LuQnHd6mRKpxzu/yzqRR2i6hqn2hINIvgrUoj95oHdSTARIcuy2INT5rZhtr+BBmQkLaR/zedkyML1DfZnESOwQrnf2uPTSGw9F9Bd+stnLE+k+FPRq+zfZYdA8LahUDTD9ojYnYeY90t4x8sAXBXY8fTi69ltAQvzkH2ckCFbWGfTpigzIldldh1U3vL6L6HOfC4ODD0=",
            secretKey = "TSDSGdt3GfphvskIZg4VYVopURj3cCmL8eJjYDxAue7Es90Ll0rbX7oM7plVxSPI0USvEwH03gnOFV/mmekktgzl7lRl0E96BOY/PhB/6uEMJv4nv2rJRy9ApNEMUFij2INF2XuWq4SEAOI0OwGS+rb3F5s/bWZLUOWm9Qm7pQu/g9P8egSqOGpphjMwu6GKSDapLV9QBT9LIOGhaI/FosVu5IUpyJ6M3KSYHFLz2YRal0prPatj2V5Phg90jBtk2UaNMjfbByPLlFNzb+le2DX2ylW5bnsTyZsBXYIVChm9QFEDm31eIurHpASFbFr1kUA3WXKvQObwFPpChd8FhbENNninsFQjm26ykEtTWiq+rF60bq0oOJZuRdrJ4Gq5dyL3AsAdsUfD2D5aoVTbVAM2a+tfHdDv4X8hsC83nwsbDVBhEYQizYlFu5Fk++qzUOEh9wR0kD/6jL2ErDCOHJbsz+6xVmwjn6BO82agCdZmpa2dyKPe0c8JuZDznS77v0hzUbIneofIfO0wZn1JF9DCFcvQMDRksHuyBDNFFLe+3TDKC2YOKQa0qDeX5KssVLMIfjCctqifU/zA8KlIJ/b+Fn/bVF1hfOOUUNa18IQ9IJrFx3R2G0jCaTGA2V2Ti4u5GsUrnEmmbcCyN1K5uY6ftSoUvWDGECEPe3m20sk="
        };

        [Tooltip("Convert API")]
        [SerializeField] public ConvertApiRequestBody convertApiRequestBody = new ConvertApiRequestBody
        {
            convertFrom = "DOGE",
            convertTo = "USDT",
            marketId = "binance"
        };
        
        [Tooltip("Buy API")]
        [SerializeField] public TradeApiRequestBody buyApiRequestBody = new TradeApiRequestBody
        {
            from = "XRP",
            to = "BUSD",
            value = 13.96000000f,
            marketId = "binance",
            option = "buy"
        };
        
        [Tooltip("Sell API")]
        [SerializeField] public TradeApiRequestBody sellApiRequestBody = new TradeApiRequestBody
        {
            from = "XRP",
            to = "BUSD",
            value = 13.96000000f,
            marketId = "binance",
            option = "sell"
        };

        [Tooltip("Api Response")] [TextArea(15,20)] [SerializeField]
        public string apiResponse;

        public void OnClickSignUpApi()
        {
            if (signUpApiRequestBody == null)
            {
                Debug.Log("OnClickSignUpApi: Parameters are null!");
                return;
            }

            StartCoroutine(Api((success, result) =>
            {
                if (!success)
                {
                    Debug.Log("SignUp Api Response: Failed!");
                    return;
                }
                
                Debug.Log("SignUp Api Response: " + result);
                apiResponse = result;

            }, urlPath: "/auth/register", authValue: jwtToken, bodyObject: signUpApiRequestBody));
        }

        public void OnClickLoginApi()
        {
            if (loginApiRequestBody == null)
            {
                Debug.Log("OnClickLoginApi: Parameters are null!");
                return;
            }

            StartCoroutine(Api((success, result) =>
            {
                if (!success)
                {
                    Debug.Log("Login Api Response: Failed!");
                    return;
                }
                
                Debug.Log("Login Api Response: " + result);
                apiResponse = result;
                var loginApiResponse = JsonConvert.DeserializeObject<LoginApiResponse>(result);
                jwtToken = "Bearer " + loginApiResponse.token;

            }, urlPath: "/auth/login", authValue: jwtToken, bodyObject: loginApiRequestBody));
        }

        public void OnClickPostKeysApi()
        {
            if (postKeysApiRequestBody == null)
            {
                Debug.Log("OnClickPostKeysApi: Parameters are null!");
                return;
            }

            StartCoroutine(Api((success, result) =>
            {
                if (!success)
                {
                    Debug.Log("PostKeys Api Response: Failed!");
                    return;
                }
                
                Debug.Log("PostKeys Api Response: " + result);
                apiResponse = result;

            }, urlPath: "/auth/postKeys", authValue: jwtToken, bodyObject: postKeysApiRequestBody));
        }

        public void OnClickConvertApi()
        {
            if (convertApiRequestBody == null)
            {
                Debug.Log("OnClickConvertApi: Parameters are null!");
                return;
            }

            StartCoroutine(Api((success, result) =>
            {
                if (!success)
                {
                    Debug.Log("Convert Api Response: Failed!");
                    return;
                }
                
                Debug.Log("Convert Api Response: " + result);
                apiResponse = result;

            }, urlPath: "/currency/convert", authValue: jwtToken, bodyObject: convertApiRequestBody));
        }

        public void OnClickBuyApi()
        {
            if (buyApiRequestBody == null)
            {
                Debug.Log("OnClickBuyApi: Parameters are null!");
                return;
            }

            StartCoroutine(Api((success, result) =>
            {
                if (!success)
                {
                    Debug.Log("Buy Api Response: Failed!");
                    return;
                }

                Debug.Log("Buy Api Response: " + result);
                apiResponse = result;

            }, urlPath: "/currency/trade", authValue: jwtToken, bodyObject: buyApiRequestBody));
        }
        
        public void OnClickSellApi()
        {
            if (sellApiRequestBody == null)
            {
                Debug.Log("OnClickSellApi: Parameters are null!");
                return;
            }

            StartCoroutine(Api((success, result) =>
            {
                if (!success)
                {
                    Debug.Log("Sell Api Response: Failed!");
                    return;
                }
                
                Debug.Log("Sell Api Response: " + result);
                apiResponse = result;

            }, urlPath: "/currency/sell", authValue: jwtToken, bodyObject: sellApiRequestBody));
        }
        
        private IEnumerator Api(Action<bool, string> callback, string urlPath, string authValue, object bodyObject)
        {
            // Check Android Permissions
            CheckAndroidPermissions();
            
            // url where we want to send request
            var url = "https://crypto-exchange-git-feature-server-config-mudassir742.vercel.app" + urlPath;
            Debug.Log(url);

            // set post data for body
            var body = JsonUtility.ToJson(bodyObject);

            // sending request
            using var request = new UnityWebRequest(url, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", authValue);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                // Debug.Log("Error : " + request.error);
                callback(false, request.error);
            }
            else
            {
                var result = request.downloadHandler.text;
                // Debug.Log("Success : " + result);
                callback(true, result);
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
    
    // To extract Login Api Response
    [Serializable]
    public class LoginApiResponse
    {
        public string _id;
        public string email;
        public string firstName;
        public string lastName;
        public string publicKey;
        public string token;
    }

    // To send Post Data for Convert Api
    [Serializable]
    public class ConvertApiRequestBody
    {
        public string convertFrom;
        public string convertTo;
        public string marketId;
    }
    
    // To send Post Data for Trade Api
    [Serializable]
    public class TradeApiRequestBody
    {
        public string from;
        public string to;
        public float value; // TODO: Confirm datatype
        public string marketId;
        public string option;
    }
    
    // To send Post Data for Login Api
    [Serializable]
    public class LoginApiRequestBody
    {
        public string email;
        public string password;
    }
    
    // To send Post Data for Sign Up Api
    [Serializable]
    public class SignUpApiRequestBody
    {
        public string firstName;
        public string lastName;
        public string email;
        public string password;
    }
    
    // To send Post Data for PostKeys Api
    [Serializable]
    public class PostKeysApiRequestBody
    {
        public string marketId;
        public string apiKey;
        public string secretKey;
    }
}
