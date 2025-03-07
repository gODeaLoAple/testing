﻿using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class ObjectComparison
    {
        [Test, Timeout(100)]
        [Description("Проверка текущего царя")]
        [Category("ToRefactor")]
        public void CheckCurrentTsar()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();

            var parent = new Person("Vasili III of Russia", 28, 170, 60, null);
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70, parent);

            actualTsar.Should().BeEquivalentTo(expectedTsar,
                opt => opt
                .AllowingInfiniteRecursion()
                .Excluding(member => member.SelectedMemberInfo.Name == nameof(Person.Id) 
                                  && member.SelectedMemberInfo.DeclaringType == typeof(Person)));
        }

        [Test]
        [Description("Альтернативное решение. Какие у него недостатки?")]
        public void CheckCurrentTsar_WithCustomEquality()
        {
            var actualTsar = TsarRegistry.GetCurrentTsar();
            var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));

            // Какие недостатки у такого подхода? 
            // 1. Один большой assert, который просто проверяет, равны объекты или нет.
            //    В случае, если тест валится, будет сложно понять, где происходят различия.
            // 2. Вручную написан (читать: скопипасчен) код для Equals.
            //    Есть подозрение, что стоило просто воспользоваться Equlas. А для кастомизации использовать что-то вроде EquialentTo.
            //    Иначе при добавлении поля тест может проходить, хотя объекты не будут равны
            // 3. Читается хуже, чем если бы был написан через FluenAPI или хотя бы через Assert.AreEqual

            // Чем решение лучше?
            // 1. Меньше кода
            // 2. В случае добавления поля не нужно переписывать тест, добавляя проверку.
            //    Если тестовые данные изменятся на это поле, то тест упадет, и его нужно будет дополнить этим изменением.
            //    Однако CheckCurrentTsar_WithCustomEquality будет все так же зеленым, т.к. у него не будет автоматически проверяться новое поле или property.
            // 3. Более простая кастомизация проверки - можно исключить поле, все поля, добавить лишь одно для проверки и т.д.
            Assert.True(AreEqual(actualTsar, expectedTsar));
        }

        private bool AreEqual(Person? actual, Person? expected)
        {
            if (actual == expected) return true;
            if (actual == null || expected == null) return false;
            return
                actual.Name == expected.Name
                && actual.Age == expected.Age
                && actual.Height == expected.Height
                && actual.Weight == expected.Weight
                && AreEqual(actual.Parent, expected.Parent);
        }
    }

    public class TsarRegistry
    {
        public static Person GetCurrentTsar()
        {
            return new Person(
                "Ivan IV The Terrible", 54, 170, 70,
                new Person("Vasili III of Russia", 28, 170, 60, null));
        }
    }

    public class Person
    {
        public static int IdCounter = 0;
        public int Age, Height, Weight;
        public string Name;
        public Person? Parent;
        public int Id;

        public Person(string name, int age, int height, int weight, Person? parent)
        {
            Id = IdCounter++;
            Name = name;
            Age = age;
            Height = height;
            Weight = weight;
            Parent = parent;
        }
    }
}