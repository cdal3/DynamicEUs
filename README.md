# Opening the Project
The following sample project can be opened with or without FacotoryTalk Optix Studio Pro
## With FactoryTalk Optix Studio Pro
1. Select the green **Code** dropdown
2. Copy the repository URL to the clipboard
3. Open FactoryTalk Optix Studio and sign in to obtain the FactoryTalk Optix Studio Pro Entitlement
4. Select **Open** > **Remote**, paste the repository URL in the **Remote URL** field, set the **Location**, and Select **Open**
## Without FactoryTalk Optix Studio Pro
1. Select the green **Code** dropdown
2. Select **Download ZIP**
3. Unzip the downloaded Repository to the desired location
4. Open the **.optix** file

# Introduction
The following sample project demonstrates how to change an analog variable's engineering units at runtime.

>[!Note]
>Reboot of the application is required after a change at runtime to reset the localization properties

## SetpointObject
A *SetpointObject* was created in the Model folder that contains a custom behavior with the logic below. Upon startup, the EUID is set for the analog variable. EUID has an on change event that calls PromptForReboot() when changed.
```csharp
#region Using directives
using System;
using System.Collections.Generic;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using System.ComponentModel;
using FTOptix.OPCUAServer;
#endregion

[CustomBehavior]
public class SepointObjectBehavior : BaseNetBehavior
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined behavior is started
        var euid = Node.GetVariable("EUID").Value;
        UpdateEUInfo(euid);
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined behavior is stopped
    }

   [ExportMethod]
    public void UpdateEUInfo(uint eUID)
    {
        var targetVariable = Node.GetVariable("SP").NodeId;
        // Check if the target variable is a valid IUAVariable
        if (Node.GetVariable("SP") is IUAVariable variableToAnalyze)
        {
            // Check if the variable already has an EngineeringUnits property
            // If it does, update the UnitId value
            var euInfo = variableToAnalyze.Get<EUInformation>("EngineeringUnits");
            if (euInfo != null)
            {
                euInfo.ModellingRule = NamingRuleType.Optional;
                euInfo.SetModellingRuleRecursive();
                var unitId = euInfo.GetVariable("UnitId");
                unitId.Value = eUID;
                // Ensure the ModellingRule is set to Mandatory
                if (euInfo.ModellingRule != NamingRuleType.Mandatory)
                {
                    euInfo.ModellingRule = NamingRuleType.Mandatory;
                    euInfo.SetModellingRuleRecursive();
                }
            }
            // If it doesn't, create a new EngineeringUnits property and set the UnitId value
            else
            {
                euInfo = InformationModel.MakeVariable<EUInformation>("EngineeringUnits", OpcUa.DataTypes.EUInformation);
                euInfo.QualifiedBrowseName = new QualifiedName(0, "EngineeringUnits");
                euInfo.GetOrCreateVariable("UnitId").Value = eUID;
                euInfo.ModellingRule = NamingRuleType.Mandatory;
                euInfo.SetModellingRuleRecursive();
                variableToAnalyze.Refs.AddReference(OpcUa.ReferenceTypes.HasProperty, euInfo);
            }
            // Check if this is a custom engineering unit of 'None' and if so, add the NamespaceIndex variable
            if (eUID == 999)
            {
                euInfo.GetOrCreateVariable("NamespaceIndex").Value = targetVariable.NamespaceIndex;
            }
            Log.Info($"Updated Engineering Units for variable '{variableToAnalyze.BrowseName}' with EUID: {eUID}");
        }
    }

    [ExportMethod]
    public void PromptForReboot()
    {
        DialogType dialogBox = (DialogType)Project.Current.Get($"UI/Dialogs/DialogEUChanged");  // depends on path to Screens folder
    
        var npe = Project.Current.Get("UI/NativePresentationEngine");   // depends on UI and NativePresentationEngine paths
        var npeSessions = npe.Get("Sessions");      // npe has a single session
        var npeSession = npeSessions.Children[0];   // the single npe session is at [0]
        var npeWindow = npeSession.Get("UIRoot");   // "UIRoot" is the browsename of the main window
        UICommands.OpenDialog(npeWindow, dialogBox);
    
        var wpe = Project.Current.Get("UI/WebPresentationEngine");   // depends on UI and WebPresentationEngine paths
        var wpeSessions = wpe.Get("Sessions");      // wpe could have multiple sessions
        foreach (var wpeSession in wpeSessions.Children)
        {
            var wpeWindow = wpeSession.Get("UIRoot");   // "UIRoot" is the browsename of the main window
            if (wpeWindow != null)
            {
                UICommands.OpenDialog(wpeWindow, dialogBox);
            }
        }
    }

    

#region Auto-generated code, do not edit!
    protected new SepointObject Node => (SepointObject)base.Node;
#endregion
}
```
