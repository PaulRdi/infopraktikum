using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SummonParticlesTest
    {
        private GameObject summonParticlesPrefab;

        [OneTimeSetUp]
        public void Setup() {
            summonParticlesPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SummonParticles.prefab");
            Assert.NotNull(summonParticlesPrefab.GetComponent<DestroyNotifier>());
            Assert.NotNull(summonParticlesPrefab.GetComponent<ParticleSystem>());
            Assert.IsFalse(summonParticlesPrefab.GetComponent<ParticleSystem>().main.loop, "ParticleSystem should not loop, help!");
        }

        [UnityTest]
        public IEnumerator PrefabDestroysItselfAfterPlaying() {
            var summonParticles = Object.Instantiate(summonParticlesPrefab);

            bool isDestroyed = false;
            summonParticles.GetComponent<DestroyNotifier>().Destroyed += (GameObject gameObject) => {
                isDestroyed = true;
            };
            yield return new WaitForSeconds(summonParticles.GetComponent<ParticleSystem>().main.duration + 1);

            Assert.IsTrue(isDestroyed);
        }
    }
}
