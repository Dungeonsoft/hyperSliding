using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AmondSDK.AmondSDKPlugin;

namespace AmondSDK
{
    public class AmondPlugin
    {
        private static AmondPlugin instance;

        private AmondSDKPlugin amondSDKPlugin;

        public bool isInit = false;
        public bool isAvailable = false;
        public long ConnectedID = 0;
        public bool isAccessToken = false;

        private UserDTO userData = null;

        private AmondPlugin()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR)
        amondSDKPlugin = new AmondAndroidPlugin();
#elif (UNITY_IPHONE && !UNITY_EDITOR)
        amondSDKPlugin = new AmondiOSPlugin();
#else
            amondSDKPlugin = new AmondEditorPlugin();
#endif
        }


        /// <summary>
        /// Singleton인스턴스 반환
        /// </summary>
        /// <returns></returns>
        public static AmondPlugin GetInstance()
        {
            if (instance == null)
            {
                instance = new AmondPlugin();
            }

            return instance;
        }

        /// <summary>
        /// 플러그인 초기화
        /// </summary>
        /// <param name="gameName">게임 이름</param>
        /// <param name="type">환경 정보</param>
        /// <returns>플로그인 초기화 여부</returns>
        public bool Init(string gameName, EnvironmentType type = EnvironmentType.DEV)
        {
            isInit = amondSDKPlugin.Init(gameName, type);

            return isInit;
        }

        /// <summary>
        /// 플러그인 사용 가능 여부
        /// </summary>
        /// <returns>사용 가능 여부</returns>
        public bool IsAvailable()
        {
            if (isInit == false)
            {
                return false;
            }

            isAvailable = amondSDKPlugin.IsAvailable();

            return isAvailable;
        }

        /// <summary>
        /// 인증 여부
        /// </summary>
        /// <returns>인증 여부</returns>
        public bool IsAuthenticated()
        {
            if (isInit == false || isAvailable == false)
            {
                return false;
            }

            return amondSDKPlugin.IsAuthenticated();
        }

        /// <summary>
        /// 서버 접속
        /// </summary>
        /// <returns>서버 접속 여부</returns>
        public long ConnectAmond()
        {
            if (isInit == false || isAvailable == false)
            {
                return 0;
            }


            ConnectedID = amondSDKPlugin.ConnectAmond();
            return ConnectedID;
        }

        /// <summary>
        /// 엑세스 토큰 서버에서 가져옴
        /// </summary>
        /// <returns>엑세스 토큰 여부</returns>
        public bool GetAccessToken()
        {
            if (isInit == false || isAvailable == false)
            {
                return false;
            }

            isAccessToken = amondSDKPlugin.GetAccessToken();
            return isAccessToken;
        }

        /// <summary>
        /// 유저정보를 서버에서 가져와서 JSON String으로 반환
        /// GetUserData()로 유저 정보 접근 가능
        /// </summary>
        /// <returns></returns>
        public string GetUser()
        {
            if (isInit == false || isAvailable == false || isAccessToken == false)
            {
                return null;
            }

            string result = amondSDKPlugin.GetUser();

            if (result != null)
            {
                Debug.Log(result);
                //userData = JsonUtility.FromJson<UserDTO>(result);
            }

            return result;
        }

        /// <summary>
        /// 유저 정보 반환
        /// </summary>
        /// <returns></returns>
        public UserDTO GetUserData()
        {
            return userData;
        }

    }

    public class AmondAndroidPlugin : AmondSDKPlugin
    {
        //1. init.
        //2. isAvailable
        //3. isAuthenticated
        //4. connectAmond
        //5. getAccessToken
        //6. getUser

        private AndroidJavaObject GetHelper()
        {
            AndroidJavaClass javaClass = new AndroidJavaClass("io.amond.sdk.unity.PluginHelper");

            AndroidJavaObject javaObject = javaClass.CallStatic<AndroidJavaObject>("getInstance");

            return javaObject;
        }
        public override bool Init(string gameName, EnvironmentType type = EnvironmentType.DEV)
        {
            var javaObject = GetHelper();

            return javaObject.Call<bool>("init", gameName, (int)type);
        }

        public override bool IsAvailable()
        {
            var javaObject = GetHelper();

            return javaObject.Call<bool>("isAvailable");
        }

        public override bool IsAuthenticated()
        {
            var javaObject = GetHelper();

            return javaObject.Call<bool>("isAuthenticated");
        }

        public override long ConnectAmond()
        {
            var javaObject = GetHelper();

            return javaObject.Call<long>("connectAmond");
        }

        public override bool GetAccessToken()
        {
            var javaObject = GetHelper();

            return javaObject.Call<bool>("getAccessToken");
        }

        public override string GetUser()
        {
            var javaObject = GetHelper();

            return javaObject.Call<string>("getUser");
        }
    }

    public class AmondiOSPlugin : AmondSDKPlugin
    {

        public override bool Init(string gameName, EnvironmentType type = EnvironmentType.DEV)
        {
            return true;
        }

        public override bool IsAvailable()
        {
            return true;
        }

        public override bool IsAuthenticated()
        {
            return true;
        }

        public override long ConnectAmond()
        {
            return 1024;
        }

        public override bool GetAccessToken()
        {
            return true;
        }

        public override string GetUser()
        {
            return null;
        }
    }

    public class AmondEditorPlugin : AmondSDKPlugin
    {

        public override bool Init(string gameName, EnvironmentType type = EnvironmentType.DEV)
        {
            return true;
        }

        public override bool IsAvailable()
        {
            return true;
        }

        public override bool IsAuthenticated()
        {
            return true;
        }

        public override long ConnectAmond()
        {
            return 1024;
        }

        public override bool GetAccessToken()
        {
            return true;
        }

        public override string GetUser()
        {
            return null;
        }
    }


    public abstract class AmondSDKPlugin
    {
        public enum EnvironmentType
        {
            LOCAL,
            DEV,
            STAGE,
            PROD,
        }

        public abstract bool Init(string gameName, EnvironmentType type = EnvironmentType.DEV);


        public abstract bool IsAvailable();


        public abstract bool IsAuthenticated();


        public abstract long ConnectAmond();

        public abstract bool GetAccessToken();


        public abstract string GetUser();
    }


    [Serializable]
    public class UserDTO
    {
        public long id;
        public string deviceId;
        public int birthYear;
        public string gender;
        public bool authenticated;
    }
}