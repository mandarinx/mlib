using UnityEngine;

namespace Mandarin {
    public static class GameObjectPoolExtensions {

        public static GameObjectPool Pool(this GameObject go, int num) {
            return GameObjectPool.Create()
                .SetSize(num)
                .Fill(i => {
                    GameObject instance = GameObject.Instantiate(go);
                    instance.name = go.name + "_" + i;
                    instance.transform.parent = go.transform.parent;
                    return instance.transform;
                });
        }
    }
}
