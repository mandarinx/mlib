﻿using UnityEngine;using Mandarin;public class PoolCreator : MonoBehaviour {    void Start() {        GOPool p = GOPool.Create(2)            .SetOnSpawn(t => {                t.transform.position = Vector3.right;                t.gameObject.SetActive(false);            })            .Fill("Cube");        Transform i;        p.Spawn(out i);    }}