using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SimulationControllerTests
{
    int initialPrey = 7;
    int maxPrey = 8;
    int initialFood = 5;
    int maxFood = 6;
    int initialPredator = 9;
    int maxPredator = 10;
    int spawnRate = 0;

    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("Environment");
    }

    [UnityTest]
    public IEnumerator SimulationIntializesCorrectly()
    {

        var environment = GameObject.FindGameObjectWithTag("Environment");
        Assert.IsNotNull(environment);

        Simulation simulation = environment.GetComponent<Simulation>();
        Assert.IsNotNull(simulation);

        simulation.SetStats(initialPrey, maxPrey, initialFood, maxFood, initialPredator, maxPredator, spawnRate);
        simulation.InitializeSimulation();

        yield return new WaitForSeconds(1);

        // Correct number of Prey
        Assert.AreEqual(simulation.GetCurrentPreyCount(), initialPrey);
        Assert.AreEqual(simulation.GetMaxPreyCount(), maxPrey);
        // Correct number of Food
        Assert.AreEqual(simulation.GetCurrentFoodCount(), initialFood);
        Assert.AreEqual(simulation.GetMaxFoodCount(), maxFood);
        // Correct number of Predators
        Assert.AreEqual(simulation.GetCurrentPredatorCount(), initialPredator);
        Assert.AreEqual(simulation.GetMaxPredatorCount(), maxPredator);
    }

    [UnityTest]
    public IEnumerator SimulationSpawnsPrey()
    {

        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(initialPrey, maxPrey, initialFood, maxFood, initialPredator, maxPredator, spawnRate);
        simulation.InitializeSimulation();

        yield return new WaitForSeconds(1);

        simulation.SpawnPrey(new Vector3(0, 0, 0));

        Assert.AreEqual(initialPrey + 1, simulation.GetCurrentPreyCount());
    }

    [UnityTest]
    public IEnumerator SimulationSpawnsPredator()
    {

        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(initialPrey, maxPrey, initialFood, maxFood, initialPredator, maxPredator, spawnRate);
        simulation.InitializeSimulation();

        yield return new WaitForSeconds(1);

        simulation.SpawnPredator(new Vector3(0, 0, 0));

        Assert.AreEqual(initialPredator + 1, simulation.GetCurrentPredatorCount());
    }

    [UnityTest]
    public IEnumerator SimulationDestroysFood()
    {

        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(initialPrey, maxPrey, initialFood, maxFood, initialPredator, maxPredator, spawnRate);
        simulation.InitializeSimulation();

        yield return new WaitForSeconds(1);
        GameObject food = GameObject.FindGameObjectWithTag("Food");
        simulation.DestroyFood(food);

        Assert.AreEqual(initialFood - 1, simulation.GetCurrentFoodCount());
    }

    [UnityTest]
    public IEnumerator SimulationDestroysPrey()
    {

        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(initialPrey, maxPrey, initialFood, maxFood, initialPredator, maxPredator, spawnRate);
        simulation.InitializeSimulation();

        yield return new WaitForSeconds(1);
        GameObject prey = GameObject.FindGameObjectWithTag("Prey");
        simulation.DestroyPrey(prey);

        Assert.AreEqual(initialPrey - 1, simulation.GetCurrentPreyCount());
    }

    [UnityTest]
    public IEnumerator SimulationDestroysPredator()
    {

        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(initialPrey, maxPrey, initialFood, maxFood, initialPredator, maxPredator, spawnRate);
        simulation.InitializeSimulation();

        yield return new WaitForSeconds(1);
        GameObject predator = GameObject.FindGameObjectWithTag("Predator");
        simulation.DestroyPredator(predator);

        Assert.AreEqual(initialPredator - 1, simulation.GetCurrentPredatorCount());
    }

    [UnityTest]
    public IEnumerator GetRandomPositionReturnsValidPosition()
    {
        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(initialPrey, maxPrey, initialFood, maxFood, initialPredator, maxPredator, spawnRate);
        simulation.InitializeSimulation();
        yield return new WaitForSeconds(1);

        Vector3 position = simulation.GetRandomPosition(1);

        Assert.IsFalse(position.x >= 100 && position.x <= 200 && position.z >= 100 && position.z <= 200);

        yield return null;
    }

}
