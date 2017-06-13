namespace Ravioli.ArchiveInterface
{
    using System;

    public interface IClassInfo
    {
        string[] Extensions { get; }

        string TypeName { get; }
    }
}

