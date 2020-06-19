﻿using GeneratorV2.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeneratorV2.Writing
{
    public class ProjectNode : WriterNode
    {
        private readonly string _rootNamespace;

        public ProjectNode(string rootNamespace)
        {
            _rootNamespace = rootNamespace;
        }

        public override void Write()
        {
            Writer.WriteLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            using (Writer.Indentation())
            {
                Writer.WriteLine("<PropertyGroup>");

                using (Writer.Indentation())
                {
                    Writer.WriteLine("<TargetFramework>netcoreapp3.1</TargetFramework>");
                    Writer.WriteLine("<Nullable>enable</Nullable>");
                    Writer.WriteLine($"<RootNamespace>{_rootNamespace}</RootNamespace>");
                    Writer.WriteLine("<AllowUnsafeBlocks>true</AllowUnsafeBlocks>");

                }

                Writer.WriteLine("</PropertyGroup>");
            }
            Writer.WriteLine("</Project>");
        }
    }
}
