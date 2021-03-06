﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Store;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.SearchAsync
{
    [TestFixture]
    public class WhenDirectoriesAreRecursed : FileStoreTestBase
    {
        private List<string> _paths;
        private string _fullPath1;
        private string _fullPath2;
        private string _subPath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _paths = new List<string>();

            _subPath = $"{Guid.NewGuid()}";
            Directory.CreateDirectory($"{RootPath}{Path.DirectorySeparatorChar}{_subPath}");
            _fullPath1 = $"{RootPath}{Path.DirectorySeparatorChar}{_subPath}{Path.DirectorySeparatorChar}Testfile1.txt";
            _fullPath2 = $"{RootPath}{Path.DirectorySeparatorChar}{_subPath}{Path.DirectorySeparatorChar}Testfile2.txt";
            await File.WriteAllTextAsync(_fullPath1, "some text", CancellationToken);
            await File.WriteAllTextAsync(_fullPath2, "some text", CancellationToken);

            await foreach (var val in ClassInTest.SearchAsync("", new RecurseAllActionDecider(), CancellationToken))
                _paths.Add(val);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(RootPath, true);
        }

        [Test]
        public void Paths_Are_Returned()
        {
            Assert.That(_paths, Has.One.EqualTo($"{_subPath}{Path.DirectorySeparatorChar}Testfile1.txt"));
            Assert.That(_paths, Has.One.EqualTo($"{_subPath}{Path.DirectorySeparatorChar}Testfile2.txt"));
        }

        public class RecurseAllActionDecider : IPathActions
        {
            public PathAction DecideAction(string path)
            {
                if (path.EndsWith(".txt"))
                    return PathAction.Collect;

                return PathAction.Recurse;
            }
        }
    }
}