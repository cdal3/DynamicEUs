#region Using directives
using System;
using System.Linq;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.Retentivity;
using FTOptix.OPCUAServer;
using FTOptix.CoreBase;
using FTOptix.Core;
#endregion

public class DialogLogic : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void CloseAllDialogs()
    {
        // DialogBoxes are children of the current Session, so we can iterate in
        // children to get all of them
        foreach (Dialog item in Session.Get("UIRoot").Children.OfType<DialogEUChanged>().ToList()) {
            item.Close();
        }
    }
}
