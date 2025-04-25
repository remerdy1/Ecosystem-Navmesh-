using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class FOVTests
{
    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("FOVTest");
    }

    [UnityTest]
    public IEnumerator CorrectNumberOfItemsFromPreyPerspective()
    {
        var environment = GameObject.FindGameObjectWithTag("Environment");
        Assert.IsNotNull(environment);

        Simulation simulation = environment.GetComponent<Simulation>();
        Assert.IsNotNull(simulation);

        simulation.SetStats(0, 0, 0, 0, 0, 0, 0);
        simulation.InitializeSimulation();

        GameObject agent = GameObject.FindGameObjectWithTag("Prey");
        Assert.NotNull(agent);
        AgentController agentController = agent.GetComponent<AgentController>();
        Assert.NotNull(agentController);

        yield return new WaitForSeconds(1);
        Assert.AreEqual(1, agentController.fov.preyInViewRadius.Count);
        Assert.AreEqual(5, agentController.fov.foodInViewRadius.Count);
        Assert.AreEqual(3, agentController.fov.predatorsInViewRadius.Count);
    }

    [UnityTest]
    public IEnumerator CorrectNumberOfItemsFromPredatorPerspective()
    {
        var environment = GameObject.FindGameObjectWithTag("Environment");
        Assert.IsNotNull(environment);

        Simulation simulation = environment.GetComponent<Simulation>();
        Assert.IsNotNull(simulation);

        simulation.SetStats(0, 0, 0, 0, 0, 0, 0);
        simulation.InitializeSimulation();

        GameObject agent = GameObject.FindGameObjectWithTag("Predator");
        Assert.NotNull(agent);
        AgentController agentController = agent.GetComponent<AgentController>();
        Assert.NotNull(agentController);

        yield return new WaitForSeconds(1);
        Assert.AreEqual(2, agentController.fov.preyInViewRadius.Count);
        Assert.AreEqual(5, agentController.fov.foodInViewRadius.Count);
        Assert.AreEqual(2, agentController.fov.predatorsInViewRadius.Count);
    }
}