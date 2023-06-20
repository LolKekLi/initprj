using Project;
using UnityEngine;

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    [SerializeField]
    private float _offset;
    
    private void Update()
    {
        if (!Application.isPlaying)
        {
            var inGameUITests = GetComponentsInChildren<InGameUITest>();

            for (var i = 0; i < inGameUITests.Length; i++)
            {
                inGameUITests[i].transform.position =
                    inGameUITests[i].transform.position.ChangeX(transform.position.x + i * _offset);
            }
        }
    }
}