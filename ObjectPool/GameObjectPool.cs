using System;
using UnityEngine;

namespace Mandarin {
    public class GameObjectPool : ObjectPool<Transform> {

        public GameObjectPool() {
            OnSpawn = OnSpawnGO;
            OnDespawn = OnDespawnGO;
            OnDestroy = OnDestroyGO;
        }

        static public new GameObjectPool Create() {
            return new GameObjectPool();
        }

        public new GameObjectPool SetSize(int size) {
            base.SetSize(size);
            return this;
        }

        public new GameObjectPool Grow(bool grow) {
            base.Grow(grow);
            return this;
        }

        public new GameObjectPool Fill(Func<int, Transform> Instantiate) {
            base.Fill(Instantiate);
            return this;
        }

        private void OnSpawnGO(Transform instance) {
            instance.gameObject.SetActive(true);
        }

        private void OnDespawnGO(Transform instance) {
            instance.gameObject.SetActive(false);
        }

        private void OnDestroyGO(Transform instance) {
            GameObject.Destroy(instance);
        }
    }
}
