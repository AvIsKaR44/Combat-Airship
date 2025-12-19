using UnityEngine;
using UnityEngine.Rendering;


[CreateAssetMenu]
public class GraphicsQualitySetting : Setting
{
    [SerializeField] private RenderPipelineAsset[] qualityPipelineAssets;
 
    private int currentLevelIndex = 0;
    public override bool isMinValue { get => currentLevelIndex == 0; }
    public override bool isMaxValue { get =>  currentLevelIndex == QualitySettings.names.Length - 1; }

    public override void SetNextValue()
    {
        if (!isMaxValue) currentLevelIndex++;        
    }

    public override void SetPreviousValue()
    {
        if (!isMinValue) currentLevelIndex--;      
    }

    public override object GetValue()
    {
        return QualitySettings.names[currentLevelIndex];
    }

    public override string GetStringValue()
    {
        return QualitySettings.names[currentLevelIndex];
    }

    public override void Apply()
    {
        QualitySettings.SetQualityLevel(currentLevelIndex, true);

        if (qualityPipelineAssets != null && currentLevelIndex < qualityPipelineAssets.Length)
        {
            QualitySettings.renderPipeline = qualityPipelineAssets[currentLevelIndex];
        }
        else
        {
            Debug.LogWarning("URP Assets not assigned for quality levels!");
        }

        Save();
    }

    public override void Load()
    {
        currentLevelIndex = PlayerPrefs.GetInt(title, QualitySettings.names.Length - 1);
    }

    private void Save()
    {
        PlayerPrefs.SetInt(title, currentLevelIndex);
    }
}
