namespace Ravioli.ArchiveInterface
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SubArchiveAbstractorDefinition : AbstractorDefinition
    {
        private AbstractorListMode listMode;
        private IList<SubArchiveAbstractorDefinition> nestedSubArchives;
        private IList<Type> pluginTypes;

        public SubArchiveAbstractorDefinition(string sourceFile, Type pluginType) : base(sourceFile)
        {
            this.pluginTypes = new List<Type>();
            this.pluginTypes.Add(pluginType);
            this.listMode = AbstractorListMode.InTargetDirectory;
            this.nestedSubArchives = new List<SubArchiveAbstractorDefinition>();
        }

        public SubArchiveAbstractorDefinition(string sourceFile, IList<Type> pluginTypes) : base(sourceFile)
        {
            this.pluginTypes = new List<Type>(pluginTypes);
            this.listMode = AbstractorListMode.InTargetDirectory;
            this.nestedSubArchives = new List<SubArchiveAbstractorDefinition>();
        }

        public SubArchiveAbstractorDefinition(string sourceFile, Type pluginType, string targetDirectory) : base(sourceFile, targetDirectory)
        {
            this.pluginTypes = new List<Type>();
            this.pluginTypes.Add(pluginType);
            this.listMode = AbstractorListMode.InTargetDirectory;
            this.nestedSubArchives = new List<SubArchiveAbstractorDefinition>();
        }

        public SubArchiveAbstractorDefinition(string sourceFile, IList<Type> pluginTypes, string targetDirectory, SearchOption searchOption, AbstractorListMode listMode) : base(sourceFile, targetDirectory, searchOption)
        {
            this.pluginTypes = new List<Type>(pluginTypes);
            this.listMode = listMode;
            this.nestedSubArchives = new List<SubArchiveAbstractorDefinition>();
        }

        public SubArchiveAbstractorDefinition(string sourceFile, Type pluginType, string targetDirectory, SearchOption searchOption, AbstractorListMode listMode) : base(sourceFile, targetDirectory, searchOption)
        {
            this.pluginTypes = new List<Type>();
            this.pluginTypes.Add(pluginType);
            this.listMode = listMode;
            this.nestedSubArchives = new List<SubArchiveAbstractorDefinition>();
        }

        public SubArchiveAbstractorDefinition(string sourceFile, Type pluginType, string targetDirectory, SearchOption searchOption, AbstractorListMode listMode, bool optional) : base(sourceFile, targetDirectory, searchOption, optional)
        {
            this.pluginTypes = new List<Type>();
            this.pluginTypes.Add(pluginType);
            this.listMode = listMode;
            this.nestedSubArchives = new List<SubArchiveAbstractorDefinition>();
        }

        public SubArchiveAbstractorDefinition(string sourceFile, Type pluginType, string targetDirectory, SearchOption searchOption, AbstractorListMode listMode, bool optional, CasingRule fileNameCasing) : base(sourceFile, targetDirectory, searchOption, optional, fileNameCasing)
        {
            this.pluginTypes = new List<Type>();
            this.pluginTypes.Add(pluginType);
            this.listMode = listMode;
            this.nestedSubArchives = new List<SubArchiveAbstractorDefinition>();
        }

        public AbstractorListMode ListMode
        {
            get
            {
                return this.listMode;
            }
            set
            {
                this.listMode = value;
            }
        }

        public IList<SubArchiveAbstractorDefinition> NestedSubArchives
        {
            get
            {
                return this.nestedSubArchives;
            }
            set
            {
                this.nestedSubArchives = value;
            }
        }

        public IList<Type> PluginTypes
        {
            get
            {
                return this.pluginTypes;
            }
            set
            {
                this.pluginTypes = value;
            }
        }
    }
}

