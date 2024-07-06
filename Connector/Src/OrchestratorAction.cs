namespace LongreachAi.Connectors.UiPath;

public partial class Orchestrator
{
    public bool AssignMachine(long folderId, long machineId)
    {
        FolderAssignMachinesRequest assignReq = new()
        {
            Assignments = new MachineAssignmentsDto()
            {
                FolderIds = [folderId],
                MachineIds = [machineId]
            }
        };

        return GetApi().Folders_AssignMachinesAsync(assignReq).Result.IsSuccessful();
    }

    public bool AssignUser(long folderId, long userId, int roleId)
    {
        FolderAssignUsersRequest assignReq = new()
        {
            Assignments = new UserAssignmentsDto
            {
                RolesPerFolder = [new FolderRolesDto() { FolderId = folderId, RoleIds = [roleId] }],
                UserIds = [userId]
            }
        };

        return GetApi().Folders_AssignUsersAsync(assignReq).Result.IsSuccessful();
    }
    public JobDto? StartJob(long folderId, string processKey, string machineTemplate, string robotUserName, long? hostMachineId = null)
    {
        StartJobsRequest startJobsDto = new()
        {
            StartInfo = new StartProcessDto
            {
                ReleaseKey = processKey,
                Strategy = StartProcessDtoStrategy.ModernJobsCount,
                MachineRobots = [new MachineRobotDto() { MachineName = machineTemplate, RobotUserName = robotUserName }],
                MachineSessionIds = hostMachineId.HasValue ? [(long)hostMachineId] : null
            }
        };

        var result = GetApi().Jobs_StartJobsAsync(startJobsDto, "", "", "", "", null, folderId).Result;
        return result.IsSuccessful() ? result.Result.Value.First() : null;
    }


}