using UnityEngine;
using UnityEngine.UI;

public class GameControllsManager : MonoBehaviour
{

    public static bool toggleCrouch, toggleADS, toggleCanter;
    public static float mouseSense, audioVolume;

    [SerializeField] Slider audioSlider, mouseSlider;
    [SerializeField] Toggle adsT, crouchT, canterT;

    [System.Serializable] public enum Settings { toggleCrouch, toggleADS, toggleCanter, mouseSense, audioVolume };
    private void Start()
    {
        toggleCrouch = LoadBool("tCrouch");
        toggleADS = LoadBool("tADS");
        toggleCanter = LoadBool("tCanter");

        mouseSense = LoadFloat("MSense");
        audioVolume = LoadFloat("Volume");

        crouchT.isOn = LoadBool("tCrouch");
        adsT.isOn = LoadBool("tADS");
        canterT.isOn = LoadBool("tCanter");

       

        audioSlider.value = audioVolume;
        mouseSlider.value = mouseSense;

    }

    private void Update()
    {
        if (!DebugManager.DebugMode)
            return;
        if(Input.GetKeyDown(KeyCode.F12))
        {
            UICommunicator.refrence.PopupText("PLAYERPREFS CLEARED!", 4);
            PlayerPrefs.DeleteAll();
        }
    }

    bool LoadBool(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetInt(key) == 1;
        }
        else
        {
            PlayerPrefs.SetInt(key, 0);
            return false;
        }
    }

    float LoadFloat(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetFloat(key);
        }
        else
        {
            PlayerPrefs.SetFloat(key, 1f);
            return 1f;
        }
    }

    void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }




    public void ToggleADS()
    {
        toggleADS = !toggleADS;
        SetBool("tADS", toggleADS);
    }

    public void ToggleCrouch()
    {
        toggleCrouch = !toggleCrouch;
        SetBool("tCrouch", toggleCrouch);
    }

    public void ToggleCanter()
    {
        toggleCanter = !toggleCanter;
        SetBool("tCanter", toggleCanter);
    }

    public void AudioSlider()
    {
        audioVolume = audioSlider.value;
        PlayerPrefs.SetFloat("Volume", audioVolume);
    }

    public void SenseSlider()
    {
        mouseSense = mouseSlider.value;
        PlayerPrefs.SetFloat("MSense", mouseSense);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }


}
