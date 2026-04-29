namespace NGDtuanh.Types {
    public class SceneSingleton<T> : Singleton<T> where T : SceneSingleton<T> {
        private protected sealed override bool IsDontDestroyOnLoad => false;
    }
}