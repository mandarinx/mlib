using System;
using System.Collections.Generic;

namespace Mandarin {

    public class ObjectPool<T> where T : class {

        private List<T>         deactives;
        private int             max;

        protected Func<int, T>  Instantiate;
        protected Action<T>     OnDestroy;
        protected Action<T>     OnSpawn;
        protected Action<T>     OnDespawn;

        public int capacity {
            get { return max < 0 ? int.MaxValue : max; }
        }

        public int numItems {
            get { return numActives + numDeactives; }
        }

        public int      numActives { get; private set; }
        public int      numDeactives { get; private set; }
        public List<T>  actives { get; private set; }

        public ObjectPool(int size) {
            OnDestroy = t => {};
            OnSpawn = t => {};
            OnDespawn = t => {};
            max = size;
            actives = new List<T>(size);
            deactives = new List<T>(size);
            numActives = 0;
            numDeactives = 0;
        }

        static public ObjectPool<T> Create(int size) {
            return new ObjectPool<T>(size);
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

        // Resize capacity
        public virtual ObjectPool<T> Resize(int size) {
            actives.Capacity = size;
            deactives.Capacity = size;
            return this;
        }

        // Set whether it should grow or not when trying to spawn more
        // instances than there are slots in the pool.
        public virtual ObjectPool<T> Grow(bool grow) {
            max = grow ? -1 : max;
            return this;
        }

        // Fills the pool with instances of the specified type.
        public virtual ObjectPool<T> Fill(Func<int, T> Instantiate) {
            this.Instantiate = Instantiate;
            while (numDeactives < deactives.Capacity) {
                AddInstance(numDeactives);
            }
            return this;
        }

        // Fills the pool with instances of the specified type.
        public virtual ObjectPool<T> Fill() {
            Instantiate = i => {
                return default(T);
            };
            while (numDeactives < deactives.Capacity) {
                AddInstance(numDeactives);
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
            numDeactives++;
            actives.RemoveAt(index);
            numActives--;
        }

        // Despawn all active instances.
        public void Reset() {
            while (numActives > 0) {
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
                list[i] = null;
            }
        }

        // Add a new instance to the pool
        private void AddInstance(int num) {
            T instance = Instantiate(num);
            deactives.Add(instance);
            numDeactives++;
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
                AddInstance(numActives);
            }

            T instance = deactives[0];
            actives.Add(instance);
            numActives++;
            deactives.RemoveAt(0);
            numDeactives--;
            return instance;
        }

        public ObjectPool<T> Each(Action<T> apply) {
            for (int i=0; i<numActives; i++) {
                apply(actives[i]);
            }
            for (int i=0; i<numDeactives; i++) {
                apply(deactives[i]);
            }
            return this;
        }

        override public string ToString() {
            return "ObjectPool<" + typeof(T) + "> " +
                "Actives: " + numActives + "/" + actives.Capacity + " " +
                "Deactives: " + numDeactives + "/" + deactives.Capacity + " " +
                "Grow: " + (max < 0 ? "true" : "false");
        }
    }
}
