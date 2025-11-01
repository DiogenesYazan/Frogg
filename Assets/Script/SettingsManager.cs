using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    
    [Header("Graphics")]
    public Dropdown qualityDropdown;
    
    [Header("Other")]
    public Toggle cursorLockToggle;
    
    void Start()
    {
        LoadSettings();
        
        // Conecta os listeners diretamente no código
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(SetQuality);
    }
    
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    // AUDIO METHODS
    public void SetMasterVolume(float volume)
    {
        if (volume <= 0.01f)
        {
            audioMixer.SetFloat("MasterVolume", -80f);
        }
        else
        {
            float dbValue = Mathf.Log10(volume) * 20;
            audioMixer.SetFloat("MasterVolume", dbValue);
        }
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        if (volume <= 0.01f)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
        }
        else
        {
            float dbValue = Mathf.Log10(volume) * 20;
            audioMixer.SetFloat("MusicVolume", dbValue);
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        if (volume <= 0.01f)
        {
            audioMixer.SetFloat("SFXVolume", -80f);
        }
        else
        {
            float dbValue = Mathf.Log10(volume) * 20;
            audioMixer.SetFloat("SFXVolume", dbValue);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    
    // GRAPHICS METHODS
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }
    
    // OTHER METHODS
    public void SetCursorLock(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        PlayerPrefs.SetInt("CursorLock", lockCursor ? 1 : 0);
    }
    
    // LOAD SETTINGS
    void LoadSettings()
    {
        // Audio
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;
            
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        
        // Graphics - Pega a qualidade atual ou a salva
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        if (qualityDropdown != null)
            qualityDropdown.value = qualityLevel;
        QualitySettings.SetQualityLevel(qualityLevel);
        
        // Cursor
        bool cursorLock = PlayerPrefs.GetInt("CursorLock", 1) == 1;
        if (cursorLockToggle != null)
            cursorLockToggle.isOn = cursorLock;
    }
    
    void OnDisable()
    {
        bool cursorLock = PlayerPrefs.GetInt("CursorLock", 1) == 1;
        SetCursorLock(cursorLock);
    }
    
    void OnDestroy()
    {
        // Remove os listeners para evitar memory leaks
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.RemoveListener(SetQuality);
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}
