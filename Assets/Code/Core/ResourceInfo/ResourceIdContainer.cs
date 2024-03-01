
using ResourceInfo.Code.Core.ResourceInfo.ProjectResources;

namespace ResourceInfo.Code.Core.ResourceInfo
{
public static class ResourceIdContainer
{
    private const string ModuleResourcesGroupId = "ModulesResources";
    private const string ProjectResourcesGroupId = "ProjectResources";

    public static ModulesResourceContainer ModulesResourceContainer { get; }
    public static ProjectResourceContainer ProjectResourceContainer { get; }

    static ResourceIdContainer()
    {
        ModulesResourceContainer = new ModulesResourceContainer(ModuleResourcesGroupId);
        ProjectResourceContainer = new ProjectResourceContainer(ProjectResourcesGroupId);
    }
}
}