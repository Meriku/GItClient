using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using MS.WindowsAPICodePack.Internal;
using System.Threading;

namespace GItClient.Core.Controllers
{
    internal class GitController
    {
        // TODO: add logger for all git commands
        internal string GetGitVersion()
        {
            using PowerShell powershell = PowerShell.Create();

            powershell.AddScript(@"git version");

            Collection<PSObject> results = powershell.Invoke();

            return ParseVersion(results[0]);  
        }

        internal bool InitRepository(string directory)
        {
            using PowerShell powershell = PowerShell.Create();

            powershell.AddScript($"cd {directory}");
            powershell.AddScript(@"git init");

            Collection<PSObject> results = powershell.Invoke();
            return results.Count > 0;
            
        }

        private string ParseVersion(PSObject psObject)
        {
            return psObject.ToString()[12..];
        }

    }
}
