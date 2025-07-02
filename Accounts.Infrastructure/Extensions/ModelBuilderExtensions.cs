using Microsoft.EntityFrameworkCore;

namespace Accounts.Infrastructure.Extensions;

public static class ModelBuilderExtensions
{
    public static void UseLowerCaseNamingConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Schema (لو بتستخدمه)
            entity.SetSchema(entity.GetSchema()?.ToLowerInvariant());

            // Table name
            entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());

            // Properties (columns)
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToLowerInvariant());
            }

            // Primary keys
            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()?.ToLowerInvariant());
            }

            // Foreign keys
            foreach (var fk in entity.GetForeignKeys())
            {
                fk.SetConstraintName(fk.GetConstraintName()?.ToLowerInvariant());
            }

            // Indexes
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName()?.ToLowerInvariant());
            }

            // Navigations (relationships)
            foreach (var navigation in entity.GetNavigations())
            {
                navigation.SetPropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
                navigation.ForeignKey.SetConstraintName(navigation.ForeignKey.GetConstraintName()?.ToLowerInvariant());
            }
        }
    }
}