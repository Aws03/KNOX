# Fix for Migration Error: QuizSource Default Value

## Problem
Error: `Default value '0' of type 'int' cannot be set on property 'Source' of type 'JadaraITKnowledgeSystem.Domain.Quizzes.Enums.QuizSource' in entity type 'Quiz'.`

## Root Cause
Entity Framework Core design-time tools have cached the old configuration or there's a mismatch in how the enum default value is specified.

## Solution Steps

### Step 1: Clean Build
```bash
# Clean the solution
dotnet clean

# Clean bin and obj folders manually if needed
# Delete bin and obj folders in all projects

# Rebuild
dotnet build
```

### Step 2: Verify QuizConfiguration
The file `JadaraITKnowledgeSystem.Infrastructure/Persistence/Configurations/QuizConfigration.cs` should have:

```csharp
builder.Property(q => q.Source)
    .IsRequired()
    .HasConversion<int>()
    .HasDefaultValue((int)QuizSource.Manual);  // Cast to int is crucial
```

### Step 3: Clear EF Core Design-Time Cache
```bash
# Navigate to Infrastructure project
cd JadaraITKnowledgeSystem.Infrastructure

# Remove any existing migrations related to quiz generation (if you haven't applied them yet)
# If you already tried to create AddQuizGenerationFeature migration, delete it:
# Delete the migration file and update AppDbContextModelSnapshot

# Clear NuGet cache (optional but can help)
dotnet nuget locals all --clear
```

### Step 4: Alternative Approach - Remove Default Value from Configuration

If the issue persists, try this alternative approach:

**Option A: Remove HasDefaultValue from Configuration**

Edit `QuizConfigration.cs`:
```csharp
builder.Property(q => q.Source)
    .IsRequired()
    .HasConversion<int>();
    // Remove .HasDefaultValue((int)QuizSource.Manual);
```

The default value is already set in the entity:
```csharp
public QuizSource Source { get; private set; } = QuizSource.Manual;
```

**Option B: Use SQL Default Constraint Instead**

In the migration file (after it's generated), manually add:
```csharp
migrationBuilder.AddColumn<int>(
    name: "Source",
    table: "Quizzes",
    nullable: false,
    defaultValue: 0);  // Use plain int
```

### Step 5: Generate Migration (Try Different Approaches)

**Approach 1: Standard Migration**
```bash
cd JadaraITKnowledgeSystem.Infrastructure
dotnet ef migrations add AddQuizGenerationFeature --startup-project ../JadaraITKnowledgeSystem.API --verbose
```

**Approach 2: Force Rebuild**
```bash
dotnet build --no-incremental
dotnet ef migrations add AddQuizGenerationFeature --startup-project ../JadaraITKnowledgeSystem.API --force
```

**Approach 3: Specify Context Explicitly**
```bash
dotnet ef migrations add AddQuizGenerationFeature --context AppDbContext --startup-project ../JadaraITKnowledgeSystem.API
```

### Step 6: If All Else Fails - Manual Migration Approach

1. Remove `.HasDefaultValue()` from QuizConfiguration
2. Generate migration successfully
3. Manually edit the generated migration file to add the default value:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // ... other changes ...
    
    migrationBuilder.AddColumn<int>(
        name: "Source",
        table: "Quizzes",
        type: "int",
        nullable: false,
        defaultValue: 0);  // 0 = QuizSource.Manual
}
```

### Step 7: Verify Configuration is Correct

Run this command to check your DbContext configuration:
```bash
dotnet ef dbcontext info --startup-project ../JadaraITKnowledgeSystem.API
```

## Quick Fix (Recommended)

**Edit QuizConfigration.cs and remove HasDefaultValue:**

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            // ... existing code ...

            builder.Property(q => q.Source)
                .IsRequired()
                .HasConversion<int>();
                // Default value already set in entity: = QuizSource.Manual

            // ... rest of configuration ...
        }
    }
}
```

Then run:
```bash
dotnet clean
dotnet build
cd JadaraITKnowledgeSystem.Infrastructure
dotnet ef migrations add AddQuizGenerationFeature --startup-project ../JadaraITKnowledgeSystem.API
```

## Why This Works

1. **Entity-level default**: The Quiz entity already has `public QuizSource Source { get; private set; } = QuizSource.Manual;`
2. **EF Core honors this**: EF Core will use this default value when creating new entities in-memory
3. **Database default**: The migration will still add a default constraint in SQL Server as `0`
4. **No type mismatch**: By removing the explicit `.HasDefaultValue()`, we avoid the type conversion issue

## After Migration Success

Apply the migration:
```bash
dotnet ef database update --startup-project ../JadaraITKnowledgeSystem.API
```

Verify:
```sql
SELECT * FROM SystemSettings;
SELECT COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Quizzes' AND COLUMN_NAME = 'Source';
```
