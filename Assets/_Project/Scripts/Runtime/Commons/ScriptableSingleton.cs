using System.Collections;
using UnityEngine;
namespace WaterSort
{
    public class ScriptableSingleton<I> : ScriptableObject where I : ScriptableObject
    {
        protected static I instance;

        public static IEnumerator LoadAsync()
        {
            var name = typeof(I).Name;
            var request = Resources.LoadAsync<I>($"Managers/{name}");
            yield return new WaitUntil(() => request.isDone);
            instance = (I)request.asset;
        }
        [SerializeField]
        private bool load;
        private void OnValidate()
        {
            if (!load)
            {
                return;
            }
            load = false;
            OnLoad();
        }
        protected virtual void OnLoad()
        {
            I a = Resources.Load<I>($"Managers/{name}");
            if (a == null)
            {
                EDebug.Log($"Not found {typeof(I).Name} in Resources\\Managers\\ : FAILD");
            }
            else
            {
                EDebug.Log($"Load {typeof(I).Name} : SUCCESS");
            }
        }


    }
}
