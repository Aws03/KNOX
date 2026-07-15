# Fix: DbContext Disposed Error in Background Quiz Generation

## Problem
Error: **"Cannot access a disposed context instance"**

### Root Cause
The background task (`Task.Run`) was trying to use the `DbContext` after the HTTP request scope ended and the `DbContext` was disposed by the DI container.

**Timeline:**
1. HTTP request arrives ? DI creates scope with `AppDbContext`
2. `GenerateQuizFromMaterialCommand` executes ? creates job ? returns response
3. HTTP response sent ? **DI scope disposed ? AppDbContext disposed**
4. Background `Task.Run` tries to execute `ProcessQuizGenerationJobCommand`
5. MediatR pipeline tries to access disposed `AppDbContext` ? **ERROR**

## Solution: Use IServiceScopeFactory

Create a **new DI scope** for the background task, which gets its own fresh `DbContext` instance.

### Changes Made

#### 1. GenerateQuizFromMaterialCommandHandler.cs

**Before:**
```csharp
private readonly IMediator _mediator;

// Background task
_ = Task.Run(async () =>
{
    await _mediator.Send(new ProcessQuizGenerationJobCommand(jobId), ...);
}, CancellationToken.None);
```

**After:**
```csharp
private readonly IServiceScopeFactory _serviceScopeFactory;

// Background task with new scope
_ = Task.Run(async () =>
{
    await Task.Delay(100);
    
    // Create new DI scope
    using var scope = _serviceScopeFactory.CreateScope();
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    
    // Use fresh DbContext from new scope
    await mediator.Send(new ProcessQuizGenerationJobCommand(jobId), ...);
}, CancellationToken.None);
```

### Why This Works

Each background task now gets:
- ? **New DI scope** - Independent of HTTP request
- ? **Fresh DbContext** - Not disposed
- ? **Own transaction** - Via TransactionBehavior
- ? **Proper cleanup** - Scope disposed when task completes

## Required Action

?? **RESTART THE APPLICATION** - Hot reload cannot apply these changes.

```
Stop debugging (Shift+F5) ? Then restart (F5)
```

## Status

? **FIXED** - DbContext disposal issue resolved  
?? **ACTION REQUIRED** - Restart application  
? **READY** - For testing

## Additional Issue Noted

Your logs show:
```
HTTP 404: jadara-hub/Lesson/81e64f1b89a5436f8f394c8bb2daf09e.pdf
```

The file doesn't exist in storage. Make sure files are uploaded before generating quizzes.
