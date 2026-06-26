using System.Collections.Generic;
using System.Threading;

namespace ScumChecker.Core.Modules
{
    public interface IScanModule
    {
        string Name { get; }
        IEnumerable<ScumChecker.Core.ScanItem> Run(CancellationToken ct);
    }
}
