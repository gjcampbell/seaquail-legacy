using SeaQuail.DiagramTool.Domain.Models;
using SeaQuail.DiagramTool.Domain.Services;
using SeaQuail.DiagramTool.Domain.Shared.Exceptions;

namespace SeaQuail.DiagramTool.Domain.Apis
{
    public class DiagramApi
    {
        private readonly DiagramService diagramService;
        private readonly CurrentUserService userService;

        public DiagramApi(
            DiagramService diagramService,
            CurrentUserService userService
            )
        {
            this.diagramService = diagramService;
            this.userService = userService;
        }

        public record SaveDiagramRequest(string Name, Guid? ID, string DiagramData);
        public record SaveDiagramResponse(Guid? ID);
        public record AddSnapshotRequest(string Snapshot);
        public record SnapshotResponse(string Name, Guid ID, DateTime ModifyDate);

        public async Task<Diagram> GetDiagram(Guid? id)
            => await diagramService.GetCurrentUserDiagram(id ?? Guid.Parse("00000000-0000-0000-0000-000000000001"));

        public async Task<IEnumerable<Diagram>> GetDiagrams()
            => await diagramService.GetCurrentUserDiagrams();

        public async Task<IEnumerable<Diagram>> GetSharedDiagrams()
            => await diagramService.GetSharedDiagrams();

        public async Task<User> GetCurrentUser()
            => await userService.GetUser();

        // todo: update return type
        public async Task<bool> TogglePublicAccess(Guid id)
            => (await diagramService.TogglePublicAccess(id))?.AllowPublicAccess == true;

        public async Task<Guid> AddSnapshot(AddSnapshotRequest snapshotReq, string name, Guid dgid)
            => (await diagramService.AddSnapshot(snapshotReq.Snapshot, name, dgid)).DiagramId;

        public async Task<DiagramSnapshot> LoadSnapshot(Guid id)
            => await diagramService.GetSnapshot(id);

        public async Task<IEnumerable<DiagramSnapshot>> GetSnapshots(Guid dgid)
            => await diagramService.GetSnapshots(dgid);

        public async Task<bool> DeleteSnapshot(Guid id)
            => await diagramService.DeleteSnapshot(id);

        public async Task<List<DiagramShare>> UnshareDiagram(string email, Guid diagramId)
            => await diagramService.UpdateSharing(diagramId, remove: [email]);

        public async Task<List<DiagramShare>> ShareDiagram(string email, Guid diagramId)
            => await diagramService.UpdateSharing(diagramId, add: [email]);

        public async Task<List<DiagramShare>> GetSharing(Guid diagramId)
            => await diagramService.GetSharing(diagramId);

        public async Task<SaveDiagramResponse> SaveDiagram(SaveDiagramRequest saveDiagramReq)
            => new ((await diagramService.SaveState(
                saveDiagramReq?.Name ?? "Unnamed", 
                saveDiagramReq?.ID ?? Guid.Empty, 
                saveDiagramReq?.DiagramData ?? throw ServiceException.InvalidRequest("Diagram data was null?")
                )).Id);
    }
}
