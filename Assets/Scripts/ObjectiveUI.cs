using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public TMP_Text objectiveText;

    void Update()
    {
        if (!MissionManager.Instance.radarDisabled)
        {
            objectiveText.text =
                "Disable Radar Dish";
        }
        else if (
            !MissionManager.Instance.intelDownloaded)
        {
            objectiveText.text =
                "Download Intel from VIP Building";
        }
        else
        {
            objectiveText.text =
                "Escape To Helipad";
        }
    }
}