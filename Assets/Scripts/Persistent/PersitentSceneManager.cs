using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    public class LoadSceneManager : MonoBehaviour
    {
        public const string PERSISTENT_SCENE = "PersistentScebe";
        public const string MENU_SCENE = "MenuScene";
        public const string GAME_SCENE = "GameScene";
        private void Start()
        {
            if (!SceneLoaded(MENU_SCENE))
            {
                LoadSceneAsyncAdditive(MENU_SCENE);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            // Any Event
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public static void UnloadSceneAsync(string name)
        {
            SceneManager.UnloadSceneAsync(name);
        }

        public static async Task LoadSceneAsyncAdditive(string name)
        {
            await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        }

        public static void CheckForPersistentScene()
        {
            if (!SceneLoaded(PERSISTENT_SCENE))
            {
                SceneManager.LoadScene(PERSISTENT_SCENE);
            }
        }

        internal static bool SceneLoaded(string name)
        {
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene.name == name)
                {
                    return true;
                }
            }
            return false;
        }

    }
}