using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.AudioPooler
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null && SceneManager.GetActiveScene().isLoaded)
                    {
                        GameObject singleton = new GameObject("[Audio Pooler] " + typeof(T).Name);
                        instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);
                    }
                }

                return instance;
            }
        }
    }
}
