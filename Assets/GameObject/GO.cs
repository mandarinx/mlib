using System;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Mlib {

    public class GO {
        public static GO Create(string name = "GameObject") {
            return new GO(name);
        }

        public static GO Create(PrimitiveType type) {
            return new GO(GameObject.CreatePrimitive(type));
        }

        public static GO Modify(GameObject go) {
            return new GO(go);
        }

        public static GO Instantiate(GameObject prefab) {
            return new GO(Object.Instantiate(prefab));
        }

        public GO(string name = "GameObject") {
            GameObject = new GameObject(name);
        }

        public GO(GameObject go) {
            GameObject = go;
        }

        public GO SetName(string name) {
            GameObject.name = name;
            return this;
        }

        public GO SetLayer(string name) {
            GameObject.layer = LayerMask.NameToLayer(name);
            return this;
        }

        public GO SetParent(Transform parent, bool worldPositionStays = true) {
            GameObject.transform.SetParent(parent, worldPositionStays);
            return this;
        }

        public GO SetParent(GO parent, bool worldPositionStays = true) {
            GameObject.transform.SetParent(parent.Transform, worldPositionStays);
            return this;
        }

        public GO SetAsLastSibling() {
            GameObject.transform.SetAsLastSibling();
            return this;
        }

        public GO SetActive(bool active = true) {
            GameObject.SetActive(active);
            return this;
        }

        public GO SetMesh(Mesh mesh) {
            GetSetComp<MeshFilter>().sharedMesh = mesh;
            return this;
        }

        public GO SetMaterial(Material mat) {
            GetSetComp<MeshRenderer>().material = mat;
            return this;
        }

        public GO SetSharedMaterial(Material mat) {
            GetSetComp<MeshRenderer>().sharedMaterial = mat;
            return this;
        }

        public GO ReceiveShadows(bool receiveShadows) {
            GetSetComp<MeshRenderer>().receiveShadows = receiveShadows;
            return this;
        }

        public GO CastShadows(ShadowCastingMode castShadows) {
            GetSetComp<MeshRenderer>().shadowCastingMode = castShadows;
            return this;
        }

        public GO SetScale(Vector3 scale) {
            GameObject.transform.localScale = scale;
            return this;
        }

        public GO SetPosition(Vector3 position) {
            GameObject.transform.position = position;
            return this;
        }

        public GO SetLocalPosition(Vector3 position) {
            GameObject.transform.localPosition = position;
            return this;
        }

        public GO SetRotation(Quaternion quaternion, bool local = false) {
            if (local) {
                GameObject.transform.localRotation = quaternion;
            } else {
                GameObject.transform.rotation = quaternion;
            }
            return this;
        }

        public GO AddBoxCollider(Vector3 size, Vector3 center = default(Vector3)) {
            BoxCollider collider = GetSetComp<BoxCollider>();
            collider.size = size;
            collider.center = center;
            return this;
        }

        public GO AddComponent<T>(Action<T> callback = null) where T : Component {
            callback?.Invoke(GameObject.AddComponent<T>());
            return this;
        }

        public GO AddComponent(Type type, Action<Component> callback = null) {
            callback?.Invoke(GameObject.AddComponent(type));
            return this;
        }

        public GO GetComponent<T>(Action<T> callback) where T : Component {
            T comp = GameObject.GetComponent<T>();
            if (comp != null) {
                callback(comp);
            }
            return this;
        }

        public void Destroy() {
            Object.Destroy(GameObject);
        }

        public void Destroy(float afterSeconds) {
            Object.Destroy(GameObject, afterSeconds);
        }

        public GameObject Duplicate() {
            return Object.Instantiate(GameObject);
        }

        private T GetSetComp<T>() where T : Component {
            T comp = GameObject.GetComponent<T>();
            if (comp == null) {
                comp = GameObject.AddComponent<T>();
            }
            return comp;
        }

        public GameObject GameObject { get; }
        public Transform Transform => GameObject.transform;
    }
}
