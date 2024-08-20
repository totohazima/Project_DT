using UnityEngine;

public class CustomUpdateManager : MonoBehaviour
{
    public static BetterList<ICustomUpdateMono> customUpdateMonos = new BetterList<ICustomUpdateMono>();
    public int customUpdateMonoCount = 0;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < customUpdateMonos.size; i++)
        {
            customUpdateMonos.buffer[i].CustomUpdate();
        }
        customUpdateMonoCount = customUpdateMonos.size;

    }
}
