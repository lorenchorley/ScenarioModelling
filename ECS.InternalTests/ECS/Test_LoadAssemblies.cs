using NUnit.Framework;
using System;
using System.Reflection;

namespace Internal.ECS
{

    // ReSharper disable once InconsistentNaming
    public static class Test_LoadAssemblies
    {
        [Test, Category("ECS")]
        public static void LoadAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var first = assemblies[0];
            var result = Assembly.Load(first.FullName!);
            Assert.AreSame(first, result);
            // loading an already loaded assembly is nearly instant
            int count = 1; // 100_000 ~ #PC: 266 ms
            for (int n = 0; n < count; n++)
            {
                result = Assembly.Load(first.FullName!);
                Assert.AreSame(first, result);
            }
        }
    }

}