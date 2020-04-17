using UnityEngine;

namespace Amond.Plugins
{
    /// <summary>
    /// Andorid Callback 받기 위한 클래스, 자동 생성
    /// 수정 하지 마시오
    /// </summary>
    public class AmondCallback : MonoBehaviour
    {
        private static GameObject _instance;

        private void Awake()
        {
            //Only One Unity GameObject
            if (_instance)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = gameObject;
                DontDestroyOnLoad(this);
            }
        }

        public void SetAvailable(string isAvailable)
        {
            AmondPlugin.GetInstance().SetAvailable(isAvailable);
            AmondPlugin.GetInstance().SetCallbackIsAvailable(isAvailable);
        }
    }
}