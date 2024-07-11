namespace LongreachAi.Semantic.Plugins;

using Microsoft.Extensions.Configuration;
using LongreachAi.Connectors.UiPath;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Namotion.Reflection;
using System.Security.Cryptography;
using System.Reflection.Metadata.Ecma335;

[Description(@"Enables connection to UiPath Orchestrator 
and provides access to various functionalities. 
Supports the following queries on Orchestrator: Folders, Users, Roles, Machine Groups, Packages, Processes, Jobs, Triggers. 
Support the following actions on Orchestrator: Assign User to Folder, Assign Machine Group to Folder, Execute Process. 
Supports Modern Folders. Does not support Classic Folders. 
Supports Machine Templates. Does not support Standard Machines.")]
public class UiPathPlugin(IOptions<UiPathOptions> options)
{
        readonly Orchestrator orch = new(options.Value);

        [KernelFunction("get_All_Folders_in_Tenant")]
        [Description(@"Purpose: Gets All folders at Tenant level. Supports max 50 folders.
        Return: List of folders and their sub folders at Tenant level.")]
        public List<string> GetAllFolders()
        {
                IEnumerable<FolderDto>? folders = orch.GetFolders();

                foreach (var folder in folders!)
                {
                        var subfolders = folders.Where(f => f.ParentId == folder.Id).Select(f => f.FullyQualifiedName).ToJson();
                        folder.Description = folder.Description + " Subfolders: " + subfolders;
                }

                return folders?.Select(f => $@"name:{f.FullyQualifiedName}, displayName: {f.DisplayName},
                        description: {f.Description}, id: {f.Id}, parentFolderId: {f.ParentId}").Take(50).ToList()
                        ?? ["Error: No Folders found in Tenant"];
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
                var machines = orch.GetMachineGroups()?.Select(m => $@"name: {m.Name}, id: {m.Id},
                        description: {m.Description}, type: {m.Type}").ToList();
                return machines ?? ["Error: No Machine Groups found in Tenant"];
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

        [KernelFunction("get_users_in_Folder")]
        [Description(@"Purpose: Given a specific folder name, gets the users allocated to the folder.
         Return: List of Users in specified folder. Error message is returned if the folder name is not found.")]
        public List<string> GetUsersinFolder([Description(@"Folder name")] string folderName)
        {
                var users = orch.GetUsersinFolder(folderName)?.Select(u => $@" id: {u.UserEntity.Id}, name: {u.UserEntity.UserName}, 
                        fullName: {u.UserEntity.FullName}, userRoles: {u.Roles.ToJson()}").ToList();
                return users ?? ["Error: No Users found in folder"];
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
                description: {m.Description}").ToList();
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
                var processes = orch.GetProcesses(folderName)?.Select(p => $@"name: {p.Name}, id: {p.Id}, releaseKey: {p.Key}, 
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
        [Description(@"Purpose: Gets trigger details.
        Return: List containing trigger details.")]
        public List<string> GetTriggers([Description(@"Folder name")] string folderName)
        {
                var triggers = orch.GetTriggers(folderName)?.Select(t => $@"name: {t.Name}, id: {t.Id}, 
                        description: {t.Description}, processName: {t.ReleaseName}, scheduleCron: {t.StartProcessCron}, 
                        scheduleTimeZone: {t.TimeZoneIana}, queueName: {t.QueueDefinitionName}, enabled: {t.Enabled},
                        priority: {t.SpecificPriorityValue}, ${t.MachineRobots.Select(m => "machineGroup:" + m.MachineName +
                        ", hostMachineName:" + m.SessionName + ", robotName:" + m.RobotUserName).ToList().ToJson()}").ToList();
                return triggers ?? ["Error: No Triggers found in folder"];
        }

        [KernelFunction("get_All_Jobs_in_Folder")]
        [Description(@"Purpose: Gets list of top 50 jobs in Folder. Return: JSON string containing essential job info.")]
        public List<string> GetAllJobsinFolder([Description(@"Folder Name")] string folderName)
        {
                var jobs = orch.GetJobs(folderName)?.Select(j => ($@"id: {j.Id}, processName: {j.ReleaseName}, 
                        machineGroup: {j.Machine}, hostMachine: {j.HostMachineName}, jobStartTime: {j.StartTime}, 
                        jobEndTime: {j.EndTime}, jobState: {j.State}", j.StartTime))
                        .OrderByDescending(j => j.StartTime).Take(50).Select(j => j.Item1).ToList();
                return jobs ?? ["Error: No Jobs found in folder"];
        }

        [KernelFunction("get_Specific_Job_Details")]
        [Description(@"Purpose: Given a specific Job Id in a folder, gets the full Job details.
         Return: JSON object containing Job info.")]
        public string GetJobDetails([Description(@"Folder name")] string folderName, [Description(@"Job Id")] string jobId)
        {
                var job = orch.GetJobs(folderName)?.Where(j => j.Id.ToString() == jobId).ToJson();
                return job ?? "Error: Job not found in Tenant";
        }

        [KernelFunction("get_robots_in_Folder")]
        [Description(@"Purpose: Given a specific folder name, gets the robots allocated to the folder.
         Return: List of Robots in specified folder. Error message is returned if the folder name is not found.")]
        public List<string> GetRobotsinFolder([Description(@"Folder name")] string folderName)
        {
                var robots = orch.GetRobotsUsersinFolder(folderName)?.Select(r => $@" robotId: {r.UnattendedRobot.RobotId}, 
                        name: {r.RobotProvision.UserName}").ToList();
                        
                return robots ?? ["Error: No Robots found in folder"];
        }

        [KernelFunction("execute_Process_in_Folder")]
        [Description(@"Purpose: Given a specific Folder Id, Process Id, Machine Group Id, robot Id,
        and optional Host Machine Id, executes the Process. The Process, Machine Group, and Robot must be available in the same folder.
        The Host Machine must belong to the Machine Group.")]
        public string ExecuteProcess([Description(@"Folder Id")] string folderId, [Description(@"Release Key of Process")] string releaseKey,
                [Description(@"Machine Group Id")] string machineGroupId, [Description(@"User Id")] string robotUserId,
                [Description(@"Host Machine Id")] long? hostMachineId)
        {
                if (!long.TryParse(folderId, out long lfolderId)) throw new ArgumentException("Invalid Folder Id");
                if (!long.TryParse(machineGroupId, out long lmachineGroupId)) throw new ArgumentException("Invalid Machine Group Id");
                if (!long.TryParse(robotUserId, out long lrobotUserId)) throw new ArgumentException("Invalid User Id");

                var job = orch.StartJob(lfolderId, releaseKey, lmachineGroupId, lrobotUserId, hostMachineId);
                return job?.ToJson() ?? "Error: Process execution failed";
        }



}
