using UnityEngine;

public class DebugWordSpawner : MonoBehaviour
{
    [SerializeField] WordData[] debugWords; // Inspector에서 테스트용 단어들 등록

    private void Update()
    {
        for (int i = 0; i < debugWords.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                DeckManager.Instance.AddWord(debugWords[i]);
                Debug.Log($"[디버그] {debugWords[i].word} 추가됨");
            }
        }
    }
}