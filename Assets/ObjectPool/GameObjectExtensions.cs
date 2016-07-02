using UnityEngine;

namespace Mandarin {
    public static class GameObjectExtensions {

        public static GOPool Pool(this GameObject go, int num) {
            return GOPool.Create(num)
                .Fill(i => {
                    GameObject instance = GameObject.Instantiate(go);
                    instance.name = go.name + "_" + i;
                    instance.transform.parent = go.transform.parent;
                    return instance.transform;
                });
        }
    }
}
