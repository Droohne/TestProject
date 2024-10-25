using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
namespace Forsight_Test
{

    [TestFixture]
    public class MyCollectionTests
    {
        private MyCollection<int, string> simpleKeyCollection; // using SetUp makes this warning

        private MyCollection<int[], int> complexKeyCollection; // using SetUp makes this warning

        private MyCollection<int, int[]> simpleKeyComplexValues;

        [SetUp]
        public void SetUp()
        {
            // change it to multiple setups?
            simpleKeyCollection = new MyCollection<int, string>();
            complexKeyCollection = new MyCollection<int[], int>();
            simpleKeyComplexValues = new MyCollection<int, int[]>();
        }

        [Test]
        public void Add_NewKey_ShouldIncreaseCount()
        {
            simpleKeyCollection.Add(1, "Value1");
            Assert.That(simpleKeyCollection.Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_NullValue_IsAllowed()
        {
            simpleKeyCollection.Add(1, null);
            Assert.That(simpleKeyCollection.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetKeyByValue_ForNullValue_ReturnsKey()
        {
            simpleKeyCollection.Add(1, null);
            Assert.That(simpleKeyCollection.GetKeyByValue(null), Is.EqualTo(1));
        }

        [Test]
        public void GetKeyByValue_ForEqualValues_ReturnsFirstKey()
        {
            simpleKeyCollection.Add(1, "Value1");
            simpleKeyCollection.Add(2, "Value1");
            Assert.That(simpleKeyCollection.GetKeyByValue("Value1"), Is.EqualTo(1));
        }

        [Test]
        public void GetKeyByValue_ForExistentValue_ReturnsCorrespondingKey()
        {
            simpleKeyCollection.Add(1, "Value1");
            Assert.That(simpleKeyCollection.GetKeyByValue("Value1"), Is.EqualTo(1));
        }

        [Test]
        public void GetKeyByValue_ForNonExistentValue_ThrowKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => simpleKeyCollection.GetKeyByValue("1"));
        }

        [Test]
        public void Add_ExistingKey_ShouldNotIncreaseCuunt()
        {
            simpleKeyCollection.Add(1, "Value1");
            simpleKeyCollection.Add(1, "Value2");
            Assert.That(simpleKeyCollection.Count, Is.EqualTo(1));
        }

        [Test]
        public void Clear_ShouldResetCount()
        {
            simpleKeyCollection.Add(1, "Value1");
            simpleKeyCollection.Clear();
            Assert.That(simpleKeyCollection.Count, Is.EqualTo(0));
        }
        [Test]
        public void Clear_GetAllKeysAndValues_ShouldReturnEmptyArrays()
        {
            simpleKeyCollection.Add(1, "Value1");
            simpleKeyCollection.Clear();
            Assert.That(simpleKeyCollection.GetAllKeys, Is.EqualTo(new int[0]));
            Assert.That(simpleKeyCollection.GetAllValues, Is.EqualTo(new string[0]));
        }

        [Test]
        public void ContainsKey_ExistingKey_ShouldReturnTrue()
        {
            simpleKeyCollection.Add(1, "Value1");
            Assert.That(simpleKeyCollection.ContainsKey(1), Is.EqualTo(true));
        }

        [Test]
        public void ContainsKey_NonExistingKey_ShouldReturnFalse()
        {
            Assert.That(simpleKeyCollection.ContainsKey(1), Is.EqualTo(false));
        }

        [Test]
        public void ContainsValue_ExistingValue_ShouldReturnTrue()
        {
            simpleKeyCollection.Add(1, "Value1");
            Assert.That(simpleKeyCollection.ContainsValue("Value1"), Is.EqualTo(true));
        }

        [Test]
        public void ContainsValue_NonExistingValue_ShouldReturnFalse()
        {
            Assert.That(simpleKeyCollection.ContainsValue("Value1"), Is.EqualTo(false));
        }

        [Test]
        public void GetAllKeys_ShouldReturnAllKeys()
        {
            simpleKeyCollection.Add(1, "Value1");
            simpleKeyCollection.Add(2, "Value2");
            var keys = simpleKeyCollection.GetAllKeys();
            Assert.That(simpleKeyCollection.GetAllKeys(), Is.EqualTo(new int[] { 1, 2 }));
        }

        [Test]
        public void GetAllValues_ShouldReturnAllValues()
        {
            simpleKeyCollection.Add(1, "Value1");
            simpleKeyCollection.Add(2, "Value2");
            var vals = simpleKeyCollection.GetAllValues();
            Assert.That(simpleKeyCollection.GetAllValues(), Is.EqualTo(new string[] { "Value1", "Value2" }));
        }

        [Test]
        public void GetValueByKey_ExistingKey_ShouldReturnValue()
        {
            simpleKeyCollection.Add(1, "Value1");
            Assert.That(simpleKeyCollection.GetValueByKey(1), Is.EqualTo("Value1"));
        }

        [Test]
        public void GetValueByKey_NonExistingKey_ShouldThrowKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => simpleKeyCollection.GetValueByKey(1));
        }

        [Test]
        public void Remove_ExistingKey_ShouldReturnValueAndDecreaseCount()
        {
            simpleKeyCollection.Add(1, "Value1");
            Assert.That(simpleKeyCollection.Remove(1), Is.EqualTo("Value1"));
            Assert.That(simpleKeyCollection.Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_NonExistingKey_ShouldThrowKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => simpleKeyCollection.Remove(1));
        }

        [Test]
        public void Add_ArrayKey_ShouldWork() // rename it?
        {
            complexKeyCollection.Add(new int[] { 1, 2, 3 }, 1);
        }

        [Test]
        public void Merge_SameTypeCollectionWithoutOverride_DoesntOverrideKeys()
        {
            simpleKeyCollection.Add(1, "Value1");
            simpleKeyCollection.Add(2, "Value2");
            simpleKeyCollection.Add(3, "Value3");
            var other = new MyCollection<int, string>();
            other.Add(1, "New Value 1");
            other.Add(4, "New Value 4");
            simpleKeyCollection.Merge(other);
            Assert.That(simpleKeyCollection.Count(), Is.EqualTo(4));
            Assert.That(simpleKeyCollection.GetValueByKey(4), Is.EqualTo("New Value 4"));
            Assert.That(simpleKeyCollection.GetValueByKey(1), Is.EqualTo("Value1"));
        }

        [Test]
        public void Merge_SameTypeCollectionWithOverride_OverrideKeys()
        {
            simpleKeyCollection.Add(1, "Value1");
            simpleKeyCollection.Add(2, "Value2");
            simpleKeyCollection.Add(3, "Value3");
            var other = new MyCollection<int, string>();
            other.Add(1, "New Value 1");
            other.Add(4, "New Value 4");
            simpleKeyCollection.Merge(other, false);
            Assert.That(simpleKeyCollection.Count(), Is.EqualTo(4));
            Assert.That(simpleKeyCollection.GetValueByKey(4), Is.EqualTo("New Value 4"));
            Assert.That(simpleKeyCollection.GetValueByKey(1), Is.EqualTo("New Value 1"));
        }

        [Test]
        public void ContainsKey_ArrayKey_ShouldWork() 
        {
            complexKeyCollection.Add([1, 2, 3], 1);
            Assert.That(complexKeyCollection.ContainsKey([1, 2, 3]), Is.EqualTo(true));
        }

        [Test]
        public void Add_ExistingArrayKey_ShouldChangeValue() 
        {
            complexKeyCollection.Add([1, 2, 3], 1);
            complexKeyCollection.Add([1, 2, 3], 2);
            complexKeyCollection.Add([1, 2, 4], 1);
            Assert.That(complexKeyCollection.Count(), Is.EqualTo(2));
            Assert.That(complexKeyCollection.GetValueByKey([1,2,3]), Is.EqualTo(2));
        }

        [Test]
        public void GetKeyByValue_ExistantComplexValue_ShouldReturnKey()
        {
            simpleKeyComplexValues.Add(1, [1,2,3]);
            Assert.That(simpleKeyComplexValues.GetKeyByValue([1, 2, 3]), Is.EqualTo(1));
        }

        [Test]
        public void GetKeyByValue_NonExistantComplexValue_ShouldReturnKey()
        {
            Assert.Throws<KeyNotFoundException>(() => simpleKeyComplexValues.GetKeyByValue([1, 2, 3]));
        }

        [Test]
        public void GetAllValues_ComplexValue_ShouldWork()
        {
            simpleKeyComplexValues.Add(1, [1, 2, 3]);
            simpleKeyComplexValues.Add(2, [1, 4, 3]);
            Assert.That(simpleKeyComplexValues.GetAllValues(), Is.EqualTo(new[] { 
                new[] { 1, 2, 3 }, 
                new[] { 1, 4, 3 } 
            }
            ));
        }
        [Test]
        public void Add_IterateOverCollectionBothWays_ShouldBeEqual()
        {

            for (int i = 1; i <= 5; i++)
            {
                simpleKeyCollection.Add(i, $"Value{i}");
            }
            int[] front = new int[5];
            int[] end = new int[5];
            int j = 0;
            while (j < 5)
            {
                front[j] = simpleKeyCollection[j].Key;
                j++;
            }
            while (j > 0)
            {
                j--;
                end[j] = simpleKeyCollection[j].Key;
            }
            Assert.That(front, Is.EquivalentTo(end));
        }

        [Test]
        public void ForeachLoop_ThroughtAllValues_ShouldWork()
        {
            for (int i = 1; i <= 5; i++)
            {
                simpleKeyCollection.Add(i, $"Value{i}");
            }
            int[] keys = new int[5];
            string[] values = new string[5];
            int j = 0;
            foreach (Node<int,string> value in simpleKeyCollection) // var is not supported here
            {
                keys[j] = value.Key;
                values[j] = value.Value;
                j++;
            }
            Assert.That(keys, Is.EquivalentTo(simpleKeyCollection.GetAllKeys()));
            Assert.That(values, Is.EquivalentTo(simpleKeyCollection.GetAllValues()));
        }

        [Test]
        public void ForLoopGetting_ThroughtAllValues_ShouldWork()
        {
            for (int i = 1; i <= 5; i++)
            {
                simpleKeyCollection.Add(i, $"Value{i}");
            }
            int[] keys = new int[5];
            string[] values = new string[5];
            for (int j = 0; j < simpleKeyCollection.Count(); j++)
            {
                keys[j] = simpleKeyCollection[j].Key;
                values[j] = simpleKeyCollection[j].Value;
            }
            Assert.That(keys, Is.EquivalentTo(simpleKeyCollection.GetAllKeys()));
            Assert.That(values, Is.EquivalentTo(simpleKeyCollection.GetAllValues()));
        }

        [Test]
        public void Setting_Value_ShouldReplaceNode()
        {

            for (int i = 1; i <= 5; i++)
            {
                simpleKeyCollection.Add(i, $"Value{i}");
            }
            var node = new Node<int, string>(10, "Value10");
            simpleKeyCollection[3] = node; 
            Assert.That(simpleKeyCollection.GetAllKeys(), Is.EquivalentTo(new int[] {1,2,3,10,5}));
            Assert.That(simpleKeyCollection.GetAllValues(), Is.EquivalentTo(new string[] { "Value1", "Value2", "Value3", "Value10", "Value5" }));
        }
    }

}
