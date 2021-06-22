using Autofac;
using System;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mentoz.AspNet.Api
{
    public class MentozDbContext : DbContext, ITransaction
    {
        public MentozDbContext() : base("name=DefaultConnection") { }
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();
        protected override void OnModelCreating(DbModelBuilder modelBuilder) // Add-Migration INIT -Verbose、Update-Database -Verbose
        {
            modelBuilder.Properties<DateTime>()
                        .Configure(x => x.HasColumnType("dateTime2"));
            modelBuilder.Properties()
                        .Where(x => x.PropertyType == typeof(byte[]) && x.Name == "Version")
                        .Configure(x => x.IsRowVersion());
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            BeforeSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void BeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            foreach (var entry in ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted))
            {
                var changeTime = DateTime.Now;
                var changor = Guid.Empty;
                if (HttpContext.Current?.User?.Identity != null)
                {
                    var subject = (HttpContext.Current.User.Identity as ClaimsIdentity).FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub);
                    if (subject != null) Guid.TryParse(subject.Value, out changor);
                }
                if (entry.Entity is Entity extra)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            extra.CreateTime = changeTime;
                            extra.Creator = changor;
                            break;
                        case EntityState.Modified:
                            extra.UpdateTime = changeTime;
                            extra.Updator = changor;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Unchanged;
                            extra.DeleteTime = changeTime;
                            extra.Deletor = changor;
                            break;
                    }
                }

                /*
                var metadataWorkspace = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;
                var objectItemCollection = (ObjectItemCollection)metadataWorkspace.GetItemCollection(DataSpace.OSpace);
                var entityType = metadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).Single(x => objectItemCollection.GetClrType(x) == entry.Entity.GetType());
                var entityContainer = metadataWorkspace.GetItems<EntityContainer>(DataSpace.CSpace)[0].EntitySets.Single(x => x.ElementType.Name == entityType.Name);
                var entityContainerMapping = metadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace)[0].EntitySetMappings.Single(s => s.EntitySet == entityContainer);
                var storeEntitySet = entityContainerMapping.EntityTypeMappings[0].Fragments[0].StoreEntitySet; // https://romiller.com/2014/04/08/ef6-1-mapping-between-types-tables/

                var note = new Note();
                note.Table = (string)storeEntitySet.MetadataProperties["Table"].Value ?? storeEntitySet.Name;
                note.OperateTime = DateTime.Now;
                note.OperateBy = (HttpContext.Current.User.Identity as ClaimsIdentity).FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
                switch (entry.State)
                {
                    case EntityState.Added:
                        note.Type = NoteType.Create;
                        note.KeyValues = JsonConvert.SerializeObject(entityType.KeyProperties
                            .ToDictionary(x => x.Name, x => entry.CurrentValues[x.Name])); // https://romiller.com/2014/10/07/ef6-1-getting-key-properties-for-an-entity/
                        note.CurrentValues = JsonConvert.SerializeObject(entry.CurrentValues.PropertyNames
                            .ToDictionary(x => x, x => entry.CurrentValues[x]));
                        break;
                    case EntityState.Modified:
                        note.Type = NoteType.Update;
                        note.KeyValues = JsonConvert.SerializeObject(entityType.KeyProperties
                            .ToDictionary(x => x.Name, x => entry.CurrentValues[x.Name]));
                        note.CurrentValues = JsonConvert.SerializeObject(entry.OriginalValues.PropertyNames
                            .ToDictionary(x => x, x => entry.CurrentValues[x]));
                        note.OriginalValues = JsonConvert.SerializeObject(entry.OriginalValues.PropertyNames
                            .ToDictionary(x => x, x => entry.OriginalValues[x]));
                        break;
                    case EntityState.Deleted:
                        note.Type = NoteType.Delete;
                        note.KeyValues = JsonConvert.SerializeObject(entityType.KeyProperties
                            .ToDictionary(x => x.Name, x => entry.OriginalValues[x.Name]));
                        note.OriginalValues = JsonConvert.SerializeObject(entry.OriginalValues.PropertyNames
                            .ToDictionary(x => x, x => entry.OriginalValues[x])
                        );
                        if (entry.Entity is IDeleted)
                        {
                            entry.State = EntityState.Unchanged;
                            ((IDeleted)entry.Entity).Deleted = true;
                        }
                        break;
                }
                Set<Note>().Add(note);
                */
            }
        }
    }
}