using SeaQuailDiagramTool.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeaQuailDiagramTool.Domain.Services
{
    public class DiagramService
    {
        private readonly IPersistenceService<Diagram> diagramPersistence;
        private readonly IPersistenceService<DiagramSnapshot> snapshotPersistence;
        private readonly IPersistenceService<DiagramShare> sharePersistence;
        private readonly CurrentUserService userService;

        public DiagramService(
            IPersistenceService<Diagram> diagramPersistence, 
            IPersistenceService<DiagramSnapshot> snapshotPersistence,
            IPersistenceService<DiagramShare> sharePersistence,
            CurrentUserService userService)
        {
            this.diagramPersistence = diagramPersistence;
            this.snapshotPersistence = snapshotPersistence;
            this.sharePersistence = sharePersistence;
            this.userService = userService;
        }

        public async Task<IEnumerable<Diagram>> GetCurrentUserDiagrams()
        {
            var userId = (await userService.GetUser())?.Id;
            return await diagramPersistence.Filter(d => d.OwnerId == userId);
        }

        public async Task<IEnumerable<Diagram>> GetSharedDiagrams()
        {
            var email = (await userService.GetUser())?.Email;
            var shares = await sharePersistence.Filter(s => s.Email == email);
            var sharedDiagrams = shares.Select(s => s.DiagramId);
            var results = await Task.WhenAll(sharedDiagrams.Select(async id => await diagramPersistence.GetById(id)));
            return results;
        }

        public async Task<Diagram> SaveState(string name, Guid id, string diagramState)
        {
            var user = await userService.GetUser();
            var diagram = id == Guid.Empty ? new Diagram { OwnerId = user.Id } : await diagramPersistence.GetById(id);
            if (diagram.OwnerId == user.Id)
            {
                diagram.Name = name;
                var snapshot = diagram.Id != Guid.Empty ? await snapshotPersistence.GetOne(s => s.DiagramId == id && s.IsDefault == true)
                    : new() { Name = "Primary Diagram", IsDefault = true };
                var savedDiagram = await diagramPersistence.Save(diagram);
                
                snapshot.DiagramId = savedDiagram.Id;
                snapshot.DiagramData = diagramState;
                await snapshotPersistence.Save(snapshot);
                return savedDiagram;
            }
            throw new NotImplementedException();
        }

        public async Task<List<DiagramShare>> UpdateSharing(Guid diagramId, string[] add = null, string[] remove = null)
        {
            var diagram = await GetDiagramForEdit(diagramId);
            if (diagram != null)
            {
                var shareLookup = (await GetSharing(diagramId))
                    .ToDictionary(s => s.Email, StringComparer.OrdinalIgnoreCase);

                if (add != null)
                {
                    foreach (var email in add.Where(e => !shareLookup.ContainsKey(e)))
                    {
                        await sharePersistence.Save(new() { DiagramId = diagramId, Permission = DiagramSharePermission.View, Email = email });
                    }
                }
                if (remove != null)
                {
                    foreach (var share in remove.Select(e => shareLookup.TryGetValue(e, out var item) ? item : null))
                    {
                        if (share != null)
                        {
                            await sharePersistence.Delete(share.Id);
                        }
                    }
                }

                return await GetSharing(diagramId);
            }
            return new();
        }

        public async Task<List<DiagramShare>> GetSharing(Guid diagramId)
            => await sharePersistence.GetList(s => s.DiagramId == diagramId);

        public async Task<DiagramSnapshot> AddSnapshot(string diagramData, string name, Guid dgid)
        {
            var diagram = await GetDiagramForEdit(dgid);
            if (diagram != null)
            {
                var snapshot = new DiagramSnapshot() { Name = name, DiagramData = diagramData, DiagramId = dgid };
                return await snapshotPersistence.Save(snapshot);
            }
            return null;
        }

        public async Task<IEnumerable<DiagramSnapshot>> GetSnapshots(Guid dgid)
        {
            var diagram = await GetDiagramForEdit(dgid);
            if (diagram != null)
            {
                return await snapshotPersistence.GetList(s => s.DiagramId == dgid);
            }
            return null;
        }

        public async Task<DiagramSnapshot> GetSnapshot(Guid id)
        {
            var snapshot = await snapshotPersistence.GetById(id);
            var diagram = await GetDiagramForEdit(snapshot.DiagramId);
            if (diagram != null)
            {
                return snapshot;
            }
            return null;
        }

        public async Task<bool> DeleteSnapshot(Guid id)
        {
            var snapshot = await snapshotPersistence.GetById(id);
            var diagram = await GetDiagramForEdit(snapshot.DiagramId);
            if (diagram != null && !snapshot.IsDefault)
            {
                await snapshotPersistence.Delete(id);
                return true;
            }
            return false;
        }

        private async Task<Diagram> GetDiagramForEdit(Guid id)
        {
            var user = await userService.GetUser();
            var diagram = await diagramPersistence.GetById(id);
            return diagram?.OwnerId == user?.Id ? diagram : null;
        }

        public async Task<Diagram> TogglePublicAccess(Guid id)
        {
            var diagram = await GetDiagramForEdit(id);
            if (diagram != null)
            {
                diagram.AllowPublicAccess = !diagram.AllowPublicAccess;
                return await diagramPersistence.Save(diagram);                
            }
            return null;
        }

        public async Task<Diagram> GetById(Guid id)
        {
            var diagram = await diagramPersistence.GetById(id);
            if (diagram != null)
            {
                diagram.Snapshots = (await snapshotPersistence.Filter(s => s.DiagramId == id)).ToList();
            }
            return diagram;
        }

        public async Task<Diagram> GetCurrentUserDiagram(Guid? id)
        {
            var user = await userService.GetUser();
            var diagram = await diagramPersistence.GetById(id ?? Guid.Empty);
            if (diagram == null) return null;
            if (diagram?.OwnerId != user?.Id && diagram.AllowPublicAccess == false)
            {
                var email = user.Email;
                var share = await sharePersistence.Filter(s => s.Email == email && s.DiagramId == id);
                if (!share.Any())
                {
                    return null;
                }
            }
            var snapshots = await snapshotPersistence.GetList(s => s.DiagramId == id);
            diagram.Snapshots = snapshots;
            return diagram;
        }
    }
}
