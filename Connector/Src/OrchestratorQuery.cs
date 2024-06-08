namespace LongreachAi.Connectors.UiPath
{
    public partial class Orchestrator
    {
        public IEnumerable<FolderDto>? GetFolders(string folderName = "")
        {
            var f = folderName != "" ? Folders?.Where(f => f.FullyQualifiedName.IsMatchOf(folderName)) : null;
            if (f != null)
                return f;

            Folders = GetApi().Folders_GetAsync("", "", "", "", 1000, 0, false).Result.Result.Value;
            return Folders?.Where(f => folderName == "" || f.FullyQualifiedName.IsMatchOf(folderName));
        }

        public IEnumerable<MachineDto>? GetMachineTemplates(string machineTemplate = "")
        {
            var m = machineTemplate != "" ? MachineTemplates?.Where(m => m.Name.IsMatchOf(machineTemplate)) : null;
            if (m != null)
                return m;

            MachineTemplates = GetApi().Machines_GetAsync("", "", "", "", 1000, 0, false).Result.Result.Value;
            return MachineTemplates?.Where(m => m.Type == MachineDtoType.Template
                && (machineTemplate == "" || m.Name.IsMatchOf(machineTemplate)));
        }

        public IEnumerable<UserDto>? GetUsers(string userName = "")
        {
            var u = userName != "" ? Users?.Where(u => u.UserName.IsMatchOf(userName)) : null;
            if (u != null)
                return u;

            Users = GetApi().Users_GetAsync("", "", "", "", 1000, 0, false).Result.Result.Value;
            return Users?.Where(u => userName == "" || u.UserName.IsMatchOf(userName));
        }

        public IEnumerable<ProcessDto>? GetPackages(string packageName = "")
        {
            var p = packageName != "" ? Packages?.Where(p => p.Id.IsMatchOf(packageName)) : null;
            if (p == null)
            {
                Packages = GetApi().Processes_GetAsync("", null, "", "", "", "", 1000, 0, false).Result.Result.Value;
            }
            //if no package name is given, just return all top level package versions
            if (packageName == "") return Packages;
            //else fetch the package versions of the specific package
            var process = Packages!.Where(p => p.Id.IsMatchOf(packageName));
            return process == null ? null :
                GetApi().Processes_GetProcessVersionsByProcessidAsync(process.First().Id, null, "", "", "", "", 1000, 0, false).Result.Result.Value;
        }

        public IEnumerable<ReleaseDto>? GetProcesses(string folderName, string processName = "")
        {
            if (folderName == "") return null;
            var folderId = GetFolders(folderName)?.First().Id;
            var processes = GetApi().Releases_GetAsync([""], [""], "", "", "", "", 1000, 0, false, folderId).Result.Result.Value;
            return processName == "" ? processes : processes.Where(p => p.Name.IsMatchOf(processName));
        }

        public IEnumerable<ProcessScheduleDto>? GetTriggers(string folderName)
        {
            if (folderName == "") return null;
            var folderId = GetFolders(folderName)?.First().Id;
            return GetApi().ProcessSchedules_GetAsync("", "", "", "", 1000, 0, false, folderId).Result.Result.Value;
        }

        public IEnumerable<JobDto>? GetJobs(string folderName)
        {
            if (folderName == "") return null;
            var folderId = GetFolders(folderName)?.First().Id;
            return GetApi().Jobs_GetAsync([""], [""], "", "", "", "", 1000, 0, false, folderId).Result.Result.Value;
        }

        public IEnumerable<RoleDto>? GetRoles(string roleName = "")
        {
            var r = roleName != "" ? Roles?.Where(r => r.Name.IsMatchOf(roleName)) : null;
            if (r != null)
                return r;

            Roles = GetApi().Roles_GetAsync("", "", "", "", 1000, 0, false).Result.Result.Value;
            return Roles?.Where(r => roleName == "" || r.Name.IsMatchOf(roleName));
        }

        public IEnumerable<FolderAssignmentsDto>? GetFolderRolesForUser(string userName)
        {
            return GetApi().Folders_GetAllRolesForUserByUsernameAndSkipAndTakeAsync(userName, 0, 100, null, "", "", "").Result.Result.PageItems;
        }

        public IEnumerable<RoleUsersDto>? GetTenantRolesForUser(string userName)
        {
            return GetApi().Folders_GetAllRolesForUserByUsernameAndSkipAndTakeAsync(userName, 0, 100, null, "", "", "").Result.Result.TenantRoles;
        }

        public IEnumerable<MachineDto>? GetMachineGroupsinFolder(string folderName)
        {
            if (folderName == "") return null;

            FolderDto? folder = GetFolders(folderName)?.First();
            if (folder == null) return null;
            var folderId = (long)folder.Id!;
            var taskResult = GetApi().Machines_GetAssignedMachinesByFolderidAsync(folderId, null, "", "", "", "", 100, 0, false).Result;
            return taskResult.Result.Value.Where(m => m.Type == MachineDtoType.Template);
        }

        public IEnumerable<MachineSessionDto>? GetMachines(string MachineGroup)
        {
            if (MachineGroup == "") return null;
            long? machineId = GetMachineTemplates(MachineGroup)?.First().Id;
            if (machineId == null) return null;
            return GetApi().Sessions_GetMachineSessionsByKeyAsync((long)machineId, "", "", "", "", null, null, null).Result.Result.Value;
        }

        public IEnumerable<MachineSessionDto>? GetMachinesinFolder(string folderName)
        {
            var machinegroups = GetMachineGroupsinFolder(folderName);
            if (machinegroups == null) return null;
            IEnumerable<MachineSessionDto> machines = [];

            foreach (var machinegroup in machinegroups)
            {
                var taskResult = GetApi().Sessions_GetMachineSessionsByKeyAsync((long)machinegroup.Id!, "", "", "", "", null, null, null).Result.Result.Value;
                if (taskResult.Any()) machines = machines.Concat(taskResult);
            }
            return machines;
        }

        ///<summary>
        ///Gets the execution logs within a folder. Filter - If JobKey is given, processName will be ignored.
        ///</summary>
        public IEnumerable<LogDto>? GetLogs(long folderId, string jobKey = "", string processName = "")
        {
            var filter = jobKey != "" ? "JobKey=" + jobKey : processName != "" ? "ProcessName=" + processName : "";

            return GetApi().RobotLogs_GetAsync("", filter, "", "TimeStamp desc", 50, 0, false, folderId).Result.Result.Value;
        }
    }
}
