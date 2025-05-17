using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SeaQuailDiagramTool.Domain.Models;
using SeaQuailDiagramTool.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeaQuailDiagramTool.Application
{
    public class DiagramController : Controller
    {
        private readonly DiagramService diagramService;
        private readonly DiagramSerializer serializer;
        private readonly CurrentUserService userService;

        public DiagramController(
            DiagramService diagramService,
            DiagramSerializer serializer,
            CurrentUserService userService
            )
        {
            this.diagramService = diagramService;
            this.serializer = serializer;
            this.userService = userService;
        }

        public record SaveDiagramRequest(string Name, Guid? ID);
        public record SnapshotResult(string Name, Guid ID, DateTime ModifyDate);

        public async Task<Diagram> GetDiagram(Guid? id)
            => await diagramService.GetCurrentUserDiagram(id ?? Guid.Parse("00000000-0000-0000-0000-000000000001"));

        public async Task<IEnumerable<Diagram>> GetDiagrams()
            => await diagramService.GetCurrentUserDiagrams();

        public async Task<IEnumerable<Diagram>> GetSharedDiagrams()
            => await diagramService.GetSharedDiagrams();

        public async Task<User> GetCurrentUser()
            => await userService.GetUser();

        public async Task<bool> TogglePublicAccess(Guid id)
            => (await diagramService.TogglePublicAccess(id))?.AllowPublicAccess == true;

        [HttpPost]
        public async Task<Guid> AddSnapshot(string snapshot, string name, Guid dgid)
            => (await diagramService.AddSnapshot(snapshot, name, dgid)).DiagramId;

        public async Task<DiagramSnapshot> LoadSnapshot(Guid id)
            => await diagramService.GetSnapshot(id);

        public async Task<IEnumerable<DiagramSnapshot>> GetSnapshots(Guid dgid)
            => await diagramService.GetSnapshots(dgid);

        public async Task<bool> DeleteSnapshot(Guid id)
            => await diagramService.DeleteSnapshot(id);

        public async Task<List<DiagramShare>> UnshareDiagram(string email, Guid diagramId)
            => await diagramService.UpdateSharing(diagramId, remove: new[] { email });

        public async Task<List<DiagramShare>> ShareDiagram(string email, Guid diagramId)
            => await diagramService.UpdateSharing(diagramId, add: new[] { email });

        public async Task<List<DiagramShare>> GetSharing(Guid diagramId)
            => await diagramService.GetSharing(diagramId);

        [HttpPost]
        public async Task<Guid> SaveDiagram(string diagram)
        {
            var saveDiagram = serializer.Deserialize<SaveDiagramRequest>(diagram);
            
            var result = await diagramService.SaveState(saveDiagram.Name, saveDiagram.ID ?? Guid.Empty, diagram);
            return result.Id;
        }
    }
}
