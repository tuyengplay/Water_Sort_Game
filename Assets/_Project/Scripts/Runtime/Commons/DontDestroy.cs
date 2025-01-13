using UnityEngine;
namespace WaterSort
{
    public class DontDestroy : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}