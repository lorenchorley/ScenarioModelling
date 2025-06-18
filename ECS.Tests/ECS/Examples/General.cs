using Friflo.Engine.ECS;
using NUnit.Framework;
using System;
using Tests.Utils;
using static Friflo.Engine.ECS.ComponentChangedAction;

// ReSharper disable UnusedVariable
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable CheckNamespace
namespace Tests.Examples
{

    // See: https://friflo.gitbook.io/friflo.engine.ecs
    public static class General
    {

        [Test, Category("ECS")]
        public static void CreateStore()
        {
            var store = new EntityStore();
        }

        [Test, Category("ECS")]
        public static void CreateEntity()
        {
            var store = new EntityStore();
            store.CreateEntity();
            store.CreateEntity();

            Assert.IsTrue(store.entityCount == 2, "EntityStore should have 2 entities.");

            foreach (var entity in store.Entities)
            {
                Console.WriteLine($"entity {entity}");
                Assert.AreEqual(0, entity.ComponentCount());
            }
            // > entity id: 1  []       Info:  [] entity has no components
            // > entity id: 2  []
        }

        [Test, Category("ECS")]
        public static void DeleteEntity()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity();
            entity.DeleteEntity();
            var isDeleted = entity.IsDeleted;
            Console.WriteLine($"deleted: {isDeleted}");     // > deleted: True

            Assert.IsTrue(isDeleted, "Entity should be deleted.");
        }

        [Test, Category("ECS")]
        public static void DisableEntity()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity();
            entity.Enabled = false;
            Console.WriteLine(entity);                      // > id: 1  [#Disabled]

            Assert.IsFalse(entity.Enabled, "Entity should be disabled.");
            Assert.IsTrue(entity.Tags.Has<Disabled>(), "Entity should be disabled via tag.");

            var query = store.Query();
            Console.WriteLine($"default - {query}");        // > default - Query: []  Count: 0

            Assert.IsTrue(query.Count == 0, "Query should not return disabled entities by default.");

            var disabled = store.Query().WithDisabled();
            Console.WriteLine($"disabled - {disabled}");    // > disabled - Query: []  Count: 1

            Assert.IsTrue(disabled.Count == 1, "Query should return disabled entities when WithDisabled is used.");
        }

        [ComponentKey("my-component")]
        public struct MyComponent : IComponent
        {
            public int value;
        }

        [Test]
        public static void AddComponents()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity();
            
            // add components
            entity.AddComponent(new EntityName("Hello World!"));// EntityName is build-in
            entity.AddComponent(new MyComponent { value = 42 });
            Console.WriteLine($"entity: {entity}");             // > entity: id: 1  "Hello World!"  [EntityName, Position]

            // get component
            Console.WriteLine($"name: {entity.Name.value}");    // > name: Hello World!
            var value = entity.GetComponent<MyComponent>().value;
            Console.WriteLine($"MyComponent: {value}");         // > MyComponent: 42

            // Serialize entity to JSON
            Console.WriteLine(entity.DebugJSON);
        }

        /// <summary>
        /// <see cref="EntityStoreBase.GetUniqueEntity"/> is used to reduce code coupling.
        /// It enables access to a unique entity without the need to pass the entity by external code.
        /// </summary>
        [Test, Category("ECS")]
        public static void GetUniqueEntity()
        {
            var store = new EntityStore();
            store.CreateEntity(new UniqueEntity("Player"));     // UniqueEntity is build-in

            var player = store.GetUniqueEntity("Player");
            Console.WriteLine($"entity: {player}");             // > entity: id: 1  [UniqueEntity]

            Assert.IsNotNull(player, "Unique entity 'Player' should be found in the store.");
            Assert.IsTrue(player.HasComponent<UniqueEntity>(), "Unique entity 'Player' should have UniqueEntity component.");
            Assert.AreEqual("Player", player.GetComponent<UniqueEntity>().uid, "UniqueEntity uid should match 'Player'.");

            Assert.Throws<InvalidOperationException>(() => store.GetUniqueEntity("NonExistantEntity"));
        }


        public struct MyTag1 : ITag { }
        public struct MyTag2 : ITag { }

        [Test, Category("ECS")]
        public static void AddTags()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity();

            // add tags
            entity.AddTag<MyTag1>();
            entity.AddTag<MyTag2>();
            Console.WriteLine($"entity: {entity}");     // > entity: id: 1  [#MyTag1, #MyTag2]

            // get tag
            var tag1 = entity.Tags.Has<MyTag1>();
            Console.WriteLine($"tag1: {tag1}");         // > tag1: True

            Assert.AreEqual("id: 1  [#MyTag1, #MyTag2]", entity.ToString());
            Assert.IsTrue(tag1);
        }

        [Test, Category("ECS")]
        public static void EntityQueries()
        {
            var store = new EntityStore();
            store.CreateEntity(new EntityName("entity-1"));
            store.CreateEntity(new EntityName("entity-2"), Tags.Get<MyTag1>());
            store.CreateEntity(new EntityName("entity-3"), Tags.Get<MyTag1, MyTag2>());

            // --- query components
            var queryNames = store.Query<EntityName>();
            queryNames.ForEachEntity((ref EntityName name, Entity entity) =>
            {
                // ... 3 matches
            });
            Assert.AreEqual(3, queryNames.Count, "Query should return 3 entities with EntityName component.");

            // --- query components with tags
            var namesWithTags = store.Query<EntityName>().AllTags(Tags.Get<MyTag1, MyTag2>());
            namesWithTags.ForEachEntity((ref EntityName name, Entity entity) =>
            {
                // ... 1 match
            });
            Assert.AreEqual(1, namesWithTags.Count, "Query should return 1 entity with EntityName and MyTag1, MyTag2 tags.");

            // --- use query.Entities in case an iteration requires no component access
            foreach (var entity in queryNames.Entities)
            {
                // ... 3 matches
            }
            Assert.AreEqual(3, queryNames.Entities.Count, "Query should return 3 entities with EntityName component.");
        }

        public class MyScript : Script { public int data; }

        [Test, Category("ECS")]
        public static void AddScript()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity();

            // add script
            entity.AddScript(new MyScript { data = 123 });
            Console.WriteLine($"entity: {entity}");             // > entity: id: 1  [*MyScript]

            // get script
            var myScript = entity.GetScript<MyScript>();
            Console.WriteLine($"data: {myScript.data}");        // > data: 123

            Assert.AreEqual("id: 1  [*MyScript]", entity.ToString());
            Assert.AreEqual(123, myScript.data);
        }

        [Test, Category("ECS")]
        public static void AddChildEntities()
        {
            var store = new EntityStore();
            var root = store.CreateEntity();
            var child1 = store.CreateEntity();
            var child2 = store.CreateEntity();

            // add child entities
            root.AddChild(child1);
            root.AddChild(child2);

            Console.WriteLine($"entities: {root.ChildEntities}"); // > entities: Count: 2

            Assert.AreEqual(2, root.ChildEntities.Count);
        }

        [Test, Category("ECS")]
        public static void EventHandlers()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity();

            var capturer = new Capturer();

            entity.OnComponentChanged += ev => { Console.WriteLine(ev); capturer.Grab("OnComponentChanged", ev); }; // > entity: 1 - event > Add Component: [MyComponent]
            entity.OnTagsChanged += ev => { Console.WriteLine(ev); capturer.Grab("OnTagsChanged", ev); }; // > entity: 1 - event > Add Tags: [#MyTag1]
            entity.OnScriptChanged += ev => { Console.WriteLine(ev); capturer.Grab("OnScriptChanged", ev); }; // > entity: 1 - event > Add Script: [*MyScript]
            entity.OnChildEntitiesChanged += ev => { Console.WriteLine(ev); capturer.Grab("OnChildEntitiesChanged", ev); }; // > entity: 1 - event > Add Child[0] = 2

            entity.AddComponent(new MyComponent());
            entity.AddTag<MyTag1>();
            entity.AddScript(new MyScript());
            entity.AddChild(store.CreateEntity());

            capturer.AssertIsEqual("""
                OnComponentChanged : entity: 1 - event > Add Component: [MyComponent]
                OnTagsChanged : entity: 1 - event > Add Tags: [#MyTag1]
                OnScriptChanged : entity: 1 - event > Add Script: [*MyScript]
                OnComponentChanged : entity: 1 - event > Add Component: [TreeNode]
                OnChildEntitiesChanged : entity: 1 - event > Add Child[0] = 2
                """);
        }

        [Test, Category("ECS")]
        public static void ComponentEvents()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity();

            Capturer capturer = new();

            entity.OnComponentChanged += ev =>
            {
                if (ev.Type == typeof(EntityName))
                {
                    string log = ev.Action switch
                    {
                        Add => $"new: {ev.Component<EntityName>()}",
                        Update => $"new: {ev.Component<EntityName>()}  old: {ev.OldComponent<EntityName>()}",
                        Remove => $"old: {ev.OldComponent<EntityName>()}",
                        _ => null
                    };

                    Console.WriteLine($"entity: {ev.Entity.Id} - {ev.Action} {log}");
                    capturer.Grab($"entity: {ev.Entity.Id} - {ev.Action} {log}");
                }
            };
            entity.AddComponent(new EntityName("Peter"));
            entity.AddComponent(new EntityName("Paul"));
            entity.RemoveComponent<EntityName>();

            capturer.AssertIsEqual("""
                entity: 1 - Add new: 'Peter'
                entity: 1 - Update new: 'Paul'  old: 'Peter'
                entity: 1 - Remove old: 'Paul'
                """);
        }

        /* Output
        entity: 1 - Add new: 'Peter'
        entity: 1 - Update new: 'Paul'  old: 'Peter'
        entity: 1 - Remove old: 'Paul'
        */


        [Test, Category("ECS")]
        public static void TagEvents()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity();
            entity.OnTagsChanged += ev =>
            {
                string log = "";
                if (ev.AddedTags.Has<MyTag1>()) { log += " added:   MyTag1"; }
                if (ev.RemovedTags.Has<MyTag1>()) { log += " removed: MyTag1"; }

                if (ev.AddedTags.Has<MyTag2>()) { log += " added:   MyTag2"; }
                if (ev.RemovedTags.Has<MyTag2>()) { log += " removed: MyTag2"; }

                Console.WriteLine($"entity: {entity.Id} -{log}");
            };
            entity.AddTag<MyTag1>();
            entity.RemoveTag<MyTag1>();
            entity.AddTags(Tags.Get<MyTag1, MyTag2>());
        }

        /* Output
        entity: 1 - added:   MyTag1
        entity: 1 - removed: MyTag1
        entity: 1 - added:   MyTag1 added:   MyTag2
        */

        [Test, Category("ECS")]
        public static void CloneEntity()
        {
            var store = new EntityStore();
            var entity = store.CreateEntity(new Position(1, 2, 3), Tags.Get<MyTag1>());

            var clone = entity.CloneEntity();
            // the cloned entity have the same components and tags as the original entity.

            Assert.AreNotEqual(entity.Id, clone.Id);
            Assert.IsTrue(clone.HasComponent<Position>());
            Assert.IsTrue(clone.Tags.Has<MyTag1>());
        }

        public struct NetTag : ITag { }

        /// Copy subset of entities to another store
        [Test, Category("ECS")]
        public static void CopyEntities()
        {
            var store = new EntityStore();
            var targetStore = new EntityStore();

            store.CreateEntity(new Position(1, 1, 1));                     // 1
            store.CreateEntity(new Position(2, 2, 2), Tags.Get<NetTag>()); // 2
            store.CreateEntity(new Position(3, 3, 3));                     // 3
            store.CreateEntity(new Position(4, 4, 4), Tags.Get<NetTag>()); // 4
            store.CreateEntity(new Position(5, 5, 5));                     // 5

            // Query will copy only entities [2, 4] having a NetTag
            var query = store.Query().AnyTags(Tags.Get<NetTag>());
            foreach (var entity in query.Entities)
            {
                // preserve same entity ids in target store
                if (!targetStore.TryGetEntityById(entity.Id, out Entity targetEntity))
                {
                    targetEntity = targetStore.CreateEntity(entity.Id);
                }
                entity.CopyEntity(targetEntity);
            }

            // target store contains two entities [2, 4] with same components and tags as in the original store

            var targetQuery = store.Query().AnyTags(Tags.Get<NetTag>());
            Assert.AreEqual(2, targetStore.entityCount);
            Assert.AreEqual(2, targetQuery.Count);
        }


        public struct CollisionSignal
        {
            public Entity other;
        }

        [Test, Category("ECS")]
        public static void AddSignalHandler()
        {
            var store = new EntityStore();
            var player = store.CreateEntity(1);

            Capturer capturer = new(); 

            player.AddSignalHandler<CollisionSignal>(signal =>
            {
                Console.WriteLine($"collision signal - entity: {signal.Entity.Id} other: {signal.Event.other.Id}");
                capturer.Grab("CollisionSignal", $"entity: {signal.Entity.Id} other: {signal.Event.other.Id}");
            });

            var npc = store.CreateEntity(2);
            // ... detect collision. e.g. with a collision system. In case of collision:
            player.EmitSignal(new CollisionSignal { other = npc });

            capturer.AssertIsEqual("CollisionSignal : entity: 1 other: 2");
        }

        /* Output
        collision signal - entity: 1 other: 2
        */

    }

}