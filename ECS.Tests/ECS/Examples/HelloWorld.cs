using Friflo.Engine.ECS;
using NUnit.Framework;
using System.Numerics;

// ReSharper disable UnusedParameter.Local
// ReSharper disable CheckNamespace
namespace Tests.Examples
{

    public struct Velocity : IComponent 
    { 
        public Vector3 value;

        public Velocity(float x, float y, float z)
        {
            value = new Vector3(x, y, z);
        }
    }

    // See: https://github.com/friflo/Friflo.Engine.ECS#-examples
    public static class HelloWorldExample
    {
        [Test, Category("ECS")]
        public static void HelloWorld()
        {
            var world = new EntityStore();
            for (int n = 1; n < 10; n++)
            {
                world.CreateEntity(new Position(n, 0, 0), new Velocity { value = new Vector3(0, n, 0) });
            }
            
            var query = world.Query<Position, Velocity>();
            
            Assert.AreEqual(9, query.Count);

            query.ForEachEntity((ref Position position, ref Velocity velocity, Entity entity) =>
            {
                Assert.AreNotEqual(0, position.x);
                Assert.AreEqual(0, position.y);
            });

            query.ForEachEntity((ref Position position, ref Velocity velocity, Entity entity) =>
            {
                position.value += velocity.value;
            });

            query.ForEachEntity((ref Position position, ref Velocity velocity, Entity entity) =>
            {
                Assert.AreNotEqual(0, position.x);
                Assert.AreNotEqual(0, position.y);
            });

        }
    }
}