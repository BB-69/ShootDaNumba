using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static Dictionary<string, AudioSource> audioSource = new Dictionary<string, AudioSource>();
    private static System.Random rnd = new System.Random();

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            string name = child.name;
            AudioSource audio = child.GetComponent<AudioSource>();
            audioSource[name] = audio;
        }
    }

    // Fuck, so many overloads LMAO
    public static void PlaySound(string name) {
        if (!audioSource.Keys.Any(key => key.Contains(name))) return;
        audioSource[name].PlayOneShot(audioSource[name].clip);
    }
    public static void PlaySound(string name, float pitch) {PlaySound(name, pitch, pitch);}
    public static void PlaySound(string name, float minPitch, float maxPitch)
    {
        if (!audioSource.Keys.Any(key => key.Contains(name))) return;
        float pitch = (minPitch == maxPitch) ? minPitch : Random.Range(minPitch, maxPitch);

        audioSource[name].pitch = pitch;
        audioSource[name].PlayOneShot(audioSource[name].clip);
    }
    public static void PlaySound(string name, float minPitch, float maxPitch, bool pitchToVol)
    {
        if (pitchToVol) PlaySound(name, minPitch, maxPitch, minPitch, maxPitch);
        else PlaySound(name, minPitch, maxPitch);
    }
    public static void PlaySound(string name, float minPitch, float maxPitch, float vol) {PlaySound(name, minPitch, maxPitch, vol, vol);}
    public static void PlaySound(string name, float minPitch, float maxPitch, float minVol, float maxVol)
    {
        if (!audioSource.Keys.Any(key => key.Contains(name))) return;
        float pitch = (minPitch == maxPitch) ? minPitch : Random.Range(minPitch, maxPitch);
        float vol = (minVol == maxVol) ? minVol : Random.Range(minVol, maxVol);

        audioSource[name].pitch = pitch;
        audioSource[name].volume = vol;
        audioSource[name].PlayOneShot(audioSource[name].clip);
    }

    public static bool CheckPlayingSound(string name)
    {
        try {return audioSource[name].isPlaying;}
        catch {return false;}
    }

    public static void StopSound(string name)
    {
        audioSource[name].Stop();
    }

    public static void PlayBallHitSound()
    {
        PlaySound("BallHit", 0.8f, 1.0f, true);
    }

    public static void PlayBallShotSound()
    {
        int ballFX = rnd.Next(1, 2);
        PlaySound($"BallHitAfter{ballFX}", 1.2f, 1.5f, 1f);
    }

    public static void PlayShootSound(float fireRate, bool inverted)
    {
        if (!inverted) PlaySound("NormalShoot", 1.4f + 3*fireRate);
        else PlaySound("InvertedShoot", 1.2f + 6*fireRate);
    }

    private static float RandomValue(float min, float max)
    {
        if (min == max) return min; // Fixed pitch

        int decimalCount = 0;
        float negativeOffSet = 0f; negativeOffSet -= min % 1;

        while (min % 1 != 0 || max % 1 != 0) // While it has decimals
        {
            min *= 10; max*= 10;
            decimalCount++;
        }

        min += negativeOffSet * Mathf.Pow(10f, decimalCount); // OffSet to positive number to be compatible with System.Random
        max += negativeOffSet * Mathf.Pow(10f, decimalCount);

        return (rnd.Next((int)min, (int)max) / (float)decimalCount) - negativeOffSet;
    }
}
