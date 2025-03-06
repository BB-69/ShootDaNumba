using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Glock : MonoBehaviour
{
    public ObjectPool objPoolBullet, objPoolBulletInverted;

    private bool isInteracting = false, isActivated = false, toggleFiring = false;
    private RectTransform rect;
    private Vector2 defaultPos, activePos;
    private float time = 0f, duration = 0.15f, snapDelta = 0.03f;

    private Quaternion aim;
    private float originBulletSpeed, bulletSpeed = 10f;
    private float normalFireRate = 0.2f, invertedFireRate = 0.12f, fireRateIncrement = 0f, fireTimer = 0f;
    public Vector2 bulletVelocity;
    private bool bulletInverted = false;

    public static int magCount, maxMag, reloadTrigger = 0, reloadTriggerMax = 3;
    public static bool reload;

    void Start()
    {
        rect = this.GetComponent<RectTransform>();
        defaultPos = rect.anchoredPosition;
        activePos = defaultPos + new Vector2(0f, 0.35f - defaultPos.y);
        originBulletSpeed = bulletSpeed;
        reload = false;

        Reload();
    }

    void Update()
    {
        if (Level.gameOver) return;

        // Bullet Speed & Firing Speed scales with level
        bulletSpeed = originBulletSpeed + (Level.level - 1) / 10f;

        // Toggle between "Hold to Fire" or "Click Once to Toggle Firing"
        if (Input.GetKeyDown(KeyCode.E)) {toggleFiring = !toggleFiring; Debug.Log($"Toggle Firing = {toggleFiring}");}
        //if (Input.GetKeyDown(KeyCode.Q)) {bulletInverted = !bulletInverted; Debug.Log($"Bullet Inverted = {bulletInverted}");}

        if (!toggleFiring) {
            if (Input.GetMouseButton(0)) isInteracting = true;
            else isInteracting = false;
        } else if (Input.GetMouseButtonDown(0)) isInteracting = !isInteracting;
        bulletInverted = Input.GetMouseButton(1) ? true : false;

        // Reload
        if (reloadTrigger >= reloadTriggerMax || reload) {reload = false; Reload();}
        if (magCount > maxMag) magCount = maxMag;

        if (isInteracting) GlockActive();
        else GlockInActive();

        // Set Aim & Shoot
        if (isActivated && (((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !toggleFiring) || toggleFiring)) Shoot();
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

    void Reload()
    {
        maxMag = Level.level * 20;
        magCount = maxMag;
        reloadTrigger = 0;
        if (!AudioManager.CheckPlayingSound("Reload")) AudioManager.PlaySound("Reload");
    }

    void Shoot()
    {
        fireTimer += Time.deltaTime;
        if (fireRateIncrement < 0.06f + (Level.level - 1) / 100f) fireRateIncrement += 0.015f * Time.deltaTime;

        if (magCount <= maxMag && bulletInverted && fireTimer > invertedFireRate - fireRateIncrement * 0.6f) {
            objPoolBulletInverted.InstantiateObjectManually();
            fireTimer = 0f;
            magCount++;
            AudioManager.PlayShootSound(fireRateIncrement, true);
            AudioManager.PlaySound("WindWoosh");
        }

        if (magCount >= 0 && !bulletInverted && fireTimer > normalFireRate - fireRateIncrement) {
            objPoolBullet.InstantiateObjectManually();
            fireTimer = 0f;
            magCount-=2;
            AudioManager.PlayShootSound(fireRateIncrement, false);
            AudioManager.PlaySound("WindWoosh");
        }
    }

    void CeaseFire()
    {
        objPoolBullet.startInstantiate = false;
        if (fireRateIncrement > 0f + (Level.level - 1) / 100f) fireRateIncrement -= 0.03f * Time.deltaTime;
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
