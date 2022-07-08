using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace EasyOC.Workflows.Scripting.Powershell
{
    public class PowerShellExecuter
    {
        public static Collection<PSObject> Excute(string commandText)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                PowerShell ps = PowerShell.Create();
                ps.Runspace = runspace;
                ps.AddScript(commandText);
                return ps.Invoke();
            }
        }
    }
}



