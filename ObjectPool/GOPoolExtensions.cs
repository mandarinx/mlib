using UnityEngine;

namespace Mandarin {
    public static class GOPoolExtensions {

        public static GameObjectPool Pool(this GO go, int num) {
            return GameObjectPool.Create()
                .SetSize(num)
                .Fill(i => {
                    return go.Duplicate().Modify()
                        .SetName(go.gameObject.name + "_" + i)
                        .SetParent(go.transform.parent)
                        .transform;
                });
        }
    }
}
