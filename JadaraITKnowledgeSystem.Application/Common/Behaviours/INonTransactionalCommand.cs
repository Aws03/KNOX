namespace JadaraITKnowledgeSystem.Application.Common.Behaviours;

/// <summary>
/// Opts a *Command out of <see cref="TransactionBehavior{TRequest, TResponse}"/>.
/// For long-running orchestration (e.g. calling an external AI service between
/// saves) where holding one transaction open for the whole run would hide progress
/// from other readers and lock rows for minutes. Commands it sends are still
/// transactional on their own.
/// </summary>
public interface INonTransactionalCommand;
