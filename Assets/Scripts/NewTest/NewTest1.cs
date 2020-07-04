using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTest1
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTest1SimplePasses()
        {
            double[,] weightMap = new double[,] { 
                { 1, 1, 10, 1 },
                { 1, 1, 10, 1 }, 
                { 1, 1, 10, 1 }, 
                { 1, 1, 1, 1 } };

            // Use the Assert class to test conditions
           /* NavigationManager navManager = new NavigationManager(weightMap, 4);

            //Vector2Int srcPoint = new Vector2Int(1, 3);
            Vector2Int srcPoint = new Vector2Int(0, 3);
            Vector2Int destPoint = new Vector2Int(1, 1);

            List<Vector2Int> path = navManager.getPathTo(srcPoint, destPoint);

            Assert.That(path[0] == new Vector2Int(1, 3));
            Assert.That(path[1] == new Vector2Int(2, 3));
            Assert.That(path[2] == new Vector2Int(3, 3));
            Assert.That(path[3] == new Vector2Int(3, 2));
            Assert.That(path[4] == new Vector2Int(3, 1));
            Assert.That(path[5] == new Vector2Int(2, 1));
            Assert.That(path[6] == new Vector2Int(1, 1));
            */
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTest1WithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
