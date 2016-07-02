using System;
using UnityEngine;

namespace Mandarin {

    public class GOPool : GOPool<Transform> {

        static public new GOPool Create(int size) {
            return new GOPool(size);
        }

        public GOPool(int size) : base(size) {}

        public new GOPool Resize(int size) {
            base.Resize(size);
            return this;
        }

        public new GOPool Grow(bool grow) {
            base.Grow(grow);
            return this;
        }

        public new GOPool Parent(Transform parent) {
            base.parent = parent;
            return this;
        }

        // Fill whatever you choose
        public new GOPool Fill(Func<int, Transform> Instantiate) {
            base.Fill(InstantiateCb(Instantiate, parent));
            return this;
        }

        // Fill with an empty GameObject
        public new GOPool Fill() {
            base.Fill(InstantiateGO(parent));
            return this;
        }

        // Fill with a GameObject loaded from Resources
        public new GOPool Fill(string path) {
            base.Fill(InstantiateResources(path, parent));
            return this;
        }

        // Fill using a reference to an existing GameObject
        public new GOPool Fill(GameObject go) {
            base.Fill(InstantiateRef(go, parent));
            return this;
        }

        public new GOPool Despawn(Transform instance) {
            base.Despawn(instance);
            return this;
        }

        public new GOPool SetOnSpawn(Action<Transform> cb) {
            base.SetOnSpawn(t => {
                cb(t);
                OnSpawnGO(t);
            });
            return this;
        }

        public new GOPool SetOnDespawn(Action<Transform> cb) {
            base.SetOnDespawn(t => {
                cb(t);
                OnDespawnGO(t);
            });
            return this;
        }
    }
}
