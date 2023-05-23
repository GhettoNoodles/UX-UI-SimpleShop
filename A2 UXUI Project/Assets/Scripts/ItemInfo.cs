using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Shop Item", order = 0)]
    public class ItemInfo : ScriptableObject
    {
        public int price;
        public Sprite sprite;
        public bool upgrade;
    }
}