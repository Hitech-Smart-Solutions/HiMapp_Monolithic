namespace Himapp.DynamicAudit;

/// <summary>
/// Opt-in marker attribute for MediatR commands and queries.
/// Only requests decorated with this attribute are captured by
/// <see cref="Pipeline.DynamicAuditBehavior{TRequest,TResponse}"/>.
///
/// Usage:
///   [AuditLog]
///   public class CreateProjectCommand : IRequest&lt;int&gt; { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class AuditLogAttribute : Attribute { }
