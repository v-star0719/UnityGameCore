using Kernel.Core;
using Kernel.Unity;

public class UIPanelBase : PanelBaseCore
{
    private UIPanel[] panels;
    private int[] orgPanelDepths;

    public override int Depth
    {
        get => depth;
        set
        {
            if(panels == null)
            {
                LoggerX.Error("panels 尚未初始化");
                return;
            }

            for(var i = 0; i < panels.Length; i++)
            {
                panels[i].depth = orgPanelDepths[i] + value;
            }
        }
    }

    public override void Open(PanelManagerCore mgr, params object[] args)
    {
        panels = container.GetComponentsInChildren<UIPanel>(true);
        orgPanelDepths = new int[panels.Length];
        for (var i = 0; i < panels.Length; i++)
        {
            orgPanelDepths[i] = panels[i].depth;
        }
        base.Open(mgr, args);
    }

    public void OnBackBtnClick()
    {
        Close(false);
    }

    protected override void PlayOpenSound()
    {
        SoundManager.Instance.Play("Audio_PanelOpen");
    }

    protected override void PlayCloseSound()
    {
        SoundManager.Instance.Play("Audio_PanelClose");
    }
}
