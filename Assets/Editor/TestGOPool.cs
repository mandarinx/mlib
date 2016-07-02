using UnityEngine;
using NUnit.Framework;
using Mandarin;

public class TestGOPool {

    [Test]
    public void Spawn() {
        GOPool p = GOPool.Create(2).Fill();
        Transform t;
        p.Spawn(out t);
        Assert.AreEqual(p.numActives, 1);
    }

    [Test]
    public void Despawn() {
        GOPool p = GOPool.Create(2).Fill();
        Transform t;
        p.Spawn(out t);
        p.Despawn(t);
        Assert.AreEqual(p.numActives, 0);
    }

    [Test]
    public void PoolSize() {
        Transform parent = new GameObject("Parent").transform;

        GOPool.Create(5)
            .Parent(parent)
            .Fill();

        Assert.AreEqual(parent.childCount, 5);
    }

    [Test]
    public void LimitGrow() {
        GOPool p = GOPool.Create(1).Fill();

        Transform t1;
        Transform t2;
        int s = p.Spawn(out t1) ? 1 : 0;
        s += p.Spawn(out t2) ? 1 : 0;
        Assert.AreEqual(s, 1);
    }

    [Test]
    public void CanGrow() {
        GOPool p = GOPool.Create(1)
            .Grow(true)
            .Fill();

        Transform t1;
        Transform t2;
        int s = p.Spawn(out t1) ? 1 : 0;
        s += p.Spawn(out t2) ? 1 : 0;
        Assert.AreEqual(s, 2);
    }

    [Test]
    public void SetParent() {
        Transform parent = new GameObject("Parent").transform;

        GOPool p = GOPool.Create(2)
            .Parent(parent)
            .Fill();

        Transform t;
        p.Spawn(out t);
        Assert.AreEqual(t.parent, parent);
    }

    [Test]
    public void FillResourcesPath() {
        GOPool p = GOPool.Create(2)
            .Fill("Cube");

        Transform t;
        p.Spawn(out t);
        Assert.AreEqual(t.name, "Cube_0");
    }

    [Test]
    public void FillLambda() {
        GOPool p = GOPool.Create(2)
            .Fill(i => {
                GameObject go = new GameObject("GO");
                go.name += "_" + i;
                return go.transform;
            });

        Transform t;
        p.Spawn(out t);
        Assert.AreEqual(t.name, "GO_0");
    }

    [Test]
    public void FillReference() {
        GameObject prefab = Resources.Load<GameObject>("Cube");
        GameObject go = GameObject.Instantiate(prefab);

        GOPool p = GOPool.Create(2)
            .Fill(go);

        Transform t;
        p.Spawn(out t);
        Assert.AreEqual(go.GetComponent<CubeBehaviour>().GetType(),
                        t.GetComponent<CubeBehaviour>().GetType());
    }

    [Test]
    public void FillResourcesLoad() {
        GameObject go = Resources.Load<GameObject>("Cube");
        GOPool p = GOPool.Create(2)
            .Fill(go);

        Transform t;
        p.Spawn(out t);
        Assert.AreEqual(go.GetComponent<CubeBehaviour>().GetType(),
                        t.GetComponent<CubeBehaviour>().GetType());
    }

    [Test]
    public void SetOnSpawn() {
        GOPool p = GOPool.Create(2)
            .SetOnSpawn(t => {
                t.transform.position = Vector3.right;
                // GOPool activates the object after the lambda has
                // run. This should be reverted after Spawn() has run.
                t.gameObject.SetActive(false);
            })
            .Fill("Cube");

        Transform i;
        p.Spawn(out i);
        Assert.AreEqual(i.position, Vector3.right);
        Assert.AreEqual(i.gameObject.activeSelf, true);
    }

    [Test]
    public void SetOnDespawn() {
        GOPool p = GOPool.Create(2)
            .SetOnDespawn(t => {
                t.transform.position = -Vector3.right;
                // GOPool deactivates the object after the lambda has
                // run. This should be reverted after Spawn() has run.
                t.gameObject.SetActive(true);
            })
            .Fill("Cube");

        Transform i;
        p.Spawn(out i);
        p.Despawn(i);
        Assert.AreEqual(i.position, -Vector3.right);
        Assert.AreEqual(i.gameObject.activeSelf, false);
    }
}
