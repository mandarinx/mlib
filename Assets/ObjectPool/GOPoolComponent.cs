using System;
using UnityEngine;

namespace Mandarin {
    public class GOPool<T> where T : Component {

        private ObjectPool<T>   pool;
        protected Transform     parent;

        public GOPool(int size) {
            pool = ObjectPool<T>.Create(size);
            pool.SetOnSpawn(OnSpawnGO);
            pool.SetOnDespawn(OnDespawnGO);
            pool.SetOnDestroy(OnDestroyGO);
        }

        static public GOPool<T> Create(int size) {
            return new GOPool<T>(size);
        }

        public virtual GOPool<T> Resize(int size) {
            pool.Resize(size);
            return this;
        }

        public virtual GOPool<T> Grow(bool grow) {
            pool.Grow(grow);
            return this;
        }

        public virtual GOPool<T> Parent(Transform parent) {
            this.parent = parent;
            return this;
        }

        // Fill whatever you choose
        public virtual GOPool<T> Fill(Func<int, T> Instantiate) {
            pool.Fill(InstantiateCb(Instantiate, parent));
            return this;
        }

        // Fill with an empty GameObject
        public virtual GOPool<T> Fill() {
            pool.Fill(InstantiateGO(parent));
            return this;
        }

        // Fill with a GameObject loaded from Resources
        public virtual GOPool<T> Fill(string path) {
            pool.Fill(InstantiateResources(path, parent));
            return this;
        }

        // Fill using a reference to an existing GameObject
        public virtual GOPool<T> Fill(GameObject go) {
            pool.Fill(InstantiateRef(go, parent));
            return this;
        }

        public int numActives {
            get { return pool.numActives; }
        }

        public int numDeactives {
            get { return pool.numDeactives; }
        }

        public int numItems {
            get { return pool.numItems; }
        }

        public bool Spawn(out T instance) {
            return pool.Spawn(out instance);
        }

        public virtual GOPool<T> Despawn(T instance) {
            pool.Despawn(instance);
            return this;
        }

        public virtual GOPool<T> SetOnSpawn(Action<T> cb) {
            pool.SetOnSpawn(t => {
                cb(t);
                OnSpawnGO(t);
            });
            return this;
        }

        public virtual GOPool<T> SetOnDespawn(Action<T> cb) {
            pool.SetOnDespawn(t => {
                cb(t);
                OnDespawnGO(t);
            });
            return this;
        }

        static protected void OnSpawnGO(T instance) {
            instance.gameObject.SetActive(true);
        }

        static protected void OnDespawnGO(T instance) {
            instance.gameObject.SetActive(false);
        }

        static private void OnDestroyGO(T instance) {
            GameObject.Destroy(instance);
        }

        static protected Func<int, T> InstantiateCb(Func<int, T>  cb,
                                                    Transform     parent) {
            return i => {
                T t = cb(i);
                t.transform.parent = parent;
                return t;
            };
        }

        static protected Func<int, T> InstantiateRef(GameObject   go,
                                                     Transform    parent) {
            return i => {
                GameObject goi = GameObject.Instantiate(go);
                goi.name = go.name + "_" + i;
                goi.transform.parent = parent;
                return goi.GetComponent<T>();
            };
        }

        static protected Func<int, T> InstantiateResources(string     path,
                                                           Transform  parent) {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab == null) {
                throw new Exception("Cannot find GameObject "+path);
            }
            string name = prefab.name;
            return i => {
                Transform t = GameObject.Instantiate(prefab).transform;
                t.parent = parent;
                t.name = name + "_" + i;
                return t.GetComponent<T>();
            };
        }

        static protected Func<int, T> InstantiateGO(Transform parent) {
            return i => {
                Transform t = new GameObject().transform;
                t.name = "Transform_" + i;
                t.parent = parent;
                return t.GetComponent<T>() ?? t.gameObject.AddComponent<T>();
            };
        }
    }
}
