using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using UnityCipher;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Nabeel.Scripts
{
    public class CcxtApi : MonoBehaviour
    {
        [Tooltip("Base Url")]
        [SerializeField] public string baseUrl = "https://api-ccxt.vercel.app";
        
        [Tooltip("JWT Token")]
        [SerializeField] public string jwtToken = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJfaWQiOiI2NDU4YjUyYjhiNzkyM2IyY2FmMGZlZjkiLCJlbWFpbCI6ImExQGVtYWlsLmNvbSIsImlhdCI6MTY4MzUzNTM4MiwiZXhwIjoxNjgzNTM3MTgyfQ.WBeVLGxMUILQ-5XCKGZeOhS_CQP_ZB1P6FRo_Uf_ZN8";
        
        [Tooltip("API Key")]
        [SerializeField] public string apiKey = "ISkeKbS5G5amLfNPq0ckVF985wOciraESAH58bnCIt9IxmmnhIzroSqiz6GxEGLa";
        
        [Tooltip("Secret Key")]
        [SerializeField] public string secretKey = "8KuU70JzRPtA3rFxfK0ZNCoQ4nzUImmTkbdfSwmnqNS1nx1qDYxnPP1UUczRyEBA";
        
        [Tooltip("SignUp API")]
        [SerializeField] public SignUpApiRequestBody signUpApiRequestBody = new SignUpApiRequestBody
        {
            firstName = "mr",
            lastName = "a2",
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
            apiKey = "",
            secretKey = ""
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
            value = 13.96,
            marketId = "binance",
            option = "buy"
        };
        
        [Tooltip("Sell API")]
        [SerializeField] public TradeApiRequestBody sellApiRequestBody = new TradeApiRequestBody
        {
            from = "XRP",
            to = "BUSD",
            value = 13.96,
            marketId = "binance",
            option = "sell"
        };

        [Tooltip("Api Response")] [TextArea(15,20)] [SerializeField]
        public string apiResponse;

        private void Start()
        {
            return;
            const string publickey = "-----BEGIN PUBLIC KEY-----\nMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAkIgEaH5fyoc7QtakXvS1\nBwGEwxsfWmH/VKA3kioMP+4kVXmiFVEO9djwI2rWDgC6DV30YKqNtfmfV45B5Fln\nuvNcKvVrt87/ik/1DV9qepUDe5POuWrTlJD3nk5GqFGxiptmDqGAL/uoJW6FnycN\nrXp5KfRq2ZI0Zpg3k8yGjURujKHx9l38/M1YPrqLmKip7XCnUKwcEXGj+iUs2KcU\nNn9hJ631jAnNftOpT0DrYLgnEQvUXIwE4U188eOqhUWZtfGr4hvrEl/W7EPGvyyQ\nYjI2aPIWeasgsGAyZSieGqgwtvGv3L1Y4UFBbTk3MNAHQ3OKSyKgZwF2Nn6mKoEU\naVp4ootLUnTJAqbW8nXuYDrKDGOk+KxxEQVPBoCqUvTRMWSzBbsw3fWsI6b+mt76\nAZG3tU8enhKEcJ0Zs70Qnyc0Ep6Hd80A4oDo78HMjeWi7xZL29/tBmoifeoZxlBl\nXV3QWhG9Yy+8yabWpxG42y2suzcmNxcaLvOmMewhURt1ehMTbZf5hGklStIPdJLZ\njs8z2rbjReIWNuW5WbibWzk3Hp2Yionod4HiuBtqJSiuOMWZPh7dg/WRY5KtdWDK\nGoHe+h1M3mwDMBpzykfC33ldmWg07PrW55xMOZ2ZZimhTKpRCc1AeaFmYCBSwOrL\nw6sZrvkbFOUwa4MV/MFsqDECAwEAAQ==\n-----END PUBLIC KEY-----\n";
            // RSAParameters rsaParams = ConvertPemToRsaKey(publickey);
            // using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            // {
            //     rsa.ImportParameters(rsaParams);
            //
            //     byte[] plaintextBytes = Encoding.UTF8.GetBytes(apiKey);
            //     byte[] ciphertextBytes = rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.Pkcs1);
            //
            //     string ciphertext = Convert.ToBase64String(ciphertextBytes);
            //     Console.WriteLine(ciphertext);
            //     apiResponse = ciphertext;
            // }
            // apiResponse = encryptedApiKey;
            
            
            
            // Convert the PEM-encoded public key to a PublicKeyParameter instance
            var publicKeyBytes = Encoding.UTF8.GetBytes(publickey);
            var publicKeyParameter = (AsymmetricKeyParameter)new PemReader(new StreamReader(new MemoryStream(publicKeyBytes))).ReadObject();

            // Create a new instance of the RSA encryption algorithm
            var rsaEngine = new RsaEngine();

            // Initialize the RSA encryption algorithm with the public key
            rsaEngine.Init(true, publicKeyParameter);

            // Convert the plain text to bytes
            byte[] plainBytes = Encoding.UTF8.GetBytes(apiKey);

            // Encrypt the plain text
            byte[] encryptedBytes = rsaEngine.ProcessBlock(plainBytes, 0, plainBytes.Length);

            // Convert the encrypted bytes to a Base64-encoded string
            string encryptedText = Convert.ToBase64String(encryptedBytes);

            // Print or use the encrypted text as needed
            Debug.Log("Encrypted Text: " + encryptedText);

            apiResponse = encryptedText;
        }

        private static RSAParameters ConvertPemToRsaKey(string pemPublicKey)
        {
            var pemReader = new PemReader(new StringReader(pemPublicKey));
            var publicKey = (RsaKeyParameters)pemReader.ReadObject();

            RSAParameters rsaParams = new RSAParameters
            {
                Modulus = publicKey.Modulus.ToByteArrayUnsigned(),
                Exponent = publicKey.Exponent.ToByteArrayUnsigned()
            };

            return rsaParams;
        }

        private void EncryptSecurityKeysInPostKeysRequest(string publicKey)
        {
            postKeysApiRequestBody.apiKey = Encrypt(publicKey, apiKey);
            postKeysApiRequestBody.secretKey = Encrypt(publicKey, secretKey);
        }
        
        private static string Encrypt(string publickey, string plainText)
        {
            // Convert the PEM-encoded public key to a PublicKeyParameter instance
            var publicKeyBytes = Encoding.UTF8.GetBytes(publickey);
            var publicKeyParameter = (AsymmetricKeyParameter)new PemReader(new StreamReader(new MemoryStream(publicKeyBytes))).ReadObject();

            // Create a new instance of the RSA encryption algorithm
            var rsaEngine = new RsaEngine();

            // Initialize the RSA encryption algorithm with the public key
            rsaEngine.Init(true, publicKeyParameter);

            // Convert the plain text to bytes
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            // Encrypt the plain text
            byte[] encryptedBytes = rsaEngine.ProcessBlock(plainBytes, 0, plainBytes.Length);

            // Convert the encrypted bytes to a Base64-encoded string
            string encryptedText = Convert.ToBase64String(encryptedBytes);

            // Print or use the encrypted text as needed
            Debug.Log("Encrypted Text: " + encryptedText);
            
            // Return encrypted plain text
            return encryptedText;
        }

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
                    Debug.Log("SignUp Api Error: " + result);
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
                    Debug.Log("Login Api Error: " + result);
                    return;
                }
                
                Debug.Log("Login Api Response: " + result);
                apiResponse = result;
                var loginApiResponse = JsonConvert.DeserializeObject<LoginApiResponse>(result);
                jwtToken = "Bearer " + loginApiResponse.token;
                EncryptSecurityKeysInPostKeysRequest(loginApiResponse.publicKey);
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
                    Debug.Log("PostKeys Api Error: " + result);
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
                    Debug.Log("Convert Api Error: " + result);
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
                    Debug.Log("Buy Api Error: " + result);
                    return;
                }

                Debug.Log("Buy Api Response: " + result);
                apiResponse = result;

            }, urlPath: "/currency/buy", authValue: jwtToken, bodyObject: buyApiRequestBody));
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
                    Debug.Log("Sell Api Error: " + result);
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
            // var url = "https://crypto-exchange-git-feature-server-config-mudassir742.vercel.app" + urlPath;
            var url = baseUrl + urlPath;
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
        public double value; // TODO: Confirm datatype, Maybe this is making 400 Bad Request
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
