using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AgentControllerTests
{
    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("Environment");
    }

    [UnityTest]
    public IEnumerator AgentWandersOnLaunch()
    {
        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(1, 1, 0, 0, 0, 0, 0);
        simulation.InitializeSimulation();

        GameObject agent = GameObject.FindGameObjectWithTag("Prey");
        Assert.NotNull(agent);
        AgentController agentController = agent.GetComponent<AgentController>();
        Assert.NotNull(agentController);

        Vector3 startingPosition = agent.transform.position;

        yield return new WaitForSeconds(1);

        Assert.AreNotEqual(startingPosition, agent.transform.position, "Agent moved from its initial position");
    }

    [UnityTest]
    public IEnumerator AgentInitialiseCorrectly()
    {
        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(1, 1, 0, 0, 0, 0, 0);
        simulation.InitializeSimulation();

        GameObject agent = GameObject.FindGameObjectWithTag("Prey");
        Assert.NotNull(agent);
        AgentController agentController = agent.GetComponent<AgentController>();
        Assert.NotNull(agentController);

        Assert.IsFalse(agentController.IsHungry());
        Assert.IsFalse(agentController.IsThirsty());
        Assert.IsFalse(agentController.CanMate());
        Assert.LessOrEqual(agentController.attractiveness, 10);
        Assert.LessOrEqual(agentController.hungerDecreaseRate, 1);
        Assert.LessOrEqual(agentController.thirstDecreaseRate, 1);

        yield return null;
    }


    [UnityTest]
    public IEnumerator AgentSpawnInsideEnvironment()
    {
        var environment = GameObject.FindGameObjectWithTag("Environment");
        Simulation simulation = environment.GetComponent<Simulation>();

        simulation.SetStats(1, 1, 0, 0, 0, 0, 0);
        simulation.InitializeSimulation();

        GameObject agent = GameObject.FindGameObjectWithTag("Prey");
        Assert.NotNull(agent);

        Assert.GreaterOrEqual(agent.transform.position.x, 0);
        Assert.LessOrEqual(agent.transform.position.x, 300);
        Assert.GreaterOrEqual(agent.transform.position.y, 0);
        Assert.LessOrEqual(agent.transform.position.y, 300);

        yield return null;
    }
}