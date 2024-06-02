namespace LongreachAi.Connectors.UiPath;

public class OrchestratorAction(Orchestrator orchestrator)
{
    readonly OrchestratorAPI _Api = orchestrator.Api;
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

        return _Api.Folders_AssignMachinesAsync(assignReq).Result.IsSuccessful();
    }

    public bool AssignRobot(long folderId, long userId, int roleId)
    {
        FolderAssignUsersRequest assignReq = new()
        {
            Assignments = new UserAssignmentsDto
            {
                RolesPerFolder = [new FolderRolesDto() { FolderId = folderId, RoleIds = [roleId] }],
                UserIds = [userId]
            }
        };

        return _Api.Folders_AssignUsersAsync(assignReq).Result.IsSuccessful();
    }
    public JobDto? StartJob(long folderId, string processKey, string machineTemplate, string robotUserName, long? machineId = null)
    {
        StartJobsRequest startJobsDto = new()
        {
            StartInfo = new StartProcessDto
            {
                ReleaseKey = processKey,
                Strategy = StartProcessDtoStrategy.ModernJobsCount,
                MachineRobots = [new MachineRobotDto() { MachineName = machineTemplate, RobotUserName = robotUserName }],
                MachineSessionIds = machineId.HasValue ? [(long)machineId] : null
            }
        };

        var result = _Api.Jobs_StartJobsAsync(startJobsDto, "", "", "", "", null, folderId).Result;
        return result.IsSuccessful() ? result.Result.Value.First() : null;
    }
}