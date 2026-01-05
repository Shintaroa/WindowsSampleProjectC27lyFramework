using Cysharp.Threading.Tasks;
using TFramework;

public class Main : MonoSingleton<Main>
{
    //public string configPath = "";
    
    private void Start()
    {
        Init();
    }
    
    protected virtual async UniTask Init()
    {
        UIController.Instance.Init();
        await SystemConfig.Instance.Init();
        DisplayController.Instance.Init();
        LogFile.Instance.Init();
    }
}
