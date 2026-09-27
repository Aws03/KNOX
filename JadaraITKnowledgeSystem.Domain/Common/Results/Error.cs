using System.Text;

namespace JadaraITKnowledgeSystem.Domain.Common.Results
{
    public readonly record struct Error
    {
        public string Code { get; }
        public string Description { get; }
        public ErrorKind Type { get; }

        private Error(string code, string description, ErrorKind type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        public static Error Failure (string code = nameof(Failure), string description = "General Failure.") =>
            new Error(code, description, ErrorKind.Failure);

        public static Error NotFound(string code = nameof(NotFound), string description = "Not Found.") =>
            new Error(code, description, ErrorKind.NotFound);

        public static Error Validation(string code = nameof(Validation), string description = "Validation Failed.") =>
            new Error(code, description, ErrorKind.Validation);

        public static Error Unexpected(string code = nameof(Unexpected), string description = "Unexpected Error") =>
            new Error(code, description, ErrorKind.Unexpected);

        public static Error Conflict(string code = nameof(Conflict), string description = "Conflict Error") =>
            new Error(code, description, ErrorKind.Conflict);

        public static Error Unauthorized(string code = nameof(Unauthorized), string description = "Unauthorized Error") =>
            new Error(code, description, ErrorKind.Unauthorized);

        public static Error Forbidden(string code = nameof(Forbidden), string description = "Forbidden Error") =>
            new Error(code, description, ErrorKind.Forbidden);

    }
}
