using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Read Only")]
    public int activeObj = 0;

    [Header("Setup")]
    // This
    public GameObject prefab;
    public GameObject[] prefabObj;
    public Transform prefabPool;
    public int initialPoolSize = 5;
    private float instantiateInterval = 0.5f, destroyInterval = 10f;
    public float loopInterval = 2f;
    public bool startInstantiate = false;
    private List<GameObject> objectPool = new List<GameObject>();

    public Transform sideWall;
    public Transform glockTransform;

    
    private Vector2 spawnPosBall = new Vector3(0f, 1080f), targetPosBall;
    private Vector2 spawnPosBullet;
    public Vector2 despawnPos = new Vector2(0f, 0f);
    private System.Random rnd = new System.Random();

    // Ball
    [Header("Ball Setup")]
    private float ballLoop = 0f;
    public int ballMedian = 5;

    [Header("Glock Setup")]
    public Glock glock;

    [Header("Explosion Setup")]
    public Explosion explosion;

    private void Start()
    {
        despawnPos.y = sideWall.localPosition.y + sideWall.localScale.y/2;

        // Pooling
        foreach (GameObject obj in prefabObj) {objectPool.Add(obj); obj.SetActive(false); activeObj++;}

        if (objectPool.Count <= initialPoolSize)
        for (int i = 0; i < initialPoolSize - objectPool.Count; i++) {AddObjectToPool();}

        StartCoroutine(CheckActiveObj());
        StartCoroutine(CheckInActiveObj());
        if (startInstantiate) StartCoroutine(InstantiateObject());
    }

    void Update()
    {
        // Ball
        ballLoop += Time.deltaTime;
        if (ballLoop > 6f) {ballMedian++; ballLoop = 0f;}
    }

    private IEnumerator CheckInActiveObj()
    {
        while (true) {
            yield return new WaitForSeconds(destroyInterval);

            if (activeObj <= objectPool.Count - initialPoolSize && objectPool.Count >= initialPoolSize) {
                Destroy(RemoveObjectFromPool());
            }
        }
    }

    private IEnumerator CheckActiveObj()
    {
        while (true) {
            yield return new WaitForSeconds(instantiateInterval);

            if (activeObj > objectPool.Count - initialPoolSize) {
                AddObjectToPool();
            }
        }
    }

    public IEnumerator InstantiateObject()
    {
        startInstantiate = true;
        while (true) {
            yield return new WaitForSeconds(loopInterval);
            if (Level.gameOver) break;
            InstantiateObjectManually();
        }
    }

    public void InstantiateObjectManually()
    {
        GameObject obj = GetObjectFromPool();
        activeObj++;

        if (glock != null) {
            glock.SetBulletProperty(obj);
        }
        if (explosion != null) {
            Ball.SetExplosionPos(obj);
        }

        obj.SetActive(true);
    }

    private GameObject GetObjectFromPool()
    {
        RandomPos();
        foreach (GameObject obj in objectPool) if (!obj.activeInHierarchy) {
            obj.GetComponent<RectTransform>().anchoredPosition = targetPosBall;
            obj.GetComponent<RectTransform>().rotation = new Quaternion(0f, 0f, 0f, 1f);
            return obj;
        }
        return AddObjectToPool();
    }

    private GameObject AddObjectToPool()
    {
        RandomPos();
        GameObject obj = Instantiate(prefab, targetPosBall, Quaternion.identity, prefabPool);
        obj.SetActive(false);
        objectPool.Add(obj);
        return obj;
    }

    private GameObject RemoveObjectFromPool()
    {
        for (int i = objectPool.Count - 1; i >= 0; i--) if (!objectPool[i].activeInHierarchy) {
            GameObject obj = objectPool[i];
            try {objectPool.Remove(obj);}
            catch (Exception e) {Console.WriteLine(e.Message); obj = null;}
            return obj;
        }

        return null;
    }

    public void RemoveObjectFromField(GameObject obj)
    {
        if (objectPool.Contains(obj)) obj.SetActive(false);
    }

    public void ActivateObjectOnField(GameObject obj)
    {
        if (objectPool.Contains(obj) && !obj.activeInHierarchy) obj.SetActive(true);
    }

    private void RandomPos() {targetPosBall = spawnPosBall; targetPosBall.x += rnd.Next(-500, 500);}
}
