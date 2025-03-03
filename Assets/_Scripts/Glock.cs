using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Glock : MonoBehaviour
{
    public ObjectPool objPool;

    private bool isInteracting = false, isActivated = false, toggleFiring = false;
    public RectTransform rect;
    private Vector2 defaultPos, activePos;
    private float time = 0f, duration = 0.15f, snapDelta = 0.03f;

    private Quaternion aim;
    private float bulletSpeed = 10f;
    private float fireRate = 0.2f, fireTimer = 0f;
    public Vector2 bulletVelocity;

    void Start()
    {
        rect = this.GetComponent<RectTransform>();
        defaultPos = rect.anchoredPosition;
        activePos = defaultPos + new Vector2(0f, 0.35f - defaultPos.y);
    }

    void Update()
    {
        // Toggle between "Hold to Fire" or "Click Once to Toggle Firing"
        if (Input.GetKeyDown(KeyCode.E)) {toggleFiring = !toggleFiring; Debug.Log($"Toggle Firing = {toggleFiring}");}

        if (!toggleFiring) {
            if (Input.GetMouseButton(0)) isInteracting = true;
            else isInteracting = false;
        } else if (Input.GetMouseButtonDown(0)) isInteracting = !isInteracting;

        if (isInteracting) GlockActive();
        else GlockInActive();

        // Set Aim & Shoot
        if (isActivated && ((Input.GetMouseButton(0) && !toggleFiring) || toggleFiring)) Shoot();
        else CeaseFire();


    }

    void GlockActive()
    {
        if (rect.anchoredPosition == activePos) {isActivated = true; return;}
        else if (Vector2.Distance(rect.anchoredPosition, activePos) < snapDelta) {rect.anchoredPosition = activePos; time = 0f;
            fireTimer = 0f;}
        else {
            time += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(defaultPos, activePos, time / duration);
        }
    }

    void GlockInActive()
    {
        isActivated = false;

        if (rect.anchoredPosition == defaultPos) return;
        else if (Vector2.Distance(rect.anchoredPosition, defaultPos) < snapDelta) {rect.anchoredPosition = defaultPos; time = 0f;}
        else {
            time += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(activePos, defaultPos, time / duration);
        }
        
    }

    void Shoot()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer > fireRate) {
            objPool.InstantiateObjectManually();
            fireTimer = 0f;
        }
        if (fireRate > 0.08f) fireRate -= 0.03f * Time.deltaTime;
    }

    void CeaseFire()
    {
        objPool.startInstantiate = false;
        if (fireRate < 0.2f) fireRate += 0.06f * Time.deltaTime;
    }

    public void SetBulletProperty(GameObject bullet)
    {
        aim = GetComponentInParent<Transform>().transform.rotation;
        bullet.GetComponent<RectTransform>().rotation = aim;
        bullet.GetComponent<Transform>().position = GetComponent<Transform>().position;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;

        bulletVelocity = direction.normalized * bulletSpeed;
    }
}
