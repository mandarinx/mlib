using System;
using System.Collections.Generic;

namespace Mandarin {

    public class ObjectPool<T> where T : class {

        private List<T>         actives;
        private List<T>         deactives;
        private int             max;
        private Func<int, T>    Instantiate;

        protected Action<T>     OnDestroy;
        protected Action<T>     OnSpawn;
        protected Action<T>     OnDespawn;

        public ObjectPool() {
            OnDestroy = t => {};
            OnSpawn = t => {};
            OnDespawn = t => {};
            max = -1;
        }

        static public ObjectPool<T> Create() {
            return new ObjectPool<T>();
        }

        public virtual ObjectPool<T> SetOnDestroy(Action<T> cb) {
            OnDestroy = cb;
            return this;
        }

        public virtual ObjectPool<T> SetOnSpawn(Action<T> cb) {
            OnSpawn = cb;
            return this;
        }

        public virtual ObjectPool<T> SetOnDespawn(Action<T> cb) {
            OnDespawn = cb;
            return this;
        }

        // Set the initial size of the pool. This is the number of
        // objects that will be created when filling the pool.
        public virtual ObjectPool<T> SetSize(int size) {
            actives = new List<T>(size);
            deactives = new List<T>(size);
            return this;
        }

        // Set whether it should grow or not when trying to spawn more
        // instances than there are slots in the pool.
        public virtual ObjectPool<T> Grow(bool grow) {
            max = grow ? -1 : actives.Capacity;
            return this;
        }

        // Fills the pool with instances of the specified type.
        public virtual ObjectPool<T> Fill(Func<int, T> Instantiate) {
            this.Instantiate = Instantiate;
            while (deactives.Count < deactives.Capacity) {
                AddInstance(deactives.Count);
            }
            return this;
        }

        // Retrieve an object from the pool.
        // Param instance : A reference to hold the returned instance.
        // Returns : True when the pool has an available instance, and
        // false when it doesn't.
        public bool Spawn(out T instance) {
            instance = GetInstance();
            if (instance == default(T)) {
                return false;
            }
            OnSpawn(instance);
            return true;
        }

        // Puts an instance back into the pool.
        // Param instance : A reference to an instance previously
        // retrieved from the pool.
        public void Despawn(T instance) {
            int index = GetIndexOf(instance);
            if (index < 0) {
                return;
            }
            OnDespawn(instance);
            deactives.Add(instance);
            actives.RemoveAt(index);
        }

        // Despawn all active instances.
        public void Reset() {
            while (actives.Count > 0) {
                Despawn(actives[0]);
            }
        }

        // Destroy the pool and all instances. A pool needs to be
        // reinitialized to be reused after being destroyed.
        public void Destroy() {
            DestroyInstances(deactives);
            DestroyInstances(actives);

            deactives.Clear();
            deactives = null;

            actives.Clear();
            actives = null;
        }

        // Destroys all instances in a given List
        private void DestroyInstances(List<T> list) {
            int l = list.Count;
            for (int i=0; i<l; i++) {
                OnDestroy(list[i]);
            }
        }

        // Add a new instance to the pool
        private void AddInstance(int num) {
            T instance = Instantiate(num);
            deactives.Add(instance);
            OnDespawn(instance);
        }

        // Get the index of an active instance.
        private int GetIndexOf(T instance) {
            if (actives.Count == 0) {
                return -1;
            }

            int l = actives.Count;
            for (int i=0; i<l; i++) {
                if (actives[i].Equals(instance)) {
                    return i;
                }
            }

            return -1;
        }

        // Retrieves an instance from the pool. If no deactives are available,
        // and is allowed to grow, instantiates a new instance.
        private T GetInstance() {
            if (deactives.Count == 0) {
                if (max >= 0) {
                    return null;
                }
                AddInstance(actives.Count);
            }

            T instance = deactives[0];
            actives.Add(instance);
            deactives.RemoveAt(0);
            return instance;
        }

        public int Capacity {
            get { return max < 0 ? int.MaxValue : max; }
        }

        public int NumItems {
            get { return actives.Count + deactives.Count; }
        }

        public List<T> Actives {
            get { return actives; }
        }

        override public string ToString() {
            return "ObjectPool<" + typeof(T) + "> " +
                "Actives: " + actives.Count + "/" + actives.Capacity + " " +
                "Deactives: " + deactives.Count + "/" + deactives.Capacity + " " +
                "Grow: " + (max < 0 ? "true" : "false");
        }
    }
}
