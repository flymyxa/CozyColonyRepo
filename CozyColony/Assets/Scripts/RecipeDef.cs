
using UnityEngine;
namespace CozyWorld
{
    [CreateAssetMenu(fileName = "RecipeDef", menuName = "CozyWorld/Recipe Definition", order = 1)]
    public class RecipeDef : ScriptableObject
    {
        public string id;                // unique key, e.g. "cook_berry"
        public Inventory[] inputs;       // items consumed
        public Inventory[] outputs;      // items produced
        public float workTime = 3f;      // seconds of work required
    }
}