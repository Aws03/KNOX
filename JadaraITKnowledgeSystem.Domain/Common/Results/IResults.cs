using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Common.Results
{
    public interface IResults
    {
        List<Error> Errors { get; }
        bool IsSuccess { get; }
    }

    public interface IResults<out TValue> : IResults
    {
        TValue Value { get; }
    }
}
