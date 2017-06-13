namespace Ravioli.ArchiveInterface
{
    using System;
    using System.IO;

    public class AbstractorRemoveDefinition : AbstractorDefinition
    {
        public AbstractorRemoveDefinition(string sourceFile) : base(sourceFile)
        {
        }

        public AbstractorRemoveDefinition(string sourceFile, string targetDirectory) : base(sourceFile, targetDirectory)
        {
        }

        public AbstractorRemoveDefinition(string sourceFile, string targetDirectory, SearchOption searchOption) : base(sourceFile, targetDirectory, searchOption)
        {
        }

        public AbstractorRemoveDefinition(string sourceFile, string targetDirectory, SearchOption searchOption, bool optional) : base(sourceFile, targetDirectory, searchOption, optional)
        {
        }

        public AbstractorRemoveDefinition(string sourceFile, string targetDirectory, SearchOption searchOption, bool optional, CasingRule fileNameCasing) : base(sourceFile, targetDirectory, searchOption, optional, fileNameCasing)
        {
        }
    }
}

