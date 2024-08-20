using GameSystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StageActivity : MonoBehaviour
{
    public static StageActivity Instance;
    public MapGenerator mapGenerator;
    public FollowCamera followCamera;
    private GameObject playerUnit;
    void Start()
    {
        Instance = this;
        StartCoroutine(StageStartProcess());       
    }

    private IEnumerator StageStartProcess()
    {
        //mapGenerator.GenerateMap();
        PlayerSpawnProcess();

        yield return 0;
    }

    private void PlayerSpawnProcess()
    {
        GameObject player = Resources.Load<GameObject>("Prefabs/Player/Player");
        playerUnit = PoolManager.instance.Spawn(player, Vector3.zero, Vector3.one, Quaternion.identity, true, transform);
        FollowCameraSetting();
    }

    protected void FollowCameraSetting()
    {
        followCamera.player = playerUnit.transform;

        followCamera.xMin = -mapGenerator.width / 2;
        followCamera.xMax = mapGenerator.width / 2;
        followCamera.yMin = -mapGenerator.height / 2;
        followCamera.yMax = mapGenerator.height / 2;
    }



}
