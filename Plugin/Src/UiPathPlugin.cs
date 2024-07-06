namespace LongreachAi.Semantic.Plugins;

using Microsoft.Extensions.Configuration;
using LongreachAi.Connectors.UiPath;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Namotion.Reflection;

public class UiPathPlugin(IOptions<UiPathOptions> options)
{
        readonly Orchestrator orch = new(options.Value);

        [KernelFunction("get_All_Folders_in_Tenant")]
        [Description(@"Purpose: Gets All folders at Tenant level. Supports max 50 folders.
        Return: List of folders and their sub folders at Tenant level.")]
        public List<string> GetAllFolders()
        {
                var folders = orch.GetFolders()?.Select(f => $@"name:{f.FullyQualifiedName}, displayName: {f.DisplayName},
                        description: {f.Description}, id: {f.Id}, key: {f.Key}, parentFolderId: {f.ParentId}").Take(50).ToList();
                return folders ?? ["Error: No Folders found in Tenant"];
        }

        [KernelFunction("get_All_Roles_in_Tenant")]
        [Description(@"Purpose: Gets all Roles at Tenant level.
        Return:  List of Roles at Tenant level.")]
        public List<string> GetAllRoles()
        {
                var roles = orch.GetRoles()?.Select(r => $@"name: {r.Name}, id: {r.Id}, displayName: {r.DisplayName}").ToList();
                return roles ?? ["Error: No Roles found in Tenant"];
        }

        [KernelFunction("get_All_Users_in_Tenant")]
        [Description(@"Purpose: Gets all users at Tenant Level. 
        Return: List of Users at Tenant level.")]
        public List<string> GetAllUsers()
        {
                var users = orch.GetUsers()?.Select(u => $@"name: {u.UserName}, fullName: {u.FullName}, id:{u.Id},
                        email: {u.IsEmailConfirmed}, isActive: {u.IsActive}").ToList();
                return users ?? ["Error: No Users found in Tenant"];
        }

        [KernelFunction("get_All_Machine_Groups_in_Tenant")]
        [Description(@"Purpose: Gets all Machine Groups at Tenant Level.
         Return: List of Machine Groups at Tenant Level.")]
        public List<string> GetAllMachineGroups()
        {
                var machines = orch.GetMachineGroups()?.Select(m => $@"name: {m.Name}, id: {m.Id}, key: {m.Key},
                        description: {m.Description}, type: {m.Type}").ToList();
                return machines ?? ["Error: No Machine Groups found in Tenant"];
        }

        [KernelFunction("get_Specific_Folder_Details")]
        [Description(@"Purpose: Given a specific folder name, gets the folder details.
         Return: JSON string containing folder info. Error message is returned if the folder name is not found.")]
        public string GetFolderDetails([Description(@"Folder name")] string folderName)
        {
                var folder = orch.GetFolders(folderName)?.ToJson();
                return folder ?? "Error: Folder not found in Tenant";
        }

        [KernelFunction("get_Specific_Role_Details")]
        [Description(@"Purpose: Given a specific Role name, gets the Role details.
         Return: JSON string containing Role info. Error message is returned if the Role name is not found.")]
        public string GetRoleDetails([Description(@"Role name")] string roleName)
        {
                var role = orch.GetRoles(roleName)?.ToJson();
                return role ?? "Error: Role not found in Tenant";
        }

        [KernelFunction("get_Specific_User_Details")]
        [Description(@"Purpose: Given a specific User name, gets the User details.
         Return: JSON string containing User info. Error message is returned if the User name is not found.")]
        public string GetUserDetails([Description(@"User name")] string userName)
        {
                var user = orch.GetUsers(userName)?.ToJson();
                return user ?? "Error: User not found in Tenant";
        }

        [KernelFunction("get_Specific_Machine_Group_Details")]
        [Description(@"Purpose: Given a specific Machine Group name, gets the Machine Group details.
         Return: JSON string containing Machine Group info. Error message is returned if the Machine Group name is not found.")]
        public string GetMachineGroupDetails([Description(@"Machine Group name")] string machineGroupName)
        {
                var machine = orch.GetMachineGroups(machineGroupName)?.ToJson();
                return machine ?? "Error: Machine Group not found in Tenant";
        }

        [KernelFunction("get_Machine_Groups_in_Folder")]
        [Description(@"Purpose: Given a specific folder name, gets the machine groups allocated to the folder.
         Return: List of Machine Groups in specified folder. Error message is returned if the folder name is not found.")]
        public List<string> GetMachineGroupsinFolder([Description(@"Folder name")] string folderName)
        {
                var machines = orch.GetMachineGroupsinFolder(folderName)?.Select(m => $@"name: {m.Name}, id: {m.Id},
                description: {m.Description}, key: {m.Key}").ToList();
                return machines ?? ["Error: No Machine Groups found in folder"];
        }

        [KernelFunction("get_HostMachines_in_Machine_Group")]
        [Description(@"Purpose: Given a specific Machine Group Name, gets the Machines contained in the machine group.
         Return: Json object containing details of machines in specified Machine Group. Error message is returned if the Machine Group Name is not found.")]
        public string GetHostMachines([Description(@"Machine Group name")] string machineGroupName)
        {
                var machines = orch.GetMachines(machineGroupName)?.ToJson();
                return machines ?? "Error: No Host Machines found in Machine Group";
        }

        [KernelFunction("get_All_Packages_in_Tenant")]
        [Description(@"Purpose: Gets all packages at Tenant Level.
         Return: List of packages at Tenant Level.")]
        public List<string> GetAllPackages()
        {
                var packages = orch.GetPackages()?.Select(p => $@"name: {p.Title}, version: {p.Version}, isLatestVersion: {p.IsLatestVersion}").ToList();
                return packages ?? ["Error: No Packages found in Tenant"];
        }

        [KernelFunction("get_Specific_Package_Details")]
        [Description(@"Purpose: Given a specific Package name, gets the package details.
         Return: JSON object containing package info.")]
        public string GetPackages([Description(@"Package name")] string packageName)
        {
                var packages = orch.GetPackages(packageName)?.ToJson();
                return packages ?? "Error: Package not found in Tenant";
        }

        [KernelFunction("get_All_Processes_in_Folder")]
        [Description(@"Purpose: Gets all Processes in specified folder.
        Return: List of all Processes in specified folder. Error if no processes found.")]
        public List<string> GetProcesses([Description(@"Folder name")] string folderName)
        {
                var processes = orch.GetProcesses(folderName)?.Select(p => $@"name: {p.Name}, id: {p.Id}, key: {p.Key}, 
                        description: {p.Description}, version: {p.ProcessVersion}, isAttendedAutomation: {p.IsAttended}").ToList();
                return processes ?? ["Error: No Processes found in folder"];
        }

        [KernelFunction("get_Specific_Process_Details")]
        [Description(@"Purpose: Given a specific Folder name and a Process name, gets the Process details.
         Return: JSON object containing Process info.")]
        public string GetProcesses([Description(@"Folder name")] string folderName, [Description(@"Process name")] string processName)
        {
                var processes = orch.GetProcesses(folderName, processName)?.ToJson();
                return processes ?? "Error: Process not found in folder";
        }

        [KernelFunction("get_All_Trigger_Details_in_Folder")]
        [Description(@"Purpose: Gets trigger details, Return: JSON string containing trigger info.")]
        public string GetTriggers([Description(@"Folder name")] string folderName)
        {
                var triggers = orch.GetTriggers(folderName)?.ToJson();
                return triggers ?? "";
        }

        [KernelFunction("get_All_Jobs_in_Folder")]
        [Description(@"Purpose: Gets list of top 50 jobs in Folder. Return: JSON string containing job info.")]
        public List<string> GetJobs([Description(@"Folder Name")] string folderName)
        {
                var jobs = orch.GetJobs(folderName)?.Select(j =>($@"id: {j.Id}, key: {j.Key}, processName: {j.ReleaseName}, 
                        machineGroup: {j.Machine}, hostMachine: {j.HostMachineName}, startTime: {j.StartTime}, 
                        endTime: {j.EndTime}, jobState: {j.State}, jobSource: {j.Source}, jobSourceType: {j.SourceType}", j.StartTime))
                        .OrderByDescending(j => j.StartTime).Take(50).Select(j=>j.Item1).ToList();
                return jobs ?? ["Error: No Jobs found in folder"];
        }


        [KernelFunction("get_Logs_for_Process_in_Folder")]
        [Description(@"Purpose: Gets execution logs for a specified Process in a specified folder.
        Return: List of top 50 execution logs. Error if no logs found.")]
        public List<string> GetLogs([Description(@"Folder Name")] string folderName, [Description(@"Process Name")] string processName)
        {
                var folderId = orch.GetFolders(folderName)?.FirstOrDefault()?.Id;
                if (folderId == null) return ["Error: Folder not found in Tenant"];
                var logs = orch.GetLogs(folderId: (long)folderId!, processName: processName)?.Select(l => $@"logLevel: {l.Level}, 
                        processName: {l.ProcessName}, machineName: {l.HostMachineName}, windowsIdentity: {l.WindowsIdentity}, 
                        logMessage: {l.RawMessage}").ToList();

                return logs ?? ["Error: No Logs found in folder"];
        }
        
}
