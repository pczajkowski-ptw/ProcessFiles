﻿using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ProcessFilesTests
{
    public class ProcessFilesTests
    {
        private readonly string testFolder = "./testFiles";
        private readonly string testFile = "./testFiles/test1.txt";

        private readonly List<string> expectedInSubFolder = new()
        {
            "test2.txt",
            "test3.txt"
        };

        private readonly List<string> expectedInSubFolderMultipleExtensions = new()
        {
            "test.json"
        };

        private readonly string expectedInFolder = "test1.txt";

        public ProcessFilesTests()
        {
            expectedInSubFolder.Add(expectedInFolder);
            expectedInSubFolderMultipleExtensions.AddRange(expectedInSubFolder);
        }

        [Fact]
        public void ProcessFolderTest()
        {
            var result = string.Empty;
            void Callback(string value)
            {
                result = Path.GetFileName(value);
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFolder }, "txt", Callback);
            Assert.Empty(errors);
            Assert.Equal(expectedInFolder, result);
        }

        private static bool CheckResult(List<string> result, List<string> expected)
        {
            if (!result.Count.Equals(expected.Count))
                return false;

            foreach (var item in result)
            {
                if (!expected.Contains(item))
                    return false;
            }

            return true;
        }

        [Fact]
        public void ProcessFolderRecursiveTest()
        {
            var result = new List<string>();
            void Callback(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFolder }, "txt", Callback, true);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expectedInSubFolder));
        }

        [Fact]
        public void ProcessFolderAndFileTest()
        {   
            var result = new List<string>();
            void Callback(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { "./testFiles/subFolder", testFile }, "txt", Callback);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expectedInSubFolder));
        }

        [Fact]
        public void ProcessFileTest()
        {
            var result = string.Empty;
            void Callback(string value)
            {
                result = Path.GetFileName(value);
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFile }, "txt", Callback);
            Assert.Empty(errors);
            Assert.Equal(expectedInFolder, result);
        }

        [Fact]
        public void ProcessFileNotExistTest()
        {
            var result = string.Empty;
            void Callback(string value)
            {
                result = value;
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { "./testFiles/test.txt" }, "txt", Callback);
            Assert.NotEmpty(errors);
            Assert.NotEqual(expectedInFolder, result);
        }

        [Fact]
        public void ProcessFileNotMatchExtensionTest()
        {
            var result = string.Empty;
            void Callback(string value)
            {
                result = value;
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFile }, "abc", Callback);
            Assert.NotEmpty(errors);
            Assert.Empty(result);
        }

        [Fact]
        public void ProcessFolderRecursiveMultipleExtensionsTest()
        {
            var result = new List<string>();
            void Callback(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFolder }, new string[] { "txt", "json" }, Callback, true);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expectedInSubFolderMultipleExtensions));
        }
    }
}
