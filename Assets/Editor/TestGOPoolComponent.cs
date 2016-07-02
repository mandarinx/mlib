using UnityEngine;
using NUnit.Framework;
using Mandarin;

public class TestGOPoolComponent {

    [Test]
    public void Spawn() {
        GOPool<CubeBehaviour> p = GOPool<CubeBehaviour>
            .Create(2)
            .Fill();
        CubeBehaviour cb;
        p.Spawn(out cb);
        Assert.AreEqual(p.numActives, 1);
    }

    [Test]
    public void Despawn() {
        CubeBehaviour cb;
        GOPool<CubeBehaviour> p = GOPool<CubeBehaviour>
            .Create(2)
            .Fill();
        p.Spawn(out cb);
        p.Despawn(cb);
        Assert.AreEqual(p.numActives, 0);
    }

    [Test]
    public void PoolSize() {
        Transform parent = new GameObject("Parent").transform;

        GOPool<CubeBehaviour>.Create(5)
            .Parent(parent)
            .Fill();

        Assert.AreEqual(parent.childCount, 5);
    }

    [Test]
    public void LimitGrow() {
        GOPool<CubeBehaviour> p = GOPool<CubeBehaviour>.Create(1).Fill();

        CubeBehaviour cb1;
        CubeBehaviour cb2;
        int s = p.Spawn(out cb1) ? 1 : 0;
        s += p.Spawn(out cb2) ? 1 : 0;
        Assert.AreEqual(s, 1);
    }

    [Test]
    public void CanGrow() {
        GOPool<CubeBehaviour> p = GOPool<CubeBehaviour>
            .Create(1)
            .Grow(true)
            .Fill();

        CubeBehaviour cb1;
        CubeBehaviour cb2;
        int s = p.Spawn(out cb1) ? 1 : 0;
        s += p.Spawn(out cb2) ? 1 : 0;
        Assert.AreEqual(s, 2);
    }

    [Test]
    public void SetParent() {
        Transform parent = new GameObject("Parent").transform;

        GOPool<CubeBehaviour> p = GOPool<CubeBehaviour>
            .Create(2)
            .Parent(parent)
            .Fill();

        CubeBehaviour cb;
        p.Spawn(out cb);
        Assert.AreEqual(cb.transform.parent, parent);
    }

    [Test]
    public void DefaultFill() {
        CubeBehaviour cb;
        GOPool<CubeBehaviour>
            .Create(2)
            .Fill()
            .Spawn(out cb);
        Assert.AreEqual(cb.GetType(), typeof(CubeBehaviour));
    }

    [Test]
    public void FillResourcesPath() {
        CubeBehaviour cb;
        GOPool<CubeBehaviour>
            .Create(2)
            .Fill("Cube")
            .Spawn(out cb);
        Assert.AreEqual(cb.GetType(), typeof(CubeBehaviour));
    }

    [Test]
    public void FillLambda() {
        CubeBehaviour cb;
        GOPool<CubeBehaviour>
            .Create(2)
            .Fill(i => {
                return new GameObject().AddComponent<CubeBehaviour>();
            })
            .Spawn(out cb);
        Assert.AreEqual(cb.GetType(), typeof(CubeBehaviour));
    }

    [Test]
    public void FillReference() {
        GameObject prefab = Resources.Load<GameObject>("Cube");
        GameObject go = GameObject.Instantiate(prefab);
        CubeBehaviour cb;
        GOPool<CubeBehaviour>
            .Create(2)
            .Fill(go)
            .Spawn(out cb);
        Assert.AreEqual(cb.GetType(), typeof(CubeBehaviour));
    }

    [Test]
    public void FillResourcesLoad() {
        GameObject go = Resources.Load<GameObject>("Cube");
        CubeBehaviour cb;
        GOPool<CubeBehaviour>
            .Create(2)
            .Fill(go)
            .Spawn(out cb);
        Assert.AreEqual(cb.GetType(), typeof(CubeBehaviour));
    }
}
